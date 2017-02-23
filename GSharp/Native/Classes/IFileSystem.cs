using GSharp.Attributes;

namespace GSharp.Native.Classes
{
    [InterfaceVersion("VFileSystem022")]
    public interface IFileSystem
    {
        [VTableSlot(63)]
        void PrintSearchPaths();
    }
}
