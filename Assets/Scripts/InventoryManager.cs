using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Msg;
    void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        Msg.text += msg + "\n";
    }
    void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }
    public void LoadScene(string scn)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scn);
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r =>
            {
                int coins = r.VirtualCurrency["CN"];
                UpdateMsg("Coins" + coins);
            }, OnError);
    }

    public void GetCatalog()
    {
        var catreq = new GetCatalogItemsRequest
        {
            CatalogVersion = "catalogname"
        };
        PlayFabClientAPI.GetCatalogItems(catreq,
            result =>
            {
                List<CatalogItem> items = result.Catalog;
                UpdateMsg("Catalog Items");
                foreach(CatalogItem i in items){
                    UpdateMsg(i.DisplayName + "," + i.VirtualCurrencyPrices["CN"]);
            }
            },OnError);
    }

    public void GetPlayerInventory()
    {
        var UserInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInv,
            result =>
            {
                List<ItemInstance> ii = result.Inventory;
                UpdateMsg("Player Inventory");
                foreach (ItemInstance i in ii)
                {
                    UpdateMsg(i.DisplayName + "," + i.ItemId + "," + i.ItemInstanceId);
                }
            }, OnError);
    }

    public void BuyItem()
    {
        var buyreq = new PurchaseItemRequest
        {
            CatalogVersion = "Fruits",
            ItemId = "F01_Apple",
            VirtualCurrency = "GD",
            Price = 5

        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result => { UpdateMsg("Bought!"); },
            OnError);
    }
}