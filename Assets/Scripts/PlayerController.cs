using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour{
        #region Private Serialized Fields
        [Header("Player Movement")]
        [SerializeField] private Vector2 lookDir;
        [SerializeField] private Vector2 moveDir;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float interactDist;
        [SerializeField] private bool holdingItem;
        [SerializeField] private GameObject holdingPrefab;
        [SerializeField] private ItemController holdingCont;
        [SerializeField] private InventoryController m_InventoryCont;
        #endregion
        #region Private Fields
        private StockpileController stockpileCont;
        private WorkstationController workstationCont;
        private Animator m_Animator;
        private int lookXHash, lookYHash, isMoving, moveXHash, moveYHash;
        private int layerMask, stockpileLayer, workstationLayer;
        private RaycastHit2D hit; 
        #endregion
        #region Public Members
        public RaycastHit2D  PlayerLOS{get{return hit;}}
        public bool DropObj(){
            //Make sure we have reference to component in players LOS
            if(stockpileCont==null)
                stockpileCont = hit.transform.GetComponent<StockpileController>();
            //If what the player is holding is an appropriate item for a stockpile and the stockpile is not full we add the item
            //If the stockpile cant take in the item we set the playerHolding to true
            if(stockpileCont.Deposit(1, holdingPrefab, holdingCont))
                m_InventoryCont.DroppingItem();
            return holdingItem;
        }
        public bool PickUpObj(){
            //Make sure we have reference to component in players LOS
            if(stockpileCont==null)
                stockpileCont = hit.transform.GetComponent<StockpileController>();
            if(!stockpileCont.IsEmpty){
                holdingItem = true;
                holdingPrefab = stockpileCont.ItemPrefab;
                holdingCont = stockpileCont.ItemPrefab.GetComponent<ItemController>();
                m_InventoryCont.SlotItem();
                stockpileCont.Withdraw(1);
            }
            return holdingItem;
        }
        public bool UseWorkstation(){
            //Make sure we have reference to component in players LOS
            stockpileCont = hit.transform.GetComponent<StockpileController>();
            workstationCont  = hit.transform.GetComponent<WorkstationController>();
            if(stockpileCont.Quantity==0){
                workstationCont.ToggleUse();
                return false;
            }
            else
                return PickUpObj();   
        }
        public bool InteractWorkstation(){
            if(workstationCont == null)
                workstationCont = hit.transform.GetComponent<WorkstationController>();
            switch(hit.transform.tag){
                case "Forge":
                    switch(holdingCont.PrefabItemStruct.itemTag){
                        case "Fuel":
                            //First we check if this fuel deposit will be more than what the forge can handle
                            workstationCont.Overflow(holdingCont.PrefabItemStruct.fuelAmnt);
                            //If the item can be used as fuel and we are not using workstation that doesnt use fuel and if refueling the workstation wont overflow
                            //Workstation that dont require fuel such as forgepump will simply have their Fuel Full boolean set to true thereby !true.
                            if(!(holdingCont.PrefabItemStruct.fuelAmnt==0.0f) && (!workstationCont.BarFull)){
                                workstationCont.Refuel(holdingCont.PrefabItemStruct.fuelAmnt);                                                                                                 
                                m_InventoryCont.DroppingItem();
                                return false;
                            }
                            else
                                return true;
                        case "Ore":
                            if(workstationCont.InUse && !workstationCont.DoingWork){
                                workstationCont.Work(holdingCont.PrefabItemStruct);
                                m_InventoryCont.DroppingItem();
                                workstationCont.DoingWork = true;
                            }
                            return false;
                        default:
                            return holdingItem;   
                    }
                default:
                    return holdingItem;
            }
        }
        #endregion
        #region "Getter and Setters"
        public bool HoldingItem{get{return holdingItem;}set{holdingItem = value;}}
        public GameObject HoldingPrefab{get{return holdingPrefab;}set{holdingPrefab = value;}}
        public ItemController HoldingCont{get{return holdingCont;}set{holdingCont = value;}}
        #endregion
        // Start is called before the first frame update
        void Start(){
            m_InventoryCont = transform.Find("Main Camera/Canvas/InventorySlots").gameObject.GetComponent<InventoryController>();
            lookDir = -transform.up;
            m_Animator = gameObject.GetComponent<Animator>();
            lookXHash = Animator.StringToHash("LookX");
            lookYHash = Animator.StringToHash("LookY");
            moveXHash = Animator.StringToHash("MoveX");
            moveYHash = Animator.StringToHash("MoveY");
            isMoving = Animator.StringToHash("isMoving");
            workstationLayer = LayerMask.NameToLayer("workstation");
            stockpileLayer = LayerMask.NameToLayer("stockpile");
            layerMask = ((1<<stockpileLayer)|(1 <<workstationLayer));
            stockpileCont = null;
        }

        // Update is called once per frame
        void Update(){
            ///<summary>
            ///Player Movement
            ///Player movement inpurttaken as 2D Vector and is translted to movement of gameObject
            ///The last dir the player moves in is the players looking direction
            ///</summary>
            moveDir = new Vector2(Mathf.Round(Input.GetAxis("Horizontal")), Mathf.Round(Input.GetAxis("Vertical")));
            if(moveDir!=Vector2.zero){
                m_Animator.SetBool(isMoving, true);
                m_Animator.SetFloat(moveXHash, moveDir.x);
                m_Animator.SetFloat(moveYHash, moveDir.y);
                transform.Translate(moveDir*Time.deltaTime*walkSpeed);
                lookDir = moveDir;
            }
            else{
                m_Animator.SetBool(isMoving, false);
                m_Animator.SetFloat(lookXHash, lookDir.x);
                m_Animator.SetFloat(lookYHash, lookDir.y);
            }
            //Is the player looking at a interactable object + within an interactable distance?
            hit = Physics2D.Raycast(transform.position, lookDir, interactDist, layerMask); 
            //If so is the player prompting to interact with said item?
            if(hit.transform != null){
                if(Input.GetButtonDown("Use")){
                    //Diff scenarios based on what the player is interacting with
                    switch(hit.transform.gameObject.layer){
                        //Forge, Quelcher, Sandstone, etc...
                        case 8:
                            holdingItem = (!holdingItem)?(UseWorkstation()):(InteractWorkstation());
                            break;
                        //Coal pile, wood pile, etc...
                        case 10:
                            holdingItem = (!holdingItem)?((m_InventoryCont.SlotsAreFull)?(false):PickUpObj()):(DropObj());
                            break;
                        default:
                            break;
                    }
                }
            }
            else
                stockpileCont = null;    
        }
    }
}
