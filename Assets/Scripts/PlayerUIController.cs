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
        [SerializeField] private TextMeshProUGUI counterText;  
        #endregion     
        // Start is called before the first frame update
        void Start(){
            playerCam = transform.Find("Main Camera").gameObject.GetComponent<Camera>();
            playerCont = GetComponent<PlayerController>();
            circleUI = playerCam.transform.Find("Canvas/circleUI").gameObject.GetComponent<ProgressBarCircle>();
            barUI = playerCam.transform.Find("Canvas/barUI").gameObject.GetComponent<ProgressBar>();
            counterText = playerCam.transform.Find("Canvas/CounterText").gameObject.GetComponent<TextMeshProUGUI>();

        }
        // Update is called once per frame
        void Update(){
            if(playerCont.PlayerLOS.transform != null){
                switch(playerCont.PlayerLOS.transform.tag){
                    case "Workstation":
                        WorkstationController  workstationCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<WorkstationController>();
                        circleUI.gameObject.transform.position =  playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("circleUILOC").position);
                        barUI.gameObject.transform.position = playerCam.WorldToScreenPoint(playerCont.PlayerLOS.transform.Find("barUILOC").position);
                        circleUI.BarValue = workstationCont.CircleAmnt;
                        barUI.BarValue = workstationCont.BarAmnt;
                        if(!circleUI.gameObject.activeInHierarchy || !barUI.gameObject.activeInHierarchy){
                            circleUI.gameObject.SetActive(true);
                            barUI.gameObject.SetActive(true);
                            circleUI.Title = workstationCont.WorkstationUIStruct.circleTitle;
                            barUI.Title = workstationCont.WorkstationUIStruct.barTitle;
                        }
                        break;
                    case "Stockpile":
                        StockpileController stockpileCont = playerCont.PlayerLOS.transform.gameObject.GetComponent<StockpileController>();
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
                circleUI.gameObject.SetActive(false);
                barUI.gameObject.SetActive(false);
                counterText.gameObject.SetActive(false);                                             
            }
        }  
    }
}
