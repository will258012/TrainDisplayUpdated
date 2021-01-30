using ICities;
using UnityEngine;
using TrainDisplay.Utils;
using TrainDisplay.UI;

namespace TrainDisplay
{
    public class DisplayUIManager: MonoBehaviour
    {
        VehicleManager vManager;
        TransportManager tManager;

        void Awake()
        {
            vManager = VehicleManager.instance;
            tManager = TransportManager.instance;
        }
        public void SetTrain(ushort followInstance)
        {
            Vehicle v = vManager.m_vehicles.m_buffer[followInstance];
            ushort firstVehicle = vManager.m_vehicles.m_buffer[(int)followInstance].GetFirstVehicle(followInstance);
            v = vManager.m_vehicles.m_buffer[firstVehicle];
            InstanceID instanceID2 = default(InstanceID);

            VehicleInfo info = v.Info;

            info.m_vehicleAI.GetLocalizedStatus(firstVehicle, ref vManager.m_vehicles.m_buffer[firstVehicle], out instanceID2);
            if (info.m_vehicleType == VehicleInfo.VehicleType.Train || info.m_vehicleType == VehicleInfo.VehicleType.Metro) {
                ushort lineId = v.m_transportLine;
                TransportLine line = tManager.m_lines.m_buffer[lineId];
                int stopsNumber = line.CountStops(lineId);

                ItemClass.SubService ss = info.GetSubService();

                DisplayUI.Instance.testString = "";
                for (int i = 0; i < stopsNumber; i++)
                {
                    ushort stationId = line.GetStop(i);
                    string stationName = StationUtils.GetStationName(stationId, ss);
                    DisplayUI.Instance.testString += stationName + " ";
                }
            }
        }
    }
}
