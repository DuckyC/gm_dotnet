using ClangSharp;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NativeClassGenerator
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

            var files = new List<string>();
            var WantedClassesHashSet = new HashSet<string> { };

            foreach (var type in config.WantedTypes)
            {
                files.Add(type.File);
                WantedClassesHashSet.Add(type.Name);
            }


            var folderExists = Directory.Exists(config.OutputFolder);
            if (!config.AlwaysOverwrite && folderExists)
            {
                Console.WriteLine("Output folder is not empty, press enter to overwrite files");
                Console.ReadLine();
            }
            if (!folderExists) { Directory.CreateDirectory(config.OutputFolder); }

            string[] arr = { "-x", "c++" };
            arr = arr.Concat(config.Includes?.Select(x => "-I" + x)).ToArray();


            var createIndex = clang.createIndex(0, 0);
            List<CXTranslationUnit> translationUnits = new List<CXTranslationUnit>();

            foreach (var file in files)
            {
                Console.WriteLine($"Parsing: {Path.GetFileName(file)}");
                CXTranslationUnit translationUnit;
                CXUnsavedFile unsavedFile;
                var translationUnitError = clang.parseTranslationUnit2(createIndex, file, arr, 3, out unsavedFile, 0, 0, out translationUnit);

                if (translationUnitError != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine("Error: " + translationUnitError);
                    var numDiagnostics = clang.getNumDiagnostics(translationUnit);

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = clang.getDiagnostic(translationUnit, i);
                        Console.WriteLine(clang.getDiagnosticSpelling(diagnostic).ToString());
                        clang.disposeDiagnostic(diagnostic);
                    }
                }

                translationUnits.Add(translationUnit);
            }

            var classList = new List<VirtualClassInfo>();
            var abstractClassVisitor = new AbstractClassVisitor(ref classList, WantedClassesHashSet);
            foreach (var tu in translationUnits)
            {
                if (WantedClassesHashSet.Count > classList.Count)
                {
                    clang.visitChildren(clang.getTranslationUnitCursor(tu), abstractClassVisitor.VisitClass, new CXClientData(IntPtr.Zero));
                }
            }

            foreach (var classDecl in classList)
            {
                var provider = new CSharpCodeProvider();
                var compileUnit = new CodeCompileUnit();
                var Classes = new CodeNamespace("GSharp.Native.Classes");
                Classes.Imports.Add(new CodeNamespaceImport(nameof(System)));
                compileUnit.Namespaces.Add(Classes);

                var classInterface = Generate(classDecl);
                // libraryInterface.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(GSharp.Attributes.LuaLibraryLocationAttribute), new CodeAttributeArgument(new CodePrimitiveExpression(LibraryInfo.Location))));
                Classes.Types.Add(classInterface);

                var sourceOutput = Path.Combine(config.OutputFolder, $"{classDecl.Name}.cs");
                using (StreamWriter sw = new StreamWriter(sourceOutput, false))
                {
                    IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
                    provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
                    tw.Close();
                }
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();

        }

        private static CodeTypeDeclaration Generate(VirtualClassInfo classInfo)
        {
            var interf = new CodeTypeDeclaration(classInfo.Name);
            interf.IsInterface = true;
            interf.TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public;
            interf.BaseTypes.AddRange(classInfo.Parents.Select(s => new CodeTypeReference(s)).ToArray());

            foreach (var methodInfo in classInfo.Methods)
            {
                var newMethod = new CodeMemberMethod();
                newMethod.Name = methodInfo.Name;
                newMethod.ReturnType = new CodeTypeReference(TranslateType(methodInfo.Return.Type));

                foreach (var arg in methodInfo.Arguments)
                {
                    var param = new CodeParameterDeclarationExpression(TranslateType(arg.Type), arg.Name);
                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        param.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(DefaultValueAttribute), new CodeAttributeArgument(new CodeSnippetExpression(arg.Default))));
                    }
                    newMethod.Parameters.Add(param);
                }
                interf.Members.Add(newMethod);
            }
            return interf;
        }

        private static Type TranslateType(string type) //BECAUSE WHY THE FUCK WOULD YOU CHECK A TYPE FOR KEYWORDS YOU FUCKING MONG https://referencesource.microsoft.com/#System/compmod/microsoft/csharp/csharpcodeprovider.cs,3295
        {
            switch (type.ToLowerInvariant())
            {
                case "void":
                    return typeof(void);
                case "bool":
                    return typeof(bool);
                case "int":
                    return typeof(int);
                case "intptr":
                    return typeof(IntPtr);
                case "float":
                    return typeof(float);
                case "string":
                    return typeof(string);
                case "uint":
                    return typeof(uint);
                case "ulong":
                    return typeof(ulong);
                default:
                    Console.WriteLine("TranslateType default: " + type);
                    return typeof(IntPtr);
            }
        }

    }
}
