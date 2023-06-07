using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class PlayerUIController : MonoBehaviour{
        #region Private Serialized Fields
        [SerializeField] private Camera playerCam;
        [SerializeField] private PlayerController playerCont;
        [SerializeField] private ProgressBar barUI;
        [SerializeField] private ProgressBarCircle circleUI;
        [SerializeField] private Image itemUI;
        [SerializeField] private TextMeshProUGUI counterText;
        [SerializeField] private GameObject backPackObj;
        #endregion
        #region Private Fields
        private Transform circleTransform, barTransform, itemUiTransform;
        #endregion
        #region Public Members
        public void ToggleInventory(){
            backPackObj.SetActive(!backPackObj.activeInHierarchy);
        }
        #endregion
        // Start is called before the first frame update
        void Awake(){
            playerCam = transform.Find("Main Camera").gameObject.GetComponent<Camera>();
            playerCont = GetComponent<PlayerController>();
            circleUI = playerCam.transform.Find("Canvas/LOSUI/circleUI").gameObject.GetComponent<ProgressBarCircle>();
            barUI = playerCam.transform.Find("Canvas/LOSUI/barUI").gameObject.GetComponent<ProgressBar>();
            counterText = playerCam.transform.Find("Canvas/CounterText").gameObject.GetComponent<TextMeshProUGUI>();
            itemUI = playerCam.transform.Find("Canvas/itemImage").gameObject.GetComponent<Image>();
            backPackObj = transform.Find("Main Camera/Canvas/Inventory/ImageInventory").gameObject;
            backPackObj.SetActive(false);

        }
        // Update is called once per frame
        void Update(){
            if(playerCont.PlayerLOS.transform != null){
                switch(playerCont.PlayerLOS.transform.gameObject.layer){
                    //Player sees a workstation
                    case 8:
                        WorkstationController  workstationCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<WorkstationController>();
                        circleTransform = playerCont.PlayerLOS.transform.Find("circleUILOC");
                        barTransform = playerCont.PlayerLOS.transform.Find("barUILOC");
                        itemUiTransform = playerCont.PlayerLOS.transform.Find("productUILOC");
                        if(circleTransform != null){
                            circleUI.gameObject.transform.position =  playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("circleUILOC").position);
                            circleUI.BarValue = workstationCont.CircleAmnt;
                            if(!circleUI.gameObject.activeInHierarchy){
                                circleUI.gameObject.SetActive(true);
                                circleUI.Title = workstationCont.WorkstationUIStruct.circleTitle;
                            }
                        }
                        if(barTransform != null){
                            barUI.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("barUILOC").position);
                            barUI.BarValue = workstationCont.BarAmnt;
                            //Toggle UI items visibility  assign Title of UI activated UI items
                            if(!barUI.gameObject.activeInHierarchy){
                                barUI.gameObject.SetActive(true);
                                barUI.Title = workstationCont.WorkstationUIStruct.barTitle;
                            }
                        }

                        if(itemUiTransform != null){
                            StockpileController stockPileCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<StockpileController>();
                            if(stockPileCont != null  && (stockPileCont.Quantity != 0)){
                                itemUI.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("productUILOC").position);
                                itemUI.gameObject.SetActive(true);
                                itemUI.sprite = stockPileCont.ItemPrefab.GetComponent<SpriteRenderer>().sprite;
                            }
                            else if(stockPileCont.Quantity == 0 && itemUI.gameObject.activeInHierarchy)
                                itemUI.gameObject.SetActive(false);
                        }
                        break;
                        //Player sees a Stockpile
                    case 10:
                        StockpileController stockpileCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<StockpileController>();
                        //Assign location value and title of coresponding UI Items
                        counterText.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("counterUILOC").position);
                        counterText.text = $"{stockpileCont.Quantity}/{stockpileCont.MaxQuantity}";
                        if(!counterText.gameObject.activeInHierarchy)
                            counterText.gameObject.SetActive(true);
                        break;   
                    default:
                        break;
                }
            }
            else{
                //player sees nothing that is giving us UI prompts so make UI elements not needed invisible
                circleUI.gameObject.SetActive(false);
                barUI.gameObject.SetActive(false);
                counterText.gameObject.SetActive(false); 
                itemUI.gameObject.SetActive(false);

            }
        }  
    }
}
