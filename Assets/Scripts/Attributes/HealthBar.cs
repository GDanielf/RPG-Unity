using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {                
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;


        private void Update()
        {
            if (foreground == null) return;
            if (healthComponent == null) return;
            
            if(Mathf.Approximately(healthComponent.GetFraction(), 0))
            {
                rootCanvas.enabled = false;
                return;
            }
            else if(Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = true;
                return;
            }
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
            
        }
    }
}
