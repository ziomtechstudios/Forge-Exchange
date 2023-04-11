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
        [SerializeField] private Sprite holdingSprite;
        [SerializeField] private ItemStruct holdingStruct;
        [SerializeField] private InventoryController m_InventoryCont;
        #endregion
        #region Private Fields
        private Animator m_Animator;
        private int lookXHash, lookYHash, isMoving, moveXHash, moveYHash;
        private int layerMask;
        private RaycastHit2D hit;
        #endregion
        #region Public Members
        public RaycastHit2D PlayerLOS{get{return hit;}}
        public bool DropObj(){
            StockpileController stockpileCont = hit.transform.GetComponent<StockpileController>();
            //If what the player is holding is an appropriate item into a stockpile and the stockpile is not full we add the item
            //return false so that we set to player holding an item to false
            if((stockpileCont.ItemSprite == holdingSprite)){
                m_InventoryCont.DroppingItem();
                return !hit.transform.GetComponent<StockpileController>().Deposit(1);
            }
            //If not above item? should not be able to be deposited return true to indicate player is still holding onto that item
            else
                return true;
        }
        public bool PickUpObj(){
            StockpileController stockpileCont = hit.transform.GetComponent<StockpileController>();
            holdingItem = true;
            holdingSprite = stockpileCont.ItemSprite;
            holdingStruct = stockpileCont.ItemStruct;
            m_InventoryCont.SlotItem();
            return !stockpileCont.Withdraw(1);
        }
        public bool UseWorkstation(){
            hit.transform.GetComponent<WorkstationController>().ToggleUse();
            return false;
        }
        public bool InteractWorkstation(){
            WorkstationController workstationCont = hit.transform.GetComponent<WorkstationController>();
            switch(holdingStruct.itemTag){
                case "Fuel":
                    workstationCont.Overflow(holdingStruct.fuelAmnt);
                    //If the item can be used as fuel and we are not using workstation that doesnt use fuel and if refueling the workstation wont overflow
                    if(!(holdingStruct.fuelAmnt==0.0f) && hit.transform.GetComponent<ForgePumpController>()==null&&(!workstationCont.FuelFull)){
                        workstationCont.Refuel(holdingStruct.fuelAmnt);                                                                                                 
                        m_InventoryCont.DroppingItem();
                        return false;
                    }
                    else
                        return true;
                case "Ore":
                    return false;
                default:
                    return holdingItem;
            }
        }
        public bool HoldingItem{get{return holdingItem;}set{holdingItem = value;}}
        public Sprite HoldingSprite{get {return holdingSprite;} set{holdingSprite = value;}}
        public ItemStruct HoldingStruct{get{return holdingStruct;}set{holdingStruct = value;}}
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
            layerMask = 1 << 8;
        }

        // Update is called once per frame
        void Update(){
            ///<summary>
            ///Player Movement
            ///Player movement inpur taken as 2D Vector and is translted to movement of gameObject
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
                    switch(hit.transform.tag){
                        //Forge, Quelcher, Sandstone, etc...
                        case "Workstation":
                            holdingItem = (!holdingItem)?(UseWorkstation()):(InteractWorkstation());
                            break;
                        //Coal pile, wood pile, etc...
                        case "Stockpile":
                            holdingItem = (!holdingItem)?((m_InventoryCont.SlotsAreFull)?(false):PickUpObj()):(DropObj());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
