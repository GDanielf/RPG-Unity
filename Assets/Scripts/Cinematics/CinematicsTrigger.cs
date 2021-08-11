using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics 
{
    public class CinematicsTrigger : MonoBehaviour
    {
        int triggerCinematicCounter = 1;
        private void OnTriggerEnter(Collider other)
        {            
            if(other.gameObject.CompareTag("Player") && triggerCinematicCounter > 0)
            {
                GetComponent<PlayableDirector>().Play();
                triggerCinematicCounter = triggerCinematicCounter - 1;
            }                
        }
    }
}


