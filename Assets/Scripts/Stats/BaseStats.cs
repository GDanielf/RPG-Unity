using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Stats
{    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass charachterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        //add an onlevelup event/deçlegate
        

        public event Action onLevelUp;
        LazyValue<int> currentLevel;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat))*(1 + GetPercentageModifier(stat)/100);
        }

        

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, charachterClass, GetLevel());
        }

        public int GetLevel()
        {
            
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            //if (!shouldUseModifiers) return 0f;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0f;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = GetComponent<Experience>().GetExperience();
            int penultimateLeve = progression.GetLevels(Stat.ExperienceToLevelUp, charachterClass);
            for (int level = 1; level <= penultimateLeve; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, charachterClass, level);
                if(currentXP < xpToLevelUp)
                {
                    return level;
                }
            }

            return penultimateLeve + 1;
        }


    }
}

