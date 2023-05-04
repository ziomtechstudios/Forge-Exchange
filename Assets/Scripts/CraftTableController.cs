using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class CraftTableController : WorkstationController{
        #region Serialized Fields
        [SerializeField] private Animator m_Animator;
        #endregion
        #region Private Fields
        private int inUseHash;
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
        }

        // Update is called once per frame
        void FixedUpdate(){
            
        }
    }
}
