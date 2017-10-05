using GSharpInterfaceGenerator.Models;
using System.Collections.Generic;
using System.Linq;

namespace GSharpInterfaceGenerator.Native
{

    public class VirtualClassListInfo : IDescribeInterfaceList
    {

        public IList<IDescribeInterface> Interfaces { get; set; }

        public TypeSource Source => TypeSource.CPPHeader;

        public IList<string> Namespaces => new List<string> { "System" };
    }

    public class VirtualClassInfo : IDescribeInterface
    {
        public string Name { get; set; }
        public List<string> Parents { get; set; } = new List<string>();
        public List<VirtualMethodInfo> Methods { get; set; } = new List<VirtualMethodInfo>();

        public string Description { get; set; }
        public TypeSource Source => TypeSource.CPPHeader;
        IList<string> IDescribeInterface.Parents => Parents;
        IList<IDescribeMethod> IDescribeInterface.Methods => Methods.Cast<IDescribeMethod>().ToList();
    }

    public class VirtualMethodInfo : IDescribeMethod
    {
        public string Name { get; set; }
        public List<VirtualReturnInfo> Returns { get; set; } = new List<VirtualReturnInfo>();
        public List<VirtualArgumentInfo> Arguments { get; set; } = new List<VirtualArgumentInfo>();
        public string Description { get; set; }
        IList<IDescribeReturn> IDescribeMethod.Returns { get => Returns.Cast<IDescribeReturn>().ToList(); }
        IList<IDescribeArgument> IDescribeMethod.Arguments { get => Arguments.Cast<IDescribeArgument>().ToList(); }
    }

    public class VirtualReturnInfo : IDescribeReturn
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
    public class VirtualArgumentInfo : IDescribeArgument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
    }
}
