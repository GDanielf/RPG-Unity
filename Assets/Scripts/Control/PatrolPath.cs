using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField]
        const float wapointGizmosRadius = 0.2f;

        private void OnDrawGizmos()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), wapointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        // Static method could give some problems
        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount) return 0;
            return i + 1;            
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
