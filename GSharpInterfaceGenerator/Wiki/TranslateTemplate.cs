using GSharpInterfaceGenerator.Models;
using System;
using System.Collections.Generic;

namespace GSharpInterfaceGenerator.Wiki
{
    public abstract class Template
    {
        public static string Title { get; }
    }

    public class FuncTemplate : Template
    {
        public static new string Title { get; } = "Func";
        public string Description { get; set; }
        public string Realm { get; set; }
        public string IsClass { get; set; }
    }

    public class ArgTemplate : Template, IDescribeArgument
    {
        public static new string Title { get; } = "Arg";
        public string Type { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Default { get; set; }

        public string Description => Desc;
    }

    public class RetTemplate : Template, IDescribeReturn
    {
        public static new string Title { get; } = "Ret";
        public string Type { get; set; }
        public string Desc { get; set; }

        public string Description => Desc;
    }

    public class ExampleTemplate : Template
    {
        public static new string Title { get; } = "Example";
        public string Description { get; set; }
        public string Code { get; set; }
        public string Output { get; set; }
    }

    public static class TranslateTemplate
    {
        private static List<Type> templateTypes = new List<Type> { typeof(FuncTemplate), typeof(ArgTemplate), typeof(RetTemplate), typeof(ExampleTemplate) };

        public static Template Translate(XTemplate xtemplate)
        {
            foreach (var type in templateTypes)
            {
                var titleProp = type.GetProperty(nameof(Template.Title));
                var templateTitle = titleProp.GetValue(null) as string;
                if(xtemplate.Title == templateTitle)
                {
                    var instance = Activator.CreateInstance(type);
                    foreach (var part in xtemplate.Parts)
                    {
                        foreach (var prop in type.GetProperties())
                        {
                            if(prop.Name.ToLowerInvariant() == part.Name.ToLowerInvariant())
                            {
                                prop.SetValue(instance, part.Value);
                            }
                        }
                    }
                    return instance as Template;
                }
            }
            throw new Exception("Unsupported template type: " + xtemplate.Title );
        }
    }
}
