using System;
using UnityEngine;

public class UserWellBeingManager : MonoBehaviour
{
    public UserMovement User;
    public Renderer AvatarRenderer;
    public float WellBeing { get; set; } = 5;
    private const float OptimalTemperature = 23f;
    private const float WellBeingChangeRate = 0.005f;

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = Time.fixedDeltaTime * TimeManager.Instance.TimeScale;
            Room currentRoom = User.GetCurrentRoom();
            if (currentRoom != null)
            {
                UpdateWellBeingBasedOnRoom(currentRoom, timeDelta);
                UpdateAvatarColor();
            }
        }
    }

    private void UpdateWellBeingBasedOnRoom(Room room, float timeDelta)
    {
        float temperatureDifference = Mathf.Abs(room.Temperature - OptimalTemperature);
        if (temperatureDifference > 1.0f) // If not at optimal temperature
        {
            float wellBeingChange = WellBeingChangeRate * temperatureDifference * timeDelta;
            WellBeing = Mathf.Max(0, WellBeing - wellBeingChange); // Decrease well-being
        }
        else // If at optimal temperature
        {
            WellBeing = Mathf.Min(10, WellBeing + WellBeingChangeRate * timeDelta); // Increase well-being
        }
    }

    private void UpdateAvatarColor()
    {
        if (AvatarRenderer != null)
        {
            Color newColor = Color.Lerp(Color.red, Color.green, WellBeing / 10f);

            AvatarRenderer.material.SetColor("_Color", newColor);
        }
    }
}
