using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float TimeScale = 1.0f;
    public static TimeManager Instance { get; set; }
    public float SimulatedTime { get; set; }
    public int DaysPassed { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        SimulatedTime += Time.fixedDeltaTime * TimeScale;
        //Debug.Log("SimulatedTime " + SimulatedTime);
        CheckDayChange();
    }

    private void CheckDayChange()
    {
        if (SimulatedTime >= 1440) // New day
        {
            SimulatedTime -= 1440;
            DaysPassed++;
        }
    }
}