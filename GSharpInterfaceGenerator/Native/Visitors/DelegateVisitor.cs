using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSharpInterfaceGenerator.Models;
using ClangSharp;
using System.Runtime.InteropServices;
using System.CodeDom;

namespace GSharpInterfaceGenerator.Native.Visitors
{
    public class DelegateVisitor : Visitor
    {
        public DelegateVisitor(VirtualClassListInfo info, Configuration config) : base(info, config) { }

        protected override void VisitChild(VisitorCursor cursor, List<VisitorCursor> parents)
        {
            if (cursor.Kind == CXCursorKind.CXCursor_TypedefDecl)
            {
                if (!config.NativeDelegates.Any(i => i.Name == cursor.Spelling))
                {
                    CXType type = clang.getCanonicalType(clang.getTypedefDeclUnderlyingType(cursor.Cursor));
                    var pointee = clang.getPointeeType(type);
                    if (pointee.kind == CXTypeKind.CXType_FunctionProto)
                    {
                        var callingConv = clang.getFunctionTypeCallingConv(pointee);
                        var returnType = clang.getResultType(pointee);
                        var name = cursor.Spelling;

                        var delegateDecl = new IDescribeMethod
                        {
                            Name = cursor.Spelling,
                            Returns = new List<IDescribeReturn> { new IDescribeReturn { Type = returnType.ToPlainType() } },
                        };


                        CodeAttributeArgument arg;
                        switch (callingConv)
                        {
                            case CXCallingConv.CXCallingConv_X86StdCall:
                            case CXCallingConv.CXCallingConv_X86_64Win64:
                                arg = new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CallingConvention)), "StdCall"));
                                break;
                            default:
                                arg = new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CallingConvention)), "Cdecl"));
                                break;
                        }


                        uint parmCounter = 0;
                        cursor.VisitChildren((childCursor) =>
                        {
                            if (childCursor.Kind == CXCursorKind.CXCursor_ParmDecl)
                            {
                                var paramType = clang.getArgType(pointee, parmCounter);
                                var paramSpelling = childCursor.Spelling;
                                if (string.IsNullOrEmpty(paramSpelling))
                                {
                                    paramSpelling = "param" + parmCounter;
                                }

                                delegateDecl.Arguments.Add(new IDescribeArgument { Name = paramSpelling, Type = paramType.ToPlainType() });

                                parmCounter++;
                            }
                        });

                        if (delegateDecl.Arguments.Any(a => a.Type == typeof(string)) || delegateDecl.Returns.Any(a => a.Type == typeof(string)))
                        {
                            delegateDecl.Attributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(UnmanagedFunctionPointerAttribute)), arg,
                                new CodeAttributeArgument(nameof(UnmanagedFunctionPointerAttribute.CharSet), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CharSet)), nameof(CharSet.Ansi)))));
                        }
                        else
                        {
                            delegateDecl.Attributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(UnmanagedFunctionPointerAttribute)), arg));
                        }

                        info.Delegates.Add(delegateDecl);
                    }
                }
            }
        }
    }
}
