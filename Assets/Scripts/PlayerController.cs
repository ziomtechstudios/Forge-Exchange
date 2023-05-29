using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour{
        #region Private Serialized Fields
        [Header("Player Movement")]
        [SerializeField] private bool isMoving;
        [SerializeField] private Vector2 lookDir;
        [SerializeField] private Vector2 moveDir;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float interactDist;
        [SerializeField] private bool holdingItem;
        [SerializeField] private GameObject holdingPrefab;
        [SerializeField] private ItemController holdingCont;
        [SerializeField] private InventoryController m_InventoryCont;
        [SerializeField] private PolygonCollider2D m_Collider;
        #endregion
        #region Private Fields
        private StockpileController stockpileCont;
        private WorkstationController workstationCont;
        private Animator m_Animator;
        private int lookXHash, lookYHash, isMovingHash, moveXHash, moveYHash;
        private int layerMask, stockpileLayer, workstationLayer, boundsLayer;
        private RaycastHit2D hit;
        private void MovePlayer(bool moving){
            if(moving){
                m_Animator.SetBool(isMovingHash, true);
                m_Animator.SetFloat(moveXHash, moveDir.x);
                m_Animator.SetFloat(moveYHash, moveDir.y);
                transform.Translate(moveDir*Time.deltaTime*walkSpeed);
            }
            else{
                m_Animator.SetBool(isMovingHash, false);
                m_Animator.SetFloat(lookXHash, lookDir.x);
                m_Animator.SetFloat(lookYHash, lookDir.y);
            }
        }
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
            //Make sure that the stickpile is not empty
            if(!stockpileCont.IsEmpty){
                //Occupy the objects in the players hands and have them slot it into first available slot
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
                            //Make sure that the forge is on and that its not already smelting ore(s)
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
        public void OnMove(InputAction.CallbackContext context){
            moveDir = context.ReadValue<Vector2>();
            Debug.Log(moveDir);
            isMoving = moveDir != Vector2.zero;
            lookDir = (isMoving)?(moveDir.normalized):(lookDir);
        }
        public void OnInteraction(InputAction.CallbackContext context){
             //If so is the player prompting to interact with said item?
            if(hit.transform != null){
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
            else
                stockpileCont = null;   
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
            isMoving = false;
            isMovingHash = Animator.StringToHash("isMoving");
            workstationLayer = (1<<LayerMask.NameToLayer("workstation"));
            stockpileLayer = (1<<LayerMask.NameToLayer("stockpile"));
            boundsLayer =  (1 << LayerMask.NameToLayer("bounds"));
            layerMask = (stockpileLayer|workstationLayer|boundsLayer);
            stockpileCont = null;
            m_Collider = GetComponent<PolygonCollider2D>();
        }
        // Update is called once per frame
        void Update(){
            ///<summary>
            ///Player Movement
            ///Player movement inpurttaken as 2D Vector and is translted to movement of gameObject
            ///The last dir the player moves in is the players looking direction
            ///</summary>
            //Is the player looking at a interactable object + within an interactable distance?
            hit = Physics2D.Raycast(transform.position, lookDir, interactDist, layerMask); 
            //If player wants to move
            if(isMoving){
                //If player is touching bounds and the player is trying to move towards the bounds
                if((m_Collider.IsTouchingLayers(boundsLayer)) && (hit.transform != null))
                    MovePlayer(false);
                //The player is either no longer touching bounds or is attempting to walk away from bounds
                else
                    MovePlayer(true);
                }
            else if(!isMoving && m_Animator.GetBool(isMovingHash))
                MovePlayer(false); 
        }
    }
}