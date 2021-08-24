using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control 
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicousTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointStayTime = 1f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Mover mover;
        Fighter fighter;
        GameObject player;
        Health health;
        ActionSchedule actionSchedule;
        
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        LazyValue<Vector3> guardPosition;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            actionSchedule = GetComponent<ActionSchedule>();
            player = GameObject.FindWithTag("Player");

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            guardPosition.ForceInit();           
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (DistanceToPlayer(player) && fighter.CanAttack(player))
            {                
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicousTime)
            {
                SuspiciousBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {                    
                    CycleWaypoint();
                    timeSinceLastWaypoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceLastWaypoint > waypointStayTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }            
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);   
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspiciousBehaviour()
        {
            actionSchedule.CancelCurrentAction();            
            print("Suspiciousssss!!!");
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool DistanceToPlayer(GameObject player)
        {            
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }        
    }
}
