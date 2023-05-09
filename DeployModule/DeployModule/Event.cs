using Microsoft.PowerShell.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordCodec;
using System.Windows.Forms;

namespace deploy_v_1
{
    internal class Event
    {
        private Designer designer = Designer.Instance;
        public const string WARNINGMESSAGE = "[warning]";
        public void ButtonClick(object? o, EventArgs e)
        {
            if (o == null)
            {
                return;
            }
            Button b = (Button)o;
            switch (b.Name)
            {
                case "Install":
                    b.Enabled = !b.Enabled;
                    Install();
                    break;
                case "Exit":
                    b.Enabled = !b.Enabled;
                    Cancel();
                    break;
                default:
                    return;
            }
        }
        public void LinkClick(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            LinkLabel lb = (LinkLabel)sender;
            TaskScope(lb);
        }
        public void ComboBoxIndexChanged(object? o, EventArgs e)
        {
            string country = "";
            string version = "";
            if (IsNotEmptyOrNull(designer.ComboBoxes[0].Text) && IsNotEmptyOrNull(designer.ComboBoxes[1].Text))
            {
                foreach (Button b in designer.Buttons)
                {
                    if (b.Name == "Install")
                        b.Enabled = true;
                }
                foreach (ComboBox cb in designer.ComboBoxes)
                {
                    if (cb.Name == "countries")
                        country = cb.Text;
                    if (cb.Name == "versions")
                        version = cb.Text;
                }
                SetCheckboxes(country, version);
            }
        }
        private void SetCheckboxes(string country, string version)
        {
            foreach (CheckBox cb in designer.CheckBoxes)
            {
                cb.Checked = false;
            }
            foreach (Step s in designer.Workflow.Steps)
            {
                if (s.Countries.Split(new char[] { '\u002C' }, StringSplitOptions.RemoveEmptyEntries).Contains(country)
                    && s.Versions.Split(new char[] { '\u002C' }, StringSplitOptions.RemoveEmptyEntries).Contains(version))
                {
                    designer.CheckBoxes[s.Id].Checked = true;
                }
            }
        }
        private void Install()
        {
            //designer.Buttons.First(item => item.Name == "Exit").Text = "Abort";
            designer.Buttons.First(item => item.Name == "Exit").Visible = false;
            LogLog.logger.Information("****** Start Deployment for " + designer.ComboBoxes[0].Text + " - " +
                designer.ComboBoxes[1].Text + " ******");
            int activejobs = 0;
            foreach(CheckBox cb in designer.CheckBoxes)
            {
                if (cb.Checked)
                {
                    activejobs++;
                }
            }
            AbordForm form = new AbordForm((activejobs));
            //MessageBox.Show(activejobs.ToString());
            Thread thread = new Thread(new ThreadStart(form.openForm));
            thread.Start();
            foreach (ComboBox c in designer.ComboBoxes)
            {
                c.Enabled = !c.Enabled;
            }
            for (int i = 0; i < designer.Workflow.Steps.Length; i++)
            {
                if (designer.CheckBoxes[i].Checked == false)
                {
                    designer.CheckBoxes[i].Enabled = false;
                }
                if (designer.CheckBoxes[i].Checked == true)
                {
                    designer.CheckBoxes[i].Enabled = false;
                    LogLog.logger.Information("Run Job... " + designer.Labels[i].Text);
                    //designer.RichTextBoxes[0].Text = "Execute Job... " + designer.Labels[i].Text + " ...";
                    RunTask(designer.Workflow.Steps[i].Script, designer.ComboBoxes[0].Text, designer.ComboBoxes[1].Text, designer.TextBoxes[0].Text, designer.Labels[i].Text);
                }
            }
            designer.RichTextBoxes[0].Text = "Process Finished...";
            //signer.Buttons.First(item => item.Name == "Exit").Text = "Exit";
            designer.Buttons.First(item => item.Name == "Exit").Visible = true;
        }
        private void Cancel()
        {
            LogLog.logger.Dispose();
            Environment.Exit(0);
        }
        private void TaskScope(LinkLabel lb)
        {
            String[] files =
            {
                "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe",
                "C:\\Program Files\\Internet Explorer\\iexplore.exe",
            };
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                {
                    if (File.Exists((Path.Combine(System.Environment.CurrentDirectory, designer.HelpFile[lb.TabIndex]))))
                    {
                        System.Diagnostics.Process.Start(files[i], (Path.Combine(System.Environment.CurrentDirectory, designer.HelpFile[lb.TabIndex])));
                    }
                    else
                    {
                        MessageBox.Show(designer.HelpFile[lb.TabIndex] + " Not Exists!");
                    }
                    break;
                }
                else
                {
                    MessageBox.Show("Chrome/Edge/IExploere Not exis!");
                }
            }
        }
        private bool IsNotEmptyOrNull(string text)
        {
            if (text is null)
                return false;
            if (text.Length == 0)
                return false;
            if (text == "")
                return false;
            return true;
        }
        private void RunTask(string script, string country, string version, string artifactUri, string message)
        {
            
            Job job = new Job();
            job.StartJob += StartJobMessage;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (script.Length > WARNINGMESSAGE.Length &&
                script.ToLower().Substring(0, WARNINGMESSAGE.Length).Equals(WARNINGMESSAGE))
            {
                job.RunMessageTask(script);
                return;
            }
            string psScript;
            parameters = GetParameters(script, country, version, artifactUri, out psScript);
            if (parameters.Count > 0)
            {
                //job.StartJob += StartJobMessage;
                job.RunPowershellTask(psScript, parameters, message);
            }
            else
            {
                //job.StartJob += StartJobMessage;
                job.RunPowershellTask(script, message);
            }
        }
        private void StartJobMessage(object s, string message)
        {
                designer.RichTextBoxes[0].Text = "Execute ... " + message + " ...";
                //throw new NotImplementedException();
        }

