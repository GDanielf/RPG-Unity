using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematic 
{
    public class CinematicsTrigger : MonoBehaviour, ISaveable
    {
        bool isPlayed = false;

        private void OnTriggerEnter(Collider other)
        {            
            if(other.gameObject.CompareTag("Player") && !isPlayed)
            {
                GetComponent<PlayableDirector>().Play();
                isPlayed = true;
            }                
        }

        //save-load state
        public object CaptureState()
        {
            return isPlayed;
        }

        public void RestoreState(object state)
        {
            bool playedState = (bool)state;
            playedState = isPlayed;
            if (isPlayed == false) isPlayed = true; 
        }
    }
}


