using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class ForgeController : WorkstationController{
        #region Private serialized members
        [SerializeField] private float curTemp;
        [SerializeField] private float maxTemp;
        [SerializeField] private float fuelAmnt;
        [SerializeField] private float maxFuelAmnt;
        [SerializeField] private float burnRate;
        [SerializeField] private float ttsTimer;
        [SerializeField] private float idealTTS;
        [SerializeField] private float ttsScaler;
        [SerializeField] private Animator forgeAnimator;
        [SerializeField] private ForgePumpController forgePumpCont;
        [SerializeField] private StockpileController forgeStockPileCont;
        [SerializeField] private IDictionary<Sprite, Sprite> oresToBars;
        [Tooltip("The Struct of the item being smelted.")] [SerializeField] private ItemStruct smeltStruct;
        #endregion
        #region Private memebers
        private int inUseHash;
        #endregion
        #region Private Funcs
        // On/Off switch for forge where base temp for forge is defined at runtime
        public void SetForge(bool state, float temp){
            InUse = state;
            forgeAnimator.SetBool(inUseHash, state);
            curTemp = temp;
        }
        #endregion
        #region Overriden Funcs
        //When turning on forge make sure conditions are met to turn on forge
        //If the Forge is On then lets toggle it Off
        public override void ToggleUse(){
            bool hasFuel = fuelAmnt>0.0f;
            if(!InUse && (curTemp != maxTemp) && hasFuel)
                SetForge(true, maxTemp);
            else if(InUse)
                SetForge(false, 0.0f);
        }
        //Smelting Ore to Metal Bar
        public override void Work(ItemStruct itemStruct){
            if(InUse && !DoingWork){
                DoingWork = true;
                smeltStruct = itemStruct;
                idealTTS = (((MaxTemp+forgePumpCont.MaxBoostTemp)-smeltStruct.meltingTemp)/smeltStruct.meltingTemp) * ttsScaler;
            }   
        }
        public override void Refuel(float fuel){
            fuelAmnt += fuel;
        }
        //Checking to see if player adding fuel will amount in more fuel than the forge can hold.
        public override void Overflow(float amount){
            FuelFull = ((fuelAmnt+amount)>maxFuelAmnt);
        }
        #endregion 
        #region Getters/Setters
        public float CurTemp{get{return curTemp;} set{curTemp = value;}}
        public float MaxTemp{get{return maxTemp;} set{maxTemp = value;}}
        public float FuelAmnt{get{return fuelAmnt;}set{fuelAmnt = value;}}
        public float MaxFuelAmnt{get{return maxFuelAmnt;}set{maxFuelAmnt=value;}}
        #endregion
        // Start is called before the first frame update
        void Start(){
            forgeAnimator = GetComponent<Animator>();
            inUseHash = Animator.StringToHash("inUse");
            forgePumpCont = transform.parent.transform.Find("forge_pump").gameObject.GetComponent<ForgePumpController>();
            SetForge(false, 0.0f);
            ttsTimer = 0.0f;
            forgeStockPileCont = GetComponent<StockpileController>();
        }
        // Update is called once per frame
        void Update(){ 
            //As the forge is in use make sure fuel is being used
            if(InUse && (fuelAmnt > 0.0f)){
                //Burn fuel
                fuelAmnt -= (burnRate*Time.deltaTime+(forgePumpCont.InUse?(burnRate*Time.deltaTime):(0.0f)));
                //If forge is not on there is no point in seeing if its smelting
                if(DoingWork){
                    ttsTimer += (((curTemp-smeltStruct.meltingTemp)/smeltStruct.meltingTemp)*smeltStruct.refinement);
                    //Forge has smeltted ore return to player appropriate bar
                    if(ttsTimer == idealTTS){
                        
                    }

                }
            }
            //Ran out of fuel
            else if(fuelAmnt <= 0.0f && InUse)
                SetForge(false,  0.0f);
            //Display current amount of fuel and temperature of forge
            CircleAmnt = (100.0f*fuelAmnt)/maxFuelAmnt;
            BarAmnt = (curTemp)/(maxTemp+forgePumpCont.MaxBoostTemp);


        }
    }
}
