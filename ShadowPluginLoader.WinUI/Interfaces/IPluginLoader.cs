using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces
{
    /// <summary>
    /// PluginLoader Interface
    /// </summary>
    /// <typeparam name="IPT">Plugin Base Interface, Default: <see cref="IPlugin"/></typeparam>
    public interface IPluginLoader<IPT> where IPT: IPlugin
    {
        /// <summary>
        /// 导入一个插件,从类型T中
        /// </summary>
        void ImportOne<T>() where T : IPT;
        /// <summary>
        /// 从文件夹中导入一个插件
        /// </summary>
        Task ImportOneAsync(string pluginPath);
        /// <summary>
        /// 从插件文件夹导入插件
        /// </summary>
        Task ImportAllAsync(string directoryPath);

        /// <summary>
        /// 获取已经启动的插件
        /// </summary>
        IList<IPT> GetEnabledPlugins();
        /// <summary>
        /// 获取所有插件
        /// </summary>
        IList<IPT> GetPlugins();
        /// <summary>
        /// 获取插件
        /// </summary>
        IPT? GetPlugin(string id);
        /// <summary>
        /// 获取已经启动的插件
        /// </summary>
        IPT? GetEnabledPlugin(string id);

        /// <summary>
        /// 插件是否启用
        /// </summary>
        bool? IsEnabled(string id);

        /// <summary>
        /// 启用插件
        /// </summary>
        void PluginEnabled(string id);

        /// <summary>
        /// 禁用插件
        /// </summary>
        void PluginDisabled(string id);
        /// <summary>
        /// 删除插件
        /// </summary>
        void Delete(string id);
    }
}
