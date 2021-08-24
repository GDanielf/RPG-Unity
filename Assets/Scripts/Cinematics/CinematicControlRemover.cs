using RPG.Control;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        private void Awake()
        {
            player = GameObject.FindWithTag("Player");            
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().stopped += EnableControl;
            GetComponent<PlayableDirector>().played += DisableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().stopped -= EnableControl;
            GetComponent<PlayableDirector>().played -= DisableControl;
        }
        private void DisableControl(PlayableDirector obj)
        {
            //print("Disable Control");
            player.GetComponent<PlayerControler>().enabled = false;
            player.GetComponent<ActionSchedule>().CancelCurrentAction();
        }

        private void EnableControl(PlayableDirector obj)
        {
            //print("Enable Control");
            player.GetComponent<PlayerControler>().enabled = true;
        }
    }
}

