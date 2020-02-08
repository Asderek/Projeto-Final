using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class measurer : MonoBehaviour {

    private int cont = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cont++;
        if (cont > 30)
        {
            print(Vector2.Distance(GameObject.FindGameObjectWithTag("Start").transform.position, GameObject.FindGameObjectWithTag("End").transform.position));
            cont = 0;
        }
	}
}
