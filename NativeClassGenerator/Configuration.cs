using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NativeClassGenerator
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("OutputFolder")]
        public string OutputFolder { get; set; }

        [XmlElement("AlwaysOverwrite")]
        public bool AlwaysOverwrite { get; set; }

        [XmlElement("WantedType")]
        public List<WantedType> WantedTypes { get; set; }

        [XmlElement("Include")]
        public List<string> Includes { get; set; }


        public static Configuration CreateDefault()
        {
            var def = new Configuration();
            def.OutputFolder = "./NativeClasses";
            def.AlwaysOverwrite = false;
            def.WantedTypes = new List<WantedType> { new WantedType { Name = "NameOfVirtualClass", File = "PathToFile/WhereVirtualClassIs.h", InterfaceVersion = "INTERFACEVERSION001", ModuleName = "server" } };
            def.Includes = new List<string> { "./sourcesdk-minimal/public" };
            return def;
        }
    }

    public class WantedType
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("File")]
        public string File { get; set; }

        [XmlAttribute("InterfaceVersion")]
        public string InterfaceVersion { get; set; }

        [XmlAttribute("ModuleName")]
        public string ModuleName { get; set; }
    }
}