        private Dictionary<string, string> GetParameters(string script, string country, string version, string artifactUri, out string psScript)
        {
            Dictionary<string, string> psParams = new Dictionary<string, string>();
            string[] psArgs = script.Split("--", StringSplitOptions.TrimEntries);
            for (int i=1;i < psArgs.Length; i++)
            {
                if(psArgs[i] == "country")
                {
                    psParams.Add(psArgs[i], country);
                    continue;
                }
                if (psArgs[i] == "artifactUri")
                {
                    psParams.Add(psArgs[i], artifactUri);
                    continue;
                }
                if (psArgs[i] == "version")
                {
                    psParams.Add(psArgs[i], version);
                    continue;
                }
                if (!GetPSParameters().Contains(psArgs[i]))
                {
                    MessageBox.Show("PowerShell parameter " + psArgs[i] + " is NOT defined in App.config file");
                    LogLog.logger.Error("PowerShell parameter " + psArgs[i] + " is NOT defined in App.config file");
                    Environment.Exit(1);
                }
                else
                {
                    psParams.Add(psArgs[i], GetConfigManagerAppSettinsValue(psArgs[i]));
                }
            }
            psScript = script.Split("--", StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            return psParams;
        }
        private string GetConfigManagerAppSettinsValue(string key)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if ((ConfigurationManager.AppSettings[key].Length > 4) && (ConfigurationManager.AppSettings[key].Substring(0,4).ToLower() == "enc("))
            {
                return Codec.DecryptText(ConfigurationManager.AppSettings[key].Substring(4,
                    ConfigurationManager.AppSettings[key].Length - 5));
            }
            else
            {
                return ConfigurationManager.AppSettings[key] ?? "";
            }
        }
        private string[] GetPSParameters()
        {
#pragma warning disable CS8619 // Dereference of a possibly null reference.
            return ConfigurationManager.AppSettings.AllKeys;
        }
    }

    class AbordForm
    {
        Form form;
        public AbordForm(int time)
        {
            form = new Form();
            this.timer.Interval = time * 15000;
            form.BackColor = System.Drawing.Color.YellowGreen;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Text = "Wait Form...";
            form.Size = new System.Drawing.Size(500, 100);
            //form.Location = System.Drawing.Point.Empty;
            form.StartPosition = FormStartPosition.WindowsDefaultLocation;// .CenterParent;
            Label l = new Label();
            Button cancel = new Button();
            l.Text = "Wait until Refresh is finissed... " + "\n" + "Press Abord button to stop Refreshing process...";// + i.ToString();
            l.Size = new System.Drawing.Size(400, 75);
            cancel.BackColor = System.Drawing.Color.Gray;
            cancel.Location = new System.Drawing.Point(400, 20);
            cancel.Size = new System.Drawing.Size(75, 30);
            cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancel.Text = "Abord";
            cancel.Click += new EventHandler((s, e) => Environment.Exit(1));
            form.Controls.Add(l);
            form.Controls.Add(cancel);
        }
        public void openForm()
        {
            form.Load += Form_Load;
            form.ShowDialog();
        }
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private void Form_Load(object sender, EventArgs e)
        {
            //timer.Interval = 10000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            form.Close();
        }
    }


}
