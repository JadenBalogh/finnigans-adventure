using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followTime = 0.2f;
    [SerializeField] private float yBottomLimit = 1f;

    private Vector2 currVel;

    private void FixedUpdate()
    {
        Vector3 targetPos = Vector2.SmoothDamp(transform.position, target.position, ref currVel, followTime);
        Vector3 clampedTargetPos = new Vector2(targetPos.x, Mathf.Max(targetPos.y, yBottomLimit));
        transform.position = clampedTargetPos + Vector3.forward * transform.position.z;
    }
}
