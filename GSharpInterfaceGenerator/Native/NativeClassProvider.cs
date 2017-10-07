using ClangSharp;
using GSharpInterfaceGenerator.Models;
using GSharpInterfaceGenerator.Native.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GSharpInterfaceGenerator.Native
{

    public class VirtualClassListInfo : IDescribeInterfaceList
    {
        public override TypeSource Source => TypeSource.CPPHeader;
        public override List<string> Namespaces { get; set; } = new List<string> { "System" };
    }

    public class NativeClassProvider : IProvideInterfaces
    {
        public IDescribeInterfaceList MakeInterfaces(Configuration config)
        {
            var files = new List<string>();
           
            foreach (var file in config.NativeFiles)
            {
                if (string.IsNullOrWhiteSpace(file.Path)) { continue; }
                var fullpath = Path.GetFullPath(file.Path);
                if (!File.Exists(fullpath)) { Console.WriteLine("File does not exist: " + file.Path); continue; }
                files.Add(fullpath);
            }

            string[] arr = { "-x", "c++" };
            arr = arr.Concat(config.Includes?.Select(x => "-I" + Path.GetFullPath(x))).ToArray();

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
                else
                {
                    translationUnits.Add(translationUnit);
                }

            }
            var classListInfo = new VirtualClassListInfo();

            var baseVisitor = new DependencyVisitor(classListInfo, config);
            foreach (var tu in translationUnits)
            {
                clang.visitChildren(clang.getTranslationUnitCursor(tu), baseVisitor.Visit, new CXClientData(IntPtr.Zero));
            }

            var visitors = new List<Visitor> {
                new DependencyVisitor(classListInfo, config),
                new AbstractClassVisitor(classListInfo, config),
                new StructVisitor(classListInfo, config),
                new EnumVisitor(classListInfo, config),
                new DelegateVisitor(classListInfo, config),
            };

            foreach (var visitor in visitors)
            {
                foreach (var tu in translationUnits)
                {
                    clang.visitChildren(clang.getTranslationUnitCursor(tu), visitor.Visit, new CXClientData(IntPtr.Zero));
                }
            }

            return classListInfo;
        }

        public Type TranslateType(string type) //BECAUSE WHY THE FUCK WOULD YOU CHECK A TYPE FOR KEYWORDS YOU FUCKING MONG https://referencesource.microsoft.com/#System/compmod/microsoft/csharp/csharpcodeprovider.cs,3295
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
