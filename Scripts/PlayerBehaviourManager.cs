using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/********************************************
 * 
 * Module PlayerBehaviourManager
 * Analyses and store the behaviour of all the players
 * as well as store players empathy levels among themselves
 * Empathy Values range from [-1.0f,1.0f]
 * 
 * 
 * *****************************************/
public class PlayerBehaviourManager : MonoBehaviour
{

    [System.Serializable]
    public class Player
    {
        [System.Serializable]
        public class Empathy
        {
            public GameObject playerObj;
            [Range(-1,1)]
            public float empathy;

            public Empathy(GameObject playerOBJ)
            {
                this.playerObj = playerOBJ;
                this.empathy = UnityEngine.Random.Range(-1f,1f) ;
            }
        }

        public GameObject playerObj;
        public int playerID;
        public int enemiesKilled;
        public int victimsSaved = 0;
        public int itemsObtained;
        public int numAttacks = 0;
        public float lastMovement;
        public float lastAttack;
        public float avgItemObtTime;
        public float avgVicSaveTime;
        public float avgEnemyKillTime;
        public Vector3 lastPosition;
        public List<Empathy> empathies;

        public Player(GameObject newPlayer)
        {
            this.playerObj = newPlayer;
            this.empathies = new List<Empathy>();
        }

        public Player(Player template)
        {
            this.playerObj = template.playerObj;
            this.playerID = template.playerID;
            this.enemiesKilled = template.enemiesKilled;
            this.lastMovement = template.lastMovement;
            this.lastAttack = template.lastAttack;
            this.numAttacks = template.numAttacks;
            this.victimsSaved = template.victimsSaved;
            this.itemsObtained = template.itemsObtained;
            this.lastPosition = template.playerObj.transform.position;
            this.avgItemObtTime = template.avgItemObtTime;
            this.avgVicSaveTime = template.avgVicSaveTime;
            this.avgEnemyKillTime = template.avgEnemyKillTime;
            this.empathies = template.empathies;
        }
    }


    public List<Player> players;
    public List<Player> playersScreenShot;
    public float ScreenShotCD = 10f;
    private float lastScreenShotTime;
    private static PlayerBehaviourManager pbManager;
    private int empathyCases = 1;

    public static PlayerBehaviourManager GetInstance()
    {
        if (pbManager != null)
            return pbManager;
        throw new System.Exception("pbManager = null");
    }
    private void Start()
    {
        pbManager = this;
        players = new List<Player>();
        lastScreenShotTime = -ScreenShotCD;
    }

    public void NewPlayer(GameObject newPlayer, int playerID)
    {
        Player novo = new Player(newPlayer);
        novo.playerID = playerID;
        players.Add(novo);


        foreach (Player p in players)
        {
            if (p.playerObj == newPlayer)
                continue;
            Player.Empathy empatiaPNew = new Player.Empathy(newPlayer);
            p.empathies.Add(empatiaPNew);
            Player.Empathy empatiaNewP = new Player.Empathy(p.playerObj);
            novo.empathies.Add(empatiaNewP);
        }


    }

    void Update () {
	    if (Time.time - lastScreenShotTime > ScreenShotCD)
        {
            lastScreenShotTime = Time.time;
            DeltaPlayers();
            playersScreenShot.Clear();
            foreach (Player player in players)
            {
                playersScreenShot.Add(new Player(player));
            }
        }

        for (int i = 0; i <= empathyCases; i++)
        {
            if (Input.GetButton("EmpathyCase"+i))
            {
                SetEmpathies(i);
            }
        }
	}

    private void SetEmpathies(int empathyCase)
    {
        switch (empathyCase)
        {
            case 0: //Caso 1 empatia 0, outros empatia positiva
                foreach (Player.Empathy emp in players[0].empathies)
                {
                    emp.empathy = 0;
                }
                foreach (Player p in players)
                {
                    foreach (Player.Empathy emp in p.empathies)
                    {
                        if (emp.playerObj == players[0].playerObj)
                            emp.empathy = 1;
                    }
                }
                break;
            case 1: //Caso Empatias Random
                foreach (Player p in players)
                {
                    foreach (Player.Empathy emp in p.empathies)
                    {
                        emp.empathy = UnityEngine.Random.Range(-1f, 1f);
                    }
                }
                break;
            default:
                break;
        }
        
    }

    private void DeltaPlayers()
    {
        foreach (Player player in players)
        {
            foreach(Player template in playersScreenShot)
            {
                if (player.playerID == template.playerID)
                {
                    CheckDeltaDistance(player, template);
                    CheckDeltaAttacks(player, template);
                    CheckDeltaVictims(player, template);   
                }
            }
        }
    }

    public void HitEnemy(GameObject player, GameObject enemy)
    {
         foreach (Player play in players)
        {
            if (play.playerObj == player)
            {
                float lifeTime = Time.time - enemy.GetComponent<Enemy>().spawnTime;

                float asd = play.enemiesKilled * play.avgEnemyKillTime;

                play.enemiesKilled++;
                play.avgEnemyKillTime = (asd + lifeTime) / play.enemiesKilled;

            }
        }
    }

