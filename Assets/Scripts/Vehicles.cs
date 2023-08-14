using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicles", menuName = "ScriptableObjects/Vehicles")]
public class Vehicles : ScriptableObject
{
    public List<VehicleData> Data;

    public VehicleData GetVehicle(VehicleID ID)
    {
        return Data.FirstOrDefault(v => v.ID == ID);
    }
}
