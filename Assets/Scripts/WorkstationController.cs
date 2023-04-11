using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class WorkstationController : MonoBehaviour{
        [SerializeField] private PlayerUIStruct workstationUIStruct;
        [SerializeField] private bool inUse;
        [SerializeField] private bool doingWork;
        [SerializeField] private bool fuelFull;
        [SerializeField] private float barAmnt;
        [SerializeField] private float circleAmnt;    
        public PlayerUIStruct WorkstationUIStruct{get{return workstationUIStruct;}}
        public bool InUse{get{return inUse;}set{inUse = value;}}
        public float BarAmnt{get{return barAmnt;}set{barAmnt = value;}}
        public float CircleAmnt{get{return circleAmnt;}set{circleAmnt = value;}}
        public bool FuelFull{get{return fuelFull;}set{fuelFull = value;}}
        public bool DoingWork{get{return doingWork;}set{doingWork=value;}}
        public virtual void Use(){}
        public virtual void Refuel(float fuel){}
        public virtual void Overflow(float amount){}
        public virtual void ToggleUse(){}
    }
}
