using UnityEngine;

public class Room : MonoBehaviour
{
    private const float HeatingRate = 0.038f;
    private const float CoolingRate = 0.025f;

    public float Temperature { get; set; }
    public bool IsHeaterOn { get; set; }
    public float EnergyConsumption { get; set; }

    private void Start()
    {
        Temperature = 23f;
    }

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = Time.fixedDeltaTime * TimeManager.Instance.TimeScale;

            if (IsHeaterOn)
                Temperature += CalculateDynamicHeatingRate(Temperature) * timeDelta;
            else
                Temperature -= CalculateDynamicCoolingRate(Temperature) * timeDelta;

            if (IsHeaterOn)
                EnergyConsumption += CalculateEnergyUsage(Temperature, timeDelta);
        }
    }

    private float CalculateDynamicHeatingRate(float temperature)
    {
        if (temperature > 25f)
            return HeatingRate / (1 + (temperature - 25f));
        else
            return HeatingRate;
    }

    private float CalculateDynamicCoolingRate(float temperature)
    {
        if (temperature < 5f)
            return CoolingRate / (1 + (5f - temperature));
        else
            return CoolingRate;
    }

    private float CalculateEnergyUsage(float temperature, float timeDelta)
    {
        // Base energy consumption rate per unit of time when the heater is on.
        float baseEnergyRate = 0.01f;

        // Additional energy consumption when heating is needed.
        float heatingEnergyRate = 0.03f;

        // Calculate the energy needed to maintain the current temperature.
        float energyToMaintain = baseEnergyRate * timeDelta;

        // Calculate the additional energy needed to heat the room.
        float additionalHeatingEnergy = 0f;
        if (temperature < 23f && IsHeaterOn)
        {
            additionalHeatingEnergy = (23f - temperature) * heatingEnergyRate * timeDelta;
        }

        // Total energy consumption is the sum of maintaining plus any additional heating.
        return energyToMaintain + additionalHeatingEnergy;
    }

    public void SetHeater(bool status)
    {
        IsHeaterOn = status;
    }
}