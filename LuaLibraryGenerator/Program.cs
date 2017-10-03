using LuaLibraryGenerator.WikiDefinitions;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LuaLibraryGenerator
{

    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        static void Main(string[] args)
        {
            var wiki = new Wiki(new Uri("http://wiki.garrysmod.com/"));

            var libraries = new List<string> { "Global", "file", "string", "math" };

            var path = "./LuaLibraries";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            foreach (var libName in libraries)
            {
                var LibraryInfo = wiki.FetchLibraryInfo(libName);
                LibraryInfo.Location = libName == "Global" ? "_G" : libName;

                var compileUnit = new CodeCompileUnit();
                var LuaLibraries = new CodeNamespace("GSharp.LuaLibraries");
                //LuaLibraries.Imports.Add(new CodeNamespaceImport(nameof(System)));
                LuaLibraries.Imports.Add(new CodeNamespaceImport($"{nameof(System)}.{nameof(System.ComponentModel)}"));
                LuaLibraries.Imports.Add(new CodeNamespaceImport($"{nameof(GSharp)}.{nameof(GSharp.Attributes)}"));
                compileUnit.Namespaces.Add(LuaLibraries);
                var interfaceName = $"I{FirstCharToUpper(libName)}";
                var libraryInterface = Generate(LibraryInfo, interfaceName);
                libraryInterface.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(GSharp.Attributes.LuaLibraryLocationAttribute), new CodeAttributeArgument(new CodePrimitiveExpression(LibraryInfo.Location))));
                LuaLibraries.Types.Add(libraryInterface);

                var provider = new CSharpCodeProvider();


                var sourceFile = Path.Combine(path, $"{interfaceName}.cs");
                using (StreamWriter sw = new StreamWriter(sourceFile, false))
                {
                    IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

                    // Generate source code using the code provider.
                    provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());

                    // Close the output file.
                    tw.Close();
                }
            }


        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        private static CodeTypeDeclaration Generate(LuaLibraryInfo classInfo, string name)
        {
            var interf = new CodeTypeDeclaration(name);
            interf.IsInterface = true;
            interf.TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public;

            interf.Comments.Add(new CodeCommentStatement("<summary>", true));
            var desc = WikiParse.BuildDescription(classInfo.Description);
            if (desc != null)
            {
                interf.Comments.AddRange(desc.ToArray());
            }
            else
            {
                interf.Comments.Add(new CodeCommentStatement(classInfo.Description, true));
            }
            interf.Comments.Add(new CodeCommentStatement("</summary>", true));


            foreach (var methodInfo in classInfo.Methods)
            {
                var newMethod = new CodeMemberMethod();
                newMethod.Name = methodInfo.Name;

                newMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                newMethod.Comments.Add(new CodeCommentStatement(methodInfo.Description, true));
                newMethod.Comments.Add(new CodeCommentStatement("</summary>", true));

                if (methodInfo.Returns.Count == 1)
                {
                    newMethod.ReturnType = new CodeTypeReference(TranslateType(methodInfo.Returns[0].Type));
                    newMethod.Comments.Add(new CodeCommentStatement($"<returns>Type: {methodInfo.Returns[0].Type} - {methodInfo.Returns[0].Desc}</returns>", true));
                }
                else
                {
                    //fucking fix it m8
                }
                foreach (var arg in methodInfo.Args)
                {
                    var param = new CodeParameterDeclarationExpression(TranslateType(arg.Type), arg.Name);
                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        param.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(DefaultValueAttribute), new CodeAttributeArgument(new CodeSnippetExpression(arg.Default))));
                    }
                    newMethod.Parameters.Add(param);


                    newMethod.Comments.Add(new CodeCommentStatement($"<param name='{arg.Name}'>{arg.Desc}</param>", true));
                }


                interf.Members.Add(newMethod);
            }
            return interf;
        }

        private static Type TranslateType(string luaName)
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
