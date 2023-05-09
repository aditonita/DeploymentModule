using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace deploy_v_1
{
    internal class Job
    {
        public event EventHandler<string> StartJob;
        public void RunMessageTask(string message)
        {
            DialogResult dr = MessageBox.Show(message, "", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes:
                    LogLog.logger.Debug(message + " - Yes.");
                    break;
                case DialogResult.No:
                    LogLog.logger.Debug(message + " - No. \nCanceled by user.");
                    LogLog.logger.Dispose();
                    Environment.Exit(1);
                    break;
            }
        }
        public void RunPowershellTask(string script, string message)
        {
            OnStartJob(message);
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Commands.Clear();
                ps.AddScript(File.ReadAllText(script));
                ps.AddStatement().AddCommand("Out-String");
                Collection<PSObject> result = ps.Invoke();
                foreach (PSObject obj in result)
                {
                    if (obj != null)
                    {
                        LogLog.logger.Information(obj.ToString());
                    }
                    if (ps.Streams.Error.Count > 0)
                    {
                        string err = "";
                        foreach (ErrorRecord e in ps.Streams.Error)
                        {
                            err = err + e.ToString() + "\n";
                        }
                        LogLog.logger.Error(err);
                        MessageBox.Show(err);
                        Environment.Exit(1);
                    }
                }
            }
        }
        public void RunPowershellTask(string script, Dictionary<string, string> args,string message)
        {
            OnStartJob(message);
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Commands.Clear();
                ps.AddScript(File.ReadAllText(script)).AddParameters(args);
                ps.AddStatement().AddCommand("Out-String");
                Collection<PSObject> result = ps.Invoke();
                foreach (PSObject o in result)
                {
                    if (o != null)
                    {
                        LogLog.logger.Information(o.ToString());
                    }
                    if (ps.Streams.Error.Count > 0)
                    {
                        string err = "";
                        foreach (ErrorRecord errorRecord in ps.Streams.Error)
                        {
                            err = err + errorRecord.ToString() + "\n";
                        }
                        LogLog.logger.Error(err);
                        MessageBox.Show(err);
                        Environment.Exit(1);
                    }
                }
            }
        }

        protected virtual void OnStartJob(string message)
        {
            StartJob?.Invoke(this, message);
            //throw new NotImplementedException();
        }
    }
}
