using ICities;
using UnityEngine;
using ColossalFramework;

namespace TrainDisplay.Utils
{
    class StationUtils
    {
        public static readonly TransferManager.TransferReason[] allowedReason =
        {
            TransferManager.TransferReason.MetroTrain,
            TransferManager.TransferReason.PassengerTrain
        };
        public static string GetStopName(ushort stopId, ItemClass.SubService ss)
        {
            InstanceID id = default;
            id.NetNode = stopId;
            string savedName = InstanceManager.instance.GetName(id);
            if (savedName != null)
            {
                return savedName;
            }

            NetManager nm = Singleton<NetManager>.instance;
            BuildingManager bm = Singleton<BuildingManager>.instance;
            NetNode nn = nm.m_nodes.m_buffer[(int)stopId];
            ushort buildingId = FindBuilding(nn.m_position, 100f, ItemClass.Service.PublicTransport, ss, allowedReason, Building.Flags.None, Building.Flags.Untouchable);
            InstanceID bid = default;
            bid.Building = buildingId;
            return Singleton<BuildingManager>.instance.GetBuildingName(buildingId, bid);
        }

        public static string GetStationName(ushort stopId, ItemClass.SubService ss)
        {
            return GetStopName(stopId, ss) ?? "(" + stopId + ")";
        }

        public static ushort FindBuilding(Vector3 pos, float maxDistance, ItemClass.Service service, ItemClass.SubService subService, TransferManager.TransferReason[] allowedTypes, Building.Flags flagsRequired, Building.Flags flagsForbidden)
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;
            //if (allowedTypes == null || allowedTypes.Length == 0)
            //{
            //    return bm.FindBuilding(pos, maxDistance, service, subService, flagsRequired, flagsForbidden);
            //}


            int num = Mathf.Max((int)(((pos.x - maxDistance) / 64f) + 135f), 0);
            int num2 = Mathf.Max((int)(((pos.z - maxDistance) / 64f) + 135f), 0);
            int num3 = Mathf.Min((int)(((pos.x + maxDistance) / 64f) + 135f), 269);
            int num4 = Mathf.Min((int)(((pos.z + maxDistance) / 64f) + 135f), 269);
            ushort result = 0;
            float currentDistance = maxDistance * maxDistance;
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    ushort buildingId = bm.m_buildingGrid[(i * 270) + j];
                    //int num7 = 0;
                    while (buildingId != 0)
                    {
                        BuildingInfo info = bm.m_buildings.m_buffer[buildingId].Info;
                        //if (!CheckInfoCompatibility(pos, service, subService, allowedTypes, flagsRequired, flagsForbidden, bm, ref result, ref currentDistance, buildingId, info) && info.m_subBuildings?.Length > 0)
                        //{
                        //    foreach (BuildingInfo.SubInfo subBuilding in info.m_subBuildings)
                        //    {
                        //        if (subBuilding != null && CheckInfoCompatibility(pos, service, subService, allowedTypes, flagsRequired, flagsForbidden, bm, ref result, ref currentDistance, buildingId, subBuilding.m_buildingInfo))
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        buildingId = bm.m_buildings.m_buffer[buildingId].m_nextGridBuilding;
                        //if (++num7 >= 49152)
                        //{
                        //    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                        //    break;
                        //}
                    }
                }
            }
            return result;
        }
    }
}
