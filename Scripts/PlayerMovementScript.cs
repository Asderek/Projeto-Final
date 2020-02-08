using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {

    public int playerID = -1;
    public float maxSpeed = 0.1f;
    public float powerUpLength = 5f;
    private float lastPowerUp;
    private float lastStunned;

    public void Start()
    {
        lastStunned = -GameManagerScript.GetInstance().playerStunLength;
    }

    public virtual void FixedUpdate()
    {
        if (Time.time - lastStunned < GameManagerScript.GetInstance().playerStunLength)
            return;

        float spdModifier = 1;
        if (Time.time - lastPowerUp < powerUpLength)
            spdModifier *= 1.5f;

            //int playerID = 1;
            if (Input.GetButton("Player" + playerID + "_UP"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + spdModifier * maxSpeed, transform.position.z);
            PlayerBehaviourManager.GetInstance().movePlayer(gameObject);
        }
        else if (Input.GetButton("Player" + playerID + "_DOWN"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - spdModifier * maxSpeed, transform.position.z);
            PlayerBehaviourManager.GetInstance().movePlayer(gameObject);
        }

        if (Input.GetButton("Player" + playerID + "_RIGHT"))
        {
            transform.position = new Vector3(transform.position.x + spdModifier * maxSpeed, transform.position.y, transform.position.z);
            PlayerBehaviourManager.GetInstance().movePlayer(gameObject);
        }
        else if (Input.GetButton("Player" + playerID + "_LEFT"))
        {
            transform.position = new Vector3(transform.position.x - spdModifier * maxSpeed, transform.position.y, transform.position.z);
            PlayerBehaviourManager.GetInstance().movePlayer(gameObject);
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            ScenarioManager.GetInstance().GetItem(gameObject, collision.gameObject);
            ActivatePowerUp();
        }
    }

    public void ActivatePowerUp()
    {
        lastPowerUp = Time.time;
    }

    public void ReceiveDamage()
    {
        lastStunned = Time.time;
        GetComponent<PlayerSpriteAnimManager>().ReceiveDamage();
    }

    

}
