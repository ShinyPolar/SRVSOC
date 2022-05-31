using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class PlayFabUserMgt : MonoBehaviour
{
    [SerializeField] TMP_InputField userNameLogin, userEmailLogin, userPasswordLogin,
                                    userNameReg, userEmailReg, userPasswordReg, userPasswordAgainReg, displayName, 
                                    currentScore, XP, level,
                                    emailForgotPassword;
    [SerializeField] TextMeshProUGUI Msg;
    bool wUsername, wEmail;

    public void CheckRegisterInputFields()
    {
        if (userNameReg.text == "")
        {
            UpdateMsg("Username is empty");
        }
        else if (userPasswordReg.text == "" || userPasswordAgainReg.text == "")
        {
            UpdateMsg("Password is empty");
        }
        else if (userEmailReg.text == "")
        {
            UpdateMsg("Email is empty");
        }
        else if (userPasswordAgainReg.text != userPasswordReg.text)
        {
            UpdateMsg("Password is not the same");
        }
        else
        {
            OnButtonRegUser();
        }
    }
    public void OnButtonRegUser()
    {
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmailReg.text,
            Password = userPasswordReg.text,
            Username = userNameReg.text,
            DisplayName = displayName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegSuccess, OnError);
    }

    void OnRegSuccess(RegisterPlayFabUserResult r)
    {
        //Debug.Log("Register Success!");
        UpdateMsg("Registration success!");


        List<LevelInfo> levelInfo = new List<LevelInfo>();
        levelInfo.Add(new LevelInfo(0,1));
        string stringListAsJson = JsonUtility.ToJson(new JSListWrapper<LevelInfo>(levelInfo));
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"LevelInfo", stringListAsJson }
            }
        }, null, null);
        PlayFabClientAPI.AddOrUpdateContactEmail(new AddOrUpdateContactEmailRequest()
        {
            EmailAddress = userEmailReg.text
        }, null, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMsg("display name updated!" + r.DisplayName);
    }

    void OnError(PlayFabError e)
    {
        //Debug.Log("Error" + e.GenerateErrorReport());
        UpdateMsg("Error"+e.GenerateErrorReport());
        wUsername = wEmail = false;
    }
    void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        Msg.text = msg;
    }

    public void OnButtonLoginEmail()
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userEmailLogin.text,
            Password = userPasswordLogin.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        wEmail = true;
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
    }
    void OnLoginSuccess(LoginResult r)
    {
        UpdateMsg("Login Success!" + r.PlayFabId + r.InfoResultPayload.PlayerProfile.DisplayName);
        var lbreq = new GetAccountInfoRequest();
        if (wUsername)
            lbreq.Username = userNameLogin.text;
        else if (wEmail)
            lbreq.Email = userEmailLogin.text;
        GetPlayerInventory();
        GetUserData();
        PlayFabClientAPI.GetAccountInfo(lbreq, OnAccountInfoGet, OnError);
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
                    if (i.ItemId == "S01_Raq")
                        PlayerStatsManager.bRaq = true;
                    if (i.ItemId == "C01_ExtraLife")
                    {
                        PlayerStatsManager.bExtraLife = true;
                        PlayerStatsManager.ExtraLifeID = i.ItemInstanceId;
                    }
                }
            }, OnError);
    }
    public void OnAccountInfoGet(GetAccountInfoResult r)
    {
        PlayerStatsManager.DisplayName = r.AccountInfo.TitleInfo.DisplayName;
        PlayerStatsManager.PlayFabID = r.AccountInfo.PlayFabId;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void OnButtonLoginUserName()
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = userNameLogin.text,
            Password = userPasswordLogin.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        wUsername = true;
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, OnError);
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
        string LeaderboardStr = "Leaderboard\n";
        foreach (var item in r.Leaderboard)
        {
            string onerow = item.Position + "/" + item.PlayFabId + "/" + item.DisplayName + "/" + item.StatValue + "\n";
            LeaderboardStr += onerow; // combine all display into 1 string 1
        }
        UpdateMsg(LeaderboardStr);
    }
    public void OnButtonSendLeaderboard()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="highscore",
                    Value=int.Parse(currentScore.text)
                }
            }
        };
        UpdateMsg("Submitting score:" + currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMsg("Successful leaderboard sent:" + r.ToString());
    }

    //public void OnButtonSetUserData()
    //{
    //    PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
    //    {
    //        Data = new Dictionary<string, string>()
    //        {
    //            {"XP", XP.text.ToString() },
    //            { "Level", level.text.ToString() }
    //        }
    //    },
    //    result => UpdateMsg("Successfully updated user data"),
    //    error =>
    //    {
    //        UpdateMsg("Error setting user data");
    //        UpdateMsg(error.GenerateErrorReport());
    //    });
    //}
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                UpdateMsg("Got user data:");
                if (result.Data != null && result.Data.ContainsKey("LevelInfo"))
                {
                    JSListWrapper<LevelInfo> jlw = JsonUtility.FromJson<JSListWrapper<LevelInfo>>(result.Data["LevelInfo"].Value);
                    PlayerStatsManager.XP = jlw.list[0].xp.ToString();
                    PlayerStatsManager.Level = jlw.list[0].level.ToString();
                }
            }, (error) =>
            {
                UpdateMsg("Error retrieving user data");
                UpdateMsg(error.GenerateErrorReport());
            });

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r =>
            {
                PlayerStatsManager.Gold = r.VirtualCurrency["GD"];
            }, OnError);

    }

//    Debug.Log("received JSON data");
//        if (r.Data != null && r.Data.ContainsKey("Skills"))
//        {
//            Debug.Log(r.Data["Skills"].Value);
//            JSListWrapper<Skill> jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);
//            for(int i=0;i<SkillBoxes.Length;i++)
//            {
//                SkillBoxes[i].SetUI(jlw.list[i]);
//}
//        }

    public void GoToScene(string scn)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scn);
    }

    public void OnButtonSendEmail()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailForgotPassword.text,
            TitleId = "FDB85"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request , res =>
        {
            Debug.Log("An account recovery email has been sent to the player's email address.");
        }, FailureCallback);
    }

    void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.Log(error.GenerateErrorReport());
    }

    public void OnButtonLoginAsGuest()
    {
        System.Random rnd = new System.Random();
        var request = new LoginWithCustomIDRequest
        {
            TitleId = "FDB85",
            CustomId = rnd.Next(1000000, 10000000).ToString(),
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }
}

[System.Serializable]
public class JSListWrapper<T>
{
    public List<T> list;
    public JSListWrapper(List<T> list) => this.list = list;
}

