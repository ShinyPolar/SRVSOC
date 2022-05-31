using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerStats : MonoBehaviour
{
    public TextMeshProUGUI displayName, level, XP, gold;

    void Start()
    {
        displayName.text = PlayerStatsManager.DisplayName;
        level.text = "Level: " + PlayerStatsManager.Level;
        XP.text = PlayerStatsManager.XP + "/300";
        gold.text = "Gold: " + PlayerStatsManager.Gold;
    }

    public void UpdateGold()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r =>
            {
                PlayerStatsManager.Gold = r.VirtualCurrency["GD"];
                gold.text = "Gold: " + PlayerStatsManager.Gold;
            }, null);
    }
    private void Update()
    {
        //Debug.Log(displayName.text);
    }
}
