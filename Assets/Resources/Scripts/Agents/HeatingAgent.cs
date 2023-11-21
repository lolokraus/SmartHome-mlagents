using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HeatingAgent : Agent
{
    public RoomManager RoomManager;
    public UserWellBeingManager UserWellBeingManager;
    private float lastWellBeing = 5f;
    private float lastAverageTemperature = 23f;

    public override void OnEpisodeBegin()
    {
        // Reset the environment at the beginning of each training episode
        Debug.Log("Start Simulation");
        foreach (var room in RoomManager.Rooms)
        {
            room.Temperature = 23f;
            room.SetHeater(false);
            room.EnergyConsumption = 0f;
            
        }

        UserWellBeingManager.WellBeing = 5f;
        lastWellBeing = 5f;
        lastAverageTemperature = 23f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations
        foreach (var room in RoomManager.Rooms)
        {
            sensor.AddObservation(room.Temperature);
            sensor.AddObservation(room.IsHeaterOn ? 1 : 0);
        }

        sensor.AddObservation(UserWellBeingManager.WellBeing);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //TODO factor in energy consumption
        // Actions, size = number of rooms
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var heaterStatus = actions.DiscreteActions[i] == 1;
            RoomManager.Rooms[i].SetHeater(heaterStatus);
        }

        var currentWellBeing = UserWellBeingManager.WellBeing;
        var wellBeingChange = currentWellBeing - lastWellBeing;

        // Reward for increasing well-being
        if (wellBeingChange > 0)
        {
            AddReward(wellBeingChange / 10.0f);
        }

        // Big reward for high well-being
        if (currentWellBeing >= 9f)
        {
            AddReward(0.1f); // Constant reward for being in this high well-being range
        }

        // Penalize stagnating well-being
        if (wellBeingChange == 0 && currentWellBeing < 5f)
        {
            AddReward(-0.05f); // Penalize for not improving well-being when it's low
        }

        // Penalize stagnating temperature when well-being is low and stagnating
        bool isTemperatureStagnant = CheckTemperatureStagnation(); // Implement this method based on temperature changes
        if (isTemperatureStagnant && wellBeingChange == 0 && currentWellBeing < 5f)
        {
            AddReward(-0.1f); // Penalize for stagnant temperature in unfavorable conditions
        }

        lastWellBeing = currentWellBeing; // Update last well-being for next step
    }

    private bool CheckTemperatureStagnation()
    {
        float currentAverageTemperature = 0f;
        foreach (var room in RoomManager.Rooms)
        {
            currentAverageTemperature += room.Temperature;
        }
        currentAverageTemperature /= RoomManager.Rooms.Length;

        // Check if the change in average temperature is below a certain threshold
        bool isStagnant = Mathf.Abs(currentAverageTemperature - lastAverageTemperature) < 1f; // Threshold can be adjusted

        // Update last average temperature
        lastAverageTemperature = currentAverageTemperature;

        return isStagnant;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Heuristic method for testing the agent manually
        var discreteActionsOut = actionsOut.DiscreteActions;
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
            discreteActionsOut[i] = Input.GetKey(KeyCode.H) ? 1 : 0; // Press 'H' to toggle heaters
    }
}