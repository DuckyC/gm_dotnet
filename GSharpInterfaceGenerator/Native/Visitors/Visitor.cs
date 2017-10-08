using ClangSharp;
using GSharpInterfaceGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSharpInterfaceGenerator.Native.Visitors
{
    public abstract class Visitor
    {
        public delegate void CursorVisitor(VisitorCursor cursor);

        public class VisitorCursor
        {
            public CXCursor Cursor { get; private set; }
            public VisitorCursor(CXCursor cursor)
            {
                Cursor = cursor;
            }

            public string Spelling => clang.getCursorSpelling(Cursor).ToString();
            public CXType Type => clang.getCursorType(Cursor);
            public CXType CanonicalType => clang.getCanonicalType(Type);
            public CXCursorKind Kind => clang.getCursorKind(Cursor);
            public string Location => Cursor.GetCursorLocation();
            public Type TypeCSharp => Type.ToPlainType();
            public bool IsForwardDeclaration => Cursor.IsForwardDeclaration();
            public bool IsInSystemHeader => Cursor.IsInSystemHeader();

            public Type EnumType => clang.getEnumDeclIntegerType(Cursor).ToPlainType();
            public long EnumValue => clang.getEnumConstantDeclValue(Cursor);

            public bool IsVirtual => clang.CXXMethod_isVirtual(Cursor) == 1;
            public bool IsPureVirtual => clang.CXXMethod_isPureVirtual(Cursor) == 1;
            public CXType ReturnType => clang.getCursorResultType(Cursor);
            public Type ReturnTypeCSharp => ReturnType.ToPlainType();
            public int NumArgs => clang.getNumArgTypes(Type);

            public VisitorCursor CursorReferenced => new VisitorCursor(clang.getCursorReferenced(Cursor));
            public VisitorCursor CursorDefinition => new VisitorCursor(clang.getCursorDefinition(Cursor));

            public string GetArgSpelling(uint index)
            {
                return clang.getCursorSpelling(clang.Cursor_getArgument(Cursor, index)).ToString();
            }

            public Type GetArgType(uint index)
            {
                return clang.getArgType(Type, index).ToPlainType();
            }

            public void VisitChildren(CursorVisitor visitor)
            {
                clang.visitChildren(Cursor, (CXCursor cursor, CXCursor parent, IntPtr data) =>
                {
                    visitor(new VisitorCursor(cursor));
                    return CXChildVisitResult.CXChildVisit_Recurse;
                }, new CXClientData(IntPtr.Zero));
            }

            public static implicit operator VisitorCursor(CXCursor d)
            {
                return new VisitorCursor(d);
            }

            public static explicit operator CXCursor(VisitorCursor d)
            {
                return d.Cursor;
            }

            public override string ToString()
            {
                return Spelling + " - " + Location;
            }
        }

        protected VirtualClassListInfo info;
        protected Configuration config;


        public Visitor(VirtualClassListInfo info, Configuration config)
        {
            this.info = info;
            this.config = config;
        }

        protected abstract void VisitChild(VisitorCursor cursor, List<VisitorCursor> parents);


        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader()) { return CXChildVisitResult.CXChildVisit_Continue; }

            var parents = new List<CXCursor> { parent };
            VisitInternal(cursor, parent, parents.ShallowCopy());

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private void VisitInternal(CXCursor cursor, CXCursor parent, List<CXCursor> parents)
        {
            VisitChild(cursor, parents.ShallowCopy().Select(c => new VisitorCursor(c)).ToList());
            clang.visitChildren(cursor, (CXCursor ccursor, CXCursor cparent, IntPtr cdata) =>
            {
                var newParents = parents.ShallowCopy();
                newParents.Add(cursor);
                VisitInternal(ccursor, cursor, newParents);
                return CXChildVisitResult.CXChildVisit_Continue;
            }, new CXClientData(IntPtr.Zero));
        }
    }
}
