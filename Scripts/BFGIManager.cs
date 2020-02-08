using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************
 * 
 * Module BFGIManager
 * Analyses and store the Big Five Game Inventory of all the players
 * BFGI Values range from [2,10]
 * 
 * 
 * ***************************************/
public class BFGIManager : MonoBehaviour {

    public GameObject situacoes;
    private GameObject currentSituation;
    private bool BFGIOver = false;
    public int curSituationID = 1;
    public List<string> BFGIQuestions;

	
	/******************************************
	* 
	* public struct AveragePlayer
	* 
	* Attributes
	* 	public float O - The player's Openness value;
    *   public float C - The player's Conscientiousness value;
    *   public float E - The player's Extraversion value;
    *   public float A - The player's Neuroticism value;
    *   public float N - The player's Agreeableness value;
	* 
	* ***************************************/
    [System.Serializable]
    public struct AveragePlayer
    {
        public float O;
        public float C;
        public float E;
        public float A;
        public float N;
    }

    [System.Serializable]
    public class Player
    {
        public GameObject playerObj;
        public int playerID;
        public int currentStage;
        public int O;
        public int C;
        public int E;
        public int A;
        public int N;
        public bool ready;
        public List<int> answers;

        public Player(GameObject newPlayer)
        {
            this.playerObj = newPlayer;
            this.ready = true;
            this.answers = new List<int>();
        }
    }

    private static BFGIManager bgManager;
    public AveragePlayer avgPlayer;

	/******************************************
	* 
	* public static BFGIManager GetInstance()
	*		Singleton Implementation Method 	
	*
	* Parameters
	* 
	* Return
	*		Returns the object for the manager
	*
	* ***************************************/	
    public static BFGIManager GetInstance()
    {
        if (bgManager != null)
            return bgManager;
        throw new System.Exception("BFGIManager = null");
    }

    public List<Player> players;
	
	/******************************************
	* 
	* private void Start()
	*		Start the lists used in the class
	*
	* Parameters
	* 
	* Return
	*		
	*
	* ***************************************/	
    private void Start()
    {
        bgManager = this;
        players = new List<Player>();
        avgPlayer = new AveragePlayer();
    }
	
	/******************************************
	* 
	* public void NewPlayer(GameObject newPlayer, int playerID)
	*		Creates a new object player and adds it to the list of players
	*
	* Parameters
	* 		GameObject newPlayer - The unity game object of the player
	*		int playerID - The ID of the new player
	*
	* Return
	*
	* ***************************************/
    public void NewPlayer(GameObject newPlayer, int playerID)
    {
        Player novo = new Player(newPlayer);
        novo.currentStage = 1;
        novo.playerID = playerID;
        players.Add(novo);
    }

