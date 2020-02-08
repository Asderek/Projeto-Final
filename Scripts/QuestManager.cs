using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public enum Intensity
    {
        None,
        Low,
        Medium,
        High
    }

    public class Quest
    {
        public string questName;
        public int questID;
        public QuestManager.Intensity enemyIntensity;
        public QuestManager.Intensity victimIntensity;
        public QuestManager.Intensity objectiveIntensity;
        public int numEnemies;
        public int numVictims;
        public int numItems;
        public int numObjectives;
        public float startTime;
        public List<Quest> nextQuests;

        public Quest(int i,string questName, QuestManager.Intensity enemyIntensity, QuestManager.Intensity victimIntensity, QuestManager.Intensity objectiveIntensity)
        {
            this.questName = questName;
            this.questID = i;
            this.numItems = Random.Range(1, 3);

            this.enemyIntensity = enemyIntensity;
            this.victimIntensity = victimIntensity;
            this.objectiveIntensity= objectiveIntensity;

            this.numEnemies = Random.Range((int)enemyIntensity * 3, (int)enemyIntensity * 4);
            this.numVictims = Random.Range((int)victimIntensity * 3, (int)victimIntensity * 4);
            this.numObjectives = Random.Range((int)objectiveIntensity * 3, (int)objectiveIntensity * 4);

            this.nextQuests = new List<Quest>();
        }
    }

    protected int contEndGame = 11;
    private Quest currentQuest;
    private List<Quest> allQuests;

    private static QuestManager questManager;
    public static QuestManager GetInstance()
    {
        if (questManager != null)
            return questManager;
        throw new System.Exception("questManager = null");
    }

    public void Start()
    {
        questManager = this;
        allQuests = new List<Quest>();
        GenerateAllQuests();
        for(int i =0;i<allQuests.Count;i++)
        {
            print(allQuests[i].questID + "->" + allQuests[i].enemyIntensity.ToString() + "-" + allQuests[i].victimIntensity.ToString() + "-" + allQuests[i].objectiveIntensity.ToString());
        }
    }

    private void GenerateAllQuests()
    {
        //           (new Quest(questID, questName, qtdEnemies, qtdVictim, qtdObjectives))
        allQuests.Add(new Quest(1, "Start Quest",Intensity.None, Intensity.None, Intensity.Low)); //Starting Quest
        allQuests.Add(new Quest(2, "Empathy Check", Intensity.Medium, Intensity.Low, Intensity.None)); //Testa Empatia
        allQuests.Add(new Quest(3, "CheckPoint", Intensity.None, Intensity.None, Intensity.Low)); //Repeating Quest
        allQuests.Add(new Quest(99, "EndGame", Intensity.None, Intensity.None, Intensity.None)); //EndGame

        int questCont = 4;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Intensity enemies = Intensity.Low;
                    Intensity victims = Intensity.Low;
                    Intensity objectives = Intensity.None;

                    switch (i)
                    {
                        case 1:
                            enemies = Intensity.Medium;                            
                            break;
                        case 2:
                            enemies = Intensity.High;
                            break;
                    }
                    switch (j)
                    {
                        case 1:
                            victims = Intensity.Medium;
                            break;
                        case 2:
                            victims = Intensity.High;
                            break;
                    }
                    switch (k)
                    {
                        case 1:
                            objectives = Intensity.Low;
                            break;
                        case 2:
                            objectives = Intensity.Medium;
                            break;
                        case 3:
                            objectives = Intensity.High;
                            break;
                    }
                    string questName = enemies.ToString() + "-" + victims.ToString() + "-" + objectives.ToString();
                    allQuests.Add(new Quest(questCont, questName, enemies, victims, objectives));
                    questCont++;
                }
            }
        }

        SetQuestLines();
    }

    private void SetQuestLines()
    {
        foreach (Quest q in allQuests)
        {
            switch (q.questID)
            {
                case 1:
                    q.nextQuests.Add(GetQuest(2));
                    break;
                case 2:
                    q.nextQuests.Add(GetQuest(3));
                    break;
                case 3:
                    int questCont = 4;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                q.nextQuests.Add(GetQuest(questCont));
                                questCont++;
                            }
                        }
                    }
                    q.nextQuests.Add(GetQuest(99));
                    break;
                case 99:
                    break;
                default:
                    q.nextQuests.Add(GetQuest(3));
                    break;
                    
            }
        }
    }
    public Quest GetCurrentQuest()
    {
        return currentQuest;
    }

    public Quest GenerateNextQuest()
    {

        if (currentQuest != null)
        {
            int nextQuest = currentQuest.nextQuests[0].questID;
            if (currentQuest.nextQuests.Count > 1)
            {
                if (contEndGame > 0)
                {
                    do
                    {
                        nextQuest = BaffaAlg.GetInstance().GetBestQuest(currentQuest);
                    } while (nextQuest == 99);
                }
                else
                {
                    nextQuest = BaffaAlg.GetInstance().GetBestQuest(currentQuest);
                }
            }
            currentQuest = GetQuest(nextQuest);
        }
        else
        {
            currentQuest = allQuests[0]; 
        }
        currentQuest.startTime = Time.time;
        if (currentQuest.questID == 3)
        {
            contEndGame--;
        }
        return currentQuest;
    }

    protected Quest GetQuest(int questID)
    {
        foreach (Quest q in allQuests)
            if (q.questID == questID)
                return q;

        return null;
    }
}
