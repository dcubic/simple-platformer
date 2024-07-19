using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzSaw : MonoBehaviour {

    public Vector2 pointA;
    public Vector2 pointB;
    public float movementSpeed = 5f;
    public float rotationalSpeed = 500f;

    private Vector2 targetPoint;

    void Start() {
        targetPoint = pointA;
    }

    void FixedUpdate() {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPoint, movementSpeed * Time.fixedDeltaTime);

        if ((Vector2) transform.localPosition == targetPoint) {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }

        transform.Rotate(Vector3.forward, rotationalSpeed * Time.fixedDeltaTime);
    }
}
