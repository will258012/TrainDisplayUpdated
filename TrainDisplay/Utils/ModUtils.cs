using System.Linq;
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
                if (info.publishedFileID.AsUInt64 == 650805785 && info.isEnabled)
                {
                    return true;
                }
            }
            Log.Error("\"First Person Camera: Updated\" not found.");
            return false;
        }
    }
}
