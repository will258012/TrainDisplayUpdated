using ColossalFramework.Plugins;

namespace TrainDisplay.Utils
{
    class ModUtils
    {

        public static bool FindFPSMod()
        {
            var infos = PluginManager.instance.GetPluginsInfo();

            foreach (var info in infos)
            {
                if (info.publishedFileID.AsUInt64 == 3198388677 && info.isEnabled)
                {
                    return true;
                }
            }
            Log.Error("\"First Person Camera - Continued\" not found.");
            return false;
        }
    }
}
