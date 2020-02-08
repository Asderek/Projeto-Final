using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private GameObject currentTarget;
    public float victimPriority = 1.1f;
    public float maxSpeed = 0.001f;
    public float maxViewDistance = 5;

    public float waitTime=0;
    private float lastWait;

    public float spawnTime;
    private GameObject tempTarget;

    // Use this for initialization
    void Start () {
        gameObject.tag = GameManagerScript.Tags.Enemy.ToString();
        spawnTime = Time.time;
        transform.position = new Vector3 (transform.position.x, transform.position.y, -0.5f);
	}

    private void LateUpdate()
    {

        float minDistance = 65535f;
        GameObject newTarget = null;
        
        foreach (GameObject victim in ScenarioManager.GetInstance().victims)
        {
            if (Vector2.Distance(transform.position, victim.transform.position)/victimPriority < minDistance)
            {
                minDistance = Vector2.Distance(transform.position, victim.transform.position) / victimPriority;
                newTarget = victim;
            }
        }

        foreach (GameObject player in GameManagerScript.GetInstance().players)
        {
            
            float playerPriority = GetPlayerPriority(player);
           
            if (Vector2.Distance(transform.position, player.transform.position)/playerPriority < minDistance)
            {
                minDistance = Vector2.Distance(transform.position, player.transform.position);
                newTarget = player;
            }
        }


        if (minDistance > maxViewDistance)
        {
            if (tempTarget)
                currentTarget = tempTarget;
            else
                currentTarget = null;
        }
        else if (newTarget != null)
        {
            Destroy(tempTarget);
            currentTarget = newTarget;
        }
    }

    /****************************************
    * Function GetPlayerPriority(GameObject player)
    *   Returns a value that indicates the enemies priority of the target;
    * Parameters
    *   GameOject player
    *       a player object previously analized by the BFGI Manager;
    * Return
    *   float priority
    *       the modifier to the priority of the player. In this instance, it changes the distance in which the enemy considers the player, making it more or less likely to pursue;
    * Obs
    *   The enemies are more likely to pursue curious players. The same is true for careless, outgoing compassionate and nervous players.
    *   Solitary players are less likely to help other players. Same is true for detached players.
    *     
    ****************************************/
    private float GetPlayerPriority(GameObject player)
    {
        //                      [0.1]           [-0.1]
        //Openness          = curious       vs cautious
        //Conscientiouness  = organized     vs careless
        //Extraversion      = outgoing      vs solitary
        //Agreeableness     = compassionate vs detached
        //Neuroticism       = nervous       vs confident
        
        float ret = 1;
        int playerO = BFGIManager.GetInstance().GetPlayerOCEAN(player, "O") -6;
        int playerC = BFGIManager.GetInstance().GetPlayerOCEAN(player, "C") -6;
        int playerE = BFGIManager.GetInstance().GetPlayerOCEAN(player, "E") -6;
        int playerA = BFGIManager.GetInstance().GetPlayerOCEAN(player, "A") -6;
        int playerN = BFGIManager.GetInstance().GetPlayerOCEAN(player, "N") -6;

        ret += playerO * 0.025f;    //playerO = [-0.1 .. 0.1] etc...
        ret -= playerC * 0.025f;
        ret += playerE * 0.025f;
        ret += playerA * 0.025f;
        ret += playerN * 0.025f;


        return ret;
    }

    public virtual void FixedUpdate()
    {
        foreach (GameObject player in GameManagerScript.GetInstance().players)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < maxViewDistance/2f)
            {
                lastWait = 0;
            }
        }

        if (Time.time - lastWait < waitTime)
        {
            return;
        }


        if (currentTarget)
        {
            float xDif = currentTarget.transform.position.x - transform.position.x;
            float yDif = currentTarget.transform.position.y - transform.position.y;
            Vector2 difVector = new Vector2(xDif, yDif).normalized;
            difVector *= maxSpeed;

            transform.position = new Vector3(transform.position.x + difVector.x, transform.position.y + difVector.y, transform.position.z);


            if (currentTarget == tempTarget)
            {
                if (Vector2.Distance(transform.position, currentTarget.transform.position) < (maxViewDistance / 10f))
                {
                    print("currentTarget = " + currentTarget);
                    Destroy(tempTarget);
                    currentTarget = null;
                    lastWait = Time.time;
                    waitTime = UnityEngine.Random.Range(2, 5);
                    print("currentTarget = " + currentTarget);
                }
            }
        }
        else
        {
            if (tempTarget == null)
            {
                tempTarget = new GameObject();
                tempTarget.transform.position = new Vector2(transform.position.x + UnityEngine.Random.Range(-2, 3), transform.position.y + UnityEngine.Random.Range(-2, 3));
                int posRet = ScenarioManager.GetInstance().CheckValidObjectPosition(tempTarget.transform);
                if (posRet != 0)
                {
                    Destroy(tempTarget);
                }
            }
        }
        // else roam
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Victim")
        {
            ScenarioManager.GetInstance().DestroyVictim(collision.gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovementScript>().ReceiveDamage();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManagerScript.GetInstance().HitEnemy(collision.gameObject, gameObject);
        }
    }

}
