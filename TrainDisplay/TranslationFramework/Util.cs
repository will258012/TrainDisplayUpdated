using ColossalFramework;
using ColossalFramework.Plugins;
using System;
using System.Reflection;
using Log = TrainDisplay.Utils.Log;

namespace TrainDisplay.TranslationFramework
{
    public static class Util
    {
        internal static string AssemblyPath
        {
            get
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                foreach (PluginManager.PluginInfo item in Singleton<PluginManager>.instance.GetPluginsInfo())
                {
                    try
                    {
                        foreach (Assembly assembly in item.GetAssemblies())
                        {
                            if (assembly == executingAssembly)
                            {
                                return item.modPath;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TrainDisplay: " + ex?.ToString() + "exception iterating through plugins");
                    }
                }

                throw new Exception("Failed to find TrainDisplay Mod assembly!");
            }
        }
    }
}