using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Com.ZiomtechStudios.ForgeExchange{
    public class PlayerUIController : MonoBehaviour{
        #region Private Serialized Fields
        [SerializeField] private Camera playerCam;
        [SerializeField] private PlayerController playerCont;
        [SerializeField] private ProgressBar barUI;
        [SerializeField] private ProgressBarCircle circleUI;
        [SerializeField] private Image itemUI;
        [SerializeField] private TextMeshProUGUI counterText;  
        #endregion     
        // Start is called before the first frame update
        void Start(){
            playerCam = transform.Find("Main Camera").gameObject.GetComponent<Camera>();
            playerCont = GetComponent<PlayerController>();
            circleUI = playerCam.transform.Find("Canvas/circleUI").gameObject.GetComponent<ProgressBarCircle>();
            barUI = playerCam.transform.Find("Canvas/barUI").gameObject.GetComponent<ProgressBar>();
            counterText = playerCam.transform.Find("Canvas/CounterText").gameObject.GetComponent<TextMeshProUGUI>();
            itemUI = playerCam.transform.Find("Canvas/itemImage").gameObject.GetComponent<Image>();

        }
        // Update is called once per frame
        void Update(){
            if(playerCont.PlayerLOS.transform != null){
                switch(playerCont.PlayerLOS.transform.tag){
                    //Player sees a workstation
                    case "Workstation":
                        WorkstationController  workstationCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<WorkstationController>();
                        //Move appropriate UI items to the appropriate locations near the workstation
                        circleUI.gameObject.transform.position =  playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("circleUILOC").position);
                        barUI.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("barUILOC").position);
                        //Assign Values of workstation to corresponding UI items
                        circleUI.BarValue = workstationCont.CircleAmnt;
                        barUI.BarValue = workstationCont.BarAmnt;
                        //Toggle UI items visibility  assign Title of UI activated UI items
                        if(!circleUI.gameObject.activeInHierarchy || !barUI.gameObject.activeInHierarchy){
                            circleUI.gameObject.SetActive(true);
                            barUI.gameObject.SetActive(true);
                            circleUI.Title = workstationCont.WorkstationUIStruct.circleTitle;
                            barUI.Title = workstationCont.WorkstationUIStruct.barTitle;
                        }
                        StockpileController stockPileCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<StockpileController>();
                        if((stockPileCont != null || (playerCont.PlayerLOS.transform.gameObject.GetComponent<ForgePumpController>() != null)) && (stockPileCont.Quantity != 0)){
                            itemUI.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("smeltedUILOC").position);
                            itemUI.gameObject.SetActive(true);
                            itemUI.sprite = stockPileCont.ItemSprite;
                        }
                        else if(stockPileCont.Quantity == 0 && itemUI.gameObject.activeInHierarchy)
                            itemUI.gameObject.SetActive(false);
                        break;
                        //Player sees a Stockpile
                    case "Stockpile":
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
