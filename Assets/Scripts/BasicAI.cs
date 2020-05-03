using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson {
    public class BasicAI : MonoBehaviour {
        Animator anim;

        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        Vector2 smoothDeltaPosition = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        public enum State {
            PATROL,
            CHASE
        }

        public State state;

        private bool alive;

        //patrol variables
        public GameObject[] waypoints;
        public float patrolSpeed = 0.5f;

        private int waypointIndex = 0;

        //chase variables
        public float chaseSpeed = 1f;
        public GameObject target;

        private void Awake() {
            anim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
        }

        void Start() {

            agent.updatePosition = false;
            agent.updateRotation = false;

            state = State.PATROL;

            alive = true;

            //start finite state machine
            StartCoroutine(finiteStateMachine());
        }

        IEnumerator finiteStateMachine() {
            while(alive) {
                switch(state) {
                    case State.PATROL:
                        patrol();
                        break;
                    case State.CHASE:
                        chase();
                        break;
                }

                yield return null;
            }
        }

        private void patrol() {
            agent.speed = patrolSpeed;
            if(Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) >= 10) {
                agent.SetDestination(waypoints[waypointIndex].transform.position);
                movement();
            } else if(Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 10) {
                waypointIndex++;
                waypointIndex = waypointIndex == waypoints.Length ? 0 : waypointIndex;
            } else {
                character.Move(Vector3.zero, false, false);
            }
        }

        private void chase() {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            movement();
        }

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player") {
                state = State.CHASE;
                target = other.gameObject;
            }
        }
        void movement() {
            character.Move(agent.desiredVelocity, false, false);

            Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

            // Update animation parameters
            anim.SetBool("move", shouldMove);
            anim.SetFloat("velx", velocity.x);
            anim.SetFloat("vely", velocity.y);

            //GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
        }

        void OnAnimatorMove() {
            // Update position to agent position
            transform.position = agent.nextPosition;
        }
    }
}