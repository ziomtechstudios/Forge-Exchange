using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class CraftTableController : WorkstationController
    {
        #region Serialized Fields
        [SerializeField] private Animator m_Animator;
        [SerializeField] private GameObject craftMenuObj;
        [SerializeField] private bool isCrafting;
        #endregion
        #region Private Fields
        private int inUseHash;
        #endregion
        #region Public Funcs
        public override void ToggleUse(){

            m_Animator.SetBool(inUseHash, InUse); 
            craftMenuObj
        }
        #endregion
        // Start is called before the first frame update
        void Start(){
            m_Animator = transform.Find("Tools").transform.gameObject.GetComponent<Animator>();
            inUseHash = Animator.StringToHash("InUse");
            craftMenuObj = transform.Find("Canvas/CraftingMenu").gameObject;
            isCrafting = false;
        }

    }
}
