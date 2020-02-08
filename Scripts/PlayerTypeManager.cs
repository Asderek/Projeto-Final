using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/********************************************
 * 
 * Module PlayerTypeManager
 * Analyses and store the gamer type of all the players
 * these are store in the form of agression, rescuer or greed
 *    agression - players like more violent type of games
 *    rescuer   - players prefer media involving rescuing 
 *                of a helpless character
 *    greed     - players prefer media involving search and
 *                collection of items for progression
 *                
 * Player Type Values range from [1,5]
 * 
 * 
 * *****************************************/
public class PlayerTypeManager : MonoBehaviour
{
    [System.Serializable]
    public class Player
    {
        public int playerID;
        public GameObject playerObj;
        public int agression;
        public int rescuer;
        public int greed;
        public bool ready;
        public int currentStage;

        public Player(GameObject obj)
        {
            this.playerObj = obj;
            this.agression = 0;
            this.rescuer = 0;
            this.greed = 0;
            this.ready = true;
            this.currentStage = 0;
        }
    }
    private int curSituationID = 0;
    public GameObject currentSituation;
    public List<string> situations;

    private static PlayerTypeManager ptManager;

    public static PlayerTypeManager GetInstance()
    {
        if (ptManager != null)
            return ptManager;
        throw new System.Exception("PlayerTypeManager = null");
    }

    public List<Player> players;

    private void Start()
    {
        ptManager = this;
        players = new List<Player>();

        if (GameManagerScript.GetInstance().currentStage == GameManagerScript.GameStage.PlayerType)
        {
            Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
            LoadSituation(curSituationID);
            currentSituation = Instantiate(currentSituation, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.GetInstance().currentStage == GameManagerScript.GameStage.PlayerType)
        {
            foreach (Player p in players)
            {
                if (Input.GetButtonDown("Player" + p.playerID + "_X"))
                {
                    GameObject msg = UIManagerScript.GetInstance().CheckMsg(p.playerObj);
                    if (msg != null)
                    {
                        SituacaoScript script = msg.GetComponent<SituacaoScript>();
                        if (script != null)
                        {
                            switch (curSituationID)
                            {
                                case 0:
                                    p.agression = script.value;
                                    break;
                                case 1:
                                    p.rescuer = script.value;
                                    break;
                                case 2:
                                    p.greed = script.value;
                                    break;
                            }
                            p.ready = true;
                            UIManagerScript.GetInstance().StopDisplayOnScreen(p.playerObj);
                            p.playerObj.SetActive(false);
                            LoadSituation(p.currentStage + 1);
                        }
                    }
                }
            }
        }
    }

    public void NewPlayer(GameObject newPlayer, int playerID)
    {
        Player novo = new Player(newPlayer);
        novo.playerID = playerID;
        if (players.Count > 0)
            novo.ready = false;
        players.Add(novo);
    }

    public bool AllowNewPlayer()
    {
        if (curSituationID != 0)
            return false;
        return true;
    }

    private void LoadSituation(int situation)
    {
        foreach (Player p in players)
        {
            if (!p.ready)
                return;
        }
        if (situation >= situations.Count)
        {
            if (currentSituation != null)
            {
                Destroy(currentSituation);
            }
            foreach (Player p in players)
            {
                p.playerObj.SetActive(true);
            }
            GameManagerScript.GetInstance().PlayerTypeOver();
            return;
        }
        else
        {
            curSituationID = situation;
            if (currentSituation != null)
            {
                currentSituation.GetComponentsInChildren<SituacaoScript>()[0].ChangeText(situations[situation]);
            }
            
            foreach (Player p in players)
            {
                p.currentStage = situation;
                p.ready = false;
                p.playerObj.SetActive(true);
            }
        }
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public List<string> Display()
    {
        List<string> guitext = new List<string>();
        guitext.Add("");
        guitext.Add("");
        guitext[0] = "---PTManager----\n\n";
        guitext[1] = "---PTManager----\n\n";

        if (GameManagerScript.GetInstance().GetNumPlayers() < 3)
        {
            if (curSituationID == 0)
            {
                int qtdPlayersRestantes = 4 - players.Count;
                guitext[0] += "Press Start to join the Game (" + qtdPlayersRestantes + " left)\n\n";
            }
            foreach (Player p in players)
            {
                guitext[0] += "Player_" + p.playerID + "\nPlayer Preferences:\n";
                guitext[0] += "Agression = " + (p.agression - 1) * 25 + "%\n";
                guitext[0] += "Rescuer = " + (p.rescuer - 1) * 25 + "%\n";
                guitext[0] += "Greed = " + (p.greed - 1) * 25 + "%\n";
                guitext[0] += "\n";
            }
            return guitext;
        }
        else
        {
            if (curSituationID == 0)
            {
                int qtdPlayersRestantes = 4 - players.Count;
                guitext[0] += "Press Start to join the Game (" + qtdPlayersRestantes + " left)\n\n";
                guitext[1] += "Press Start to join the Game (" + qtdPlayersRestantes + " left)\n\n";
            }
            foreach (Player p in players)
            {
                int side = 0;
                if (p.playerID > 1)
                    side = 1;

                guitext[side] += "Player_" + p.playerID + "\nPlayer Preferences:\n";
                guitext[side] += "Agression = " + (p.agression - 1) * 25 + "%\n";
                guitext[side] += "Rescuer = " + (p.rescuer - 1) * 25 + "%\n";
                guitext[side] += "Greed = " + (p.greed - 1) * 25 + "%\n";
                guitext[side] += "\n";
            }
            return guitext;
        }
    }
}
