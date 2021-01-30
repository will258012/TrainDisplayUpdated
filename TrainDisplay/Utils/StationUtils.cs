using ICities;
using UnityEngine;
using ColossalFramework;

namespace TrainDisplay.Utils
{
    class StationUtils
    {
        public static string GetStopName(ushort stopId, ItemClass.SubService ss)
        {
            TransportLine t;
            InstanceID id = default;
            id.NetNode = stopId;
            return InstanceManager.instance.GetName(id);

            NetManager nm = Singleton<NetManager>.instance;
            BuildingManager bm = Singleton<BuildingManager>.instance;
            NetNode nn = nm.m_nodes.m_buffer[(int)stopId];
            ushort tempBuildingId = BuildingUtils.FindBuilding(nn.m_position, 100f, ItemClass.Service.PublicTransport, ss, TLMUtils.defaultAllowedVehicleTypes, Building.Flags.None, Building.Flags.Untouchable);
        }

        public static string GetStationName(ushort stopId, ItemClass.SubService ss)
        {
            return GetStopName(stopId, ss) ?? "(" + stopId + ")";
        }
    }
}
