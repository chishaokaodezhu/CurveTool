/*
 * 负责插件实例的加载，并建立插件名与插件实例的映射关系
 */
using PluginPort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CurveMonitor.src.Plugin
{
    class PluginLoader
    {
        List<string> pluginNames = new List<string>();
        Hashtable pluginTbl = new Hashtable();

        public void LoadPlugins()
        {
            string pluginsPath = AppDomain.CurrentDomain.BaseDirectory;
            pluginsPath = Path.Combine(pluginsPath, "plugin");

            foreach(string pluginPath in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(pluginPath);
                Type[] types = asm.GetExportedTypes();
                foreach(Type type in types)
                {
                    if(type.GetInterface("DataProvider") != null)
                    {
                        Object dp = Activator.CreateInstance(type);
                        if(dp is DataProvider dp1)
                        {
                            string name = dp1.PluginName();
                            pluginTbl.Add(name, type);
                        }
                    }
                }
            }
        }

        public DataProvider NewPluginInstance(string name)
        {
            Type t = (Type)pluginTbl[name];
            return (DataProvider)Activator.CreateInstance(t);
        }
    }
}
