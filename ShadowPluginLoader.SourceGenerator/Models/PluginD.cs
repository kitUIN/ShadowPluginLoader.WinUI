using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowPluginLoader.SourceGenerator.Models
{
    internal class PluginD
    { 
        public string Namespace { get; init; }
        public string Type { get; init; }
        public Dictionary<string,Meta> Properties { get; init; } = new();
        public List<string> Required { get; init; } = [];
    }
    
}
