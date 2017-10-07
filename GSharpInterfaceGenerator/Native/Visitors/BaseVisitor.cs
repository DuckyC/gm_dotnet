using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSharpInterfaceGenerator.Models;
using ClangSharp;

namespace GSharpInterfaceGenerator.Native.Visitors
{
    public class DependencyVisitor : Visitor
    {
        public DependencyVisitor(VirtualClassListInfo info, Configuration config) : base(info, config) {}

        protected override void VisitChild(VisitorCursor cursor, List<VisitorCursor> parents)
        {
            if (cursor.Kind == CXCursorKind.CXCursor_ClassDecl &&
                config.NativeInterfaces.Any(i => i.Name == cursor.Spelling) &&
                !cursor.IsForwardDeclaration)
            {
                cursor.VisitChildren((childCursor) =>
                {
                    if (childCursor.Kind == CXCursorKind.CXCursor_CXXBaseSpecifier)
                    {
                        if(!config.NativeInterfaces.Any(i=>i.Name == childCursor.Spelling))
                        {
                            config.NativeInterfaces.Add(new NativeInterface {
                                Name = childCursor.Spelling,
                            });
                        }
                    }
                });
            }

        }
    }
}
