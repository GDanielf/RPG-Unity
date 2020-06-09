using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] Transform target;
    
    void Update()
    {
        if (Input.GetMouseButton(0)){
            MoveToCursor();
        }
    }

    private void MoveToCursor(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool HasHit = Physics.Raycast(ray, out hit);
        if(HasHit){
            GetComponent<NavMeshAgent>().destination = hit.point;
        }
    }    
}
