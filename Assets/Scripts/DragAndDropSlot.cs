using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Com.ZiomtechStudios.ForgeExchange
{
    public static class DragAndDropSlot{
        public static void EmptySlot(int index, SlotController[] slots, SlotController movingSlotCont, Sprite noItemSprite)
        {
            //Moving item from Selected Slot to Moving Slot
            movingSlotCont.ItemImage.sprite = slots[index].ItemImage.sprite;
            movingSlotCont.SlotWithItem = true;
            movingSlotCont.ItemCont = slots[index].ItemCont;    
            movingSlotCont.SlotPrefab = slots[index].SlotPrefab;
            //Emptying Selected Slot 
            slots[index].ItemImage.sprite = noItemSprite;
            slots[index].SlotWithItem = false;
            slots[index].ItemCont = null;
            slots[index].SlotPrefab = null;
        }
        public static void OccupySlot(int index, SlotController[] slots, SlotController movingSlotCont, Sprite noItemSprite)
        {
            
        }
        public static void SelectItem(PointerEventData eventData, RectTransform canvasRectTransform, RectTransform movingSlotRectTransform, SlotController[] slots, Sprite noItemSprite, out int ogSlotIndex)
        {
            SlotController selectedSlotCont = (eventData.pointerCurrentRaycast.gameObject == null) ? (null) : (eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>());
            ogSlotIndex = 0;
            if (selectedSlotCont != null && selectedSlotCont.SlotWithItem)
            {
                ogSlotIndex = Int32.Parse(selectedSlotCont.gameObject.name.Remove(0, 4));
                EmptySlot(ogSlotIndex, slots, selectedSlotCont, noItemSprite);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
                movingSlotRectTransform.anchoredPosition = anchoredPosition;
            }
        }

        public static void MoveItem(PointerEventData eventData, RectTransform canvasRectTransform, RectTransform movingSlotRectTransform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
            movingSlotRectTransform.anchoredPosition = anchoredPosition;
        }
        public static void DropItem(PointerEventData eventData)
        {

        }


    }
}
