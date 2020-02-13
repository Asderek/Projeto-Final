using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public GameObject owner;

    /******************************************
	* 
	* private void OnTriggerEnter2D(Collider2D collision)
	*		If the attack colliders with a gameObject with the tag enemy,
    *       signal the gameManager to handle the interaction.
	*
	* Parameters
	*       Collider2D collision - Default unity parameter
    *
	* Return
	*
	* ***************************************/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameManagerScript.GetInstance().HitEnemy(owner, collision.gameObject);
        }
    }
}
