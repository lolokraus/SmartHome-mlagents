using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeatingAgent : Agent
{
    public RoomManager RoomManager;
    public UserWellBeingManager UserWellBeingManager;
    public UserMovement UserMovement;

    private float lastWellBeing = 5f;

    private int stepsSinceLastDecision = 0;
    private const int decisionInterval = 50;

    private int stepsSinceLastRewardCheck = 0;
    private const int checkRewardInterval = 5;

    private const float TemperatureRangeThreshold = 2.0f;
    private float[] temperatureMovingAverage;

    public override void OnEpisodeBegin()
    {
        Debug.Log("Start Simulation");
        foreach (var room in RoomManager.Rooms)
        {
            room.Temperature = Random.Range(15f, 30f);
            room.SetHeater(false);
            room.EnergyConsumption = 0f;
        }

        UserWellBeingManager.WellBeing = 5f;
        lastWellBeing = UserWellBeingManager.WellBeing;

        temperatureMovingAverage = new float[RoomManager.Rooms.Length];
        for (int i = 0; i < RoomManager.Rooms.Length; i++)
        {
            temperatureMovingAverage[i] = RoomManager.Rooms[i].Temperature;
        }
    }

    private void FixedUpdate()
    {
        stepsSinceLastDecision++;
        if (stepsSinceLastDecision >= decisionInterval)
        {
            RequestDecision();
            stepsSinceLastDecision = 0;
        }

        stepsSinceLastRewardCheck++;
        if (stepsSinceLastRewardCheck >= checkRewardInterval)
        {
            UpdateRewards();
            stepsSinceLastRewardCheck = 0;
        }

        for (int i = 0; i < RoomManager.Rooms.Length; i++)
        {
            temperatureMovingAverage[i] = temperatureMovingAverage[i] * 0.9f + RoomManager.Rooms[i].Temperature * 0.1f;
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
        Room currentRoom = UserMovement.GetCurrentRoom();
        float totalReward = 0f;

        for (int i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var room = RoomManager.Rooms[i];
            totalReward += CalculateRoomReward(room, currentRoom, temperatureMovingAverage[i]);
        }

        AddReward(totalReward);
        lastWellBeing = UserWellBeingManager.WellBeing;
    }

    private float CalculateRoomReward(Room room, Room currentRoom, float averageTemperature)
    {
        float reward = 0f;
        float wellBeingChange = UserWellBeingManager.WellBeing - lastWellBeing;

        if (currentRoom == room && wellBeingChange > 0)
        {
            reward += wellBeingChange;
        }

        if (Mathf.Abs(averageTemperature - 23f) > TemperatureRangeThreshold)
        {
            reward -= Mathf.Abs(averageTemperature - 23f) * 0.05f;
        }

        return reward;
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