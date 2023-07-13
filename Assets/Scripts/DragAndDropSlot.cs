using Mono.CompilerServices.SymbolWriter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Com.ZiomtechStudios.ForgeExchange
{
    public static class DragAndDropSlot{
        public static void SelectItem(PointerEventData eventData, RectTransform canvasRectTransform, RectTransform movingSlotRectTransform, SlotController movingSlotCont, SlotController[] slots, Sprite noItemSprite, out int ogSlotIndex, out string ogSlotType)
        {
            SlotController selectedSlotCont = (eventData.pointerPressRaycast.gameObject == null) ? null : eventData.pointerPressRaycast.gameObject.transform.parent.gameObject.GetComponent<SlotController>();
            ogSlotIndex = 0;
            ogSlotType = "";
            if (selectedSlotCont != null && selectedSlotCont.SlotWithItem)
            {
                ogSlotIndex = Int32.Parse(selectedSlotCont.gameObject.name.Remove(0, 4));
                ogSlotType = eventData.pointerPressRaycast.gameObject.transform.parent.parent.name;
                movingSlotCont.gameObject.SetActive(true);
                //EmptySlot(ogSlotIndex,  slots, selectedSlotCont, noItemSprite);
                //Moving item from Selected Slot to Moving Slot
                movingSlotCont.ItemImage.sprite = slots[ogSlotIndex].ItemImage.sprite;
                movingSlotCont.SlotWithItem = true;
                movingSlotCont.ItemCont = slots[ogSlotIndex].ItemCont;
                movingSlotCont.SlotPrefab = slots[ogSlotIndex].SlotPrefab;
                //Emptying Selected Slot 
                slots[ogSlotIndex].ItemImage.sprite = noItemSprite;
                slots[ogSlotIndex].SlotWithItem = false;
                slots[ogSlotIndex].ItemCont = null;
                slots[ogSlotIndex].SlotPrefab = null;
            }
        }
        public static void MoveItem(PointerEventData eventData, RectTransform canvasRectTransform, RectTransform movingSlotRectTransform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 anchoredPosition);
            movingSlotRectTransform.anchoredPosition = anchoredPosition;
        }
        public static void DropItem(SlotController movingSlotCont, SlotController[] slots,  Sprite noItemSprite, int slotIndex)
        { 
            //Sloting Item from moving slot to desured backpack slot
            slots[slotIndex].ItemImage.sprite = movingSlotCont.ItemImage.sprite;
            slots[slotIndex].SlotWithItem = true;
            slots[slotIndex].ItemCont = movingSlotCont.ItemCont;
            slots[slotIndex].SlotPrefab = movingSlotCont.SlotPrefab;
            //Emptying Moving Slot
            movingSlotCont.ItemImage.sprite = noItemSprite;
            movingSlotCont.SlotWithItem = false;
            movingSlotCont.ItemCont = null;
            movingSlotCont.SlotPrefab = null;
            movingSlotCont.gameObject.SetActive(false);
        }
    }
}
