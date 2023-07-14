using UnityEngine;
using UnityEngine.InputSystem;

namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour{
        #region Private Serialized Fields
        [Header("Player Movement")]
        [SerializeField] private bool isMoving;
        [SerializeField] private bool isRunning; 
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
        [SerializeField] private WorkstationController workstationCont;
        [SerializeField] private string backPackObjPath;
        #endregion
        #region Private Fields
        private StockpileController stockpileCont;
        private Animator m_Animator;
        private int lookXHash, lookYHash, isMovingHash, moveXHash, moveYHash;
        private int layerMask, stockpileLayer, workstationLayer, boundsLayer;
        private RaycastHit2D hit;
        private GameObject backPackObj;
        private void MovePlayer(bool moving){
            if(moving){
                m_Animator.SetBool(isMovingHash, true);
                m_Animator.SetFloat(moveXHash, moveDir.x);
                m_Animator.SetFloat(moveYHash, moveDir.y);
                transform.Translate((isRunning?runSpeed:1.00f)*Time.deltaTime*walkSpeed*moveDir);
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
            else if (stockpileCont.Quantity != 0 && !m_InventoryCont.SlotsAreFull)
                return PickUpObj();
            else
                return holdingItem;
        }
        public bool InteractWorkstation(){
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
                                return false;
                            }
                            else
                                return true;
                        default:
                            return holdingItem;   
                    }
                default:
                    return holdingItem;
            }
        }
        public void OnMove(InputAction.CallbackContext context){
            ///<summary>
            ///Player Movement
            ///Player movement inputtaken as 2D Vector and is translted to movement of gameObject
            ///The last dir the player moves in is the players looking direction
            ///</summary>
            moveDir = context.ReadValue<Vector2>();
            isMoving = (moveDir != Vector2.zero);
            lookDir = (isMoving)?(moveDir.normalized):(lookDir);
        }
        public void OnInteraction(InputAction.CallbackContext context){
            hit = Physics2D.Raycast(transform.position, lookDir, interactDist, layerMask);
            //If so is the player prompting to interact with said item?
            if (hit.transform != null && (context.started)){
                //Diff scenarios based on what the player is interacting with
                switch(hit.transform.gameObject.layer){
                     //Forge, Quelcher, Sandstone, etc...
                    case 8:
                        //If theb player is holding an object let them interact with the workstation
                        //If tehplayer does not have an item them they are going to want to uise the workstation
                        holdingItem = (holdingItem)?(InteractWorkstation()):(UseWorkstation());
                        break;
                    //Coal pile, wood pile, etc...
                    case 10:
                        //If the player is not holding an item check that the quickslots are not full
                        //and that the player does not have the backpack open in order to allow them to pick up the desired object
                        //If the player is holding an object allow them to drop the object
                        holdingItem = (!holdingItem)?((m_InventoryCont.SlotsAreFull)?(false):(backPackObj.activeInHierarchy)?(false):(PickUpObj())):(DropObj());
                        break;
                    default:
                        break;
                }
                return;
            }
            else
                stockpileCont = null;
        }
        public void ToggleRun(InputAction.CallbackContext context){
            if (context.started)
                isRunning = true;
            else if(context.canceled)
                isRunning = false;
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
            backPackObj = transform.Find(backPackObjPath).gameObject;
        }
        // Update is called once per frame
        void Update(){
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