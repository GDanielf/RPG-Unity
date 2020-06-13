
using System;
using RPG.Combat;
using UnityEngine;
using RPG.Movement;


    public class PlayerControler : MonoBehaviour{
        private void Update() {
            InteractWithCombat();
            InteractWithMovement();
        }

        private void InteractWithCombat(){
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits){
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if(target == null) continue;

                if(Input.GetMouseButtonDown(0)){
                    GetComponent<Fighter>().Attack(target);
                }
            }
        }

        private void InteractWithMovement(){
            if (Input.GetMouseButton(0)){
                MoveToCursor();
            }
        }

        private void MoveToCursor(){
            RaycastHit hit;
            bool HasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(HasHit){
                GetComponent<Mover>().MoveTo(hit.point);
            }
        }

        private static Ray GetMouseRay(){
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }


