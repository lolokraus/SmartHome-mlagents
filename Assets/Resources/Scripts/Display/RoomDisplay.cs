using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;

public class RoomDisplay : MonoBehaviour
{
    public RoomManager RoomManager;
    public TextMeshProUGUI DisplayText;

    private void Update()
    {
        if (RoomManager != null)
        {
            DisplayText.text = "";
            foreach (var room in RoomManager.Rooms)
            {
                DisplayText.text += $"Room: {room.name}\n" +
                                    $"Temperature: {room.Temperature:F1}°C\n" +
                                    $"Heater: {(room.IsHeaterOn ? "On" : "Off")}\n\n";
            }
        }
    }
}