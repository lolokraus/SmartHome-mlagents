using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HeatingAgent : Agent
{
    public RoomManager RoomManager;
    public UserWellBeingManager UserWellBeingManager;

    private float timeSinceLastDecision = 0f;
    private float timeSinceLastRewardCheck = 0f;
    private const float DecisionInterval = 10f;
    private const float RewardCheckInterval = 5f;

    public override void OnEpisodeBegin()
    {
        Debug.Log("Start Simulation");
        foreach (var room in RoomManager.Rooms)
        {
            room.Temperature = 23;
            room.SetHeater(false);
            room.EnergyConsumption = 0f;
        }

        UserWellBeingManager.WellBeing = 5f;
    }

    private void FixedUpdate()
    {
        if (TimeManager.Instance != null)
        {
            float timeDelta = Time.fixedDeltaTime * TimeManager.Instance.TimeScale;

            timeSinceLastDecision += timeDelta;
            if (timeSinceLastDecision >= DecisionInterval)
            {
                RequestDecision();
                timeSinceLastDecision = 0f;
            }

            timeSinceLastRewardCheck += timeDelta;
            if (timeSinceLastRewardCheck >= RewardCheckInterval)
            {
                UpdateRewards();
                timeSinceLastRewardCheck = 0f;
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var room in RoomManager.Rooms)
        {
            sensor.AddObservation(room.Temperature);
            sensor.AddObservation(room.IsHeaterOn ? 1 : 0);
        }

        sensor.AddObservation(UserWellBeingManager.WellBeing / 10f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var heaterStatus = actions.DiscreteActions[i] == 1;
            RoomManager.Rooms[i].SetHeater(heaterStatus);
        }
    }

    private void UpdateRewards()
    {
        
        float wellBeingDeviation = UserWellBeingManager.WellBeing - 10;
        AddReward(wellBeingDeviation);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Manual testing method for the agent
        var discreteActionsOut = actionsOut.DiscreteActions;
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            discreteActionsOut[i] = Input.GetKey(KeyCode.H) ? 1 : 0;
        }
    }
}