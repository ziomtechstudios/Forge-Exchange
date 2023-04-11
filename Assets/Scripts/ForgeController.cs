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
        [SerializeField] private Animator forgeAnimator;
        [SerializeField] private ForgePumpController forgePumpCont;
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
        public override void ToggleUse(){
            if(!InUse && curTemp != maxTemp &&(fuelAmnt>0.0f))
                SetForge(true, maxTemp);
        }
        public override void Use(){
            
        }
        public override void Refuel(float fuel){
            fuelAmnt += fuel;
        }
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

        }
        // Update is called once per frame
        void Update(){ 
            //As the forge is in use make sure fuel is being used
            if(InUse && (fuelAmnt > 0.0f))
                fuelAmnt -= (burnRate*Time.deltaTime+(forgePumpCont.InUse?(burnRate*Time.deltaTime):(0.0f)));
            //Ran out of fuel
            else if(fuelAmnt <= 0.0f && InUse)
                SetForge(false,  0.0f);
            //Display current amount of fuel and temperature of forge
            CircleAmnt = (100.0f*fuelAmnt)/maxFuelAmnt;
            BarAmnt = (curTemp)/(maxTemp+forgePumpCont.MaxBoostTemp);


        }
    }
}
