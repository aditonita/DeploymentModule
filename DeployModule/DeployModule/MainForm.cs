using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace deploy_v_1
{
    /// <summary>
    /// The MainForm class contains controlers:
    /// <list>
    /// <item>
    /// <term>comboboxes</term>
    /// <description>select country and version</description>
    /// </item>
    /// <item>
    /// <term>running label</term>
    /// <description>describe on going action</description>
    /// </item>
    /// <item>
    /// <term>checkboxes</term>
    /// <description>allow to execute an action</description>
    /// </item>
    /// <item>
    /// <term>links</term>
    /// <description>open documentation for tasks</description>
    /// </item>
    /// </list>
    /// </summary>
    public partial class MainForm : Form
    {
        Designer _designer = Designer.Instance;
        /// <summary>
        /// constructor without parameters.
        /// <remarks>Initialize a new form based on designer class</remarks>
        /// </summary>
        #region constructor
        public MainForm()
        {
            if (_designer.Y > 400)
            {
                this.AutoScroll = true;
            }

            this.ClientSize = new System.Drawing.Size(_designer.X,_designer.Y);
            GroupBox[] groupboxes = _designer.GroupBoxes;
            ComboBox[] comboboxes = _designer.ComboBoxes;
            CheckBox[] checkboxes = _designer.CheckBoxes;
            Label[] labels = _designer.Labels;
            LinkLabel[] links = _designer.Links;
            TextBox[] textboxes = _designer.TextBoxes;
            this.Text = "Refresh...";
            this.Controls.AddRange(checkboxes);
            this.Controls.AddRange(labels);
            this.Controls.AddRange(comboboxes);
            this.Controls.AddRange(textboxes);
            this.Controls.AddRange(groupboxes);
            this.Controls.AddRange(links);
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    if (c.HasChildren)
                    {
                        foreach (Control b in c.Controls)
                        {
                            if(b is Button)
                            {
                                b.Click += new EventHandler(new Event().ButtonClick);
                            }                            
                        }
                    }
                }
                if (c is LinkLabel)
                {
                    LinkLabel lb = (LinkLabel)c;
                    lb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(new Event().LinkClick);
                }
                if(c is ComboBox)
                {
                    ComboBox cb = (ComboBox)c;
                    cb.SelectedIndexChanged += new EventHandler(new Event().ComboBoxIndexChanged);
                }
            }
            InitializeComponent();
        }
        #endregion
    }
}