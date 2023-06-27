using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.ZiomtechStudios.ForgeExchange{
    public class BackpackController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Private Serialized Fields
        [SerializeField] private int numSlots;
        [SerializeField] private InventoryController m_InventoryCont;
        [SerializeField] private SlotController[] backPackSlots;
        [SerializeField] private SlotController[] quickSlots;
        [SerializeField] private bool isDragging;
        [SerializeField] private GameObject movingSlot;
        [SerializeField] private SlotController movingSlotCont;
        #endregion
        #region Private Functions + Members
        private string ogSlotType;
        private int ogSlotIndex;
        private RectTransform movingSlotRectTransform;
        #endregion
        #region Getters/Setters
        public string OGSlotType{get{return ogSlotType;}}
        public int OGSlotIndex{get{return ogSlotIndex;}}
        #endregion
        #region Public Funcs
        public void EmptySlot(){
            switch (ogSlotType){
                case "Backpack":
                    //Moving item from Backpack to Moving Slot
                    movingSlotCont.ItemImage.sprite = backPackSlots[ogSlotIndex].ItemImage.sprite;
                    movingSlotCont.SlotWithItem = true;
                    movingSlotCont.ItemCont = backPackSlots[ogSlotIndex].ItemCont;
                    movingSlotCont.SlotPrefab = backPackSlots[ogSlotIndex].SlotPrefab;
                    //Emptying BackPackSlot
                    backPackSlots[ogSlotIndex].ItemImage.sprite = m_InventoryCont.NoItemSprite;
                    backPackSlots[ogSlotIndex].SlotWithItem = false;
                    backPackSlots[ogSlotIndex].ItemCont = null;
                    backPackSlots[ogSlotIndex].SlotPrefab = null;
                    break;
                case "QuickSlots":
                    //Moving item from Qickslotto Moving Slot
                    movingSlotCont.ItemImage.sprite = quickSlots[ogSlotIndex].ItemImage.sprite;
                    movingSlotCont.SlotWithItem = true;
                    movingSlotCont.ItemCont = quickSlots[ogSlotIndex].ItemCont;
                    movingSlotCont.SlotPrefab = quickSlots[ogSlotIndex].SlotPrefab;
                    //Emptying QuickSlot
                    quickSlots[ogSlotIndex].ItemImage.sprite = m_InventoryCont.NoItemSprite;
                    quickSlots[ogSlotIndex].SlotWithItem = false;
                    quickSlots[ogSlotIndex].ItemCont = null;
                    quickSlots[ogSlotIndex].SlotPrefab = null;
                    break;
                default:
                    break;
            }
        }
        public void OccupySlot(string slotType, int slotIndex){
            switch (slotType){
                case "Backpack":
                    //Sloting Item from moving slot to desured backpack slot
                    backPackSlots[slotIndex].ItemImage.sprite = movingSlotCont.ItemImage.sprite;
                    backPackSlots[slotIndex].SlotWithItem = true;
                    backPackSlots[slotIndex].ItemCont = movingSlotCont.ItemCont;
                    backPackSlots[slotIndex].SlotPrefab = movingSlotCont.SlotPrefab;
                    //Emptying Moving Slot
                    movingSlotCont.ItemImage.sprite = m_InventoryCont.NoItemSprite;
                    movingSlotCont.SlotWithItem = false;
                    movingSlotCont.ItemCont = null;
                    movingSlotCont.SlotPrefab = null;
                    break;
                case "QuickSlots":
                    //Sloting Item from moving slot to desured quickslot
                    quickSlots[slotIndex].ItemImage.sprite = movingSlotCont.ItemImage.sprite;
                    quickSlots[slotIndex].SlotWithItem = true;
                    quickSlots[slotIndex].ItemCont = movingSlotCont.ItemCont;
                    quickSlots[slotIndex].SlotPrefab = movingSlotCont.SlotPrefab;
                    //Emptying Moving Slot
                    movingSlotCont.ItemImage.sprite = m_InventoryCont.NoItemSprite;
                    movingSlotCont.SlotWithItem = false;
                    movingSlotCont.ItemCont = null;
                    movingSlotCont.SlotPrefab = null;
                    break;
                default:
                    break;
            }
        }
        public void SyncQuickSlots(string order)
        {
            switch (order) {
                //Make Quick Slots in menu match the ones in-game
                case "InGameToMenu":
                    for (int i = 0; i < m_InventoryCont.InventoryAmnt; i++)
                    {
                        quickSlots[i].ItemImage.sprite = m_InventoryCont.SlotConts[i].ItemImage.sprite;
                        quickSlots[i].SlotWithItem = m_InventoryCont.SlotConts[i].SlotWithItem;
                        quickSlots[i].ItemCont = m_InventoryCont.SlotConts[i].ItemCont;
                        quickSlots[i].SlotPrefab = m_InventoryCont.SlotConts[i].SlotPrefab;
                    }
                    break;
                //Make the quick slots in the in-game UI to match the ones in the inventory menu
                case "MenuToInGame":
                    for (int i = 0; i < m_InventoryCont.InventoryAmnt; i++)
                    {
                        m_InventoryCont.SlotConts[i].ItemImage.sprite = quickSlots[i].ItemImage.sprite;
                        m_InventoryCont.SlotConts[i].SlotWithItem = quickSlots[i].SlotWithItem;
                        m_InventoryCont.SlotConts[i].ItemCont = quickSlots[i].ItemCont;
                        m_InventoryCont.SlotConts[i].SlotPrefab = quickSlots[i].SlotPrefab;
                    }
                    break;
            }
        }
        public void OnBeginDrag(PointerEventData eventData){
            SlotController selectedSlotCont = eventData.pointerPressRaycast.gameObject.GetComponent<SlotController>();
            if(selectedSlotCont != null && selectedSlotCont.SlotWithItem){
                isDragging = true;
                ogSlotType = selectedSlotCont.gameObject.transform.parent.name;
                ogSlotIndex = Int32.Parse(selectedSlotCont.gameObject.name.Remove(0,3));
                EmptySlot();
                movingSlotRectTransform.anchoredPosition = eventData.pointerPressRaycast.screenPosition;
            }
            selectedSlotCont = null;
        }
        public void OnDrag(PointerEventData eventData){
            if(isDragging)
                movingSlotRectTransform.anchoredPosition = eventData.pointerPressRaycast.screenPosition;
        }
        public void OnEndDrag(PointerEventData eventData){
            isDragging = false;
            SlotController destinationSlotCont = eventData.pointerPressRaycast.gameObject.GetComponent<SlotController>();
            if(destinationSlotCont != null && !destinationSlotCont.SlotWithItem)
                OccupySlot(destinationSlotCont.gameObject.transform.parent.name, Int32.Parse(destinationSlotCont.gameObject.name.Remove(0,3)));
            destinationSlotCont = null;
        }
        #endregion
        // Start is called before the first frame update
        void Awake(){
            isDragging = false;
            gameObject.SetActive(true);
            m_InventoryCont = transform.parent.parent.parent.Find("InventorySlots").gameObject.GetComponent<InventoryController>();
            backPackSlots = new SlotController[numSlots];
            for(int i = 0; i < numSlots; i++)
                backPackSlots[i] = transform.Find($"Slot{i}").gameObject.GetComponent<SlotController>();
            quickSlots = new SlotController[m_InventoryCont.InventoryAmnt];
            for(int i=0;i<m_InventoryCont.InventoryAmnt;i++)
                quickSlots[i] = transform.Find($"QuickSlots/Slot{i}").gameObject.GetComponent<SlotController>();
            movingSlot = transform.Find("Slot13").gameObject;
            movingSlotCont = movingSlot.GetComponent<SlotController>();
            movingSlotRectTransform = movingSlot.GetComponent<RectTransform>();
            movingSlotCont.ItemImage.sprite = m_InventoryCont.NoItemSprite;
        }
    }
}
