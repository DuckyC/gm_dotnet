using GSharp.Attributes;

namespace GSharp.NativeClasses
{
    [InterfaceVersion("VFileSystem022")]
    public interface IFileSystem
    {
        [VTableSlot(63)]
        void PrintSearchPaths();
    }
}
