using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {        
        [SerializeField] float projectileSpeed = 5f;
        
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject impactParticle = null;
        [SerializeField] float maxFlyingTime = 3f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        //SFX event
        [SerializeField] UnityEvent onHit; 

        Health target = null; //(1) o this se refere a esse target
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            if(!isHoming)
                transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming && !target.IsDead())
                transform.LookAt(GetAimLocation());           
                    
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);            
            
        }        

        private void OnTriggerEnter(Collider other)
        {            
            if (other.gameObject.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);
            projectileSpeed = 0;

            onHit.Invoke();
            if (impactParticle != null)
            {
               Instantiate(impactParticle, GetAimLocation(), Quaternion.identity);
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);           
            
        }

        public void SetTarget(Health target, float damage, GameObject instigator)//esse eh o target atribuido ao this.target
        {
            this.target = target; //comentario para identificar cada target (1)
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxFlyingTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height/2;
        }
    }
}
