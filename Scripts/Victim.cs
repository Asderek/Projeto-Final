using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victim : MonoBehaviour {

    public float spawnTime;
    private GameObject currentTarget;
    public float maxSpeed = 0.001f;
    public float maxDistance = 4.5f;


    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 50)]
    public float xradius = 4.5f;
    [Range(0, 50)]
    public float yradius = 4.5f;
    LineRenderer line;
    // Use this for initialization

    public void Start()
    {
        spawnTime = Time.time;
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();
        line.enabled = false;
        maxSpeed = Random.Range(0.01f, 0.03f);
        float percentage = (maxSpeed - 0.01f) / (0.03f - 0.01f);
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, percentage, 1f);
    }

    private void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, transform.position.z));

            angle += (360f / segments);
        }
    }

    // Update is called once per frame
    public void Update () {
    }

    private void LateUpdate()
    {
        if (currentTarget)
        {
            float xDif = currentTarget.transform.position.x - transform.position.x;
            float yDif = currentTarget.transform.position.y - transform.position.y;
            Vector2 difVector = new Vector2(xDif, yDif);
            if (difVector.magnitude < 0.5)
                return;
            if (difVector.magnitude > maxDistance)
            {
                currentTarget = null;
                line.enabled = false;
                return;
            }
            difVector.Normalize();
            difVector *= maxSpeed;

            transform.position = new Vector3(transform.position.x + difVector.x, transform.position.y + difVector.y, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameManagerScript.Tags.Player.ToString())
        {
            if (currentTarget == null)
            {
                currentTarget = collision.gameObject;
                line.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == GameManagerScript.Tags.SafeHaven.ToString())
        {
            ScenarioManager.GetInstance().SaveVictim(currentTarget, gameObject);
        }
    }
}
