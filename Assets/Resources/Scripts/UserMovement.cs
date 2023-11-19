using System.Collections;
using UnityEngine;

public class UserMovement : MonoBehaviour
{
    public float Speed = 1.0f;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private Vector3 _lastRoom = Vector3.positiveInfinity;

    private void Start()
    {
        StartCoroutine(DailyRoutine());
    }

    private void Update()
    {
        if (_isMoving)
        {
            MoveToTarget();
        }
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

                Vector3 newRoom = DetermineCurrentRoom(currentHour, dayOfWeek);

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
            if (currentHour >= 6 && currentHour < 9) return new Vector3(-10, 0, -10); // Kitchen
            else if (currentHour >= 9 && currentHour < 18) return new Vector3(-10, 0, 0); // WorkRoom
            else if (currentHour >= 18) return new Vector3(0, 0, 0); // LivingRoom
        }
        else // Weekend Schedule
        {
            if (currentHour >= 8) return new Vector3(0, 0, 0); // LivingRoom
        }

        return new Vector3(0, 0, -10); // Default to BedRoom
    }

    private IEnumerator MoveToRoom(Vector3 roomPosition)
    {
        _targetPosition = roomPosition;
        _isMoving = true;

        while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            yield return null; // Wait for the next frame
        }

        _isMoving = false;
        // Wait for a bit before the next action
        yield return new WaitForSeconds(0.01f);
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);
    }
}