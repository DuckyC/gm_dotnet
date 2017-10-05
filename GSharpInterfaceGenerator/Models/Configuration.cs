using System.Collections.Generic;
using System.Xml.Serialization;

namespace GSharpInterfaceGenerator.Models
{
    public enum TypeSource
    {
        [XmlEnum(Name = "Wiki")]
        Wiki,
        [XmlEnum(Name = "CPPHeader")]
        CPPHeader,
    }

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

        [XmlElement("WikiUrl")]
        public string WikiUrl { get; set; }


        public static Configuration CreateDefault()
        {
            var def = new Configuration();
            def.OutputFolder = "./NativeClasses";
            def.AlwaysOverwrite = false;
            def.WantedTypes = new List<WantedType> {
                new WantedType { Name = "NameOfVirtualClass", HeaderFile = "PathToFile/WhereVirtualClassIs.h", InterfaceVersion = "INTERFACEVERSION001", ModuleName = "server" },
                new WantedType { Name = "file", LuaLibraryLocation = "file"},
            };
            def.Includes = new List<string> { "./sourcesdk-minimal/public" };
            return def;
        }

        public static string GetSubFolder(TypeSource source)
        {
            switch (source)
            {
                case TypeSource.Wiki:
                    return "LuaLibraries";
                case TypeSource.CPPHeader:
                    return "NativeClasses";
                default:
                    return "Unknown";
            }
        }
    }

    public class WantedType
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Source")]
        public TypeSource Source { get; set; }

        #region Native
        [XmlAttribute("HeaderFile")]
        public string HeaderFile { get; set; }

        [XmlAttribute("InterfaceVersion")]
        public string InterfaceVersion { get; set; }

        [XmlAttribute("ModuleName")]
        public string ModuleName { get; set; }
        #endregion

        #region Wiki
        [XmlAttribute("LuaLibraryLocation")]
        public string LuaLibraryLocation { get; set; }
        #endregion
    }
}
