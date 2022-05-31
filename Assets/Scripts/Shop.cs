using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Shop : MonoBehaviour
{


    [SerializeField] GameObject Item_Raq, Item_ExtraLife;

    private void Start()
    {
        if (PlayerStatsManager.bRaq)
            Item_Raq.GetComponent<Item>().Purchased();
        if (PlayerStatsManager.bExtraLife)
            Item_ExtraLife.GetComponent<Item>().Purchased();
    }

    public void OnButtonBuyRaq()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                List<ItemInstance> ii = result.Inventory;
                foreach (ItemInstance i in ii)
                {
                    if (i.ItemId == "S01_Raq")
                        return;
                }
            }, OnError);
        BuyShip();
        Item_Raq.GetComponent<Item>().Purchased();
    }

    public void OnButtonBuyExtraLife()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                List<ItemInstance> ii = result.Inventory;
                foreach (ItemInstance i in ii)
                {
                    if (i.ItemId == "C01_ExtraLife")
                        return;
                }
            }, OnError);
        BuyExtraLife();
        Item_ExtraLife.GetComponent<Item>().Purchased();
    }


    void BuyShip()
    {
        var buyreq = new PurchaseItemRequest
        {
            CatalogVersion = "Ships",
            ItemId = "S01_Raq",
            VirtualCurrency = "GD",
            Price = 20

        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result => { 
                UpdateMsg("Bought!");
                gameObject.GetComponent<PlayerStats>().UpdateGold();
                PlayerStatsManager.bRaq = true;
            },
            OnError);
    }

    void BuyExtraLife()
    {
        var buyreq = new PurchaseItemRequest
        {
            CatalogVersion = "Consumables",
            ItemId = "C01_ExtraLife",
            VirtualCurrency = "GD",
            Price = 5

        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result =>
            {
                List<ItemInstance> ii = result.Items;

                UpdateMsg("Bought!");
                gameObject.GetComponent<PlayerStats>().UpdateGold();
                PlayerStatsManager.bExtraLife = true;
                foreach (ItemInstance i in ii)
                {
                    PlayerStatsManager.ExtraLifeID = i.ItemInstanceId;
                    return;
                }
            },
            OnError);
    }

    void UpdateMsg(string msg)
    {
        Debug.Log(msg);
    }
    void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }
}
