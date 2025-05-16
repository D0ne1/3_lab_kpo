    using System;

    namespace PluginContracts
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class PluginInfoAttribute : Attribute
        {
            public string Name { get; }
            public string Author { get; }
            public string Version { get; }

            public PluginInfoAttribute(string name, string author, string version)
            {
                Name = name;
                Author = author;
                Version = version;
            }
        }
    }
