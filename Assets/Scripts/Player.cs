using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float jumpVelocity;
    public float bounceVelocity;
    public Vector2 velocity;
    public float gravity;
    public LayerMask wallMask;
    public LayerMask floorMask;
    public bool changed = false;

    public Vector3 pozition;

    Currency script;

    private bool walk, walk_left, walk_right, jump;

    public enum PlayerState
    {
        jumping,
        idle,
        walking,
        bouncing
    }

    private PlayerState playerState = PlayerState.idle;

    private bool grounded = false;
    private bool bounce = false;

    // Start is called before the first frame update
    void Start()
    {
        //Fall();
     
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPosition();
        UpdateAnimationState();
    }

    //  Updates the position of the player
    void UpdatePlayerPosition()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;

        //  Controls the direction and speed of the player walking
        if (walk)
        {
            if (walk_left)
            {
                pos.x -= velocity.x * Time.deltaTime;

                scale.x = -1;
            }
            if (walk_right)
            {
                pos.x += velocity.x * Time.deltaTime;

                scale.x = 1;
            }

            pos = CheckWallRays(pos, scale.x);

        }

        //  Controls the speed of the player jumping
        if (jump && playerState != PlayerState.jumping)
        {
            playerState = PlayerState.jumping;

            velocity = new Vector2(velocity.x, jumpVelocity);
        }

        //  Controls the speed of the player falling
        if(playerState == PlayerState.jumping)
        {
            pos.y += velocity.y * Time.deltaTime;

            velocity.y -= gravity * Time.deltaTime;
        }

        if(bounce && playerState != PlayerState.bouncing)
        {
            playerState = PlayerState.bouncing;

            velocity = new Vector2(velocity.x, bounceVelocity);
        }

        if(playerState == PlayerState.bouncing)
        {
            pos.y += velocity.y * Time.deltaTime;

            velocity.y -= gravity * Time.deltaTime;
        }

        //  Ensures that the player would be steady on the ground
        if(velocity.y <= 0)
            pos = CheckFloorRays(pos);

        //  Ensures that the player would not go above the ceiling 
        if (velocity.y >= 0)
            pos = CheckCeilingRays(pos);


        transform.localPosition = pos;
        transform.localScale = scale;
        pos = pozition;
    }

    void UpdateAnimationState()
    {
        // Not moving and on the ground
        if(grounded && !walk && !bounce)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }

        // When both right and left key is pressed
        if(walk_left && walk_right) 
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);  
        }

        // Moving and on the ground
        if (grounded && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }
        
        // Character in the jumping state
        if(playerState == PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    // Function that enables the user to control the movement of the players using arrow keys and space
    void CheckPlayerInput()
    {
        bool input_left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool input_right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool input_space = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);

        walk = input_left || input_right;

        walk_left = input_left && !input_right;

        walk_right = !input_left && input_right;

        jump = input_space;
    }

    // Vector to control the boundaries of the walls
    Vector3 CheckWallRays(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * .4f, pos.y + 1f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * .4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * .4f, pos.y - 1f + 0.2f);

        RaycastHit2D wallTop = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        // If the player is colliding with the wall, it will create the same force to stop the player
        if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
        {
            pos.x -= velocity.x * Time.deltaTime * direction;
        }
        
        return pos;
    }

    //  Vector to control the boundaries of the floor
    Vector3 CheckFloorRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y - 1.2f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y - 1f);

        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        // If the player is colliding with the floor, it will create the same force to stop the player
        if (floorLeft.collider != null || floorMiddle.collider != null || floorRight.collider != null)
        {
            RaycastHit2D hitRay = floorRight;

            if (floorLeft)
            {
                hitRay = floorLeft;
            } else if (floorMiddle)
            {
                hitRay = floorMiddle;
            } else if (floorRight)
            {
                hitRay = floorRight;
            }

            // If the player hits the enemy from above
            if(hitRay.collider.tag == "Enemy")
            {
                bounce = true;

                hitRay.collider.GetComponent<EnemyAI>().Crush();
            }

            playerState = PlayerState.idle;

            grounded = true;

            velocity.y = 0;

            pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1;
        }
        else
        {
            if(playerState != PlayerState.jumping)
            {
                Fall();
            }
        }
        return pos;
    }

    // Vector to control the boundaries of the ceiling
    Vector3 CheckCeilingRays (Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y + 1f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y + 1f);

        RaycastHit2D ceilLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast(originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        // If the player is colliding with the ceiling, it will create the same force to stop the player
        if (ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null)
        {
            RaycastHit2D hitRay = ceilLeft;

            if (ceilLeft)
            {
                hitRay = ceilLeft;
            } else if (ceilMiddle)
            {
                hitRay = ceilMiddle;
            } else if (ceilRight)
            {
                hitRay = ceilRight;
            }

            // Check if the player hits the question block
            if(hitRay.collider.tag == "QuestionBlock")
            {
                hitRay.collider.GetComponent<QuestionBlock>().QuestionBlockBounce();
            }

            pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - 1;

            Fall();
        }
        return pos;
    }

    // Function to ensure that the player would fall after jumping
    void Fall()
    {
        velocity.y = 0;

        playerState = PlayerState.jumping;

        bounce = false;

        grounded = false;
    }

}

