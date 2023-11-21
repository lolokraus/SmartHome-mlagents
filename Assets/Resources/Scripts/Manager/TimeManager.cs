using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float TimeScale = 6.0f; // 1 real second = 5 simulated minutes  //Values higher than 50 don't work well
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
        SimulatedTime += Time.deltaTime * TimeScale;
       // Debug.Log(SimulatedTime);
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