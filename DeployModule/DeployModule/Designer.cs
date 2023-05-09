using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Security.Cryptography.Pkcs;
using System.Runtime.Intrinsics.X86;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using System.Drawing;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel;
using System.Configuration;

namespace deploy_v_1
{
    public sealed class Designer
    {
        private static readonly Designer _instance = new Designer(new Workflow(ConfigurationManager.AppSettings["ConfigurationFile"] ?? "Configuration.xml"));
        //        static Designer()
        //        {
        //        }
        public static Designer Instance
        {
            get { return _instance; }
        }
        private string[] _helpFile;
        private CheckBox[] _checkBoxes;
        private Label[] _labels;
        private Button[] _buttons;
        private ComboBox[] _comboBoxes;
        private TextBox[] _textBoxes;
        private GroupBox[] _groupBoxes;
        private LinkLabel[] _links;
        private RichTextBox[] _richTexts;
        private Workflow _workflow;
        private int _weight;
        private int _hight;
        private int _x;
        private int _y;
        public string[] HelpFile { get { return _helpFile; } }
        public Workflow Workflow { get { return _workflow; } }
        public CheckBox[] CheckBoxes => _checkBoxes;
        public Label[] Labels => _labels;
        public Button[] Buttons => _buttons;
        public ComboBox[] ComboBoxes => _comboBoxes;
        public TextBox[] TextBoxes => _textBoxes;
        public GroupBox[] GroupBoxes => _groupBoxes;
        public LinkLabel[] Links => _links;
        public RichTextBox[] RichTextBoxes => _richTexts;
        public int X => _x;
        public int Y => _y;

        private Designer(Workflow workflow)
        {
            _weight = 120;
            _hight = 20;
            _x = 8 * _weight;
            _y = 2 * 3 * _hight;

            if (_y < (2 * 3 * _hight + _weight + workflow.Steps.Count() * _hight))
            {
                _y = (2 * 3 * _hight + _weight + workflow.Steps.Count() * _hight);
            }

            _workflow = workflow;
            _helpFile = new string[workflow.Steps.Count()];
            _richTexts = SetRichTexts();
            _buttons = SetButtons();
            _checkBoxes = SetCheckBoxes();
            _comboBoxes = SetComboBoxes();
            _textBoxes = SetTextBoxes();
            _labels = SetLabels();
            _groupBoxes = SetGroupBoxes();
            _links = SetLinkLabel();
        }

