using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class InventoryController : MonoBehaviour{
        #region Serialized Fields
        [Tooltip("Amount of inventory slots.")][SerializeField] private int inventoryAmnt;
        [Tooltip("Sprite of what item(s) occupy any given slot")][SerializeField] private Image[] itemImages;
        [Tooltip("Sprite of what background for each slot.")][SerializeField] private Image[] slotImages;
        [Tooltip("Which slot is selected by users.")][SerializeField] private bool[] slotsInUse;
        [Tooltip("Which slots have item(s).")][SerializeField] private bool[] slotsWithItems;
        [Tooltip("Data Struct associated with each item occupying a slot.")] private ItemStruct[] itemStructs;
        [SerializeField] private PlayerController playerCont;
        [Tooltip("Are all of the items equiped with an item?")][SerializeField] private bool slotsAreFull;
        [Tooltip("Sprite used by slot to indicate there is no item.")][SerializeField] private Sprite noItemSprite;
        #endregion
        #region Private Funcs
        private bool ToggleHolding(int index){
            //If the slot selected has an item the player holds the item
            playerCont.HoldingItem = (slotsWithItems[index] && !slotsInUse[index]);
            //Update sprite of what player is holding to that of what was in the selected slot
            playerCont.HoldingSprite = (playerCont.HoldingItem)?(itemImages[index].sprite):(null);
            playerCont.HoldingStruct = (playerCont.HoldingItem)?(itemStructs[index]):(null);
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
                for(int i = 0; i < (inventoryAmnt-1); i++){
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
            if(playerCont.HoldingItem){
                for(int i = 0; i<inventoryAmnt; i++){
                    if((itemImages[i].sprite == playerCont.HoldingSprite) &&(slotsInUse[i])){
                        playerCont.HoldingSprite = null;
                        playerCont.HoldingItem = false;
                        playerCont.HoldingStruct = null;
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
                for(int i = 0; i<inventoryAmnt; i++){ 
                    if(slotsWithItems[i] == false){
                        slotsWithItems[i] = true;
                        itemImages[i].sprite = playerCont.HoldingSprite;
                        itemStructs[i] = playerCont.HoldingStruct;
                        playerCont.HoldingItem = false;
                        playerCont.HoldingSprite = null;
                        playerCont.HoldingStruct = null;
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
            itemImages = new Image[inventoryAmnt];
            slotsWithItems = new bool[inventoryAmnt];
            itemStructs = new ItemStruct[inventoryAmnt];
            slotsAreFull = false;
            for(int i = 0; i < inventoryAmnt; i++){
                slotImages[i] = transform.Find($"Slot{i}").gameObject.GetComponent<Image>();
                itemImages[i] = slotImages[i].transform.Find($"Item{i}").gameObject.GetComponent<Image>();
                slotsInUse[i] = false;
                slotImages[i].fillCenter = !(slotsInUse[i]);
                slotsWithItems[i] = false;
                itemStructs[i] = null;
            }
        }
        void Update(){
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
                case "4":
                    SelectSlot(3);
                    break;
                default:
                    break;
            }   
            slotsAreFull = Array.TrueForAll(slotsWithItems, slotWithItems => slotWithItems == true);
         }
    }
}