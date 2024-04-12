using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Models
{
    public class SortPluginData
    {
        public string[] Requires { get;}
        public string Id { get; }
        public Type? PluginType { get; set; }
        public SortPluginData(string id ,string[] req) 
        {
            Id = id;
            Requires = req;
        }
    }
}
