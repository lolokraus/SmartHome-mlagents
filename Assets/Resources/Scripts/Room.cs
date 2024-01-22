using UnityEngine;

public class Room : MonoBehaviour
{
    private const float HeatingRate = 0.038f;
    private const float CoolingRate = 0.025f;
    private const float OutsideTemperature = 10f;

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
        // Heating is more efficient when the room's temperature is closer to the outside temperature
        float temperatureDifference = Mathf.Abs(temperature - OutsideTemperature);
        return HeatingRate * Mathf.Max(0.1f, 1 - temperatureDifference / 60); // Linear decrease in heating rate
    }

    private float CalculateDynamicCoolingRate(float temperature)
    {
        // No cooling below outside temperature
        if (temperature <= OutsideTemperature)
            return 0; 

        // Cooling rate decreases as the temperature approaches the outside temperature
        float temperatureDifference = temperature - OutsideTemperature;
        return CoolingRate * Mathf.Min(1, temperatureDifference / 60); // Larger divisor for more gradual cooling
    }

    private float CalculateEnergyUsage(float temperature, float timeDelta)
    {
        float energyRate;
        if (temperature < 16.0f)
        {
            energyRate = 0.1f;
        }
        else
        {
            energyRate = 0.02f;
        }

        float energyUsed = energyRate * timeDelta;

        return energyUsed;
    }

    public void SetHeater(bool status)
    {
        IsHeaterOn = status;
    }
}