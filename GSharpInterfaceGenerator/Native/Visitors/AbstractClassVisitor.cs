using ClangSharp;
using GSharpInterfaceGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSharpInterfaceGenerator.Native.Visitors
{
    public class AbstractClassVisitor : Visitor
    {
        public AbstractClassVisitor(VirtualClassListInfo info, Configuration config) : base(info, config) { }

        protected override void VisitChild(VisitorCursor cursor, List<VisitorCursor> parents)
        {
            if (cursor.Kind == CXCursorKind.CXCursor_ClassDecl)
            {
                if (!config.NativeInterfaces.Any(i => i.Name == cursor.Spelling) || cursor.IsForwardDeclaration)
                {
                    return;
                }

                var classInfo = new IDescribeInterface
                {
                    Name = cursor.Spelling
                };


                cursor.VisitChildren((methodCursor) =>
                {
                    if (methodCursor.Kind == CXCursorKind.CXCursor_CXXMethod ||
                    methodCursor.Kind == CXCursorKind.CXCursor_Destructor ||
                    methodCursor.Kind == CXCursorKind.CXCursor_Constructor)
                    {
                        var methodName = methodCursor.Spelling;
                        if (methodCursor.Kind == CXCursorKind.CXCursor_Constructor) { methodName = "ctor" + methodName; }
                        if (methodCursor.Kind == CXCursorKind.CXCursor_Destructor) { methodName = "dtor" + (methodName.Replace("~", "")); }
                        if (methodCursor.IsVirtual)
                        {
                            var methodInfo = new IDescribeMethod
                            {
                                Name = methodName,
                                Returns = { new IDescribeReturn { Type = methodCursor.ReturnTypeCSharp } }
                            };

                            for (uint index = 0; index < methodCursor.NumArgs; ++index)
                            {
                                var paramSpelling = methodCursor.GetArgSpelling(index);

                                if (string.IsNullOrEmpty(paramSpelling))
                                {
                                    paramSpelling = "param" + index;
                                }

                                var info = new IDescribeArgument
                                {
                                    Type = methodCursor.GetArgType(index),
                                    Name = paramSpelling
                                };


                                var tokens = methodCursor.GetArgCursor(index).GetTokens();
                                var foundEquals = false;
                                for (int i = 0; i < tokens.Length-1; i++)
                                {
                                    var token = tokens[i];
                                    if (token.Kind == CXTokenKind.CXToken_Punctuation && token.Spelling == "="){
                                        foundEquals = true;
                                        continue;
                                    }
                                    if(foundEquals && token.Kind == CXTokenKind.CXToken_Identifier) { info.Default = ""; break; } //probably an enum dont support this(yet(maybe))
                                    if (foundEquals )
                                    {
                                        info.Default+= token.Spelling;
                                    }
                                }


                                methodInfo.Arguments.Add(info);
                            }
                            classInfo.Methods.Add(methodInfo);
                        }
                    }

                    if (methodCursor.Kind == CXCursorKind.CXCursor_CXXBaseSpecifier)
                    {
                        var definitionSpelling = methodCursor.CursorDefinition.Spelling;
                        classInfo.Parents.Add(definitionSpelling);
                    }
                });

                this.info.Interfaces.Add(classInfo);
            }
        }
    }
}
