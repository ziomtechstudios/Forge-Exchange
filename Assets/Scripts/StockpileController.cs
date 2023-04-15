using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StockpileController : MonoBehaviour{
        #region Private Serialized Fields
        [SerializeField] private int quantity;
        [SerializeField] private int maxQuantity;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private ItemStruct itemStruct;
        #endregion
        #region Getters/Setter
        public int Quantity{get{return quantity;}}
        public int MaxQuantity{get{return maxQuantity;}set{maxQuantity = value;}}
        public Sprite ItemSprite{get{return itemSprite;}set{itemSprite = value;}}
        public ItemStruct ItemStruct{get{return itemStruct;}set{itemStruct = value;}}
        #endregion
        public bool Deposit(int amount){
            bool canDeposit = ((quantity+amount)<=maxQuantity);
            quantity += (canDeposit?(amount):(0));
            return canDeposit;
        }
        public bool Withdraw(int amount){
            bool canWithdraw = ((quantity-amount)>=0);
            quantity -=(canWithdraw?(amount):(0));
            return canWithdraw;
        }
        
    }
}
