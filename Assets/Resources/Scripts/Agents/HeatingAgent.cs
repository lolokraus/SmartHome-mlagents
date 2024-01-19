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

    private const float WellBeingThreshold = 2f;

    public override void OnEpisodeBegin()
    {
        Debug.Log("Reset Simulation");
        foreach (var room in RoomManager.Rooms)
        {
            room.Temperature = 23;
            room.SetHeater(false);
            room.EnergyConsumption = 0f;
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.SimulatedTime = 0;
            TimeManager.Instance.DaysPassed = 0;
        }

        if (UserMovement.Instance != null)
        {
            UserMovement.Instance.ResetToStartingPosition();
        }

        UserWellBeingManager.WellBeing = 10f;
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

                // Optional: Early termination if well-being is too low
                if (UserWellBeingManager.WellBeing < WellBeingThreshold)
                {
                    Debug.Log("Ended early");
                    EndEpisode();
                }
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Room currentUserRoom = UserWellBeingManager.User.GetCurrentRoom();

        foreach (var room in RoomManager.Rooms)
        {
            float normalizedTemp = (room.Temperature - 23f) / 10f;
            sensor.AddObservation(normalizedTemp);
            sensor.AddObservation(room.IsHeaterOn ? 1 : 0);
            sensor.AddObservation(room == currentUserRoom ? 1f : 0f);
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
        Room currentUserRoom = UserWellBeingManager.User.GetCurrentRoom();

        float currentWellBeing = UserWellBeingManager.WellBeing;

        AddReward(currentWellBeing/10);

        foreach (var room in RoomManager.Rooms)
        {
            if (room != currentUserRoom && !room.IsHeaterOn)
            {
                AddReward(1);
            }
        }
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