using UnityEngine;

public class Room : MonoBehaviour
{
    private const float BaseTemperature = 15f;
    private const float MaxTemperature = 30f;
    private const float HeatingRate = 0.1f; // Temperature increase per simulated second
    private const float CoolingRate = 0.05f; // Temperature decrease per simulated second


    private float _lastUpdateTime;
    public float Temperature { get; private set; }
    public bool IsHeaterOn { get; private set; }
    public float EnergyConsumption { get; private set; }

    private void Start()
    {
        Temperature = BaseTemperature;
    }

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            var timeDelta = TimeManager.Instance.SimulatedTime - _lastUpdateTime;
            _lastUpdateTime = TimeManager.Instance.SimulatedTime;

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
        // Energy usage is higher when the temperature is lower.
        float energyRate = temperature < 25f ? (25f - temperature) * 0.01f : 0.01f;
        return energyRate * timeDelta;
    }

    public void ToggleHeater()
    {
        IsHeaterOn = !IsHeaterOn;
    }
}