using System;
using UnityEngine;
using UnityEngine.EventSystems;


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
        [SerializeField] private RectTransform backPackRectTransform;
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
            m_InventoryCont.AreAllSlotsFull();
        }
        public void OnBeginDrag(PointerEventData eventData){
            SlotController selectedSlotCont = eventData.pointerPressRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>();
            if(selectedSlotCont != null && selectedSlotCont.SlotWithItem){
                ogSlotType = selectedSlotCont.gameObject.transform.parent.name;
                ogSlotIndex = Int32.Parse(selectedSlotCont.gameObject.name.Remove(0,4));
                EmptySlot();
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(backPackRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
            movingSlotRectTransform.anchoredPosition = anchoredPosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            SlotController destinationSlotCont = (eventData.pointerCurrentRaycast.gameObject != null)? eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>():null;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.name + eventData.pointerCurrentRaycast.gameObject.transform.parent.name);
            if (destinationSlotCont != null && !destinationSlotCont.SlotWithItem)
                OccupySlot(destinationSlotCont.gameObject.transform.parent.name, Int32.Parse(destinationSlotCont.gameObject.name.Remove(0, 4)));
            else
                OccupySlot(ogSlotType,ogSlotIndex);
        }

        #endregion
        // Start is called before the first frame update
        void Awake(){
            m_InventoryCont = transform.parent.parent.parent.Find("InventorySlots").gameObject.GetComponent<InventoryController>();
            movingSlot = transform.Find("Slot13").gameObject;
            movingSlotCont = movingSlot.GetComponent<SlotController>();
            movingSlotRectTransform = movingSlot.GetComponent<RectTransform>();    
            backPackSlots = new SlotController[numSlots];
            backPackRectTransform = transform.gameObject.GetComponent<RectTransform>();
            for(int i = 0; i < numSlots; i++)
                backPackSlots[i] = transform.Find($"Slot{i}").gameObject.GetComponent<SlotController>();
            quickSlots = new SlotController[m_InventoryCont.InventoryAmnt];
            for(int i=0;i<m_InventoryCont.InventoryAmnt;i++)
                quickSlots[i] = transform.Find($"QuickSlots/Slot{i}").gameObject.GetComponent<SlotController>();

        }
    }
}
