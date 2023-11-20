using System.Collections;
using UnityEngine;

public class UserMovement : MonoBehaviour
{
    private bool _isMoving;
    private Vector3 _lastRoom = Vector3.positiveInfinity;
    private Vector3 _targetPosition;

    public RoomManager RoomManager;
    public float Speed = 1.0f;

    private void Start()
    {
        StartCoroutine(DailyRoutine());
    }

    private void Update()
    {
        if (_isMoving) MoveToTarget();
    }

    private IEnumerator DailyRoutine()
    {
        while (true)
        {
            if (TimeManager.Instance != null)
            {
                var currentTime = TimeManager.Instance.SimulatedTime;
                var currentHour = Mathf.FloorToInt(currentTime / 60) % 24;
                var dayOfWeek = Mathf.FloorToInt(currentTime / 1440) % 7; // 0 = Monday, 6 = Sunday

                var newRoom = DetermineCurrentRoom(currentHour, dayOfWeek);

                if (_lastRoom != newRoom) // Move only if the new room is different from the last
                {
                    _lastRoom = newRoom;
                    yield return StartCoroutine(MoveToRoom(newRoom));
                }
            }

            yield return null; // Wait for the next frame
        }
    }

    private Vector3 DetermineCurrentRoom(int currentHour, int dayOfWeek)
    {
        // Weekday Schedule
        if (dayOfWeek < 5)
        {
            if (currentHour is >= 8 and < 9) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour is >= 9 and < 12) return new Vector3(-10, 0, 0); // WorkRoom
            if (currentHour is >= 12 and < 13) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour is >= 13 and < 18) return new Vector3(-10, 0, 0); // WorkRoom
            if (currentHour is >= 18 and < 19) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour >= 19) return new Vector3(0, 0, 0); // LivingRoom
        }
        else // Weekend Schedule
        {
            if (currentHour is >= 9 and < 10) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour is >= 10 and < 13) return new Vector3(0, 0, 0); // LivingRoom
            if (currentHour is >= 13 and < 14) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour is >= 14 and < 19) return new Vector3(0, 0, 0); // LivingRoom
            if (currentHour is >= 19 and < 20) return new Vector3(-10, 0, -10); // Kitchen
            if (currentHour >= 20) return new Vector3(0, 0, 0); // LivingRoom
        }

        return new Vector3(0, 0, -10); // Default to BedRoom
    }

    private IEnumerator MoveToRoom(Vector3 roomPosition)
    {
        _targetPosition = roomPosition;
        _isMoving = true;

        while (Vector3.Distance(transform.position, _targetPosition) >
               0.1f) yield return null; // Wait for the next frame

        _isMoving = false;

        yield return new WaitForSeconds(0.01f);
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);
    }

    public Room GetCurrentRoom()
    {
        if (RoomManager == null) return null;

        var smallestDistance = float.MaxValue;
        Room currentRoom = null;

        foreach (var room in RoomManager.Rooms)
        {
            var distance = Vector3.Distance(transform.position, room.transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                currentRoom = room;
            }
        }

        return currentRoom;
    }
}