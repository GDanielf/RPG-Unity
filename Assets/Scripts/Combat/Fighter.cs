using UnityEngine;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Core;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;


namespace RPG.Combat{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {        
        [SerializeField] float timeBetweenAttacks = 1f;        
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        float timeLastAtack = 0;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();          
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

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHand, leftHand, animator);
        }

        public Health GetTarget()
        {
            return target;
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
            GetComponent<Animator>().SetTrigger("attack");
            GetComponent<Animator>().ResetTrigger("stopAttack");
        }

        //Animation 
        void Hit()
        {

            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage); 
            
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHand, leftHand, target, gameObject, damage);
            }
            else
            {                
                target.TakeDamage(gameObject, damage);
            }
            
        }

        void Shoot()
        {
            Hit();
        }

        public bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetWeaponRange();
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

        //Additive and perrcentage bonus statements
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();  
            }
            
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }


        //Save statement
        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
        
    }
}