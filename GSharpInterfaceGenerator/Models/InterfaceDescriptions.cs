using System.Collections.Generic;

namespace GSharpInterfaceGenerator.Models
{
    public interface IDescribeInterfaceList
    {
        IList<IDescribeInterface> Interfaces { get; }
        TypeSource Source { get; }
        IList<string> Namespaces { get; }
    }

    public interface IDescribeInterface
    {
        string Name { get; }
        IList<string> Parents { get; }
        IList<IDescribeMethod> Methods { get; }
        string Description { get; }   
    }

    public interface IDescribeMethod
    {
        string Name { get; }
        IList<IDescribeReturn> Returns { get; }
        IList<IDescribeArgument> Arguments { get; }
        string Description { get; }
    }

    public interface IDescribeReturn
    {
        string Type { get; }
        string Description { get; }
    }

    public interface IDescribeArgument
    {
        string Name { get; }
        string Type { get; }
        string Default { get; }
        string Description { get; }
    }
}
