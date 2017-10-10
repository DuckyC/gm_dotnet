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
using System.Runtime.InteropServices;

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
                var sourceOutput = Path.Combine(config.OutputFolder, subFolder, $"{CleanInterfaceName(interfaceDecl.Name)}.cs");
                WriteCodeFile(sourceOutput, (nspace) => {
                    nspace.Types.Add(GenerateInterface(interfaceDecl));
                });

            }

            if (interfaceList.Structs.Count > 0)
            {
                var sourceOutput = Path.Combine(config.OutputFolder, subFolder, "Structs.cs");
                WriteCodeFile(sourceOutput, (nspace) => {
                    foreach (var declaration in interfaceList.Structs)
                    {
                        nspace.Types.Add(GenerateStruct(declaration));
                    }
                });
            }

            if (interfaceList.Enums.Count > 0)
            {
                var sourceOutput = Path.Combine(config.OutputFolder, subFolder, "Enums.cs");
                WriteCodeFile(sourceOutput, (nspace) => {
                    foreach (var declaration in interfaceList.Enums)
                    {
                        nspace.Types.Add(GenerateEnum(declaration));
                    }
                });
            }

            if (interfaceList.Delegates.Count > 0)
            {
                var sourceOutput = Path.Combine(config.OutputFolder, subFolder, "Delegates.cs");
                WriteCodeFile(sourceOutput, (nspace) => {
                    foreach (var declaration in interfaceList.Delegates)
                    {
                        nspace.Types.Add(GenerateDelegate(declaration));
                    }
                });
            }
        }

        private delegate void AddClasses(CodeNamespace nspace);

        private void WriteCodeFile(string filePath, AddClasses addClasses)
        {
            var provider = new CSharpCodeProvider();
            var compileUnit = new CodeCompileUnit();

            var Classes = new CodeNamespace($"GSharp.Generated.{Configuration.GetSubFolder(interfaceList.Source)}");
            compileUnit.Namespaces.Add(Classes);

            Classes.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            Classes.Imports.Add(new CodeNamespaceImport("System.Runtime.InteropServices"));

            foreach (var name in interfaceList.Namespaces) { Classes.Imports.Add(new CodeNamespaceImport(name)); }

            addClasses(Classes);          

            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
                provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
                tw.Close();
            }
        }

        private CodeTypeDeclaration GenerateDelegate(IDescribeMethod declaration)
        {
            var newType = new CodeTypeDelegate
            {
                Name = declaration.Name,
            };
            newType.CustomAttributes.AddRange(declaration.Attributes.ToArray());

            if (declaration.Returns.Count == 1)
            {
                newType.ReturnType = new CodeTypeReference(declaration.Returns[0].Type);
            }

            foreach (var argumentDeclaration in declaration.Arguments)
            {
                var param = new CodeParameterDeclarationExpression(argumentDeclaration.Type, argumentDeclaration.Name);
                newType.Parameters.Add(param);
            }

            return newType;

        }

        private CodeTypeDeclaration GenerateEnum(IDescribeEnum declaration)
        {
            var newType = new CodeTypeDeclaration
            {
                Name = declaration.Name.Replace("::", "_"),
                IsEnum = true,
                TypeAttributes = TypeAttributes.Public
            };
            //TODO: Comments, we have none yet
            newType.CustomAttributes.AddRange(declaration.Attributes.ToArray());

            foreach (var valueDecl in declaration.Values)
            {
                var value = new CodeMemberField
                {
                    Name = valueDecl.Name,
                    InitExpression = new CodePrimitiveExpression(valueDecl.Value),
                    Attributes = MemberAttributes.Public,
                };
                value.CustomAttributes.AddRange(valueDecl.Attributes.ToArray());
                newType.Members.Add(value);
            }

            return newType;
        }

        private CodeTypeDeclaration GenerateStruct(IDescribeStruct declaration)
        {
            var newType = new CodeTypeDeclaration(declaration.Name)
            {
                IsStruct = true,
                TypeAttributes = TypeAttributes.SequentialLayout | TypeAttributes.Public
            };
            //TODO: Comments, we have none yet
            newType.CustomAttributes.AddRange(declaration.Attributes.ToArray());

            foreach (var fldDecl in declaration.Fields)
            {
                var fld = new CodeMemberField(fldDecl.Type, fldDecl.Name);
                fld.CustomAttributes.AddRange(fldDecl.Attributes.ToArray());
                fld.Attributes = MemberAttributes.Public;
                newType.Members.Add(fld);
            }

            return newType;
        }


        private CodeTypeDeclaration GenerateInterface(IDescribeInterface declaration)
        {
            var newType = new CodeTypeDeclaration(CleanInterfaceName(declaration.Name))
            {
                IsInterface = true,
                TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public
            };
            newType.CustomAttributes.AddRange(declaration.Attributes.ToArray());


            foreach (var parent in declaration.Parents)
            {
                newType.BaseTypes.Add(new CodeTypeReference(parent));
            }

            if (!string.IsNullOrWhiteSpace(declaration.Description))
            {
                newType.Comments.Add(new CodeCommentStatement("<summary>", true));
                var desc = BuildDescription(declaration.Description);
                if (desc != null)
                {
                    newType.Comments.AddRange(desc.ToArray());
                }
                else
                {
                    newType.Comments.Add(new CodeCommentStatement(declaration.Description, true));
                }
                newType.Comments.Add(new CodeCommentStatement("</summary>", true));
            }


            foreach (var methodDeclaration in declaration.Methods)
            {
                var newMethod = new CodeMemberMethod
                {
                    Name = methodDeclaration.Name
                };

                if (!string.IsNullOrWhiteSpace(methodDeclaration.Description))
                {
                    newMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                    newMethod.Comments.Add(new CodeCommentStatement(methodDeclaration.Description, true));
                    newMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                }

                if (methodDeclaration.Returns.Count == 1)
                {
                    newMethod.ReturnType = new CodeTypeReference(methodDeclaration.Returns[0].Type);
                    if (!string.IsNullOrWhiteSpace(methodDeclaration.Returns[0].Description))
                    {
                        newMethod.Comments.Add(new CodeCommentStatement($"<returns>Type: {methodDeclaration.Returns[0].Type} - {methodDeclaration.Returns[0].Description}</returns>", true));
                    }
                }
                else
                {
                    //fucking fix it m8
                }

                foreach (var arg in methodDeclaration.Arguments)
                {
                    var param = new CodeParameterDeclarationExpression(arg.Type, arg.Name);
                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        param.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(OptionalAttribute)));
                        param.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(DefaultValueAttribute), new CodeAttributeArgument(new CodeSnippetExpression(arg.Default))));
                    }
                    param.CustomAttributes.AddRange(arg.Attributes.ToArray());
                    newMethod.Parameters.Add(param);


                    var comment = "";

                    if (!string.IsNullOrWhiteSpace(arg.Description))
                    {
                        comment += arg.Description;
                    }

                    if (!string.IsNullOrWhiteSpace(arg.Default))
                    {
                        if (comment.Length != 0) { comment += " - "; }
                        comment += "Default: " + arg.Default;
                    }
                    newMethod.Comments.Add(new CodeCommentStatement($"<param name='{arg.Name}'>{comment}</param>", true));
                }

                //TODO: Attributes
                newType.Members.Add(newMethod);
            }
            return newType;
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
