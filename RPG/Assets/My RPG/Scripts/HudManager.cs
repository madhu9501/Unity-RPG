using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyRPG
{
    public class HudManager : MonoBehaviour
    {
        public Slider healthSlider;

        public void SetMaxHealth(int maxHealth){
            healthSlider.maxValue = maxHealth;
            SetHealth(maxHealth);
        }

        public void SetHealth(int currentHealth){
            healthSlider.value =  currentHealth;
        }
    }
}

