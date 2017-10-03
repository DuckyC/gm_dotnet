using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LuaLibraryGenerator.WikiDefinitions
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
                            var part = new XPart();
                            part.Name = child.FirstChild.InnerText; // sorry
                            part.Value = child.LastChild.InnerText;
                            template.Parts.Add(part);
                        }
                    }
                    root.Templates.Add(template);
                }
            }

            return root;
        }

        public static List<CodeCommentStatement> BuildDescription(string description)
        {
            if (description.Contains('*'))
            {
                var list = new List<CodeCommentStatement>();
                var sections = description.Split('*');

                var topText = "";
                var listHeader = "";
                var topSections = sections[0].Split(Environment.NewLine.ToCharArray());
                for (int i = 0; i < topSections.Length; i++)
                {
                    if (i == topSections.Length - 1 || string.IsNullOrWhiteSpace(topSections[i + 1]))
                    {
                        listHeader += topSections[i];
                    }
                    else
                    {
                        topText += topSections[i];
                    }
                }

                list.Add(new CodeCommentStatement(topText, true));
                list.Add(new CodeCommentStatement("<list type='bullet'>", true));
                list.Add(new CodeCommentStatement($"<listheader><description>{listHeader}</description></listheader>", true));

                for (int i = 1; i < sections.Length - 1; i++)
                {
                    list.Add(new CodeCommentStatement($"<item><description>{sections[i]}</description></item>", true));
                }

                var bottomSections = sections[sections.Length - 1].Split(Environment.NewLine.ToCharArray());

                list.Add(new CodeCommentStatement($"<item><description>{bottomSections[0]}</description></item>", true));
                list.Add(new CodeCommentStatement("</list>", true));

                list.AddRange(bottomSections.Skip(1).Select(s => new CodeCommentStatement(s, true)).ToArray());


                return list;
            }

            return null;
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
