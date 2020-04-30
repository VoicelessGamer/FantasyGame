using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderController : MonoBehaviour {

    public enum State {
        IDLE,
        WALK
    }

    public Animator animator;

    public GameObject targetBody;
    public float maxDistanceFromTarget;

    public float maxWalkSpeed;
    public float walkSpeedIncreaseRate;

    public float maxRotationSpeed;
    
    private float currentSpeedModifier = 0f;

    private float sqrMaxDistanceFromTarget;

    private State state = State.IDLE;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();

        sqrMaxDistanceFromTarget = maxDistanceFromTarget * maxDistanceFromTarget;
    }

    // Update is called once per frame
    void Update() {
    }

    private void FixedUpdate() {

        Vector3 dirTowardTargetObject = targetBody.transform.position - transform.position;

        if (dirTowardTargetObject.sqrMagnitude <= sqrMaxDistanceFromTarget) {
            state = State.IDLE;
            currentSpeedModifier = 0;
        } else {
            state = State.WALK;

            currentSpeedModifier = Mathf.Clamp(currentSpeedModifier + (Time.deltaTime * walkSpeedIncreaseRate), 0, 1);

            float speed = currentSpeedModifier * maxWalkSpeed;

            dirTowardTargetObject.Normalize();
            dirTowardTargetObject.y = transform.position.y;

            transform.position += (dirTowardTargetObject * speed * Time.deltaTime);
        }

        animator.SetFloat("Speed", currentSpeedModifier);

        Vector3 rotation = dirTowardTargetObject;
        rotation.y = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotation), maxRotationSpeed * Time.deltaTime);
    }
}
