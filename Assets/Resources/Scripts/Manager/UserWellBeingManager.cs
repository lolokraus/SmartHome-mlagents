using System;
using UnityEngine;

public class UserWellBeingManager : MonoBehaviour
{
    public UserMovement User;
    public float WellBeing { get; set; } = 5; // Initialize at a neutral value //TODO not correclty scaled with increased time
    private const float OptimalTemperature = 23f;
    private const float WellBeingChangeRate = 0.005f; // Rate of change per simulated second

    private void Update()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = Time.deltaTime * TimeManager.Instance.TimeScale;
            Room currentRoom = User.GetCurrentRoom();
            if (currentRoom != null)
            {
                UpdateWellBeingBasedOnRoom(currentRoom, timeDelta);
            }
        }
    }

    private void UpdateWellBeingBasedOnRoom(Room room, float timeDelta)
    {
        float temperatureDifference = Mathf.Abs(room.Temperature - OptimalTemperature);
        if (Math.Abs(room.Temperature - OptimalTemperature) > 0.5f) // If not at optimal temperature
        {
            float wellBeingChange = WellBeingChangeRate * temperatureDifference * timeDelta;
            WellBeing = Mathf.Max(0, WellBeing - wellBeingChange); // Decrease well-being
        }
        else // If at optimal temperature
        {
            WellBeing = Mathf.Min(10, WellBeing + WellBeingChangeRate * timeDelta); // Increase well-being
        }
    }
}