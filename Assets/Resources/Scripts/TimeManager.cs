using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float TimeScale = 6.0f; // 1 real second = 5 simulated minutes
    public static TimeManager Instance { get; private set; }
    public float SimulatedTime { get; set; }

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
        SimulatedTime += Time.fixedDeltaTime * TimeScale; //Called 50 times per second can be edited in Edit > Project Settings > Time
    }
}