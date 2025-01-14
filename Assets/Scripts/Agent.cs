using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {
    public int currentCell;
    public int currentLane;

    public int currentSpeed, maxSpeed;
    public int nextLane;

    private Vector3 targetPosition;
    private float moveDuration = 1.0f;

    public void Initialize(int _cell, int _lane, int _maxSpeed) {
        currentCell = _cell;
        currentLane = _lane;
        maxSpeed = _maxSpeed;
        transform.position = new Vector3(currentCell, 0, currentLane);
        targetPosition = transform.position;
    }
    
    public void ResetPosition() {
        currentCell = 0;
        currentLane = nextLane;
        transform.position = new Vector3(0, 0, currentLane);
    }

    public void MoveTo(int newCell, int newLane) {
        currentCell = newCell;
        currentLane = newLane;
        targetPosition = new Vector3(currentCell, 0, currentLane);
        StartCoroutine(SmoothMove(transform.position, targetPosition, moveDuration));
    }

    private IEnumerator SmoothMove(Vector3 start, Vector3 end, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }
}