using GSharpInterfaceGenerator.Models;
using System;
using System.Collections.Generic;

namespace GSharpInterfaceGenerator.Wiki
{

    /// <summary>
    /// 
    /// </summary>
    public class WikiClassProvider : IProvideInterfaces
    {
        public IDescribeInterfaceList MakeInterfaces(Configuration config)
        {
            var wiki = new Wiki(new Uri(config.WikiUrl));
            var interfaceList = new LuaLibraryList();
            interfaceList.Interfaces = new List<IDescribeInterface>();
            foreach (var type in config.WantedTypes)
            {
                if (string.IsNullOrWhiteSpace(type.LuaLibraryLocation)) { continue; }

                var LibraryInfo = wiki.FetchLibraryInfo(type.Name);
                LibraryInfo.Location = type.LuaLibraryLocation;
                interfaceList.Interfaces.Add(LibraryInfo);
            }

            return interfaceList;
        }

        public Type TranslateType(string luaName)
        {
            switch (luaName)
            {
                case "table":
                    return typeof(object[]);
                case "boolean":
                    return typeof(bool);
                case "number":
                    return typeof(double);
                case "string":
                    return typeof(string);
                default:
                    return typeof(object);
            }
        }
    }
}