    public void DestroyVictim(GameObject victimObj)
    {
        //float lifeTime = Time.time - SpawnTime;
    }

    public void movePlayer(GameObject playerObj)
    {
        foreach (Player player in players)
        {
            if (player.playerObj == playerObj)
            {
                float timeStanding = Time.time - player.lastMovement;
                player.lastMovement = Time.time;
            }
        }
    }

    public void SaveVictim(GameObject playerObj, GameObject victim)
    {
        foreach (Player player in players)
        {
            if (player.playerObj == playerObj)
            {
                float lifeTime = Time.time - victim.GetComponent<Victim>().spawnTime;
                float asd = player.victimsSaved * player.avgVicSaveTime;

                player.victimsSaved++;
                player.avgVicSaveTime = (asd + lifeTime) / player.victimsSaved;
            }
        }
    }

    public void GetItem(GameObject playerObj, ScenarioManager.Item item)
    {
        foreach (Player player in players)
        {
            if (player.playerObj == playerObj)
            {
                float itemLifeTime = Time.time - item.spawnTime;
                float asd = player.itemsObtained * player.avgItemObtTime;
                
                player.itemsObtained++;
                player.avgItemObtTime = (asd + itemLifeTime) / player.itemsObtained;
            }
        }
    }

    public void PlayerAttack(GameObject playerObj)
    {
        foreach (Player player in players)
        {
            if (player.playerObj == playerObj)
            {
                float timeBetweenAttacks = Time.time - player.lastAttack;
                player.lastAttack = Time.time;
                player.numAttacks++;
            }
        }
    }

    public List<string> Display()
    {
        List<string> guitext = new List<string>();
        guitext.Add("");
        guitext.Add("");
        guitext[0] = "---PBManager----\n\n";
        guitext[1] = "---PBManager----\n\n";


        if (GameManagerScript.GetInstance().GetNumPlayers() < 3)
        {
            foreach (Player p in players)
            {
                guitext[0] += "Player_" + p.playerID + "\nEmpathy Levels:\n";
                foreach (Player.Empathy emp in p.empathies)
                {
                    guitext[0] += emp.playerObj.name + ": " + (emp.empathy * 100).ToString("F2") + "%\n";
                    //guitext[0] += emp.playerObj.name + ": " + emp.empathy  + "\n";
                }
                guitext[0] += "\n";
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

                guitext[side] += "Player_" + p.playerID + "\nEmpathy Levels:\n";
                foreach (Player.Empathy emp in p.empathies)
                {
                    guitext[side] += emp.playerObj.name + ": " + (emp.empathy * 100).ToString("F2") + "%\n";
                    //guitext[side] += emp.playerObj.name + ": " + emp.empathy + "\n";
                }
                guitext[side] += "\n";
            }
            return guitext;
        }
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    #region DeltaFunctions
    private void CheckDeltaDistance(Player player, Player template)
    {
        Vector3 distanceTravelled = player.playerObj.transform.position - template.lastPosition;
        if (distanceTravelled.magnitude != 0)
        {
            //print("Player" + player.playerID + ">: Moved " + distanceTravelled.magnitude + "pixels");
        }
        else
        {
            //print("<Player" + player.playerID + ">: Has not moved");
        }
    }
    private void CheckDeltaVictims(Player player, Player template)
    {
        float deltaVictims = player.victimsSaved - template.victimsSaved;
        //print("<Player" + player.playerID + ">: # of victims saved -> " + deltaVictims);
    }
    private void CheckDeltaAttacks(Player player, Player template)
    {
        float deltaAttackTime = player.lastAttack - template.lastAttack;
        float deltaAttacks = player.numAttacks - template.numAttacks;
        float deltaEnemiesKilled = player.enemiesKilled - template.enemiesKilled;
        float accuracy = deltaEnemiesKilled / deltaAttacks;
        accuracy = (deltaAttacks == 0) ? 0 : accuracy;
        accuracy = (accuracy > 1) ? 1 : accuracy;
        float missesPercent = 1 - accuracy;
        float percentEnemyKilled = deltaEnemiesKilled / ScenarioManager.GetInstance().currentNumEnemies;

        /*print("<Player" + player.playerID + ">: Time between attacks -> " + ((deltaAttackTime == 0) ? "Has not attack" : deltaAttackTime.ToString()));
        print("<Player" + player.playerID + ">: # of attacks -> " + deltaAttacks);
        print("<Player" + player.playerID + ">: # of enemies killed -> " +deltaEnemiesKilled);
        print("<Player" + player.playerID + ">: Accuracy -> " + accuracy * 100 + "%");
        print("<Player" + player.playerID + ">: Percent of misses -> " + missesPercent * 100 + "%");
        print("<Player" + player.playerID + ">: Percent of enemies Killed -> " + percentEnemyKilled * 100 + "%");*/

    }

   
    #endregion
}
