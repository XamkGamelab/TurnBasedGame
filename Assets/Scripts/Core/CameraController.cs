using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Transform Target;
    public float SmoothingSpeed = 5f;

    private Vector3 camAnimationTarget;
    void Update()
    {
        camAnimationTarget = new Vector3(Target.position.x, transform.position.y, Target.position.z);
        transform.position = Vector3.Lerp(transform.position, camAnimationTarget, Time.deltaTime * SmoothingSpeed);
    }
}
