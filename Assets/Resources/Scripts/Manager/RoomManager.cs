using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Room[] Rooms;

    public float TotalEnergyConsumption
    {
        get
        {
            float total = 0f;
            foreach (var room in Rooms)
            {
                total += room.EnergyConsumption;
            }
            return total;
        }
    }
}