using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.ZiomtechStudios.ForgeExchange{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StockpileController : MonoBehaviour{
        #region Private Serialized Fields
        [Header("Stockpile Components")]
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [Header("Stockpile Metrics")]
        [SerializeField] private int quantity;
        [SerializeField] private int maxQuantity;
        [SerializeField] private bool isEmpty;
        [Header("Item data")]
        [SerializeField] private GameObject itemPrefab;
        [Tooltip("Sprite that represents when this stockpile is empty")][SerializeField] private Sprite emptyStockpileSprite;
        [Tooltip("Sprite of what corresponding items that can fill the stockpile.")][SerializeField] private Sprite[] itemSprites;
        [Tooltip("Tag of corresponding items that can fill the stockpile.")][SerializeField] private string[] itemTags;
        #endregion
        #region Getters/Setter
        public int Quantity{get{return quantity;}}
        public int MaxQuantity{get{return maxQuantity;}set{maxQuantity = value;}}
        public GameObject ItemPrefab{get{return itemPrefab;}set{itemPrefab = value;}}
        #endregion
        public Sprite TakeItem(GameObject newItem){
            ItemController m_ItemController = newItem.GetComponent<ItemController>();
            Sprite newSprite;
            bool canTakeItem = itemTagToSpriteDict.TryGetValue((m_ItemController.PrefabItemStruct.itemSubTag+m_ItemController.PrefabItemStruct.itemTag), out newSprite);
            itemPrefab = (canTakeItem)?(newItem):(null);
            isEmpty = !canTakeItem;
            m_SpriteRenderer.sprite = (!isEmpty)?(newSprite):(emptyStockpileSprite);
            return (canTakeItem)?(newSprite):(null);
        }
        private IDictionary<string, Sprite> itemTagToSpriteDict;
        public bool Deposit(int amount, GameObject newItem){
            //if player can deposit item to stockpile update current quantity and return outcome
            bool canDeposit = ((quantity+amount)<=maxQuantity);
            quantity += (canDeposit?(amount):(0));
            if(quantity == 0)
                TakeItem(newItem);
            else if(canDeposit&&(newItem == itemPrefab))
                quantity ++;
            return canDeposit;
        }
        //if player can withdraw from stock item then update quantity and return outcome
        public bool Withdraw(int amount){
            bool canWithdraw = ((quantity-amount)>=0);
            quantity -=(canWithdraw?(amount):(0));
            //if empty get rid of 
            isEmpty = quantity==0;
            m_SpriteRenderer.sprite = (isEmpty)?(emptyStockpileSprite):(m_SpriteRenderer.sprite);
            itemPrefab = (isEmpty)?(null):(itemPrefab);
            return canWithdraw;
        }
        public void Start(){
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            isEmpty = (quantity==0);
            itemTagToSpriteDict = new Dictionary<string,Sprite>();
            foreach (string itemTag in itemTags)
                itemTagToSpriteDict.Add(itemTag, itemSprites[Array.IndexOf(itemTags ,itemTag)]);
            m_SpriteRenderer.sprite = (isEmpty)?(emptyStockpileSprite):(itemTagToSpriteDict[itemPrefab.GetComponent<ItemController>().PrefabItemStruct.itemSubTag + itemPrefab.GetComponent<ItemController>().PrefabItemStruct.itemTag]);
            
        }
    }
}