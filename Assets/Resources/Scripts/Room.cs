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
        float energyRate;
        if (temperature > 24f)
            energyRate = (temperature - 24f) * 0.02f + 0.01f;
        else if (temperature < 21f)
            energyRate = (21f - temperature) * 0.02f + 0.01f;
        else
            energyRate = 0.01f;
        return energyRate * timeDelta;
    }

    public void SetHeater(bool status)
    {
        IsHeaterOn = status;
    }
}