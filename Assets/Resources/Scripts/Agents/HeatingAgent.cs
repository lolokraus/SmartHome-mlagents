using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HeatingAgent : Agent
{
    public RoomManager RoomManager;
    public UserWellBeingManager UserWellBeingManager;

    public override void OnEpisodeBegin()
    {
        // Reset the environment at the beginning of each training episode
        Debug.Log("Start Simulation");
        foreach (var room in RoomManager.Rooms)
        {
            room.Temperature = 15f;
            room.SetHeater(false);
            room.EnergyConsumption = 0f;
        }

        UserWellBeingManager.WellBeing = 5f;
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

        // Calculate reward based solely on well-being
        var wellBeing = UserWellBeingManager.WellBeing;

        // Check if well-being is at the optimal level (10)
        if (Math.Abs(wellBeing - 10) < 0.5)
        {
            // Provide a high reward for achieving optimal well-being
            AddReward(1.0f);
        }
        else
        {
            // The penalty is proportional to the deviation from the optimal level
            AddReward(-(10 - wellBeing) / 10.0f); // Penalize based on how far from 10 the well-being is
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Heuristic method for testing the agent manually
        var discreteActionsOut = actionsOut.DiscreteActions;
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
            discreteActionsOut[i] = Input.GetKey(KeyCode.H) ? 1 : 0; // Press 'H' to toggle heaters
    }
}