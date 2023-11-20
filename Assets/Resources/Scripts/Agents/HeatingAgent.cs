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
        // Actions, size = number of rooms
        /*for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var heaterStatus = actions.DiscreteActions[i] == 1;
            RoomManager.Rooms[i].SetHeater(heaterStatus);
        }

        var totalEnergy = RoomManager.TotalEnergyConsumption;
        var wellBeing = UserWellBeingManager.WellBeing;

        var normalizedEnergy = Mathf.Log10(Mathf.Max(1, totalEnergy)); 

        AddReward(wellBeing / 10 - normalizedEnergy / 10);*/

        for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var heaterStatus = actions.DiscreteActions[i] == 1;
            RoomManager.Rooms[i].SetHeater(heaterStatus);
        }

        // Calculate reward based solely on well-being
        var wellBeing = UserWellBeingManager.WellBeing;

        // Reward is proportional to well-being
        AddReward(wellBeing / 10);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Heuristic method for testing the agent manually
        var discreteActionsOut = actionsOut.DiscreteActions;
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
            discreteActionsOut[i] = Input.GetKey(KeyCode.H) ? 1 : 0; // Press 'H' to toggle heaters
    }
}