using ClangSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeClassGenerator
{
    public class AbstractClassVisitor
    {
        private List<VirtualClassInfo> classes;
        private HashSet<string> wantedClasses;

        public AbstractClassVisitor(ref List<VirtualClassInfo> classes, HashSet<string> wantedClasses)
        {
            this.classes = classes;
            this.wantedClasses = wantedClasses;
        }

        private static string TypeTranslate(CXType type)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    return type.IsPtrToConstChar() ? "string" : nameof(IntPtr); // const char* gets special treatment
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    return clang.getTypeSpelling(type).ToString();
                case CXTypeKind.CXType_IncompleteArray:
                    return TypeTranslate(clang.getArrayElementType(type)) + "[]";
                //case CXTypeKind.CXType_Elaborated:
                case CXTypeKind.CXType_Unexposed: // Often these are enums and canonical type gets you the enum spelling
                    var canonical = clang.getCanonicalType(type);
                    // unexposed decl which turns into a function proto seems to be an un-typedef'd fn pointer
                    if (canonical.kind == CXTypeKind.CXType_FunctionProto) { return nameof(IntPtr); }
                    return clang.getTypeSpelling(canonical).ToString();
                default:
                    return clang.getCanonicalType(type).ToPlainTypeString();
            }
        }

        public static TypeInfo ArgumentHelper(CXType functionType, CXCursor paramCursor, uint index)
        {
            var info = new TypeInfo();
            var type = clang.getArgType(functionType, index);
            var cursorType = clang.getCursorType(paramCursor);

            var spelling = clang.getCursorSpelling(paramCursor).ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            info.Type = TypeTranslate(type);
            info.Name = spelling;
            return info;
        }

        public CXChildVisitResult VisitClass(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader()) { return CXChildVisitResult.CXChildVisit_Continue; }
            CXCursorKind curKind = clang.getCursorKind(cursor);

            if (curKind == CXCursorKind.CXCursor_ClassDecl)
            {
                var spelling = clang.getCursorSpelling(cursor).ToString();

                if (!this.wantedClasses.Contains(spelling))
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                var classInfo = new VirtualClassInfo();
                classInfo.Name = spelling;

                CXCursorVisitor visitMethod = (CXCursor methodCursor, CXCursor methodParent, IntPtr methodData) =>
                {

                    CXCursorKind methodCurKind = clang.getCursorKind(methodCursor);
                    var methodSpelling = methodCursor.ToString();

                    if (methodCurKind == CXCursorKind.CXCursor_CXXMethod || methodCurKind == CXCursorKind.CXCursor_Destructor || methodCurKind == CXCursorKind.CXCursor_Constructor)
                    {
                        var methodName = methodSpelling;
                        if (methodCurKind == CXCursorKind.CXCursor_Constructor) { methodName = "ctor" + methodName; }
                        if (methodCurKind == CXCursorKind.CXCursor_Destructor) { methodName = "dtor" + (methodName.Replace("~", "")); }
                        if (clang.CXXMethod_isVirtual(methodCursor) == 1)
                        {

                            var methodType = clang.getCursorType(methodCursor);
                            var resultType = clang.getCursorResultType(methodCursor);
                            var translatedResultType = TypeTranslate(resultType);

                            var methodInfo = new VirtualMethodInfo
                            {
                                Name = methodName,
                                Return = new TypeInfo { Type = translatedResultType }
                            };


                            int numArgTypes = clang.getNumArgTypes(methodType);
                            for (uint i = 0; i < numArgTypes; ++i)
                            {
                                methodInfo.Arguments.Add(ArgumentHelper(methodType, clang.Cursor_getArgument(methodCursor, i), i));
                            }

                            classInfo.Methods.Add(methodInfo);
                        }
                        else
                        {
                            classInfo.Methods.Add(new VirtualMethodInfo
                            {
                                Return = new TypeInfo { Type = "void" },
                                Name = methodName+"NONVIRTUALINLINE",
                            });
                        }

                    }
                    else if (methodCurKind == CXCursorKind.CXCursor_CXXBaseSpecifier)
                    {
                        var baseCursor = clang.getCursorReferenced(methodCursor);
                        var baseSpelling = baseCursor.ToString();
                        this.wantedClasses.Add(baseSpelling);
                        this.VisitClass(baseCursor, cursor, IntPtr.Zero);
                        classInfo.Parents.Add(baseSpelling);
                    }
                    return CXChildVisitResult.CXChildVisit_Continue;
                };

                clang.visitChildren(cursor, visitMethod, new CXClientData(IntPtr.Zero));
                this.classes.Add(classInfo);
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}
