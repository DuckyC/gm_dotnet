using GSharpInterfaceGenerator.Models;
using GSharpInterfaceGenerator.Native;
using GSharpInterfaceGenerator.Wiki;
using System;
using System.IO;
using System.Xml.Serialization;

namespace GSharpInterfaceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration config;
            var configPath = string.IsNullOrWhiteSpace(args[0]) ? "config.xml" : args[0];
            try
            {
                var serializer = new XmlSerializer(typeof(Configuration));
                using (FileStream fileStream = new FileStream(configPath, FileMode.Open))
                {
                    config = serializer.Deserialize(fileStream) as Configuration;
                }

                if (config == null)
                {
                    throw new Exception("Config file not found or read incorrectly");
                }

                if (string.IsNullOrWhiteSpace(config.OutputFolder))
                {
                    throw new Exception("No output folder");
                }
                if (config.WantedTypes?.Count == 0)
                {
                    throw new Exception("No wanted types");
                }

            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    Console.WriteLine($"Config file({configPath}) does not exist, press enter to create default config");
                    Console.ReadLine();

                    try
                    {
                        
                        var serializer = new XmlSerializer(typeof(Configuration));
                        using (FileStream fileStream = new FileStream(configPath, FileMode.CreateNew))
                        {
                            serializer.Serialize(fileStream, Configuration.CreateDefault());
                        }
                        Main(args);
                        return;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Press enter to exit");
                        Console.ReadLine();
                    }
                }

                Console.WriteLine(e.Message);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                return;
            }

            var folderExists = Directory.Exists(config.OutputFolder);
            if (!config.AlwaysOverwrite && folderExists)
            {
                Console.WriteLine("Output folder is not empty, press enter to overwrite files");
                Console.ReadLine();
            }
            if (!folderExists) { Directory.CreateDirectory(config.OutputFolder); }

            foreach (TypeSource item in Enum.GetValues(typeof(TypeSource)))
            {
                EnsureFolderExists(Path.Combine(config.OutputFolder, Configuration.GetSubFolder(item)));
            }

            var nativeClassProvider = new NativeClassProvider();
            var nativeClasses = nativeClassProvider.MakeInterfaces(config);
            var nativeClassBuilder = new InterfaceBuilder(nativeClasses, nativeClassProvider, config);
            nativeClassBuilder.BuildInterfaces();

            var wikiClassProvider = new WikiClassProvider();
            var wikiClasses = wikiClassProvider.MakeInterfaces(config);
            var wikiClassBuilder = new InterfaceBuilder(wikiClasses, wikiClassProvider, config);
            wikiClassBuilder.BuildInterfaces();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        }
    }
}
