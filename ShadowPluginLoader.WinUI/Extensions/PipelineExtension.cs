using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;
using Serilog;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Pipelines;

namespace ShadowPluginLoader.WinUI.Extensions;

/// <summary>
/// 流水线扩展
/// </summary>
public static class PipelineExtension
{
    extension(IPipeline pipeline)
    {
        public IPipeline Feed(Type? type)
        {
            if (type is null) return pipeline;
            var dir = type.Assembly.Location[..^".dll".Length];
            var metaPath = Path.Combine(dir, "plugin.json");
            return pipeline.Feed(new Uri(metaPath));
        }

        public IPipeline Feed<TPlugin>()
        {
            return pipeline.Feed(typeof(TPlugin));
        }

        public IPipeline Feed(params Type?[] types)
        {
            foreach (var type in types)
            {
                pipeline.Feed(type);
            }

            return pipeline;
        }

        public IPipeline Feed(IEnumerable<Type?> types)
        {
            foreach (var type in types)
            {
                pipeline.Feed(type);
            }

            return pipeline;
        }

        public IPipeline Feed(Package package)
        {
            return pipeline.Feed(new DirectoryInfo(package.InstalledPath));
        }

        public IPipeline Feed(DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                Log.Warning("Scan Dir[{DirFullName}]: Dir Not Exists", dir.FullName);
                return pipeline;
            }

            foreach (var pluginFile in dir.EnumerateFiles("plugin.json", SearchOption.AllDirectories))
            {
                pipeline.Feed(new Uri(pluginFile.FullName));
            }

            return pipeline;
        }


        public IPipeline Feed(FileInfo pluginJson)
        {
            if (File.Exists(pluginJson.FullName)) pipeline.Feed(new Uri(pluginJson.FullName));
            return pipeline;
        }

        /// <summary>
        /// 投入原料
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IPipeline Feed(Uri uri)
        {
            if (uri.Scheme.StartsWith("http"))
            {
                if (!uri.IsFile)
                {
                    Log.Warning("Scan Uri[{DirFullName}]: Not Support", uri.LocalPath);
                }
                return pipeline.Feed(new HttpFileMaterial(uri));
            }
            else if (!uri.IsFile && Directory.Exists(uri.LocalPath))
            {
                return pipeline.Feed(new DirectoryInfo(uri.LocalPath));
            }
            else if (uri.IsFile)
            {
                if (uri.LocalPath.EndsWith("plugin.json"))
                {
                    return pipeline.Feed(new LocalFileMaterial(uri));
                }
                else if (uri.LocalPath.EndsWith(".sdow"))
                {
                    return pipeline.Feed(new CompressedFileMaterial(uri));
                }
                else
                {
                    Log.Warning("Scan Uri[{DirFullName}]: Not Support", uri.LocalPath);
                }
            }else
            {
                Log.Warning("Scan Uri[{DirFullName}]: Not Support", uri.LocalPath);
            }


            return pipeline;
        }
    }
}