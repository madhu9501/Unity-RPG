using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public class UniqueId : MonoBehaviour
    {
        [SerializeField]
        private string uid = Guid.NewGuid().ToString();
        public string Uid { get{ return uid;} }

    }
    
}

