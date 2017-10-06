using Microsoft.CSharp;
using GSharpInterfaceGenerator.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GSharpInterfaceGenerator
{
    public class InterfaceBuilder
    {
        private IDescribeInterfaceList interfaceList;
        private IProvideInterfaces provider;
        private Configuration config;

        public InterfaceBuilder(IDescribeInterfaceList interfaceList, IProvideInterfaces provider, Configuration config)
        {
            this.interfaceList = interfaceList;
            this.provider = provider;
            this.config = config;
        }

        public void BuildInterfaces()
        {
            var subFolder = Configuration.GetSubFolder(interfaceList.Source);
            foreach (var interfaceDecl in interfaceList.Interfaces)
            {
                var provider = new CSharpCodeProvider();
                var compileUnit = new CodeCompileUnit();

                var Classes = new CodeNamespace($"GSharp.Generated.{Configuration.GetSubFolder(interfaceList.Source)}");
                compileUnit.Namespaces.Add(Classes);

                Classes.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
                foreach (var name in interfaceList.Namespaces) { Classes.Imports.Add(new CodeNamespaceImport(name)); }

                var classInterface = Generate(interfaceDecl, interfaceList.Source);
                Classes.Types.Add(classInterface);

                var sourceOutput = Path.Combine(config.OutputFolder, subFolder, $"{CleanInterfaceName(interfaceDecl.Name)}.cs");
                using (StreamWriter sw = new StreamWriter(sourceOutput, false))
                {
                    IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
                    provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
                    tw.Close();
                }
            }
        }

        private CodeTypeDeclaration Generate(IDescribeInterface interfaceDecl, TypeSource source)
        {
            var interf = new CodeTypeDeclaration(CleanInterfaceName(interfaceDecl.Name));
            interf.IsInterface = true;
            interf.TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public;
            //TODO: Attributes
            if (!string.IsNullOrWhiteSpace(interfaceDecl.Description))
            {
                interf.Comments.Add(new CodeCommentStatement("<summary>", true));
                var desc = BuildDescription(interfaceDecl.Description);
                if (desc != null)
                {
                    interf.Comments.AddRange(desc.ToArray());
                }
                else
                {
                    interf.Comments.Add(new CodeCommentStatement(interfaceDecl.Description, true));
                }
                interf.Comments.Add(new CodeCommentStatement("</summary>", true));
            }


            foreach (var methodInfo in interfaceDecl.Methods)
            {
                var newMethod = new CodeMemberMethod();
                newMethod.Name = methodInfo.Name;

                if (!string.IsNullOrWhiteSpace(methodInfo.Description))
                {
                    newMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    newMethod.Comments.Add(new CodeCommentStatement(methodInfo.Description, true));
                    newMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                }

                if (methodInfo.Returns.Count == 1)
                {
                    newMethod.ReturnType = new CodeTypeReference(provider.TranslateType(methodInfo.Returns[0].Type));
                    if (!string.IsNullOrWhiteSpace(methodInfo.Returns[0].Description))
                    {
                        newMethod.Comments.Add(new CodeCommentStatement($"<returns>Type: {methodInfo.Returns[0].Type} - {methodInfo.Returns[0].Description}</returns>", true));
                    }
                }
                else
                {
                    //fucking fix it m8
                }

                foreach (var arg in methodInfo.Arguments)
                {
                    var param = new CodeParameterDeclarationExpression(provider.TranslateType(arg.Type), arg.Name);
                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        param.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(DefaultValueAttribute), new CodeAttributeArgument(new CodePrimitiveExpression(null))));
                    }
                    newMethod.Parameters.Add(param);

                    var comment = "";

                    if (!string.IsNullOrWhiteSpace(arg.Description))
                    {
                        comment += arg.Description;
                    }

                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        if(comment.Length != 0 ) { comment += " - "; }
                        comment += "Default: " + arg.Default;
                    }
                    newMethod.Comments.Add(new CodeCommentStatement($"<param name='{arg.Name}'>{comment}</param>", true));
                }

                //TODO: Attributes
                interf.Members.Add(newMethod);
            }
            return interf;
        }

        private string CleanInterfaceName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("ARGH!");

            name = name.First().ToString().ToUpper() + name.Substring(1);

            if (name[0] != 'I')
                name = "I" + name;

            return name;
        }

        private List<CodeCommentStatement> BuildDescription(string description)
        {
            if (description.Contains('*'))
            {
                var list = new List<CodeCommentStatement>();
                var sections = description.Split('*');

                var topText = "";
                var listHeader = "";
                var topSections = sections[0].Split(Environment.NewLine.ToCharArray());
                for (int i = 0; i < topSections.Length; i++)
                {
                    if (i == topSections.Length - 1 || string.IsNullOrWhiteSpace(topSections[i + 1]))
                    {
                        listHeader += topSections[i];
                    }
                    else
                    {
                        topText += topSections[i];
                    }
                }

                list.Add(new CodeCommentStatement(topText, true));
                list.Add(new CodeCommentStatement("<list type='bullet'>", true));
                list.Add(new CodeCommentStatement($"<listheader><description>{listHeader}</description></listheader>", true));

                for (int i = 1; i < sections.Length - 1; i++)
                {
                    list.Add(new CodeCommentStatement($"<item><description>{sections[i]}</description></item>", true));
                }

                var bottomSections = sections[sections.Length - 1].Split(Environment.NewLine.ToCharArray());

                list.Add(new CodeCommentStatement($"<item><description>{bottomSections[0]}</description></item>", true));
                list.Add(new CodeCommentStatement("</list>", true));

                list.AddRange(bottomSections.Skip(1).Select(s => new CodeCommentStatement(s, true)).ToArray());


                return list;
            }

            return null;
        }
    }
}
