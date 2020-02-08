using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerScript : MonoBehaviour {


    private static UIManagerScript UIManager;
    public GameObject GreenBall;
    public int limiteX,limiteY;
    private Vector2 charSizeOffset = new Vector2(26,-22);
    public Vector2 situationOffset = new Vector2(88, 0);
    public Vector2 situationRECT = new Vector2(0, 0);

    [System.Serializable]
    public class PlayerMsg
    {
        public GameObject player;
        public string msg;
        public GameObject messenger;

        public PlayerMsg(GameObject player, string msg, GameObject messenger)
        {
            this.player = player;
            this.msg = msg;
            this.messenger = messenger;
        }
    }

    public List<PlayerMsg> players;

    public void Start()
    {
        players = new List<PlayerMsg>();
    }

    public static UIManagerScript GetInstance()
    {
        if (UIManager != null)
            return UIManager;
        throw new System.Exception("UIManager = null");
    }

    public void StartDisplayOnScreen(GameObject position, string text, GameObject messenger)
    {
        PlayerMsg newMsg = new PlayerMsg( position, text, messenger);
        players.Add(newMsg);           
    }
    private void Awake()
    {
        if (UIManager == null)
            UIManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnGUI()
    {
        foreach (PlayerMsg display in players)
        {
            if (display.player == null)
                continue;

            if (!display.player.activeInHierarchy)
                continue;

            float width, height;
            width = height = 200;
            Vector3 pos = Camera.main.WorldToScreenPoint(display.player.transform.position);
            string displayMSG = display.msg;
            Rect displayRECT = new Rect(pos.x, Screen.height - pos.y, width, height);

            if (displayMSG.Contains("PlayerName_"))
            {
                //print("contains");
                displayMSG = displayMSG.Substring("PlayerName_".Length);
                //print(display.player.GetComponent<SpriteRenderer>().sprite.texture.height);
                displayRECT.y -= charSizeOffset.y;
                displayRECT.x -= charSizeOffset.x;
            }

            if (displayMSG.Contains("Situation"))
            {
                displayRECT.x += situationOffset.x;
                displayRECT.y += situationOffset.y;
                displayRECT.width += situationRECT.x;
                displayRECT.height += situationRECT.y;
            }

            GUI.Label(displayRECT, displayMSG);
        }

        if(true)//GameManagerScript.GetInstance().currentStage == GameManagerScript.GameStage.MainGame)
        { 
            List<string> ptText = PlayerTypeManager.GetInstance().Display();
            List<string> bfgiText = BFGIManager.GetInstance().Display();
            List<string> pbText = PlayerBehaviourManager.GetInstance().Display();

            Rect leftUI = new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.20f, Screen.height * 0.9f);

            if (GameManagerScript.GetInstance().GetNumPlayers() > 2)
            {
                Rect rightUI = new Rect(Screen.width * 0.75f, Screen.height * 0.05f, Screen.width * 0.20f, Screen.height * 0.9f);
                string leftText = ptText[0] + bfgiText[0] + pbText[0];
                string rightText = ptText[1] + bfgiText[1] + pbText[1];

                GUI.Box(leftUI, leftText);
                GUI.Box(rightUI, rightText);
            }
            else
            {
                string leftText = ptText[0] + bfgiText[0] + pbText[0];
                GUI.Box(leftUI, leftText);
            }
        }
        
    }



    public void StopDisplayOnScreen(GameObject stop)
    {
        foreach (PlayerMsg msg in players)
        {
            if (msg.msg.Contains("PlayerName_"))
                continue;

            if (msg.player == stop)
            {
                players.Remove(msg);
                return;
            }
        }
    }

    public GameObject CheckMsg(GameObject recipient)
    {
        foreach (PlayerMsg msg in players)
        {
            if (msg.msg.Contains("PlayerName_"))
                continue;

            if (msg.player == recipient)
            {
                return msg.messenger;
            }
        }
        return null;
    }

    public void ChangeText(GameObject caller, string newText)
    {
        StopDisplayOnScreen(caller);
        StartDisplayOnScreen(caller, newText, caller);
    }
}
