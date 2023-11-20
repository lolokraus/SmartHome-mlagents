using UnityEngine;

public class Room : MonoBehaviour
{
    private const float BaseTemperature = 15f;
    private const float MaxTemperature = 30f;
    private const float HeatingRate = 0.005f; // Temperature increase per simulated second //TODO not correclty scaled with increased time
    private const float CoolingRate = 0.001f; // Temperature decrease per simulated second //TODO not correclty scaled with increased time

    public float Temperature { get; set; }
    public bool IsHeaterOn { get; set; }
    public float EnergyConsumption { get; set; } //TODO not correclty scaled with increased time

    private void Start()
    {
        Temperature = BaseTemperature;
    }

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            // Use Time.deltaTime multiplied by TimeScale for a smoother timeDelta
            float timeDelta = Time.deltaTime * TimeManager.Instance.TimeScale;

            // Update Temperature
            if (IsHeaterOn && Temperature < MaxTemperature)
                Temperature += HeatingRate * timeDelta;
            else if (!IsHeaterOn && Temperature > BaseTemperature)
                Temperature -= CoolingRate * timeDelta;

            // Update Energy Consumption
            if (IsHeaterOn)
                EnergyConsumption += CalculateEnergyUsage(Temperature, timeDelta);
        }
    }

    private float CalculateEnergyUsage(float temperature, float timeDelta)
    {
        // Energy usage calculation remains the same
        float energyRate = temperature < 25f ? (25f - temperature) * 0.01f : 0.01f;
        return energyRate * timeDelta;
    }

    public void SetHeater(bool status)
    {
        Debug.Log("Heating change");
        IsHeaterOn = status;
    }
}