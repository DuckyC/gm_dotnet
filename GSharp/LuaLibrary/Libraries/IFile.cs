using GSharp.Attributes;

namespace GSharp.LuaLibrary.Libraries
{

    public class IFileFindReturnType
    {
        /// <summary>
        /// A table of found files
        /// </summary>
        [ReturnIndex(0)]
        public string[] Files { get; set; }

        /// <summary>
        /// A table of found directories
        /// </summary>
        [ReturnIndex(1)]
        public string[] Directories { get; set; }
    }

    public interface IFile
    {
        void Append(string name, string content);
        void CreateDir(string name);
        void Delete(string name);
        void Exists(string name, string path);
        /// <summary>
        /// Returns a list of files and directories inside a single folder.
        /// </summary>
        /// <param name="name">The wildcard to search for. "models/*.mdl" will list .mdl files in the models/ folder.</param>
        /// <param name="path">The path to look for the files and directories in.<see cref="Paths"/> </param>
        /// <param name="sorting">The sorting to be used</param>
        /// <returns></returns>
        IFileFindReturnType Find(string name, string path, string sorting = "nameasc");
        void IsDir(string name, string path);
        void Open(string fileName, string fileMode, string path);
        void Read(string fileName, string path = "DATA");
        void Size(string fileName, string path);
        void Time(string path, string gamePath);
        void Write(string fileName, string content);
    }
}
