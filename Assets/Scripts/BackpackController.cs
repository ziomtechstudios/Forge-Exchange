using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Com.ZiomtechStudios.ForgeExchange.DragAndDropSlot;

namespace Com.ZiomtechStudios.ForgeExchange{
    public class BackpackController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Private Serialized Fields
        [SerializeField] private int numSlots;
        [SerializeField] private InventoryController m_InventoryCont;
        [SerializeField] private SlotController[] backPackSlots;
        [SerializeField] private SlotController[] quickSlots;
        [SerializeField] private GameObject movingSlot;
        [SerializeField] private SlotController movingSlotCont;
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private PlayerUIController m_PlayerUIController;
        #endregion
        #region Private Functions + Members
        private string ogSlotType;
        private int ogSlotIndex;
        private RectTransform movingSlotRectTransform;
        private RectTransform backPackRectTransform;
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
                    Debug.Log(slotIndex);
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
            //Update status of if all quick slots are full
            m_InventoryCont.AreAllSlotsFull();

        }
        //Store info of original item is contained in and move the item to the moving slot
        public void OnBeginDrag(PointerEventData eventData){
            SlotController selectedSlotCont = eventData.pointerPressRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>();
            if (selectedSlotCont != null && selectedSlotCont.SlotWithItem)
            {
                ogSlotType = selectedSlotCont.gameObject.transform.parent.name;
                ogSlotIndex = Int32.Parse(selectedSlotCont.gameObject.name.Remove(0, 4));
                EmptySlot();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(backPackRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
                movingSlotRectTransform.anchoredPosition = anchoredPosition;
            }
            //DragAndDropSlot.SelectItem(eventData, backPackRectTransform, movingSlotRectTransform, backPackSlots, m_InventoryCont.NoItemSprite, out ogSlotIndex);
        }
        //Move moving slot to coressponding current touch position
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(backPackRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
            movingSlotRectTransform.anchoredPosition = anchoredPosition;
            //DragAndDropSlot.MoveItem(eventData, backPackRectTransform, movingSlotRectTransform);
        }
        /// <summary>
        /// Moving slot is at destination
        /// Check to see if destination has a slot and if that slot is not being used
        /// If there is an empty slot remove item from moving slot and insert into destination slot
        /// if no empty slot send the item back into its original slot
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            SlotController destinationSlotCont = (eventData.pointerCurrentRaycast.gameObject == null)?(null):(eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>());
            if (destinationSlotCont != null && !destinationSlotCont.SlotWithItem)
                OccupySlot(destinationSlotCont.gameObject.transform.parent.name, Int32.Parse(destinationSlotCont.gameObject.name.Remove(0, 4)));
            else if(destinationSlotCont == null || destinationSlotCont.SlotWithItem)
                OccupySlot(ogSlotType, ogSlotIndex);
        }
        #endregion
        // Start is called before the first frame update
        void Awake(){
            m_InventoryCont = transform.parent.parent.parent.Find("InventorySlots").gameObject.GetComponent<InventoryController>();
            m_PlayerUIController = m_InventoryCont.transform.parent.parent.parent.gameObject.GetComponent<PlayerUIController>();
            movingSlot = transform.Find("Slot13").gameObject;
            movingSlotCont = movingSlot.GetComponent<SlotController>();
            movingSlotRectTransform = movingSlot.GetComponent<RectTransform>();    
            backPackSlots = new SlotController[numSlots];
            backPackRectTransform = GetComponent<RectTransform>();
            for(int i = 0; i < numSlots; i++)
                backPackSlots[i] = transform.Find($"Slot{i}").gameObject.GetComponent<SlotController>();
            quickSlots = new SlotController[m_InventoryCont.InventoryAmnt];
            for(int i=0;i<m_InventoryCont.InventoryAmnt;i++)
                quickSlots[i] = transform.Find($"QuickSlots/Slot{i}").gameObject.GetComponent<SlotController>();

        }
        public void OnEnable()
        {
            m_PlayerUIController.InGameQuickSlotObjs.SetActive(false);
        }
        public void OnDisable()
        {
            m_PlayerUIController.InGameQuickSlotObjs.SetActive(true);
        }
    }
}
