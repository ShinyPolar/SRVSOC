using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class GameController : MonoBehaviour {
    public int randomGold;
    [SerializeField] TextMeshProUGUI goldText, extraLifeText;

    public Vector3 positionAsteroid;
    public GameObject asteroid;
    public GameObject asteroid2;
    public GameObject asteroid3;
    public int hazardCount;
    public float startWait;
    public float spawnWait;
    public float waitForWaves;
    public Text scoreText;
    public Text gameOverText;
    public Text restartText;
    public Text mainMenuText;

    private bool restart;
    private bool gameOver;
    private int score;
    private List<GameObject> asteroids;

    private void Start() {
        asteroids = new List<GameObject> {
            asteroid,
            asteroid2,
            asteroid3
        };
        gameOverText.text = "";
        restartText.text = "";
        mainMenuText.text = "";
        restart = false;
        gameOver = false;
        score = 0;
        StartCoroutine(spawnWaves());
        updateScore();
    }

    private void Update() {
        if(restart){
            if(Input.GetKey(KeyCode.R)){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } 
            else if(Input.GetKey(KeyCode.Q)){
                
                SceneManager.LoadScene("MainMenu");
            }
        }
        if (gameOver) {
            restartText.text = "Press R to restart game";
            mainMenuText.text = "Press Q to go back to main menu";
            restart = true;
        }
        if (!gameOver)
        {
            goldText.text = "Gold Gained: " + randomGold;
        }
        if (PlayerStatsManager.bExtraLife)
            extraLifeText.gameObject.SetActive(true);
        else
            extraLifeText.gameObject.SetActive(false);
    }

    private IEnumerator spawnWaves(){
        yield return new WaitForSeconds(startWait);
        while(true){
            for (int i = 0; i < hazardCount;i++){
                Vector3 position = new Vector3(Random.Range(-positionAsteroid.x, positionAsteroid.x), positionAsteroid.y, positionAsteroid.z);
                Quaternion rotation = Quaternion.identity;
                Instantiate(asteroids[Random.Range(0,3)], position, rotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waitForWaves);
            if(gameOver){
                break;
            }
        }
    }

    public void gameIsOver(){
        gameOverText.text = "Game Over";
        gameOver = true;
        UpdateHighscore();
        CalculateScore();
        AddGold();
    }

    public void addScore(int score){
        this.score += score;
        updateScore();
    }

    void updateScore(){
        scoreText.text = "Score:" + score;
    }

    void UpdateHighscore()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName="highscore",
                            Value=score
                        }
                    }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(req, null, null);
    }

    void CalculateScore()
    {
        int newXP = int.Parse(PlayerStatsManager.XP) + score;
        int newLevel = int.Parse(PlayerStatsManager.Level);
        if (score != 0)
        {
            while (newXP > 300)
            {
                newXP -= 300;
                newLevel += 1;
            }
        }

        PlayerStatsManager.XP = newXP.ToString();
        PlayerStatsManager.Level = newLevel.ToString();


        List<LevelInfo> levelInfo = new List<LevelInfo>();
        levelInfo.Add(new LevelInfo(newXP, newLevel));
        string stringListAsJson = JsonUtility.ToJson(new JSListWrapper<LevelInfo>(levelInfo));
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"LevelInfo", stringListAsJson }
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log("Error setting user data");
        });
    }

    void AddGold()
    {
        PlayerStatsManager.Gold += randomGold;
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest()
        {
            Amount = randomGold,
            VirtualCurrency = "GD"
        }, null, OnError);
    }

    void OnError(PlayFabError e)
    {
        Debug.Log(e.GenerateErrorReport());
    }
}
