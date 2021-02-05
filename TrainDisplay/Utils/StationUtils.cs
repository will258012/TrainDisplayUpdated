using ICities;
using UnityEngine;
using ColossalFramework;

namespace TrainDisplay.Utils
{
    class StationUtils
    {
        public static readonly TransportInfo.TransportType[] stationTransportType =
        {
            TransportInfo.TransportType.Train,
            TransportInfo.TransportType.Metro,
            TransportInfo.TransportType.Monorail
        };
        public static string GetStopName(ushort stopId)
        {
            InstanceID id = default;
            id.NetNode = stopId;
            string savedName = InstanceManager.instance.GetName(id);
            if (savedName != null)
            {
                return savedName;
            }

            NetManager nm = Singleton<NetManager>.instance;
            NetNode nn = nm.m_nodes.m_buffer[(int)stopId];
            ushort buildingId = FindBuilding(nn.m_position, 100f);
            return GetBuildingName(buildingId) ?? "[" + stopId + "," + buildingId + "]";
        }

        public static string GetBuildingName(ushort buildingId)
        {
            InstanceID bid = default;
            bid.Building = buildingId;
            return Singleton<BuildingManager>.instance.GetBuildingName(buildingId, bid);
        }

        public static string GetStationName(ushort stopId)
        {
            return GetStopName(stopId) ?? "(" + stopId + ")";
        }

        public static ushort FindBuilding(Vector3 pos, float maxDistance)
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;
            foreach (var tType in stationTransportType)
            {
                ushort stationId = bm.FindTransportBuilding(pos, maxDistance, tType);
                if (stationId != 0)
                {
                    return stationId;
                }
            }
            return 0;
            //if (allowedTypes == null || allowedTypes.Length == 0)
            //{
            //    return bm.FindBuilding(pos, maxDistance, service, subService, flagsRequired, flagsForbidden);
            //}

            /*
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
            */
        }

        public static string[] stationSuffix = { "駅", " Station", " Sta.", " Sta" };

        public static string removeStationSuffix(string stationName)
        {
            foreach (var suffix in stationSuffix)
            {
                if (stationName.ToLower().EndsWith(suffix.ToLower()))
                {
                    return stationName.Remove(stationName.Length - suffix.Length);
                }
            }
            return stationName;
        }
    }
}
