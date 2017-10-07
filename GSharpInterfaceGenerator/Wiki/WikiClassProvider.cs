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
            var interfaceList = new LuaLibraryList();
            if (string.IsNullOrWhiteSpace(config.WikiUrl)) { return interfaceList; }
            var wiki = new Wiki(new Uri(config.WikiUrl));
            interfaceList.Interfaces = new List<IDescribeInterface>();
            foreach (var type in config.WikiInterfaces)
            {
                if (string.IsNullOrWhiteSpace(type.LuaLibraryLocation)) { continue; }

                var LibraryInfo = wiki.FetchLibraryInfo(type.Category);

                //LibraryInfo.Location = type.LuaLibraryLocation; //TODO: do attribute
                interfaceList.Interfaces.Add(LibraryInfo);
            }

            return interfaceList;
        }
    }
}
