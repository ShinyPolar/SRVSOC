using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    GameController gameController;

    private void Start() {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if(gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        } 
        else{
            Debug.Log("GameController object not found");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag != "Boundary"){
            Instantiate(explosion, transform.position, transform.rotation);
            if(other.tag == "Player"){
                if (!PlayerStatsManager.bExtraLife)
                {
                    Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                    gameController.gameIsOver();
                    Destroy(other.gameObject);
                }
                else
                {
                    PlayerStatsManager.bExtraLife = false;
                    PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest()
                    {
                        ConsumeCount = 1,
                        ItemInstanceId = PlayerStatsManager.ExtraLifeID
                    }, null, null) ;
                }

            }
            gameController.addScore(scoreValue);
            System.Random rnd = new System.Random();
            if (rnd.Next(1, 5) == 1)
            {
                gameController.randomGold++;
            }
            if (other.tag != "Player")
                Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
