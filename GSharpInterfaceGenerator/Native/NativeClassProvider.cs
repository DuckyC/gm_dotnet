using ClangSharp;
using GSharpInterfaceGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GSharpInterfaceGenerator.Native
{
    public class NativeClassProvider : IProvideInterfaces
    {
        public IDescribeInterfaceList MakeInterfaces(Configuration config)
        {
            var files = new List<string>();
            var WantedClassesHashSet = new HashSet<string> { };

            foreach (var type in config.WantedTypes)
            {
                if (string.IsNullOrWhiteSpace(type.HeaderFile)) { continue; }
                files.Add(type.HeaderFile);
                WantedClassesHashSet.Add(type.Name);
            }

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
            var classListInfo = new VirtualClassListInfo();
            classListInfo.Interfaces = classList.Cast<IDescribeInterface>().ToList();
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
