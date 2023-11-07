using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    [System.Serializable]
    public class InventorySlots 
    {

        public int index;
        public GameObject itemPrefab;
        public string itemName;

        public InventorySlots(int index){
            this.index = index;            
        }

        public void Place(GameObject item){
            itemName = item.name;
            itemPrefab = item;
        }
    }

}
