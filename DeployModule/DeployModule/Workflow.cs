using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace deploy_v_1;

public class Workflow
{
    private Step[] _steps;
    public Workflow(String configFile)
    {
        ValidateXmlConfigFile(configFile);
        XmlSerializer desr = new(new Configuration().GetType());
        using var reader = new FileStream(configFile, FileMode.Open);
        Configuration? steps = desr.Deserialize(reader) as Configuration;
        _steps = new Step[steps!.Steps.Count];
        int i;
        foreach (var step in steps.Steps)
        {
            i = step.Id;
            _steps[i] = (new Step(step.Id, step.Name, step.Script, step.Scope, step.Countries, step.Versions));
        }
    }
    public Step[] Steps
    {
        get { return _steps; }
    }

    //public void WriteConfig()
    //{
    //    Configuration steps = new();
    //    steps.Steps.Add(new Step(1, "Download archive files from FTP", "PSScript1.ps1", "index.html#script1", "RO,HU", "current,latest"));
    //    steps.Steps.Add(new Step(2, "V.1 dictionaries are updated?", "PSScript2.ps1", "index.html#script1", "PL", "previous"));
    //    XmlSerializer ser = new XmlSerializer(steps.GetType());
    //    using (var writer = new FileStream("../../../Configuration_1.xml", FileMode.Create))
    //    {
    //        ser.Serialize(writer, steps);
    //    }
    //}
    private void ValidateXmlConfigFile(string configFile)
    {
        List<string> ids = new List<string>();
        if (!System.IO.File.Exists(configFile))
        {
            MessageBox.Show("Configuration File " + configFile + " Not Exists!");
            Environment.Exit(0);
        }
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new StringReader(File.ReadAllText(configFile)));
            var xmlNodes = xml.SelectNodes("/Configuration/Steps/step");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (XmlNode xmlNode in xmlNodes)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                foreach (System.Xml.XmlAttribute attrib in xmlNode?.Attributes)
                {
                    string atrrList = "";
                    string[] attrs = new Step().GetAttributes();
                    attrs.ToList().ForEach(s => { atrrList = atrrList + s + "; "; });
                    if (!(attrs.Contains(attrib.Name)))
                    {
                        MessageBox.Show("Attribute <" + attrib.Name + "> is not in attribute list {" + atrrList + "}");
                        Environment.Exit(1);
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    string elemList = "";
                    string[] elms = new Step().GetElements();
                    elms.ToList().ForEach(s => elemList = elemList + s + "; ");
                    if (!(elms.Contains(node.Name)))
                    {
                        MessageBox.Show("Node <" + node.Name + "> is not in node list {" + elemList + "}");
                        Environment.Exit(1);
                    }
                }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (Int32.Parse(xmlNode.Attributes["id"].Value) > (xmlNodes.Count - 1))
                {
                    MessageBox.Show("Attribute id = " + xmlNode.Attributes["id"].Value + " is grater than tasks number = " + (xmlNodes.Count - 1));
                    Environment.Exit(1);
                }

                if (ids.Contains(xmlNode.Attributes["id"].Value)) 
                {
                    MessageBox.Show("Duplicate id <" + xmlNode.Attributes["id"].Value + ">");
                    Environment.Exit(1);
                }
                else 
                {
                    ids.Add(xmlNode.Attributes["id"].Value);
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            Environment.Exit(1);
        }
    }
}
