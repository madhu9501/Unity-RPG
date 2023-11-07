using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyRPG
{
    public class InventoryManager : MonoBehaviour
    {
        public List<InventorySlots> inventory = new List<InventorySlots>();
        public Transform inventoryPanel;
        private int inventorySize;

        void Awake(){
            inventorySize = inventoryPanel.childCount;
            CreateInventory(inventorySize);
        }

        void CreateInventory(int size){
            for (int i = 0; i < size; i++ ){
                inventory.Add(new InventorySlots(i));
                RegisterSlotHandler(i);
            }
        }

        public void OnItemPickup(ItemSpawner spawner){
            AddItemFrom(spawner);
        }

        public void AddItemFrom(ItemSpawner spawner)
        {
            InventorySlots inventorySlot = GetFreeSlots();
            if(inventorySlot == null){ return; }

            inventorySlot.Place(spawner.itemPrefab);
            inventoryPanel.GetChild(inventorySlot.index).GetComponentInChildren<Text>().text = spawner.itemPrefab.name;
            Destroy(spawner.gameObject);
            
        }

        private InventorySlots GetFreeSlots(){
            return inventory.Find(slots => slots.itemName == null);
        }

        private void RegisterSlotHandler(int index){
            Button slotBtn = inventoryPanel.GetChild(index).GetComponentInChildren<Button>();
            slotBtn.onClick.AddListener(() => { UseItem(index);}  );

        }

        private void UseItem(int index){
            InventorySlots inventorySlot = GetSlotByIndex(index);
            if(inventorySlot.itemPrefab == null){ return; }
            PlayerMovement.Instance.UseItemFrom(inventorySlot);

        }

        private InventorySlots GetSlotByIndex(int index){
            return inventory.Find(slot => slot.index == index);
        }




    }
}

