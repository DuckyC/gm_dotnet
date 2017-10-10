using System.Collections.Generic;
using System.Xml;

namespace GSharpInterfaceGenerator.Wiki
{

    public static class WikiParse
    {
        public static XRoot ParseXML(string rawxml)
        {
            var xml = new XmlDocument();
            xml.LoadXml(rawxml);
            var root = new XRoot();
            foreach (XmlNode XTemplate in xml.ChildNodes[0].ChildNodes)
            {
                if(XTemplate.Name != "template"){ continue; }
                if (XTemplate.ChildNodes.Count > 0)
                {
                    var template = new XTemplate();
                    foreach (XmlNode child in XTemplate.ChildNodes)
                    {
                        if (child.Name == "title")
                        {
                            template.Title = child.InnerText;
                        }
                        else if (child.Name == "part")
                        {
                            var part = new XPart
                            {
                                Name = child.FirstChild.InnerText, // sorry
                                Value = child.LastChild.InnerText
                            };
                            template.Parts.Add(part);
                        }
                    }
                    root.Templates.Add(template);
                }
            }

            return root;
        }
    }



    public class XRoot
    {
        public List<XTemplate> Templates { get; set; } = new List<XTemplate>();
    }

    public class XTemplate
    {
        public string Title { get; set; }
        public List<XPart> Parts { get; set; } = new List<XPart>();
    }

    public class XPart
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
