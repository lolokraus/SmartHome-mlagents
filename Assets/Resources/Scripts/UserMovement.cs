using System.Collections;
using UnityEngine;

public class UserMovement : MonoBehaviour
{
    private bool isMoving;
    public float speed = 1.0f;
    private Vector3 targetPosition;

    private void Start()
    {
        StartCoroutine(DailyRoutine());
    }

    private void Update()
    {
        if (isMoving) MoveToTarget();
    }

    private IEnumerator DailyRoutine()
    {
        while (true) // Loop to start the routine again after a day
        {
            // Example routine, adjust times and activities
            yield return StartCoroutine(GoToRoom(new Vector3(-10, 0, -10), 7)); // Kitchen for breakfast
            yield return StartCoroutine(GoToRoom(new Vector3(-10, 0, 0), 8)); // WorkRoom
            yield return StartCoroutine(GoToRoom(new Vector3(-10, 0, -10), 1)); // Kitchen for lunch
            yield return StartCoroutine(GoToRoom(new Vector3(0, 0, 0), 2)); // LivingRoom
            yield return StartCoroutine(GoToRoom(new Vector3(-10, 0, 0), 4)); // Back to WorkRoom
            yield return StartCoroutine(GoToRoom(new Vector3(0, 0, -10), 8)); // BedRoom for sleep
        }
    }

    private IEnumerator GoToRoom(Vector3 roomPosition, int hoursSpent)
    {
        targetPosition = roomPosition;
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) yield return null;

        isMoving = false;
        yield return new WaitForSeconds(hoursSpent * 60); // Simulate time spent in the room
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}