using ClangSharp;
using GSharpInterfaceGenerator.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharpInterfaceGenerator.Native.Visitors
{
    public class StructVisitor : Visitor
    {
        private int fieldPosition;

        public StructVisitor(VirtualClassListInfo info, Configuration config) : base(info, config) { }

        protected override void VisitChild(VisitorCursor cursor, List<VisitorCursor> parents)
        {
            if (cursor.Kind == CXCursorKind.CXCursor_StructDecl)
            {
                this.fieldPosition = 0;
                if (config.NativeStructs.Any(s => s.Name == cursor.Spelling) &&
                    !this.info.Structs.Any((s) => { return s.Name == cursor.Spelling; }) &&
                    !cursor.IsForwardDeclaration)
                {
                    var structDecl = new IDescribeStruct { Name = cursor.Spelling };

                    cursor.VisitChildren((childCursor) =>
                    {
                        if (childCursor.Kind == CXCursorKind.CXCursor_FieldDecl)
                        {
                            var fieldName = childCursor.Spelling;
                            if (string.IsNullOrEmpty(fieldName))
                            {
                                fieldName = "field" + this.fieldPosition; // what if they have fields called field*? :)
                            }
                            var fieldDecl = new IDescribeField { Name = fieldName };
                            var canonical = childCursor.CanonicalType;
                            switch (childCursor.CanonicalType.kind)
                            {
                                case CXTypeKind.CXType_ConstantArray:
                                    long arraySize = clang.getArraySize(canonical);
                                    var elementType = clang.getCanonicalType(clang.getArrayElementType(canonical));

                                    fieldDecl.Type = elementType.ToPlainType().MakeArrayType();

                                    fieldDecl.Attributes.Add(new CodeAttributeDeclaration(
                                                        new CodeTypeReference(typeof(MarshalAsAttribute)),
                                                        new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(UnmanagedType)), "ByValArray")),
                                                        new CodeAttributeArgument("SizeConst", new CodePrimitiveExpression(arraySize))));

                                    break;
                                case CXTypeKind.CXType_Pointer:
                                    var pointeeType = clang.getCanonicalType(clang.getPointeeType(canonical));
                                    switch (pointeeType.kind)
                                    {
                                        case CXTypeKind.CXType_Char_S:
                                            fieldDecl.Type = typeof(string);
                                            fieldDecl.Attributes.Add(new CodeAttributeDeclaration(
                                                               new CodeTypeReference(typeof(MarshalAsAttribute)),
                                                               new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(UnmanagedType)), "LPTStr"))));

                                            break;
                                        case CXTypeKind.CXType_WChar:
                                            fieldDecl.Type = typeof(string);
                                            fieldDecl.Attributes.Add(new CodeAttributeDeclaration(
                                                               new CodeTypeReference(typeof(MarshalAsAttribute)),
                                                               new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(UnmanagedType)), "LPWStr"))));
                                            break;
                                        default:
                                            fieldDecl.Type = typeof(IntPtr);
                                            break;
                                    }
                                    break;
                                case CXTypeKind.CXType_Record:
                                case CXTypeKind.CXType_Enum:
                                    Console.WriteLine("Struct field type dunno: " + canonical.ToString());
                                    fieldDecl.Type = typeof(IntPtr);
                                    break;
                                default:
                                    fieldDecl.Type = canonical.ToPlainType();
                                    break;
                            }

                            structDecl.Fields.Add(fieldDecl);
                        }
                    });
                    info.Structs.Add(structDecl);
                }
            }
        }
    }
}
