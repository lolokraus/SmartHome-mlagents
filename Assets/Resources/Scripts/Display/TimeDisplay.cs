using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    private int _dayOfWeek = 0; // 0 = Monday, 6 = Sunday

    void Update()
    {
        if (TimeManager.Instance != null)
        {
            if (TimeManager.Instance.SimulatedTime >= 1440) // Next day
            {
                TimeManager.Instance.SimulatedTime -= 1440;
                _dayOfWeek = (_dayOfWeek + 1) % 7;
            }
            DisplayTime();
        }
    }

    void DisplayTime()
    {
        var simulatedTime = TimeManager.Instance.SimulatedTime;
        var hours = Mathf.FloorToInt(simulatedTime / 60);
        var minutes = Mathf.FloorToInt(simulatedTime % 60);
        var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        TimeText.text = $"{days[_dayOfWeek]} {hours:00}:{minutes:00}";
    }
}