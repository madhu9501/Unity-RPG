using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyRPG
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject itemPrefab;
        public LayerMask playerLayer;
        public UnityEvent<ItemSpawner> onItemPickup;

        void Start()
        {
            Instantiate(itemPrefab, transform);
            Destroy(transform.GetChild(0).gameObject);

            onItemPickup.AddListener(FindObjectOfType<InventoryManager>().OnItemPickup);
        }

        void OnTriggerEnter(Collider other){

            if( 0 != (playerLayer.value & 1 << other.gameObject.layer)){
                onItemPickup.Invoke(this);

                // Destroy(gameObject);
            }
        }

    }
    
}