        private Label[] SetLabels()
        {
            Label[] labels = new Label[_workflow.Steps.Count()];
            for (int i = 0; i < _workflow.Steps.Count(); i++)
            {
                int k = _workflow.Steps[i].Id;
                String lblText = _workflow.Steps[i].Name;
                labels[k] = new Label()
                {
                    AutoSize = true,
                    Location = new System.Drawing.Point(_weight / 3, 3 * 3 * _hight + i * 20),
                    Name = "label" + k.ToString(),
                    Size = new System.Drawing.Size(_weight / 2, _hight),
                    Text = lblText
                };
            }
            return labels;
        }
        private CheckBox[] SetCheckBoxes()
        {
            int k;
            CheckBox[] checkBoxes = new CheckBox[_workflow.Steps.Count()];
            for (int i = 0; i < _workflow.Steps.Count(); i++)
            {
                k = _workflow.Steps[i].Id;
                checkBoxes[_workflow.Steps[i].Id] = new CheckBox()
                {
                    AutoSize = true,
                    Location = new System.Drawing.Point(_weight / 6, 3 * 3 * _hight + i * 20),
                    Name = "checkBox" + k.ToString(),
                    Size = new System.Drawing.Size(_weight / 2, _hight),
                    ThreeState = false,
                    TabIndex = k,
                    UseVisualStyleBackColor = true
                };
            }
            return checkBoxes;
        }
        private LinkLabel[] SetLinkLabel()
        {
            String textLink = "GoTo: ";
            LinkLabel[] linkLabels = new LinkLabel[_workflow.Steps.Count()];
            for (int i = 0; i < _workflow.Steps.Count(); i++)
            {
                int k = _workflow.Steps[i].Id;
                if (!_workflow.Steps[k].Scope.Equals(""))
                {
                    _helpFile[k] = _workflow.Steps[k].Scope;
                    linkLabels[k] = new LinkLabel()
                    {
                        Text = textLink + _helpFile[k],
                        Location = new System.Drawing.Point(4 * _weight, 3 * 3 * _hight + i * 20),
                        Size = new System.Drawing.Size(2 * _weight, _hight),
                        AutoSize = true,
                        TabStop = true,
                        TabIndex = k,
                        LinkArea = new LinkArea(0, (textLink + " - " + _workflow.Steps[k].Scope).Length)
                    };
                }
            }
            return linkLabels;
        }
        private GroupBox[] SetGroupBoxes()
        {
            GroupBox[] groupBoxes = new GroupBox[3];
            groupBoxes[0] = new GroupBox()
            {
                Dock = DockStyle.Top,
                Name = "groupBox0",
                Size = new System.Drawing.Size(_x, 6 * _hight),
                TabStop = false,
            };
            groupBoxes[1] = new GroupBox()
            {
                Dock = DockStyle.Bottom,
                Name = "groupBox1",
                Size = new System.Drawing.Size(_x, 3 * _hight),
                TabStop = false,
            };
            groupBoxes[2] = new GroupBox()
            {
                Location = new System.Drawing.Point(0, 6 * _hight),
                Name = "groupBox3",
                Size = new System.Drawing.Size(_x, 3 * _hight),
                TabStop = false,
            };
            groupBoxes[0].Controls.AddRange(_comboBoxes);
            groupBoxes[0].Controls.AddRange(_textBoxes);
            groupBoxes[0].Controls.Add(CountryVersionLabels()[0]);
            groupBoxes[0].Controls.Add(CountryVersionLabels()[1]);
            groupBoxes[0].Controls.Add(CountryVersionLabels()[2]);
            groupBoxes[1].Controls.AddRange(_buttons);
            groupBoxes[2].Controls.Add(_richTexts[0]);
            return groupBoxes;
        }
        private Button[] SetButtons()
        {
            Button[] buttons = new Button[2];
            buttons[0] = new Button()
            {
                Location = new System.Drawing.Point(_weight, _hight),
                Name = "Install",
                Size = new System.Drawing.Size(_weight, (int)(1.5*_hight)),
                Text = "Install",
                TabIndex = 0,
                Enabled = false,
                UseVisualStyleBackColor = true
            };

            //_buttons.Add(new Button()
            buttons[1] = new Button()
            {
                Location = new System.Drawing.Point(3 * _weight, _hight),
                Name = "Exit",
                Size = new System.Drawing.Size(_weight, (int)(1.5 * _hight)),
                Text = "Exit",
                TabIndex = 1,
                UseVisualStyleBackColor = true
            };
            return buttons;
        }
        private RichTextBox[] SetRichTexts()
        {
            RichTextBox[] richTexts = new RichTextBox[1];
            richTexts[0] = new RichTextBox()
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Location = new System.Drawing.Point(20, _hight),
                Name = "richTextBox0",
                ReadOnly = true,
                Size = new System.Drawing.Size(_x, _hight),
                TabIndex = 0,
                Text = ""
            };
            return richTexts;
        }
        private TextBox[] SetTextBoxes()
        {
            TextBox[] textBoxes = new TextBox[3];
            textBoxes[0] = new TextBox() { 
                AutoSize = true,
                Location = new System.Drawing.Point(2 * _weight, 3 * _hight),
                Name = "textBox0",
                Size = new System.Drawing.Size(4 * _weight, _hight),
                TabIndex = 0,
                Text = ""
            };

            return textBoxes;
        }
        private ComboBox[] SetComboBoxes()
        {
            ComboBox[] comboBoxes = new ComboBox[2];
            comboBoxes[0] = new ComboBox()
            {
                FormattingEnabled = true,
                Location = new System.Drawing.Point(2 * _weight, _hight),
                Name = "countries",
                Size = new System.Drawing.Size(_weight, _hight),
                TabIndex = 0
            };
            comboBoxes[1] = new ComboBox()
            {
                FormattingEnabled = true,
                Location = new System.Drawing.Point(5 * _weight, _hight),
                Name = "versions",
                Size = new System.Drawing.Size(_weight, _hight),
                TabIndex = 1
            };
            comboBoxes[0].Items.AddRange(GetCountries());
            comboBoxes[1].Items.AddRange(GetVersions());
            return comboBoxes;
        }
        private List<Label> CountryVersionLabels()
        {
            List<Label> countryVersionLabels = new List<Label>();
            countryVersionLabels.Add(new Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(_weight, _hight),
                Name = "country",
                Size = new System.Drawing.Size(_weight, _hight),
                Font = new Font(Label.DefaultFont, FontStyle.Bold),
                TabIndex = 0,
                Text = "Set Country....."
            });
            countryVersionLabels.Add(new Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(4 * _weight, _hight),
                Name = "version",
                Size = new System.Drawing.Size(_weight, _hight),
                Font = new Font(Label.DefaultFont, FontStyle.Bold),
                TabIndex = 1,
                Text = "Set Version..."
            });
            countryVersionLabels.Add(new Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(_weight, 3 * _hight),
                Name = "ArtifactURI",
                Size = new System.Drawing.Size(_weight, _hight),
                Font = new Font(Label.DefaultFont, FontStyle.Bold),
                TabIndex = 1,
                Text = "Set Artifact URI..."
            });
            return countryVersionLabels;
        }
        private string[] GetCountries()
        {
            List<string> countries = new List<string>();
            foreach (Step item in _workflow.Steps)
            {
                foreach (string country in item.Countries.Split(new char[] { ',' }))
                {
                    if (!(countries.Contains(country)))
                    {
                        countries.Add(country);
                    }
                }
            }
            countries.Sort();
            return countries.ToArray();
        }
        private string[] GetVersions()
        {
            List<string> versions = new List<string>();
            foreach (Step item in _workflow.Steps)
            {
                foreach (string version in item.Versions.Split(new char[] { ',' }))
                {
                    if (!(versions.Contains(version)))
                    {
                        versions.Add(version);
                    }
                }
            }
            versions.Sort();
            return versions.ToArray();
        }
    }
}
