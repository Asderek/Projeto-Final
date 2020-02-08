using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SituacaoScript : MonoBehaviour {

    private UIManagerScript UIManager;
    public string displayText;
    public bool self=false;
    public int value;

    private void Start()
    {
        UIManager = UIManagerScript.GetInstance();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (self)
        {
            UIManager.StartDisplayOnScreen(gameObject, displayText, gameObject);
        }
        else
            UIManager.StartDisplayOnScreen(collision.gameObject, displayText, gameObject);
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (self)
            UIManager.StopDisplayOnScreen(gameObject);
        else
            UIManager.StopDisplayOnScreen(collision.gameObject);
    }

    public void ChangeText(string newText)
    {
        displayText = newText;
        UIManagerScript.GetInstance().ChangeText(gameObject, newText);
    }
}
