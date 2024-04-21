using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Models
{
    /// <summary>
    /// SortPluginData
    /// </summary>
    public class SortPluginData
    {
        /// <summary>
        /// Plugin Dependencies
        /// </summary>
        public string[] Dependencies { get;}
        /// <summary>
        /// Plugin Id
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Plugin Type
        /// </summary>
        public Type? PluginType { get; set; }
        /// <summary>
        /// SortPluginData
        /// </summary>
        /// <param name="id">Plugin Id</param>
        /// <param name="req">Dependencies</param>
        public SortPluginData(string id ,string[] req) 
        {
            Id = id;
            Dependencies = req;
        }
    }
}
