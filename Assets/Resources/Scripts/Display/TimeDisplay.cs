using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI TimeText;

    void Update()
    {
        if (TimeManager.Instance != null)
        {
            DisplayTime();
        }
    }

    void DisplayTime()
    {
        var simulatedTime = TimeManager.Instance.SimulatedTime;
        var hours = Mathf.FloorToInt(simulatedTime / 60);
        var minutes = Mathf.FloorToInt(simulatedTime % 60);
        var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Calculate the current day of the week based on DaysPassed
        int dayIndex = TimeManager.Instance.DaysPassed % 7;
        string dayName = daysOfWeek[dayIndex];

        TimeText.text = $"{dayName} {hours:00}:{minutes:00}";
    }
}