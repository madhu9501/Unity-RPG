using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public partial class Damageable : MonoBehaviour
    {
        public struct DamageMessage{
            public MonoBehaviour damager;
            public int amount;
            public GameObject damageSource;     
        }
    }
    
}

