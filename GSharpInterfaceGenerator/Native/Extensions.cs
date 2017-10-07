using ClangSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GSharpInterfaceGenerator.Native.Visitors.Visitor;

namespace GSharpInterfaceGenerator.Native
{
    internal static class Extensions
    {

        public static string ToString(this CXString str)
        {
            string retval = clang.getCString(str);
            clang.disposeString(str);
            return retval;
        }

        public static string ToString(this CXType o)
        {
            return clang.getTypeSpelling(o).ToString();
        }

        public static string ToString(this CXCursor o)
        {
            return clang.getCursorSpelling(o).ToString();
        }

        public static string ToString(this CXDiagnostic o)
        {
            return clang.getDiagnosticSpelling(o).ToString();
        }

        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
        }

        public static List<T> ShallowCopy<T>(this List<T> original)
        {
            var outArr = new T[original.Count];
            original.CopyTo(outArr, 0);
            return outArr.ToList<T>();
        }

        public static bool IsPtrToConstChar(this CXType type)
        {
            var pointee = clang.getPointeeType(type);

            if (clang.isConstQualifiedType(pointee) != 0)
            {
                switch (pointee.kind)
                {
                    case CXTypeKind.CXType_Char_S:
                        return true;
                }
            }

            return false;
        }

        public static bool IsForwardDeclaration(this CXCursor cursor)
        {
            return clang.equalCursors(clang.getCursorDefinition(cursor), cursor) == 0;
        }

        public static string GetFullCursorLocation(this CXCursor cursor)
        {
            var loc = clang.getCursorLocation(cursor);
            var file = new CXFile();
            uint line;
            uint column;
            uint offset;

            clang.getSpellingLocation(loc, out file, out line, out column, out offset);
            var fileName = clang.getFileName(file);

            return $"{fileName} Line: {line} Column:{column}";
        }

        public static string GetCursorLocation(this CXCursor cursor)
        {
            var loc = clang.getCursorLocation(cursor);
            var file = new CXFile();
            uint line;
            uint column;
            uint offset;

            clang.getSpellingLocation(loc, out file, out line, out column, out offset);
            var fileName = clang.getFileName(file).ToString();
            fileName = Path.GetFileName(fileName);

            return $"{fileName} Line: {line} Column:{column}";
        }

        public static string ToNamespace(this List<VisitorCursor> parents)
        {
            if (parents.Count == 0) { return ""; }
            var Namespace = "";

            foreach (var parent in parents)
            {
                if (parent.Kind == CXCursorKind.CXCursor_Namespace)
                {
                    Namespace += (Namespace == "" ? "" : "::") + parent.Spelling;
                }
            }
            return Namespace;
        }

        public static Type ToPlainType(this CXType type)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    return type.IsPtrToConstChar() ? typeof(string) : typeof(IntPtr); // const char* gets special treatment
                case CXTypeKind.CXType_Bool:
                    return typeof(bool);
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                    return typeof(byte);
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                    return typeof(sbyte);
                case CXTypeKind.CXType_UShort:
                    return typeof(ushort);
                case CXTypeKind.CXType_Short:
                    return typeof(short);
                case CXTypeKind.CXType_Float:
                    return typeof(float);
                case CXTypeKind.CXType_Double:
                    return typeof(double);
                case CXTypeKind.CXType_Int:
                    return typeof(int);
                case CXTypeKind.CXType_UInt:
                    return typeof(uint);
                case CXTypeKind.CXType_NullPtr: // ugh, what else can I do?
                    return typeof(IntPtr);
                case CXTypeKind.CXType_Long:
                    return typeof(int);
                case CXTypeKind.CXType_ULong:
                    return typeof(int);
                case CXTypeKind.CXType_LongLong:
                    return typeof(long);
                case CXTypeKind.CXType_ULongLong:
                    return typeof(ulong);
                case CXTypeKind.CXType_Void:
                    return typeof(void);
                //case CXTypeKind.CXType_IncompleteArray: //TODO: Fix incompleteArray
                //    return TypeTranslate(clang.getArrayElementType(type)) + "[]";
                case CXTypeKind.CXType_Unexposed:
                    var canonical = clang.getCanonicalType(type);
                    if (canonical.kind == CXTypeKind.CXType_Unexposed)
                    {
                        Console.WriteLine("ToType Unexposed:" + canonical);
                        return typeof(IntPtr);
                    }
                    return canonical.ToPlainType();
                default:
                    return typeof(IntPtr);
            }
        }
    }
}
