using GSharp.Attributes;

namespace GSharp.Native.Classes
{

    [ModuleName("FileSystem_Stdio")]
    [InterfaceVersion("VFileSystem022")]
    public interface IFileSystem
    {
        [VTableSlot(63)]
        void PrintSearchPaths();
    }
}
