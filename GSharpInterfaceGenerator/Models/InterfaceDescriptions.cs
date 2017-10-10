using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace GSharpInterfaceGenerator.Models
{
    public class IDescribeInterfaceList
    {
        public virtual List<IDescribeInterface> Interfaces { get; set; } = new List<IDescribeInterface>();
        public virtual List<IDescribeMethod> Delegates { get; set; } = new List<IDescribeMethod>();
        public virtual List<IDescribeStruct> Structs { get; set; } = new List<IDescribeStruct>();
        public virtual List<IDescribeEnum> Enums { get; set; } = new List<IDescribeEnum>();
        public virtual List<string> Namespaces { get; set; } = new List<string>();
        public virtual TypeSource Source { get; set; }
    }


    public class IDescribeStruct
    {
        public virtual string Name { get; set; }
        public virtual List<IDescribeField> Fields { get; set; } = new List<IDescribeField>();
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class IDescribeField
    {
        public virtual string Name { get; set; }
        public virtual Type Type { get; set; }
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Type + " " + Name;
        }

    }

    public class IDescribeEnum
    {
        public virtual string Name { get; set; }
        public virtual Type Type { get; set; }
        public virtual List<IDescribeEnumValue> Values { get; set; } = new List<IDescribeEnumValue>();
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class IDescribeEnumValue
    {
        public virtual string Name { get; set; }
        public virtual long Value { get; set; }
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Name + ": " + Value;
        }
    }
    

    public class IDescribeInterface
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual List<string> Parents { get; set; } = new List<string>();
        public virtual List<IDescribeMethod> Methods { get; set; } = new List<IDescribeMethod>();
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class IDescribeMethod
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual List<IDescribeReturn> Returns { get; set; } = new List<IDescribeReturn>();
        public virtual List<IDescribeArgument> Arguments { get; set; } = new List<IDescribeArgument>();
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return String.Join(" ", Returns.Select(r=>r.ToString()))+" " + Name + "(" + String.Join(", ", Arguments.Select(a=>a.ToString())) + ")";
        }
    }

    public class IDescribeReturn
    {
        public virtual Type Type { get; set; }
        public virtual string Description { get; set; }
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Type.ToString().Replace("System.", "");
        }
    }

    public class IDescribeArgument
    {
        public virtual string Name { get; set; }
        public virtual Type Type { get; set; }
        public virtual string Default { get; set; } //TODO: Use attributes directly instead
        public virtual string Description { get; set; }
        public virtual List<CodeAttributeDeclaration> Attributes { get; set; } = new List<CodeAttributeDeclaration>();

        public override string ToString()
        {
            return Type.ToString().Replace("System.", "") + " " + Name + (string.IsNullOrWhiteSpace(Default) ? "" : " = " + Default);
        }
    }
}
