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


    private int stepsSinceLastDecision = 0;
    private const int decisionInterval = 50;

    private int stepsSinceLastRewardCheck = 0;
    private const int checkRewardInterval = 5;

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

    private void FixedUpdate()
    {
        stepsSinceLastDecision++; //Decisions
        if (stepsSinceLastDecision >= decisionInterval)
        {
            RequestDecision();
            Debug.Log("Requested Decision");
            stepsSinceLastDecision = 0;
        }

        stepsSinceLastRewardCheck++; //Reward
        if (stepsSinceLastRewardCheck >= checkRewardInterval)
        {
            UpdateRewards();
            stepsSinceLastRewardCheck = 0;
        }
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
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
        {
            var heaterStatus = actions.DiscreteActions[i] == 1;
            RoomManager.Rooms[i].SetHeater(heaterStatus);
        }
    }

    private void UpdateRewards()
    {
        var currentWellBeing = UserWellBeingManager.WellBeing;

        var wellBeingChange = currentWellBeing - lastWellBeing;

        float currentAverageTemperature = 0f;
        foreach (var room in RoomManager.Rooms)
        {
            currentAverageTemperature += room.Temperature;
        }
        currentAverageTemperature /= RoomManager.Rooms.Length;
        bool isTemperatureStagnant = Mathf.Abs(currentAverageTemperature - lastAverageTemperature) < 1f;


       /* Debug.Log("currentWellBeing " + currentWellBeing + " ----- " 
                  + "lastWellBeing " + lastWellBeing + " ----- " 
                  + "currentAverageTemperature " + currentAverageTemperature + " ----- "
                  + "lastAverageTemperature " + lastAverageTemperature);*/

        float reward = CalculateNormalizedReward(currentWellBeing, wellBeingChange, isTemperatureStagnant);
        AddReward(reward);

        lastWellBeing = currentWellBeing;
        lastAverageTemperature = currentAverageTemperature;
    }

    private float CalculateNormalizedReward(float currentWellBeing, float wellBeingChange, bool isTemperatureStagnant)
    {
        float reward = 0f;

        if (currentWellBeing >= 9f) reward += 0.5f;
        if (wellBeingChange > 0) reward += 0.25f;
        if (wellBeingChange < 0) reward -= 0.25f;
        //if (isTemperatureStagnant && currentWellBeing < 5f) reward -= 0.25f;

        //return Mathf.Clamp(reward, 0f, 1f);

        return reward;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Heuristic method for testing the agent manually
        var discreteActionsOut = actionsOut.DiscreteActions;
        for (var i = 0; i < RoomManager.Rooms.Length; i++)
            discreteActionsOut[i] = Input.GetKey(KeyCode.H) ? 1 : 0; // Press 'H' to toggle heaters
    }
}