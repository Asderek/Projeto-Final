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
