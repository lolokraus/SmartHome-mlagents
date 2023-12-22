using System;
using UnityEngine;


public class Room : MonoBehaviour
{
    private const float BaseTemperature = 15f;
    private const float MaxTemperature = 30f;
    private const float HeatingRate = 0.038f; // Temperature increase per simulated second //TODO I think it scales correctly (those two values are good but not perfect can be changes and played with)
    private const float CoolingRate = 0.025f; // Temperature decrease per simulated second //TODO I think it scales correctly

    public float Temperature { get; set; }
    public bool IsHeaterOn { get; set; }
    public float EnergyConsumption { get; set; } //TODO I think it scales correctly

    private void Start()
    {
        Temperature = 23f;
    }

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = Time.fixedDeltaTime * TimeManager.Instance.TimeScale;

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

    private float CalculateEnergyUsage(float temperature, float timeDelta) //TODO needs more realistic calculation
    {
        float energyRate;
        if (temperature > 24f)
            energyRate = (temperature - 24f) * 0.02f + 0.01f; // Higher rate above 24
        else if (temperature < 21f)
            energyRate = (21f - temperature) * 0.02f + 0.01f; // Higher rate below 21
        else
            energyRate = 0.01f; // Standard rate between 21 and 24
        return energyRate * timeDelta;
    }

    public void SetHeater(bool status)
    {
        IsHeaterOn = status;
    }
}