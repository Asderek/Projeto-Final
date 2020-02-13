using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BaffaAlg : MonoBehaviour
{
    [System.Serializable]
    public class Player
    {
        public GameObject playerObj;
        public int playerID;
        public List<PlayerBehaviourManager.Player.Empathy> playerEmpathies;
        public int O;
        public int C;
        public int E;
        public int A;
        public int N;
        public int agression;
        public int rescuer;
        public int greed;

        public Player(int playerID)
        {
            this.playerID = playerID;
        }

        public float GetRelation(Player target)
        {
            foreach (PlayerBehaviourManager.Player.Empathy empathy in this.playerEmpathies)
            {
                if (empathy.playerObj == target.playerObj)
                {
                    return empathy.empathy;
                }
            }
            return 0;
        }
    }

    public Player leader;
    public List<Player> players;
    // Use this for initialization
    void Start()
    {
        baffaAlg = this;
        players = new List<Player>();
    }

    private static BaffaAlg baffaAlg;
    public static BaffaAlg GetInstance()
    {
        if (baffaAlg != null)
            return baffaAlg;
        throw new System.Exception("BaffaAlg = null");
    }

    /* *************************************************************************************
     * private void FindLeader()
     *      Given a current quest, finds a list of the best quest for this particular group
     *      of players
     *       
     * Parameters
     *      QuestManager.Quest currentQuest - The current active quest.
     * Return
     *      sampleSpace[randomQuest].questID - The id of the next quest to become active
     * *************************************************************************************/
    public int GetBestQuest(QuestManager.Quest currentQuest)
    {
        UpdatePlayers();
        FindLeader();
        List<QuestManager.Quest> sampleSpace = GenerateSampleSpace(currentQuest);
        

        //print("BAFFAALG_GETBESTQUEST currentQuest.nextQuests.Count = " + currentQuest.nextQuests.Count);
        int randomQuest = Random.Range(0, sampleSpace.Count);
        return sampleSpace[randomQuest].questID;
    }

    /* *************************************************************************************
     * private List<QuestManager.Quest> GenerateSampleSpace(QuestManager.Quest currentQuest)
     *      This method filter the currentQuest list of nextQuests (possible quests following the current)
     *      and generates a list with the best candidates for the next quest
     *       
     * Parameters
     *      currentQuest - The currentQuest of the game
     * 
     * Return
     *      List<QuestManager.Quest> - a subset of currentQuest.nextQuests filtered by the leaders preference
     *  
     *  OBS
     *      It was stipulated that, for a leader with openness < 4 (30% or lower) the players preferences 
     *      would be as follows:
     *          Considering the intensities for each aspect of a quest 
     *          (enemyIntensity, victimIntensity, objectiveIntensity) and the levels of each player type
     *          (agression, rescuer, greed) the interest table would be:
     *      
     *              intensities    
     *       /      None    Low    Medium  High 
     * t     0       v       x        x     x
     * y     25%     v       v        x     x
     * p     50%     v       v        v     v
     * e     75%     x       x        v     v
     *       100%    x       x        x     v
     *       
     *       Where v represents that a player is willing to perform quests with this intensity of *aspect*
     *       and x represents an unwillingness to perform said quest
     * 
     * *************************************************************************************/
    private List<QuestManager.Quest> GenerateSampleSpace(QuestManager.Quest currentQuest)
    {
        List<QuestManager.Quest> sampleSpace = new List<QuestManager.Quest>();
        print("currentQuest.nextQuests.count = " + currentQuest.nextQuests.Count);
        if (leader.O < 4)
        {
            foreach (QuestManager.Quest quest in currentQuest.nextQuests)
            {
                switch (leader.agression)
                {
                    case 1:
                        if (quest.enemyIntensity > QuestManager.Intensity.Low)
                            continue;
                        break;
                    case 2:
                        if (quest.enemyIntensity > QuestManager.Intensity.Medium)
                            continue;
                        break;
                    case 3:
                        break;
                    case 4:
                        if (quest.enemyIntensity < QuestManager.Intensity.Medium)
                            continue;
                        break;
                    case 5:
                        print(quest.questID + "->" + quest.enemyIntensity.ToString());
                        if (quest.enemyIntensity < QuestManager.Intensity.High)
                        {
                            print("continue");
                            continue;
                        }
                        break;
                }
                switch (leader.rescuer)
                {
                    case 1:
                        print(quest.questID + "->" + quest.victimIntensity.ToString());
                        if (quest.victimIntensity > QuestManager.Intensity.Low)
                        {
                            print("continue");
                            continue;
                        }
                        break;
                    case 2:
                        if (quest.victimIntensity > QuestManager.Intensity.Medium)
                            continue;
                        break;
                    case 3:
                        break;
                    case 4:
                        if (quest.victimIntensity < QuestManager.Intensity.Medium)
                            continue;
                        break;
                    case 5:
                        if (quest.victimIntensity < QuestManager.Intensity.High)
                            continue;
                        break;
                }
                switch (leader.greed)
                {
                    case 1:
                        if (quest.objectiveIntensity > QuestManager.Intensity.None)
                            continue;
                        break;
                    case 2:
                        if (quest.objectiveIntensity > QuestManager.Intensity.Low)
                            continue;
                        break;
                    case 3:
                        break;
                    case 4:
                        if (quest.objectiveIntensity < QuestManager.Intensity.Medium)
                            continue;
                        break;
                    case 5:
                        print(quest.questID + "->" + quest.objectiveIntensity.ToString());
                        if (quest.objectiveIntensity < QuestManager.Intensity.High)
                        {
                            print("continue");
                            continue;
                        }
                        break;
                }
                print("Added");
                sampleSpace.Add(quest);

            }
        }
        else
        {
            //Need to work on filtering the quests presented to players who are open to new experiences 
            //(how much does a open player, who doesn't like aggressive games, would like to be presented with
            // aggressive games)
            sampleSpace = currentQuest.nextQuests;
        }
        print("SampleSpace.count = " + sampleSpace.Count);
        return sampleSpace;
    }
    
    /* *************************************************************************************
     * private void FindLeader()
     *      This method updates the current leader of the players by going through each player
     *      and summing up the empathy level with each other player.
     *       
     * Parameters
     * 
     * Return
     *  
     * *************************************************************************************/
    private void FindLeader()
    {
        float maxSum = -65535;
        //print("LocalSum");
        foreach (Player p in players)
        {
            float localSum = 0;
            foreach (Player target in players)
            {
                if (p != target)
                {
                    localSum += target.GetRelation(p);
                    //print("Relation(" + p.playerObj.name + "," + target.playerObj.name + ")->"+p.GetRelation(target));
                }
            }
            
            //print(p.playerObj.name + "->" + localSum);

            if (localSum > maxSum)
            {
                maxSum = localSum;
                leader = p;
            }
        }

        //print("-----------");
        //print("maxSum = " + maxSum);
        //print("Leader = " + leader.playerObj.name);
    }

    private void UpdatePlayers()
    {
        List<PlayerBehaviourManager.Player> playerEmpathyList = PlayerBehaviourManager.GetInstance().GetPlayers();

        if (players.Count == 0)
        {
            List<BFGIManager.Player> playerBFGIList = BFGIManager.GetInstance().GetPlayers();
            List<PlayerTypeManager.Player> playerTypeList = PlayerTypeManager.GetInstance().GetPlayers();
            foreach (PlayerBehaviourManager.Player p in playerEmpathyList)
            {
                Player novoPlayer = new Player(p.playerID);
                novoPlayer.playerObj = p.playerObj;
                novoPlayer.playerEmpathies = p.empathies;
                players.Add(novoPlayer);
            }
            foreach (BFGIManager.Player p in playerBFGIList)
            {
                foreach (Player a in players)
                {
                    if (p.playerID == a.playerID)
                    {
                        a.O = p.O;
                        a.C = p.C;
                        a.E = p.E;
                        a.A = p.A;
                        a.N = p.N;
                    }
                }
            }
            foreach (PlayerTypeManager.Player p in playerTypeList)
            {
                foreach (Player a in players)
                {
                    if (p.playerID == a.playerID)
                    {
                        a.agression = p.agression;
                        a.rescuer = p.rescuer;
                        a.greed = p.greed;
                    }
                }
            }
        }
        else
        {
            foreach (Player player in players)
            {
                foreach (PlayerBehaviourManager.Player p in playerEmpathyList)
                {
                    if (player.playerID == p.playerID)
                    {
                        player.playerEmpathies = p.empathies;
                    }
                }
            }
        }
    }
}
