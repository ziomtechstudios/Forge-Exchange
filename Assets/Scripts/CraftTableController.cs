using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class CraftTableController : WorkstationController, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Serialized Fields
        [SerializeField] private Animator m_Animator;
        [SerializeField] private int slotAmnt;
        [SerializeField] private GameObject craftMenuObj;
        #endregion
        #region Private Fields
        private int inUseHash;
        private GameObject[] ingredients;
        private string[] recipes;
        private IDictionary<string, GameObject> craftingRecipes;
        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }
        public void OnDrag(PointerEventData eventData)
        {
            
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            
        }
        #endregion
        #region Public Funcs
        public override void ToggleUse(){
            InUse = !InUse;
            m_Animator.SetBool(inUseHash, InUse); 
        }
        #endregion
        // Start is called before the first frame update
        void Start(){
            m_Animator = transform.Find("Tools").transform.gameObject.GetComponent<Animator>();
            inUseHash = Animator.StringToHash("InUse");
            InUse = false;
            ingredients = new GameObject[slotAmnt];
            recipes = new string[slotAmnt];
        }
    }
}
