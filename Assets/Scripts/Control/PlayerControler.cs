
using System;
using RPG.Combat;
using UnityEngine;
using RPG.Movement;


    public class PlayerControler : MonoBehaviour{
        private void Update() {
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
            print("Nothing to do.");
        }

        private bool InteractWithCombat(){
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits){
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if(target == null) continue;

                if(Input.GetMouseButtonDown(0)){
                    GetComponent<Fighter>().Attack(target);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement(){            
            RaycastHit hit;
            bool HasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(HasHit){
                if(Input.GetMouseButton(0)){
                    GetComponent<Mover>().MoveTo(hit.point);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay(){
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }


