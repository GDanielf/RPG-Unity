using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] LazyValue<float> healthPoints; //you would never be able to obtain unles it's a initialized value        
        [Range(0,1)]
        [SerializeField] float regenerationLevelUpPercentage = 0.7f;
        //SFX events
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent OnDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        bool isDead = false;
        BaseStats stat;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();            
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += GainHpOnLevelUP;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= GainHpOnLevelUP;
        }


        public bool IsDead() 
        {
            return isDead;
        }        

        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage: " + damage);            

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            print(healthPoints.value);
            if (healthPoints.value == 0)
            {
                OnDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));        
        }

        private void GainHpOnLevelUP()
        {
            float regenerateHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationLevelUpPercentage;
            healthPoints.value = Mathf.Max(healthPoints.value, regenerateHealthPoints);
        }
         

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }     
        public float GetFraction()
        {
            return healthPoints.value/GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value =  Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionSchedule>().CancelCurrentAction();            
        }

        //save-load state
        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;            
            if(healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}
