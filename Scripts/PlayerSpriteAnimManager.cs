using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAnimManager : MonoBehaviour {

    private int playerId = -1;
    private float lastInput;
    public float idleCD = 10;
    private Animator animator;

    private float lastStunned = -5f;


    
    private int segments = 50;
    private float xradius = 0.4f;
    private float yradius = 0.22f;
    LineRenderer line;


    public enum Direction
    {
        NORTH = 1,
        NE,
        EAST,
        SE,
        SOUTH,
        SW,
        WEST,
        NW
    }
    private Direction lastDirection;

    // Use this for initialization
    void Start()
    {
        playerId = GetComponent<PlayerMovementScript>().playerID;
        animator = GetComponent<Animator>();
        lastStunned = -GameManagerScript.GetInstance().playerStunLength;


        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();

        switch (playerId)
        {
            case 0:
                line.startColor = Color.green;
                line.endColor = Color.green;
                break;
            case 1:
                line.startColor = Color.red;
                line.endColor = Color.red;
                break;
            case 2:
                line.startColor = Color.cyan;
                line.endColor = Color.cyan;
                break;
            case 3:
                line.startColor = Color.yellow;
                line.endColor = Color.yellow;
                break;
        }
    }

    void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
            y -= 0.2f;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }

    // Update is called once per frame
    void Update()
    {

        CreatePoints();
        if (Time.time - lastStunned < GameManagerScript.GetInstance().playerStunLength)
        {
            return;
        }


        if (
                animator.GetBool("startAttack") || 
                (animator.GetCurrentAnimatorStateInfo(0).IsName("attackRIGHT") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) ||
                (animator.GetCurrentAnimatorStateInfo(0).IsName("attackDOWN") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) ||
                (animator.GetCurrentAnimatorStateInfo(0).IsName("attackUP") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) ||
                (animator.GetCurrentAnimatorStateInfo(0).IsName("attackLEFT") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
           )
        {
            return;
        }
        animator.ResetTrigger("startAttack");
        if (Input.GetButtonDown("Player" + playerId + "_X"))
        {
            PlayerBehaviourManager.GetInstance().PlayerAttack(gameObject);
            animator.SetTrigger("startAttack");
            return;
        }

        if (Time.time - lastInput > idleCD)
        {
            switch (lastDirection)
            {
                case Direction.NORTH:
                case Direction.NE:
                case Direction.NW:
                    animator.SetTrigger("startIdle");
                    animator.SetInteger("Direction", (int)Direction.NORTH);
                    break;
                case Direction.EAST:
                    animator.SetTrigger("startIdle");
                    animator.SetInteger("Direction", (int)Direction.EAST);
                    break;
                case Direction.SE:
                case Direction.SOUTH:
                case Direction.SW:
                    animator.SetTrigger("startIdle");
                    animator.SetInteger("Direction", (int)Direction.SOUTH);
                    break;
                case Direction.WEST:
                    animator.SetTrigger("startIdle");
                    animator.SetInteger("Direction", (int)Direction.WEST);
                    break;
            }
        }
        else
        {
            
            switch (lastDirection)
            {
                case Direction.NORTH:
                case Direction.NE:
                case Direction.NW:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("walkUP"))
                    {
                        animator.SetTrigger("startWalk");
                        animator.SetInteger("Direction", (int)Direction.NORTH);
                    }
                    break;
                case Direction.EAST:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("walkRIGHT"))
                    {
                        animator.SetTrigger("startWalk");
                        animator.SetInteger("Direction", (int)Direction.EAST);
                    }
                    break;
                case Direction.SE:
                case Direction.SOUTH:
                case Direction.SW:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("walkDOWN"))
                    {
                        animator.SetTrigger("startWalk");
                        animator.SetInteger("Direction", (int)Direction.SOUTH);
                    }
                    break;
                case Direction.WEST:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("walkLEFT"))
                    {
                        animator.SetTrigger("startWalk");
                        animator.SetInteger("Direction", (int)Direction.WEST);
                    }
                    break;
            }
        }

        
    }

    public virtual void FixedUpdate()
    {
        if (Time.time - lastStunned < GameManagerScript.GetInstance().playerStunLength)
        {
            GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
            return;
        }
        GetComponent<SpriteRenderer>().enabled = true;

        bool up;
        bool down;
        bool left;
        bool right;

        up = down = left = right = false;

        //int playerId = 1;
        if (Input.GetButton("Player" + playerId + "_UP"))
        {
            up = true;
        }
        else if (Input.GetButton("Player" + playerId + "_DOWN"))
        {
            down = true;
        }

        if (Input.GetButton("Player" + playerId + "_RIGHT"))
        {
            right = true;
        }
        else if (Input.GetButton("Player" + playerId + "_LEFT"))
        {
            left = true;
        }

        if (up || down || left || right)
            lastInput = Time.time;

        if (up && right)
            lastDirection = Direction.NE;
        else if (up && left)
            lastDirection = Direction.NW;
        else if (up)
            lastDirection = Direction.NORTH;
        else if (down && right)
            lastDirection = Direction.SE;
        else if (down && left)
            lastDirection = Direction.SW;
        else if (down)
            lastDirection = Direction.SOUTH;
        else if (right)
            lastDirection = Direction.EAST;
        else if (left)
            lastDirection = Direction.WEST;

    }

    public void ReceiveDamage()
    {
        lastStunned = Time.time;
        animator.SetTrigger("startIdle");
    }

}
