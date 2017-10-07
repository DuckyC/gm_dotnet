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
    public class EnumVisitor : Visitor
    {
        public EnumVisitor(VirtualClassListInfo info, Configuration config) : base(info, config) { }

        protected override void VisitChild(VisitorCursor cursor, List<VisitorCursor> Parents)
        {
            if (cursor.Kind == CXCursorKind.CXCursor_EnumDecl)
            {
                var enumSpelling = string.IsNullOrWhiteSpace(cursor.Spelling) ?  Parents.ToNamespace() : cursor.Spelling;
                if (config.NativeEnums.Any(i => i.Name == enumSpelling) && !info.Enums.Any(e => e.Name == enumSpelling))
                {
                    var enumType = cursor.EnumType;
                    if (enumType == typeof(IntPtr)) { enumType = typeof(int); }

                    var enumDecl = new IDescribeEnum { Name = enumSpelling, Type = enumType };

                    cursor.VisitChildren((valueCursor) =>
                    {
                        if(valueCursor.Kind != CXCursorKind.CXCursor_EnumConstantDecl && valueCursor.Kind != CXCursorKind.CXCursor_EnumDecl) { return; }
                        var enumValue = new IDescribeEnumValue { Name = valueCursor.Spelling, Value = valueCursor.EnumValue };
                        enumDecl.Values.Add(enumValue);
                    });

                    info.Enums.Add(enumDecl);
                }
            }
        }
    }
}
