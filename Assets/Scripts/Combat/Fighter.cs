using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 10f;

        Health target;
        Animator attackAnimat;

        float timeLastAtack = 0;

        private void Start()
        {
            attackAnimat = GetComponent<Animator>();
            attackAnimat.ResetTrigger("attack");
        }

        private void Update()
        {
            timeLastAtack += Time.deltaTime;

            if(target == null) return;

            if (target.IsDead()) return;

            if(!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);                
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeLastAtack > timeBetweenAttacks) 
            {
                // This will trigger the Hit() event
                TriggerAttack();                
                timeLastAtack = 0;
            }            
        }

        private void TriggerAttack()
        {
            attackAnimat.SetTrigger("attack");
            GetComponent<Animator>().ResetTrigger("stopAttack");
        }

        //Animation
        void Hit()
        {
            if(target == null) { return; }
            target.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionSchedule>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }        
        
        private void StopAttack()
        {
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Animator>().ResetTrigger("attack");
        }
    }
}