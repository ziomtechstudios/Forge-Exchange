using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.ZiomtechStudios.ForgeExchange{
    public class BackpackController : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private int numSlots;
        [SerializeField] private InventoryController m_InventoryCont;
        [SerializeField] private SlotController[] backPackSlots;
        [SerializeField] private SlotController[] quickSlots;
        #endregion
        #region Public Funcs
        public void SyncQuickSlots()
        {
            for (int i = 0; i < m_InventoryCont.InventoryAmnt; i++)
            {
                quickSlots[i].ItemImage.sprite = m_InventoryCont.SlotConts[i].ItemImage.sprite;
                quickSlots[i].SlotWithItem = m_InventoryCont.SlotConts[i].SlotWithItem;
                quickSlots[i].ItemCont = m_InventoryCont.SlotConts[i].ItemCont;
                quickSlots[i].SlotPrefab = m_InventoryCont.SlotConts[i].SlotPrefab;
            }
        }
        #endregion
        // Start is called before the first frame update
        void Awake(){
            m_InventoryCont = transform.parent.parent.parent.Find("InventorySlots").gameObject.GetComponent<InventoryController>();
            backPackSlots = new SlotController[numSlots];
            for(int i = 0; i < numSlots; i++)
                backPackSlots[i] = transform.Find($"Slot{i}").gameObject.GetComponent<SlotController>();
            quickSlots = new SlotController[m_InventoryCont.InventoryAmnt];
            for(int i=0;i<m_InventoryCont.InventoryAmnt;i++)
                quickSlots[i] = transform.Find($"QuickSlots/Slot{i}").gameObject.GetComponent<SlotController>();          
        }
        void OnEnable(){
            SyncQuickSlots();
        }
    }
}
