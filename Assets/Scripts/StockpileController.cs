using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StockpileController : MonoBehaviour{
        #region Private Serialized Fields
        [Header("Stockpile Metrics")]
        [SerializeField] private int quantity;
        [SerializeField] private int maxQuantity;
        [Header("Item data")]
        [SerializeField] private GameObject itemPrefab;
        #endregion
        #region Getters/Setter
        public int Quantity{get{return quantity;}}
        public int MaxQuantity{get{return maxQuantity;}set{maxQuantity = value;}}
        public GameObject ItemPrefab{get{return itemPrefab;}set{itemPrefab = value;}}
        #endregion
        public bool Deposit(int amount){
            //if player can deposit item to stockpile update current quantity and return outcome
            bool canDeposit = ((quantity+amount)<=maxQuantity);
            quantity += (canDeposit?(amount):(0));
            return canDeposit;
        }
        //if player can withdraw from stock item then update quantity and return outcome
        public bool Withdraw(int amount){
            bool canWithdraw = ((quantity-amount)>=0);
            quantity -=(canWithdraw?(amount):(0));
            return canWithdraw;
        }
    }
}