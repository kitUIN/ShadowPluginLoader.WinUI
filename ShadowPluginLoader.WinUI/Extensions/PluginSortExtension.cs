using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
namespace ShadowPluginLoader.WinUI.Extensions;

public static class PluginSortExtension
{
    public static IList<T> SortPlugin<T>(IEnumerable<T> source, Func<T, string[]?> getDependencies,Dictionary<string, T> sortData) where T : SortPluginData
    {
        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>();

        foreach (var item in source)
        {
            Visit(item, getDependencies, sorted, visited, sortData);
        }

        return sorted;
    }

    public static void Visit<T>(T item, Func<T, string[]?> getDependencies, List<T> sorted, Dictionary<T, bool> visited, Dictionary<string, T> sortData) where T : SortPluginData
    {
        bool inProcess;
        var alreadyVisited = visited.TryGetValue(item, out inProcess);

        // 如果已经访问该顶点，则直接返回
        if (alreadyVisited)
        {
            // 如果处理的为当前节点，则说明存在循环引用
            if (inProcess)
            {
                throw new ArgumentException("Cyclic dependency found.");
            }
        }
        else
        {
            // 正在处理当前顶点
            visited[item] = true;

            // 获得所有依赖项
            var dependencies = getDependencies(item);
            // 如果依赖项集合不为空，遍历访问其依赖节点
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    if (sortData.ContainsKey(dependency))
                    {
                        // 递归遍历访问
                        Visit(sortData[dependency], getDependencies, sorted, visited, sortData);
                    }
                }
            }

            // 处理完成置为 false
            visited[item] = false;
            sorted.Add(item);
        }
    }

}
