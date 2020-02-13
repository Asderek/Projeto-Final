using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public int newPlayerId = 0;
    public float playerStunLength = 5f;

    public enum GameStage
    {
        PlayerType,
        BFGI,
        MainGame
    }
    public enum Tags
    {
        Objective,
        Player,
        Enemy,
        Item,
        SafeHaven
    }

    [HideInInspector]
    public List<GameObject> players;
    public GameStage currentStage = GameStage.PlayerType;

    private static GameManagerScript gameManager;

    // Update is called once per frame
    private void Start()
    {
        players = new List<GameObject>();
        gameManager = this;
    }

    void Update () {

        if (Input.GetButtonDown("Time+"))
        {
            Time.timeScale += 1;
        }
        if (Input.GetButtonDown("Time-"))
        {
            Time.timeScale -= 1;
        }
        if (Input.GetButtonDown("TimeReset"))
        {
            Time.timeScale = 1;
        }

        if (PlayerTypeManager.GetInstance().AllowNewPlayer())
        {
            for (int cont = 0; cont < 4; cont++)
            {
                if (Input.GetButtonDown("Player" + cont + "_START"))
                {
                    foreach (GameObject player in players)
                    {
                        if (player.GetComponent<PlayerMovementScript>().playerID == cont)
                        {
                            return;
                        }
                    }
                    NewPlayer(cont);
                }
            }
        }
	}

    /******************************************
    * 
    * public void NewPlayer()
    *		Creates a new player with the current playerID value,
    *		signals the other managers to insert a new player on their player list,
    *		sets the BFGI questionnaire back to 1;
    *
    * Parameters
    *
    * Return
    *
    * ***************************************/	
    public void NewPlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        newPlayer.name = "Player_" + newPlayerId;
        newPlayer.GetComponent<PlayerMovementScript>().playerID = newPlayerId;
        newPlayer.GetComponent<MeshRenderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        newPlayer.tag = GameManagerScript.Tags.Player.ToString();
        players.Add(newPlayer);

        BFGIManager.GetInstance().NewPlayer(newPlayer, newPlayerId);
        PlayerBehaviourManager.GetInstance().NewPlayer(newPlayer, newPlayerId);
        newPlayerId++;
        BFGIManager.GetInstance().LoadSituation(1);
    }

    /******************************************
    * 
    * public void NewPlayer(int playerID)
    *		Creates a new player with a specified playerID value,
    *		signals the other managers to insert a new player on their player list,
    *		sets the BFGI questionnaire back to 1;
    *
    * Parameters
    *		int playerID - The number ID for the new player.
    * Return
    *
    * ***************************************/		
    private void NewPlayer(int playerID)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        newPlayer.name = "Player_" + playerID;
        newPlayer.GetComponent<PlayerMovementScript>().playerID = playerID;
        newPlayer.GetComponent<MeshRenderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        newPlayer.tag = GameManagerScript.Tags.Player.ToString();
        players.Add(newPlayer);

        BFGIManager.GetInstance().NewPlayer(newPlayer, playerID);
        PlayerBehaviourManager.GetInstance().NewPlayer(newPlayer, playerID);
        PlayerTypeManager.GetInstance().NewPlayer(newPlayer, playerID);
        UIManagerScript.GetInstance().StartDisplayOnScreen(newPlayer,"PlayerName_"+newPlayer.name,null);
    }

    /******************************************
    * 
    * public void CreateCenario()
    *		Signals the ScenarioManager to validate and create the next Scenario
    *
    * Parameters
    *
    * Return
    *
    * ***************************************/		
    public void CreateCenario()
    {
        print("GAMEMANAGER_CreateCenario");
        ScenarioManager.GetInstance().CheckNewScenario();
    }
	
    public static GameManagerScript GetInstance()
    {
        if (gameManager != null)
            return gameManager;
        throw new System.Exception("gameManager = null");
    }

    /******************************************
    * 
    * public void HitEnemy(GameObject player, GameObject enemy)
    *		Signals the PBManager that the player hit the enemy, for statistical reference,
    *		removes the enemy from the list, checks if a new scenario should be loaded,
    *		and destryos the enemy GameObject;
    *
    * Parameters
    *		int playerID - The number ID for the new player.
    * Return
    *
    * ***************************************/		
    public void HitEnemy(GameObject player, GameObject enemy)
    {
        PlayerBehaviourManager.GetInstance().HitEnemy(player, enemy);
        ScenarioManager.GetInstance().enemies.Remove(enemy);
        ScenarioManager.GetInstance().CheckNewScenario();
        Destroy(enemy);

    }

    /******************************************
    * 
    * public void BFGIOver()
    *		Moves the game to the next stage;
    *
    * Parameters
    *		
    * Return
    *
    * ***************************************/		
    public void BFGIOver()
    {
        currentStage++;
        CreateCenario();
    }

    /******************************************
    * 
    * public void PlayerTypeOver()
    *		Moves the game to the next stage;
    *
    * Parameters
    *		
    * Return
    *
    * ***************************************/		
    public void PlayerTypeOver()
    {
        currentStage++;
        print(currentStage.ToString());
        BFGIManager.GetInstance().LoadSituation(1);
    }

    public int GetNumPlayers()
    {
        return players.Count;
    }


}