	/******************************************
	* 
	* public void Update()
	*		If the game is in the correct state, waits for the input of each player, 
	*		and gets their answer for the BFGI questionnaire.
	*
	* Parameters
	*
	* Return
	*
	* ***************************************/
    public void Update()
    {
        if (GameManagerScript.GetInstance().currentStage == GameManagerScript.GameStage.BFGI)
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
                            p.answers.Add(script.value);
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

	/******************************************
	* 
	* public void LoadSituation(int situationID)
    *		Checks if all the players are ready.
	*		Loads the next situation on the questionnaire or ends the questionnaire if it's over.
	*
	* Parameters
	*		int situationID - The id of the situation to be loaded
	*
	* Return
	*
	* ***************************************/
    public void LoadSituation(int situationID)
    {
        foreach (Player p in players)
        {
            if (p.ready == false)
            {
                return;
            }
        }
        if (situationID >10)
        {
            if (currentSituation != null)
            {
                Destroy(currentSituation);
            }
            CalculaOCEAN();
            foreach (Player p in players)
            {
                p.playerObj.SetActive(true);
            }
            BFGIOver = true;
            GameManagerScript.GetInstance().BFGIOver();
            return;
        }
        if (currentSituation != null)
        {
            Destroy(currentSituation);
        }
        curSituationID = situationID;
        foreach(Transform transSituation in situacoes.GetComponentsInChildren<Transform>())
        {
            GameObject situacao = transSituation.gameObject;
            if (situacao.name == "Situacao" + situationID)
            {
                GameObject newInstance = Instantiate(situacao, Vector3.zero, Quaternion.identity);
                newInstance.transform.localScale = new Vector3(2.5f,2.5f,2);
                newInstance.transform.position = new Vector3(-3.68f, 0, 0);
                Camera.main.transform.position = new Vector3(0,0,Camera.main.transform.position.z);
                currentSituation = newInstance;
                currentSituation.GetComponentsInChildren<SituacaoScript>()[0].displayText = BFGIQuestions[situationID - 1];
            }
        }
        foreach (Player p in players)
        {
                p.currentStage = situationID;
                p.ready = false;
                //p.playerObj.transform.position = Vector3.zero;
                p.playerObj.SetActive(true);
        }
    }
	
	/******************************************
	* 
	* private void CalculaOCEAN()
    *		Once the questionnaire is over, gets the answer of each player and calculates their OCEAN values.
	*
	* Parameters
	*
	* Return
	*
	* ***************************************/
    private void CalculaOCEAN()
    {
        foreach (Player p in players)
        {
            p.O = p.answers[6] + (6 - p.answers[2]);
            p.C = p.answers[7] + (6 - p.answers[1]);
            p.E = p.answers[0] + (6 - p.answers[9]);
            p.A = p.answers[8] + (6 - p.answers[4]);
            p.N = p.answers[3] + (6 - p.answers[5]);
        }
        CalculaAveragePlayer();
    }

	/******************************************
	* 
	* private void CalculaAveragePlayer()
    *		Averages the values of all players into a single player object.
	*
	* Parameters
	*
	* Return
	*
	* ***************************************/
    private void CalculaAveragePlayer()
    {
        foreach (Player p in players)
        {
            avgPlayer.O += p.O;
            avgPlayer.C += p.C;
            avgPlayer.E += p.E;
            avgPlayer.A += p.A;
            avgPlayer.N += p.N;
        }

        avgPlayer.O /= players.Count;
        avgPlayer.C /= players.Count;
        avgPlayer.E /= players.Count;
        avgPlayer.A /= players.Count;
        avgPlayer.N /= players.Count;
        
    }

	/******************************************
	* 
	* private int GetPlayerOCEAN()
    *		Get function of a specific OCEAN value of a single player.
	*
	* Parameters
	*		GameObject obj - The unity object of a player;
	*		string status - The value requested by the user;
	* Return
	*		returns the specific value requested, gotten from the local list of players.
	*
	* ***************************************/
    public int GetPlayerOCEAN(GameObject obj, string status)
    {
        foreach (Player p in players)
        {
            if (p.playerObj == obj)
            {
                switch (status)
                {
                    case "O":
                        return p.O;
                    case "C":
                        return p.C;
                    case "E":
                        return p.E;
                    case "A":
                        return p.A;
                    case "N":
                        return p.N;
                }
            }
        }
        return -1;
    }
	
	/******************************************
	* 
	* public List<Player> GetPlayers()
    *		Get function for the local list of players.
	*
	* Parameters
	*
	* Return
	*		Returns the local list of players.
	*
	* ***************************************/
    public List<Player> GetPlayers()
    {
        return players;
    }
	
	/******************************************
	* 
	* public bool AllowNewPlayer()
    *		The verification function that allows or denies the inclusion of a new player.
	*
	* Parameters
	*
	* Return
	*		Returns true if a player is allowed to enter the game, or false otherwise.
	*
	* ***************************************/
    public bool AllowNewPlayer()
    {
        if (curSituationID != 1)
            return false;
        return true;
    }

	/******************************************
	* 
	* private List<string> Display()
    *		The UI display function for the current class. Generates the lists to be displayed on either side of the screen.
	*		Creates a list of messages to be displayed on screen regarding the players OCEAN values.
	*
	* Parameters
	*
	* Return
	*		Returns the list of messages to be displayed by the UI Manager.
	*
	* ***************************************/
    public List<string> Display()
    {
        List<string> guitext = new List<string>();
        guitext.Add("");
        guitext.Add("");
        guitext[0] = "---BFGI----\n\n";
        guitext[1] = "---BFGI----\n\n";

        if (GameManagerScript.GetInstance().GetNumPlayers() < 3)
        {
            foreach (Player p in players)
            {
                guitext[0] += "Player_" + p.playerID + ".O = " + p.O + "\t";
                guitext[0] += "Player_" + p.playerID + ".C = " + p.C + "\n";
                guitext[0] += "Player_" + p.playerID + ".E = " + p.E + "\t";
                guitext[0] += "Player_" + p.playerID + ".A = " + p.A + "\n";
                guitext[0] += "Player_" + p.playerID + ".N = " + p.N + "\n\n";
            }

            return guitext;
        }
        else
        {
            foreach (Player p in players)
            {
                int side = 0;
                if (p.playerID > 1)
                    side = 1;

                guitext[side] += "Player_" + p.playerID + ".O = " + p.O + "\t";
                guitext[side] += "Player_" + p.playerID + ".C = " + p.C + "\n";
                guitext[side] += "Player_" + p.playerID + ".E = " + p.E + "\t";
                guitext[side] += "Player_" + p.playerID + ".A = " + p.A + "\n";
                guitext[side] += "Player_" + p.playerID + ".N = " + p.N + "\n\n";

            }
            return guitext;
        }

        

    }
}
