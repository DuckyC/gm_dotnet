using LinqToWiki;
using LinqToWiki.Generated;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LuaLibraryGenerator
{
    public class LuaMethodInfo
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public List<Ret> Returns { get; set; } = new List<Ret>();
        public List<Arg> Args { get; set; } = new List<Arg>();
    }

    public class LuaClassInfo
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public List<LuaMethodInfo> Methods { get; set; } = new List<LuaMethodInfo>();
    }

    public static class Program
    {
        static void Main(string[] args)
        {

            var wiki = new Wiki("dotnet", "wiki.garrysmod.com", "api.php");
            var classInfo = wiki.GetLibrary("string");

            var compileUnit = new CodeCompileUnit();

            var LuaLibraries = new CodeNamespace("GSharp.LuaLibraries");
            LuaLibraries.Imports.Add(new CodeNamespaceImport("System"));
            compileUnit.Namespaces.Add(LuaLibraries);
            var playerInterface = Generate(classInfo, "IString");
            LuaLibraries.Types.Add(playerInterface);

            var provider = new CSharpCodeProvider();
            var sourceFile = "LuaLibs.cs";
            using (StreamWriter sw = new StreamWriter(sourceFile, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

                // Generate source code using the code provider.
                provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());

                // Close the output file.
                tw.Close();
            }
        }
        private static CodeTypeDeclaration Generate(LuaClassInfo classInfo, string name)
        {
            var interf = new CodeTypeDeclaration(name);
            interf.IsInterface = true;
            interf.TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public;

            foreach (var methodInfo in classInfo.Methods)
            {
                var newMethod = new CodeMemberMethod();
                newMethod.Name = methodInfo.Name;
                if (methodInfo.Returns.Count == 1)
                {
                    newMethod.ReturnType = new CodeTypeReference(TranslateType(methodInfo.Returns[0].Type));
                }
                else
                {
                    //fucking fix it m8
                }
                foreach (var arg in methodInfo.Args)
                {
                    newMethod.Parameters.Add(new CodeParameterDeclarationExpression(TranslateType(arg.Type), arg.Name));
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


        private static LuaMethodInfo GetMethod(string raw)
        {
            var methodinfo = new LuaMethodInfo();
            var article = new WikiArticle(raw);

            methodinfo.Name = WikiArticle.GetValue(raw, "Name");
            methodinfo.Description = WikiArticle.GetValue(raw, "Description");
            methodinfo.Returns = article.GetReturnValues() ?? new List<Ret> { new Ret { Type = "void", } };
            methodinfo.Args = article.GetArgs() ?? new List<Arg>();

            return methodinfo;
        }

        public static LuaClassInfo GetLibrary(this Wiki wiki, string category)
        {
            var classInfo = new LuaClassInfo();
            classInfo.Name = category;

            var pages = (from cm in wiki.Query.categorymembers() where cm.title == "Category:" + category select cm).Pages.Select(page => page.revisions().Select(r => r.value).FirstOrDefault()).ToEnumerable();
            foreach (var raw in pages)
            {
                classInfo.Methods.Add(GetMethod(raw));
            }

            return classInfo;
        }
    }
}
