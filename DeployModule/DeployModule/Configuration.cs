using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace deploy_v_1;

[Serializable]
public class Configuration
{
	public Configuration()
	{
		this.Steps = new List<Step>();
	}
    [XmlArray]
    [XmlArrayItem(ElementName = "step", Type = typeof(Step))]
    public List<Step> Steps { get; set; }
	  
}
