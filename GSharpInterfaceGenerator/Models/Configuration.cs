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

        [XmlElement("NativeInterface")]
        public List<NativeInterface> NativeInterfaces { get; set; }

        [XmlElement("NativeEnum")]
        public List<NativeEnum> NativeEnums { get; set; }

        [XmlElement("NativeStruct")]
        public List<NativeStruct> NativeStructs { get; set; }

        [XmlElement("NativeDelegate")]
        public List<NativeDelegate> NativeDelegates { get; set; }

        [XmlElement("NativeFile")]
        public List<NativeFile> NativeFiles { get; set; }

        [XmlElement("WikiInterface")]
        public List<WikiInterface> WikiInterfaces { get; set; }

        [XmlElement("Include")]
        public List<string> Includes { get; set; }

        [XmlElement("WikiUrl")]
        public string WikiUrl { get; set; }


        public static Configuration CreateDefault()
        {
            var def = new Configuration();
            def.OutputFolder = "./NativeClasses";
            def.AlwaysOverwrite = false;
            //def.WantedTypes = new List<WantedType> {
            //    new WantedType { Name = "NameOfVirtualClass", HeaderFile = "PathToFile/WhereVirtualClassIs.h", InterfaceVersion = "INTERFACEVERSION001", ModuleName = "server" },
            //    new WantedType { Name = "file", LuaLibraryLocation = "file"},
            //};
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

    public class NativeInterface
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("InterfaceVersion")]
        public string InterfaceVersion { get; set; }

        [XmlAttribute("ModuleName")]
        public string ModuleName { get; set; }
    }

    public class NativeEnum
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    public class NativeStruct
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    public class NativeDelegate
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    public class NativeFile
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }
    }

    public class WikiInterface
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlAttribute("LuaLibraryLocation")]
        public string LuaLibraryLocation { get; set; }

    }
}
