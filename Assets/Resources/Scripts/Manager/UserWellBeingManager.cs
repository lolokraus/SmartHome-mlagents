using UnityEngine;

public class UserWellBeingManager : MonoBehaviour
{
    public UserMovement User;
    public float WellBeing { get; private set; } = 5; // Initialize at a neutral value
    private const float OptimalTemperature = 23f;
    private const float WellBeingChangeRate = 0.001f; // Rate of change per simulated second

    private float lastUpdateTime = 0f;

    private void Update()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = TimeManager.Instance.SimulatedTime - lastUpdateTime;
            lastUpdateTime = TimeManager.Instance.SimulatedTime;

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

        if (room.Temperature != OptimalTemperature) // If not at optimal temperature
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