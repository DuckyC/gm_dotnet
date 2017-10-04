using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeClassGenerator
{
    public class VirtualClassInfo
    {
        public string Name { get; set; }
        public List<string> Parents { get; set; } = new List<string>();
        public List<VirtualMethodInfo> Methods { get; set; } = new List<VirtualMethodInfo>();
    }

    public class VirtualMethodInfo
    {
        public string Name { get; set; }
        public TypeInfo Return { get; set; }
        public List<TypeInfo> Arguments { get; set; } = new List<TypeInfo>();
    }

    public class TypeInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
    }
}
