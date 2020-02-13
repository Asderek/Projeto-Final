using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour
{
    public bool finalized;
    // Use this for initialization
    void Start()
    {
        gameObject.tag = GameManagerScript.Tags.Objective.ToString();
    }
    
    /******************************************
    * 
    * private void OnTriggerEnter2D(Collider2D collision)
    *	If the objective collides with a player object, signals the ScenarioManager to check for a new Scenario
    *       and destroys the objective gameObject
    *
    * Parameters
    *       Collider2D collison - Unity default parameter       
    *
    * Return
    *
    * ***************************************/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        if (collision.gameObject.tag == GameManagerScript.Tags.Player.ToString())
        {
            finalized = true;
            ScenarioManager.GetInstance().CheckNewScenario();
            Destroy(gameObject);
        }
    }
}
