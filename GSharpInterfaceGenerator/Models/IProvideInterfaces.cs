﻿using System;

namespace GSharpInterfaceGenerator.Models
{
    public interface IProvideInterfaces
    {
        IDescribeInterfaceList MakeInterfaces(Configuration config);
    }
}
