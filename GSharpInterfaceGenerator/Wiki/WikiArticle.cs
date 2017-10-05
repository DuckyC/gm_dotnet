using GSharpInterfaceGenerator.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace GSharpInterfaceGenerator.Wiki
{

    public class LuaLibraryList : IDescribeInterfaceList
    {
        public IList<IDescribeInterface> Interfaces { get; set; }

        public TypeSource Source => TypeSource.Wiki;

        public IList<string> Namespaces => new List<string> { "System" };
    }

    public class LuaLibraryInfo : IDescribeInterface
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public List<LuaFunctionInfo> Methods { get; set; } = new List<LuaFunctionInfo>();
        public IList<string> Parents => new List<string>();
        IList<IDescribeMethod> IDescribeInterface.Methods => Methods.Cast<IDescribeMethod>().ToList();
    }

    public class Wiki
    {
        public Uri ApiPath { get; set; }
        public RestClient API;

        public Wiki(Uri apiPath)
        {
            ApiPath = apiPath;
            API = new RestClient(ApiPath);
        }

        public XRoot FetchPage(int pageid)
        {
            var request = new RestRequest("api.php");
            request.AddParameter("action", "query");
            request.AddParameter("prop", "revisions");
            request.AddParameter("rvprop", "content");
            request.AddParameter("rvgeneratexml", "1");
            request.AddParameter("rvparse", "1");
            request.AddParameter("format", "xml");
            request.AddParameter("pageids", pageid);

            var response = API.Execute(request);

            var xml = new XmlDocument();
            xml.LoadXml(response.Content);
            var rootXML = xml.SelectSingleNode("//api/query/pages/page/revisions/rev/@parsetree").Value;
            return WikiParse.ParseXML(Regex.Replace(rootXML, @"\t|\n|\r", ""));
        }

        public Dictionary<int, XRoot> FetchPages(int[] pageids)
        {
            var request = new RestRequest("api.php");
            request.AddParameter("action", "query");
            request.AddParameter("prop", "revisions");
            request.AddParameter("rvprop", "content");
            request.AddParameter("rvgeneratexml", "1");
            request.AddParameter("rvparse", "1");
            request.AddParameter("format", "xml");
            request.AddParameter("pageids", String.Join("|", pageids));

            var response = API.Execute(request);
            var xml = new XmlDocument();
            xml.LoadXml(response.Content);

            var dict = new Dictionary<int, XRoot>();
            foreach (XmlNode page in xml.SelectNodes("//api/query/pages/page"))
            {
                var pageid = int.Parse(page.Attributes["pageid"].Value);
                var rootXML = page.SelectSingleNode("revisions/rev/@parsetree").Value;
                dict.Add(pageid, WikiParse.ParseXML(Regex.Replace(rootXML, @"\t|\n|\r", "")));
            }

            return dict;
        }

        public LuaFunctionInfo FetchFunctionInfo(int pageid, string name)
        {
            Console.WriteLine($"    Fetching function: {name} - {pageid}");

            var root = FetchPage(pageid);
            var info = LuaFunctionInfo.ConvertRoot(root);
            if (info != null) { info.Name = name; }
            return info;
        }

        public List<LuaFunctionInfo> FetchFunctionInfo(Dictionary<int, string> dict)
        {
            Console.WriteLine("    Fetching functions - " + String.Join(", ", dict.Values));
            var pages = FetchPages(dict.Keys.ToArray());
            var infos = new List<LuaFunctionInfo>();
            foreach (var entry in pages)
            {
                var info = LuaFunctionInfo.ConvertRoot(entry.Value);
                if (info != null)
                {
                    info.Name = dict[entry.Key];
                    infos.Add(info);
                }
            }
            return infos;
        }


        public string FetchCategoryDescription(string categoryName)
        {
            Console.WriteLine("  Fetching description");
            var request = new RestRequest("api.php");
            request.AddParameter("action", "query");
            request.AddParameter("prop", "revisions");
            request.AddParameter("rvprop", "content");
            request.AddParameter("rvgeneratexml", "1");
            request.AddParameter("rvparse", "1");
            request.AddParameter("format", "xml");
            request.AddParameter("titles", "Category:" + categoryName);

            var response = API.Execute(request);

            var xml = new XmlDocument();
            xml.LoadXml(response.Content);
            var rootXMLString = xml.SelectSingleNode("//api/query/pages/page/revisions/rev/@parsetree").Value;

            var rootXml = new XmlDocument();
            rootXml.LoadXml(rootXMLString);
            var description = rootXml.FirstChild.InnerText;

            return description;
        }

        public LuaLibraryInfo FetchLibraryInfo(string categoryName)
        {
            Console.WriteLine($"Fetching library: {categoryName}");
            var info = new LuaLibraryInfo { Name = categoryName };
            info.Description = FetchCategoryDescription(categoryName);

            var request = new RestRequest("api.php");
            request.AddParameter("action", "query");
            request.AddParameter("format", "xml");
            request.AddParameter("list", "categorymembers");
            request.AddParameter("cmlimit", 500);
            request.AddParameter("cmtitle", "Category:" + categoryName);

            var response = API.Execute(request);

            var xml = new XmlDocument();
            xml.LoadXml(response.Content);

            var members = xml.SelectNodes("//api/query/categorymembers/cm");
            var dict = new Dictionary<int, string>();
            foreach (XmlNode member in members)
            {
                var pageid = int.Parse(member.Attributes["pageid"].Value);
                var name = member.Attributes["title"].Value.Replace(categoryName + "/", "");
                dict.Add(pageid, name);
            }
            info.Methods = FetchFunctionInfo(dict);
            return info;
        }

    }

    public class LuaFunctionInfo : IDescribeMethod
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RetTemplate> Returns { get; set; } = new List<RetTemplate>();
        public List<ArgTemplate> Args { get; set; } = new List<ArgTemplate>();
        public List<ExampleTemplate> Examples { get; set; } = new List<ExampleTemplate>();

        public IList<IDescribeArgument> Arguments => Args.Cast<IDescribeArgument>().ToList();
        IList<IDescribeReturn> IDescribeMethod.Returns => Returns.Cast<IDescribeReturn>().ToList();

        public static LuaFunctionInfo ConvertRoot(XRoot root)
        {
            bool isFunction = false;
            var info = new LuaFunctionInfo();

            foreach (var template in root.Templates)
            {
                var translated = TranslateTemplate.Translate(template);
                if (translated is FuncTemplate func)
                {
                    isFunction = true;
                    info.Description = func.Description;
                }
                else if (translated is ArgTemplate arg)
                {
                    info.Args.Add(arg);
                }
                else if (translated is RetTemplate ret)
                {
                    info.Returns.Add(ret);
                }
                else if (translated is ExampleTemplate example)
                {
                    info.Examples.Add(example);
                }
            }
            if (!isFunction) { return null; }
            return info;
        }
    }
}
