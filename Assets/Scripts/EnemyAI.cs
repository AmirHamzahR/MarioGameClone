using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    Health script2;

    public float gravity;
    public Vector2 velocity;
    public bool isWalkingLeft = true;

    public LayerMask floorMask;
    public LayerMask wallMask;

    private bool grounded = false;

    private bool shouldDie = false;
    private float deathTimer = 0;

    public float timeBeforeDestroy = 1.0f;

    //  States of the enemy to be in
    private enum EnemyState
    {
        walking,
        falling,
        dead
    }

    private EnemyState state = EnemyState.falling;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;

        Fall();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyPosition();

        CheckCrushed();
    }

    // If player jumps on enemy, it will trigger this
    public void Crush()
    {
        state = EnemyState.dead;

        GetComponent<Animator>().SetBool("isCrushed", true);

        GetComponent<Collider2D>().enabled = false;

        shouldDie = true;
    }

    // Check if the enemy is crushed, the enemy will have some time for its animation to run until it dies
    void CheckCrushed()
    {
        if (shouldDie)
        {
            if (deathTimer <= timeBeforeDestroy)
            {
                deathTimer += Time.deltaTime;
            }
            else
            {
                shouldDie = false;
                Destroy(gameObject);
            }
        }
            
    }

    // To update enemy's position if its alive or dead
    void UpdateEnemyPosition()
    {
        if(state != EnemyState.dead)
        {
            Vector3 pos = transform.localPosition;
            Vector3 scale = transform.localScale;

            if(state == EnemyState.falling)
            {
                pos.y += velocity.y * Time.deltaTime;

                velocity.y -= gravity * Time.deltaTime;
            }
            if(state == EnemyState.walking)
            {
                if (isWalkingLeft)
                {
                    pos.x -= velocity.x * Time.deltaTime;

                    scale.x = -1;
                }
                else
                {
                    pos.x += velocity.x * Time.deltaTime;

                    scale.x = 1;
                }
            }
            if (velocity.y <= 0)
                pos = CheckGround(pos);

            CheckWalls(pos, scale.x);
          
            
            transform.localPosition = pos;
            transform.localScale = scale;
        }
    }

    // A function to control the ground collisions
    Vector3 CheckGround(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - .5f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y - .5f);
        Vector2 originRight = new Vector2(pos.x + 0.5f, pos.y - .5f);

        RaycastHit2D groundLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if(groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null)
        {
            RaycastHit2D hitRay = groundLeft;

            if (groundLeft)
            {
                hitRay = groundLeft;
            } else if (groundMiddle)
            {
                hitRay = groundMiddle; 
            } else if (groundRight)
            {
                hitRay = groundRight;
            }

            pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + .5f;

            grounded = true;

            velocity.y = 0;

            state = EnemyState.walking;
        }
        else
        {
            if(state != EnemyState.falling)
            {
                Fall();
            }
        }
        return pos;
    }

    // To control the collision of the enemy with the wall
    void CheckWalls(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * 0.4f, pos.y + .5f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 0.4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * 0.4f, pos.y - .5f + 0.2f);

        RaycastHit2D wallTop = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
        {
            RaycastHit2D hitRay = wallTop;

            if (wallTop)
            {
                hitRay = wallTop;
            } else if (wallMiddle)
            {
                hitRay = wallMiddle;
            } else if (wallBottom)
            {
                hitRay = wallBottom;
            }

            isWalkingLeft = !isWalkingLeft;
            
        }
        
    }

    // To enable enemy's state when camera is set on the enemy
    private void OnBecameVisible()
    {
        enabled = true;
    }

    // To make the enemy fall
    void Fall()
    {
        velocity.y = 0;

        state = EnemyState.falling;

        grounded = false;
    }
}
