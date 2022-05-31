using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Leaderboard : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI displayText_DisplayName, displayText_Score;
    void Start()
    {
        
    }

    public void OnButtonGetLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highscore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        displayText_DisplayName.text = displayText_Score.text = "";
        foreach (var item in r.Leaderboard)
        {
            //string onerow = item.Position + "/" + item.PlayFabId + "/" + item.DisplayName + "/" + item.StatValue + "\n";
            displayText_DisplayName.text += (item.DisplayName + "\n");
            displayText_Score.text += (item.StatValue + "\n");
        }
    }

    void OnError(PlayFabError e)
    {
        Debug.Log("Error" + e.GenerateErrorReport());
        //UpdateMsg("Error" + e.GenerateErrorReport());
    }
}
