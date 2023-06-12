using System.Collections;
using UnityEngine;

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
        #endregion
        // Start is called before the first frame update
        void Awake(){
            gameObject.SetActive(true);
            m_InventoryCont = transform.parent.parent.parent.Find("InventorySlots").gameObject.GetComponent<InventoryController>();
            backPackSlots = new SlotController[numSlots];
            for(int i = 0; i < numSlots; i++)
                backPackSlots[i] = transform.Find($"Slot{i}").gameObject.GetComponent<SlotController>();
            quickSlots = new SlotController[m_InventoryCont.InventoryAmnt];
            for(int i=0;i<m_InventoryCont.InventoryAmnt;i++)
                quickSlots[i] = transform.Find($"QuickSlots/Slot{i}").gameObject.GetComponent<SlotController>();          
        }
    }
}