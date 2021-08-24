
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOveride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float weaponPercentageBonus = 0f;
        [SerializeField] bool isRighHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";


        public void Spawn(Transform rightHand, Transform leftHand,Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if(equippedPrefab != null)
            {
                GameObject weapon = Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOveride != null)
            {
                animator.runtimeAnimatorController = animatorOveride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) // mudar num possivel sistema de inventario - ao inves de destruir, adicione ao inventario
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRighHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, calculatedDamage, instigator);
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }
        public float GetWeaponRange()
        {
            return weaponRange;
        }
    }
}