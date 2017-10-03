using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuaLibraryGenerator
{
    public class WikiArticle
    {
        public static readonly Dictionary<string, string> WikiDictionary = new Dictionary<string, string>
        {
            {@"{{Type\|(.*)}}", "<a href=\"http://wiki.garrysmod.com/page/Category:{0}\">{0}</a>"}
        };

        public string Raw { get; set; }

        public WikiArticle(string raw)
        {
            Raw = raw;
        }

        public List<Arg> GetArgs()
        {
            MatchCollection matches = Regex.Matches(Raw, @"{{Arg([\s\S]*?)\n}}");
            List<Arg> args = (from Match m in matches where m.Success select new Arg(m.Groups[1].Value)).ToList();

            return args.Count > 0 ? args : null;
        }

        public List<Ret> GetReturnValues()
        {
            MatchCollection matches = Regex.Matches(Raw, @"{{Ret([\s\S]*?)\n}}");
            List<Ret> rets = (from Match m in matches where m.Success select new Ret(m.Groups[1].Value)).ToList();

            return rets.Count > 0 ? rets : null;
        }

        public static string GetValue(string str, string selector)
        {
            Match match = Regex.Match(str, @"\|" + selector + "=(.*)");
            return match.Success ? match.Groups[1].Value : "";
        }

        public static string ParseDescription(string str)
        {
            foreach (string key in WikiDictionary.Keys)
            {
                Match m = Regex.Match(str, key);
                if (m.Success)
                    str = str.Replace(m.Groups[0].Value, string.Format(WikiDictionary[key], m.Groups[1].Value));
            }
            return str;
        }
    }

    public class Arg
    {
        public Arg(string raw)
        {
            Type = WikiArticle.GetValue(raw, "type");
            Name = WikiArticle.GetValue(raw, "name");
            Desc = WikiArticle.GetValue(raw, "desc");
            Default = ConvertDefaultValue(WikiArticle.GetValue(raw, "default"));
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Default { get; set; }

        private string ConvertDefaultValue(string defaultValue)
        {
            Match m = Regex.Match(defaultValue, @"\{\{(.*)\|(.*)\}\}\(\)");
            if (m.Success)
            {
                switch (m.Groups[1].Value)
                {
                    case "GlobalFunction":
                        return $"{m.Groups[2].Value}()";
                    // TODO Add more cases if necessary
                    default:
                        return defaultValue;
                }
            }
            return defaultValue;
        }
    }

    public class Ret
    {
        public string Type { get; set; }

        public Ret()
        {

        }

        public Ret(string raw)
        {
            Type = WikiArticle.GetValue(raw, "type");
        }
    }
}
