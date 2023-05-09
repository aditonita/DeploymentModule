using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace deploy_v_1;

[Serializable]
public class Step
{
	public Step() : this(0, "", "", "", "", "")
	{
	}
	public Step(int number, string name, string script, string scope, string countries, string versions)
	{
		Name = name;
		Id = number;
		Script = script;
		Scope = scope;
		Countries = countries;
		Versions = versions;
	}
    [XmlAttribute("name")]
	public String Name { get; set; }
	[XmlAttribute("id")]
	public int Id { get; set; }
	[XmlAttribute("countries")]
	public String Countries { get; set; }
	[XmlAttribute("versions")]
	public String Versions { get; set; }	
	[XmlElement("script")]
	public string Script { get; set; }
	[XmlElement("scope")]
	public String Scope { get; set; }
	public string[] GetAttributes()
	{
		return new string[] { "id", "name", "countries", "versions" };
	}
	public string[] GetElements()
	{
		return new string[] { "script", "scope"};
	}
}
