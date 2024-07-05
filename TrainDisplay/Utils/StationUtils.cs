using ColossalFramework;
using System;
using System.Linq;
using TrainDisplay.Config;
using UnityEngine;

namespace TrainDisplay.Utils
{
    class StationUtils
    {
        public static readonly TransportInfo.TransportType[] stationTransportType =
        {
            TransportInfo.TransportType.Train,
            TransportInfo.TransportType.Metro,
            TransportInfo.TransportType.Monorail,
            TransportInfo.TransportType.Tram,
        };
        private static string GetStopName(ushort stopId)
        {
            InstanceID id = default;
            id.NetNode = stopId;
            string savedName = Singleton<InstanceManager>.instance.GetName(id);
            if (!savedName.IsNullOrWhiteSpace())
            {
                return savedName;
            }

            NetManager nm = Singleton<NetManager>.instance;
            NetNode nn = nm.m_nodes.m_buffer[stopId];
            var pos = nn.m_position;
            //building
            ushort buildingId = FindTransportBuilding(pos, 100f);
            savedName = GetTransportBuildingName(buildingId);
            if (!savedName.IsNullOrWhiteSpace())
            {
                return savedName;
            }
            //road 
            //bool isNum = false; int number = 0;
            //isNum = Commons.Utils.SegmentUtils.GetBasicAddressStreetAndNumber(pos, pos, out number, out _); 
            savedName = $"{stopId} {GetStationRoadName(pos)}";
            if (!savedName.IsNullOrWhiteSpace())
            {
                return savedName;
            }
            //district
            savedName = $"{GetStationDistrictName(pos)}";
            if (!savedName.IsNullOrWhiteSpace())
            {
                return savedName;
            }
            return $"<Somewhere>[{stopId}]";
        }

        public static string GetTransportBuildingName(ushort buildingId)
        {
            InstanceID bid = default;
            bid.Building = buildingId;
            return Singleton<BuildingManager>.instance.GetBuildingName(buildingId, bid);
        }

        public static string GetStationName(ushort stopId) => GetStopName(stopId) ?? "(" + stopId + ")";
        public static string GetStationRoadName(Vector3 pos)
        {
            var segmentid = CSkyL.Game.Map.RayCastRoad(new CSkyL.Transform.Position { x = pos.x, up = pos.y, y = pos.z });
            var name = CSkyL.Game.Object.Segment.GetName(segmentid);
            return name;
        }
        public static object GetStationDistrictName(Vector3 pos)
        {
            var districtId = CSkyL.Game.Map.RayCastDistrict(new CSkyL.Transform.Position { x = pos.x, up = pos.y, y = pos.z });
            var name = CSkyL.Game.Object.District.GetName(districtId);
            return name;
        }
        public static ushort FindTransportBuilding(Vector3 pos, float maxDistance)
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;

            foreach (var tType in stationTransportType)
            {
                ushort buildingid = bm.FindTransportBuilding(pos, maxDistance, tType);

                if (buildingid != 0)
                {
                    if (bm.m_buildings.m_buffer[buildingid].m_parentBuilding != 0)
                    {
                        buildingid = Building.FindParentBuilding(buildingid);
                    }
                    return buildingid;
                }
            }
            return default;
        }



        private static string[] stationSuffix => TrainDisplayConfig.Instance.StationSuffix
            .Split(new[] { "\",\"" }, StringSplitOptions.None)
            .Select(suffix => suffix.Trim('"'))
            .ToArray();

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
