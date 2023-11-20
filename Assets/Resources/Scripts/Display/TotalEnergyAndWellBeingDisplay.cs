using UnityEngine;
using TMPro;

public class TotalEnergyAndWellBeingDisplay : MonoBehaviour
{
    public RoomManager RoomManager;
    public UserWellBeingManager UserWellBeingManager;
    public TextMeshProUGUI DisplayText;

    private void Update()
    {
        if (RoomManager != null && UserWellBeingManager != null)
        {
            DisplayText.text = $"Total Energy Consumption: {RoomManager.TotalEnergyConsumption:F2}\n" +
                               $"User Well-Being: {UserWellBeingManager.WellBeing:F1}/10";
        }
    }
}