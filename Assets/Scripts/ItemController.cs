using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private float refinement;
        [SerializeField] private ItemStruct prefabItemStruct;
        public float Refinement{get{return refinement;} set{refinement = value;}}
        public ItemStruct PrefabItemStruct{get{return prefabItemStruct;}set{prefabItemStruct = value;}}

        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
