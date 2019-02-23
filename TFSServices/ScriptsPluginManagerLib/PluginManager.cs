using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScriptsPluginManagerLib
{
    public sealed class PluginManager
    {
        private const string PluginRootDir = @"Plugins\";

        private List<IPlugin> pluginInstances;

        public PluginManager()
        {

        }

        public IPlugin this[string plugin_name] => pluginInstances.Where(pl => pl.Name == plugin_name).FirstOrDefault();


        public void Initialize()
        {
            pluginInstances = pluginInstances ?? GetPlugins();
        }

        private List<IPlugin> GetPlugins()
        {
            List<IPlugin> existingPlugins = new List<IPlugin>();

            foreach (string dir in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginRootDir)))
            {

                existingPlugins.AddRange((from file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir), "*_scrplugin.dll")
                                          from type in Assembly.LoadFrom(file).GetTypes()
                                          where type.GetInterfaces().Contains(typeof(IPlugin))
                                          select (IPlugin)Activator.CreateInstance(type)).ToList());
            }

            return existingPlugins;
        }
    }
}
