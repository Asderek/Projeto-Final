using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScenarioManager : MonoBehaviour
{

    public class Item
    {
        public GameObject itemObj;
        public float spawnTime;

        public Item(GameObject obj, float time)
        {
            this.itemObj = obj;
            this.spawnTime = time;
        }
    }

    public GameObject terrain;
    private GameObject currentTerrain;
    private static ScenarioManager scenarioManager;
    public GameObject victimPrefab;
    public GameObject itemPrefab;
    public GameObject objectivePrefab;
    public GameObject safeHavenPrefab;
    public int currentNumEnemies;

    [HideInInspector]
    public List<GameObject> victims;
    [HideInInspector]
    public List<Item> items;
    [HideInInspector]
    public List<GameObject> enemies;

    // Update is called once per frame
    private void Start()
    {
        scenarioManager = this;
        victims = new List<GameObject>();
        items = new List<Item>();
        enemies = new List<GameObject>();
    }

    public void CreateCenario()
    {
        CreateTerrain();
        currentNumEnemies = UnityEngine.Random.Range(5, 10);
        CreateEnemies(currentNumEnemies);
        int numVictims = UnityEngine.Random.Range(1, 5);
        CreateVictims(numVictims);
        SetPlayers();
    }

    public void CreateCenario(QuestManager.Quest newQuest)
    {
        foreach(GameObject item in GameObject.FindGameObjectsWithTag(GameManagerScript.Tags.Item.ToString()))
        {
            Destroy(item);
        }
        foreach(GameObject item in GameObject.FindGameObjectsWithTag(GameManagerScript.Tags.SafeHaven.ToString()))
        {
            Destroy(item);
        }
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        foreach (GameObject victim in victims)
        {
            Destroy(victim);
        }
        enemies.Clear();
        victims.Clear();

        if (newQuest.questID != 99)
        {
            CreateTerrain();
            CreateEnemies(newQuest.numEnemies);
            CreateVictims(newQuest.numVictims);
            CreateItems(newQuest.numItems);
            CreateObjective(newQuest.numObjectives);
            SetPlayers();
        }
        else
        {
            EndGame();
        }

        UIManagerScript.GetInstance().StartDisplayOnScreen(currentTerrain, newQuest.questName, null);
    }


    private void CreateTerrain()
    {
        if (currentTerrain != null)
        {
            UIManagerScript.GetInstance().StopDisplayOnScreen(currentTerrain);
            Destroy(currentTerrain);
        }
        currentTerrain = Instantiate(terrain, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void CreateEnemies(int currentNumEnemies)
    {
        float xSize = currentTerrain.transform.localScale.x / 2f;
        float ySize = currentTerrain.transform.localScale.y / 2f;
        for (int i = 0; i < currentNumEnemies; i++)
        {
            float xPos = currentTerrain.transform.position.x + UnityEngine.Random.Range(-xSize, xSize);
            float yPos = currentTerrain.transform.position.y + UnityEngine.Random.Range(-ySize, ySize);

            Vector3 instancePosition = new Vector3(xPos, yPos, currentTerrain.transform.position.z - 1);
            enemies.Add(Instantiate(GameManagerScript.GetInstance().enemyPrefab, instancePosition, Quaternion.identity));
        }
    }
    private void CreateVictims(int numVictims)
    {
        float xSize = currentTerrain.transform.localScale.x / 2f;
        float ySize = currentTerrain.transform.localScale.y / 2f;
        for (int i = 0; i < numVictims; i++)
        {
            float xPos = currentTerrain.transform.position.x + UnityEngine.Random.Range(-xSize, xSize);
            float yPos = currentTerrain.transform.position.y + UnityEngine.Random.Range(-ySize, ySize);

            Vector3 instancePosition = new Vector3(xPos, yPos, currentTerrain.transform.position.z - 0.02f);
            GameObject victim = Instantiate(victimPrefab, instancePosition, Quaternion.identity);
            victims.Add(victim);
        }
        if (numVictims != 0)
            CreateSafeHaven();
    }
    private void CreateSafeHaven()
    {
        float xSize = currentTerrain.transform.localScale.x / 2f;
        float ySize = currentTerrain.transform.localScale.y / 2f;
        Vector3 instancePosition = Vector3.zero;
        int ret = -1;
        int limitCont = 0;
        do
        {
            limitCont++;
            float xPos = currentTerrain.transform.position.x + UnityEngine.Random.Range(-xSize, xSize);
            float yPos = currentTerrain.transform.position.y + UnityEngine.Random.Range(-ySize, ySize);

            instancePosition = new Vector3(xPos, yPos, currentTerrain.transform.position.z - 0.01f);
            ret = CheckValidObjectPosition(instancePosition, 2, 2);
            if (limitCont > 1000)
                return;
        } while (ret != 0);
        Instantiate(safeHavenPrefab, instancePosition, Quaternion.identity);
    }
    private void CreateItems(int numItems)
    {
        float xSize = currentTerrain.transform.localScale.x / 2f;
        float ySize = currentTerrain.transform.localScale.y / 2f;
        for (int i = 0; i < numItems; i++)
        {
            float xPos = currentTerrain.transform.position.x + UnityEngine.Random.Range(-xSize, xSize);
            float yPos = currentTerrain.transform.position.y + UnityEngine.Random.Range(-ySize, ySize);

            Vector3 instancePosition = new Vector3(xPos, yPos, currentTerrain.transform.position.z - 1);
            GameObject item = Instantiate(itemPrefab, instancePosition, Quaternion.identity);
            item.tag = GameManagerScript.Tags.Item.ToString();
            Item newItem = new Item(item, Time.time);
            items.Add(newItem);
        }
    }
    private void CreateObjective(int numObjectives)
    {
        float xSize = currentTerrain.transform.localScale.x / 2f;
        float ySize = currentTerrain.transform.localScale.y / 2f;

        for (int i = 0; i < numObjectives; i++)
        {
            float xPos = currentTerrain.transform.position.x + UnityEngine.Random.Range(-xSize, xSize);
            float yPos = currentTerrain.transform.position.y + UnityEngine.Random.Range(-ySize, ySize);
            Vector3 instancePosition = new Vector3(xPos, yPos, currentTerrain.transform.position.z - 1);
            GameObject objective = Instantiate(objectivePrefab, instancePosition, Quaternion.identity);
            objective.AddComponent<Objective>();
        }
    }
    private void SetPlayers()
    {

        foreach (GameObject player in GameManagerScript.GetInstance().players)
        {
            float z = currentTerrain.transform.position.z -0.03f;
            player.transform.position = new Vector3(currentTerrain.transform.position.x,currentTerrain.transform.position.y,z);
            
        }
    }

    private void EndGame()
    {
        CreateTerrain();
        SetPlayers();

        GameObject youwin = new GameObject("You Win");
        youwin.AddComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load("a", typeof(Sprite)) as Sprite;
        youwin.GetComponent<SpriteRenderer>().sprite = sprite;
        youwin.transform.localScale = new Vector3(3, 3, 3);
        
    }


    public static ScenarioManager GetInstance()
    {
        if (scenarioManager != null)
            return scenarioManager;
        throw new System.Exception("ScenarioManager = null");
    }

    public void DestroyVictim(GameObject victimObj)
    {
        foreach (GameObject vic in victims)
        {
            if (vic == victimObj)
            {
                victims.Remove(vic);
                PlayerBehaviourManager.GetInstance().DestroyVictim(vic);
                Destroy(vic);
                return;
            }
        }

    }

    public void SaveVictim(GameObject player, GameObject victimObj)
    {
        foreach (GameObject vic in victims)
        {
            if (vic == victimObj)
            {
                victims.Remove(vic);
                PlayerBehaviourManager.GetInstance().SaveVictim(player, vic);
                Destroy(vic);
                CheckNewScenario();
                return;
            }
        }
    }


    internal void GetItem(GameObject player, GameObject itemObj)
    {
        foreach (Item item in items)
        {
            if (item.itemObj == itemObj)
            {
                items.Remove(item);
                PlayerBehaviourManager.GetInstance().GetItem(player, item);
                Destroy(item.itemObj);
                return;
            }
        }
    }

    public void CheckNewScenario()
    {

        if (QuestManager.GetInstance().GetCurrentQuest() != null)
        {
            if (QuestManager.GetInstance().GetCurrentQuest().numObjectives != 0)
            {
                GameObject[] objectiveList = GameObject.FindGameObjectsWithTag(GameManagerScript.Tags.Objective.ToString());
                if (objectiveList.Length == 1 && objectiveList[0].GetComponent<Objective>().finalized == true)
                {
                    CreateCenario(QuestManager.GetInstance().GenerateNextQuest());
                    return;
                }
                foreach (GameObject obj in objectiveList)
                {
                    if (obj.GetComponent<Objective>().finalized == false)
                    {
                        return;
                    }
                }
            }
        }

        if ((enemies.Count > 0) || (victims.Count > 0))
            return;

        CreateCenario(QuestManager.GetInstance().GenerateNextQuest());

    }

    public int CheckValidObjectPosition(Transform objTransform)
    {
        float width = currentTerrain.transform.localScale.x;
        float height = currentTerrain.transform.localScale.y;

        float limitLeft = currentTerrain.transform.position.x - width / 2;
        float limitRight = currentTerrain.transform.position.x + width / 2;
        float limitUp = currentTerrain.transform.position.y - height / 2;
        float limitDown = currentTerrain.transform.position.y + height / 2;
        
        if (objTransform.position.x < limitLeft)
            return 3;
        if (objTransform.position.x > limitRight)
            return 4;
        if (objTransform.position.y < limitUp)
            return 1;
        if (objTransform.position.y > limitDown)
            return 2;


        return 0;
    }    
    public int CheckValidObjectPosition(Vector3 objPosition, float width, float height)
    {
        float terrainWidth = currentTerrain.transform.localScale.x;
        float terrainHeight = currentTerrain.transform.localScale.y;

        float limitLeft = currentTerrain.transform.position.x - terrainWidth / 2;
        float limitRight = currentTerrain.transform.position.x + terrainWidth / 2;
        float limitUp = currentTerrain.transform.position.y - terrainHeight / 2;
        float limitDown = currentTerrain.transform.position.y + terrainHeight / 2;
        
        if (objPosition.x - width/2 < limitLeft)
            return 3;
        if (objPosition.x + width / 2 > limitRight)
            return 4;
        if (objPosition.y - height / 2 < limitUp)
            return 1;
        if (objPosition.y + width / 2 > limitDown)
            return 2;


        return 0;
    }


}
