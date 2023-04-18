using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class InventoryController : MonoBehaviour{
        #region Serialized Fields
        [Tooltip("Amount of inventory slots.")][SerializeField] private int inventoryAmnt;
        [Tooltip("Sprite of what background for each slot.")][SerializeField] private Image[] slotImages;
        [Tooltip("Which slot is selected by users.")][SerializeField] private bool[] slotsInUse;
        [Tooltip("Which slots have item(s).")][SerializeField] private bool[] slotsWithItems;
        [Tooltip("UI Images that show the given sprite of the items in the inventory")] [SerializeField] private Image[] itemImages;
        [Tooltip("Prefab correlating to inventoried items.")][SerializeField] private GameObject[] slotPrefabs;
        [Tooltip("ItemController for correlating inventory items")][SerializeField] private ItemController[] itemConts;
        [SerializeField] private PlayerController playerCont;
        [Tooltip("Are all of the items equiped with an item?")][SerializeField] private bool slotsAreFull;
        [Tooltip("Sprite used by slot to indicate there is no item.")][SerializeField] private Sprite noItemSprite;
        #endregion
        #region Private Funcs
        private bool ToggleHolding(int index){
            //If the slot selected has an item the player holds the item
            playerCont.HoldingItem = (slotsWithItems[index] && !slotsInUse[index]);
            //Update sprite of what player is holding to that of what was in the selected slot
            playerCont.HoldingPrefab = (playerCont.HoldingItem)?(slotPrefabs[index]):(null);
            playerCont.HoldingCont = (playerCont.HoldingItem)?(itemConts[index]):(null);
            return (!slotsInUse[index]);
        }
        //Player selects which slot in their inventory they want to select, makes that obj the one the player is holding
        private void SelectSlot(int slotIndex){
            //Makes it so that no slot is selected
            if(slotIndex != (-1)){
                for(int i = 0; i<inventoryAmnt; i++){
                    slotsInUse[i] = (i == slotIndex)?(ToggleHolding(i)):false;
                    slotImages[i].fillCenter = !slotsInUse[i];
                }
            }
            //selected slot is highlighted
            else{
                for(int i = 0; i <=(inventoryAmnt-1); i++){
                    slotsInUse[i] = false;
                    slotImages[i].fillCenter = !slotsInUse[i];
                }
            }
        }
        #endregion
        #region Getters/Setters
        public bool SlotsAreFull{get{return slotsAreFull;}}
        #endregion
        #region Public funcs
        public void DroppingItem(){
            //If the player is holding item we look for coresponding slot holding said item
            if(playerCont.HoldingItem){
                for(int i = 0; i<inventoryAmnt; i++){
                    if((slotPrefabs[i] == playerCont.HoldingPrefab) &&(slotsInUse[i])){
                        //desired slot found
                        //Empty players hands
                        //Empty slot
                        playerCont.HoldingPrefab = null;
                        playerCont.HoldingCont = null;
                        itemImages[i].sprite = noItemSprite;
                        slotsWithItems[i] = false;
                        SelectSlot(-1);
                        break;
                    }
                }
            }
        }
        public void SlotItem(){
            //If the player is holding an object and all their slots are not occupied
            if(playerCont.HoldingItem && !(slotsAreFull)){
                //iterating through slots we find the first empty slot
                for(int i = 0; i<=(inventoryAmnt-1); i++){ 
                    if(slotsWithItems[i] == false){
                        //Fill slot with item
                        slotsWithItems[i] = true;
                        slotPrefabs[i] = playerCont.HoldingPrefab;
                        itemConts[i] = playerCont.HoldingCont;
                        itemImages[i].sprite = playerCont.HoldingPrefab.GetComponent<SpriteRenderer>().sprite;
                        //Empty players hands
                        playerCont.HoldingItem = false;
                        playerCont.HoldingPrefab = null;
                        playerCont.HoldingCont = null;
                        break;
                    }
                }
            }        
        }
        #endregion
        // Start is called before the first frame update
        void Start(){
            playerCont = transform.parent.parent.parent.GetComponent<PlayerController>();
            slotImages = new Image[inventoryAmnt];
            slotsInUse = new bool[inventoryAmnt];
            slotsWithItems = new bool[inventoryAmnt];
            slotPrefabs = new GameObject[inventoryAmnt];
            itemConts = new ItemController[inventoryAmnt];
            itemImages = new Image[inventoryAmnt];
            slotsAreFull = false;
            //Setting inventory to empty, should change in future when saves are implemented
            for(int i = 0; i<inventoryAmnt; i++){
                slotImages[i] = transform.Find($"Slot{i}").gameObject.GetComponent<Image>();
                itemImages[i] = slotImages[i].transform.Find($"Item{i}").gameObject.GetComponent<Image>();
                slotsInUse[i] = false;
                slotImages[i].fillCenter = !(slotsInUse[i]);
                slotsWithItems[i] = false;
                slotPrefabs[i] = null;
            }
        }
        void Update(){
            //Corresponding slot mapped to number keys on keyboard
            //Future implementation to include inputs via inout manager
            switch(Input.inputString){
                case"1":
                    SelectSlot(0);
                    break;
                case"2":
                    SelectSlot(1);
                    break;
                case"3":
                    SelectSlot(2);
                    break;
                case"4":
                    SelectSlot(3);
                    break;
                default:
                    break;
            } 
            //Helps avoid non-needed work  
            slotsAreFull = Array.TrueForAll(slotsWithItems, slotWithItems => slotWithItems == true);
         }
    }
}