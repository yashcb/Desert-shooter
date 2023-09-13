using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChasingEnemy : Enemy
{
    public Transform playerTransform;
    public Rigidbody2D _rigidb;
    public Animator _anim;
    public float chaseRadius;
    public float attackRadius;
    public float distance;

    public Vector2 move;
    public Vector2 dir;
    protected Vector2 rayDir;

    protected bool enemyMoving;
    protected bool enemyAttacking;
    protected bool obstacleAhead;

    protected RaycastHit2D rayHit;

    // Patrolling enemy variables
    public Transform currentGoal;
    public int currentPoint;
    public float roundingDistance;
    public Transform?[] path;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    private void Start()
    {
        currentState = EnemyState.Idle;
        _anim = GetComponent<Animator>();
        _rigidb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            _anim.SetTrigger("Died");
            StartCoroutine(Death());
        }
    }

    private void FixedUpdate()
    {
        distance = Vector3.Distance(playerTransform.position, transform.position);
        rayDir = playerTransform.position - transform.position;

        if (currentEnemyType == EnemyType.Chasing)
        {
            obstacleAhead = CastRayNormal(rayDir);

            if (distance < attackRadius && !obstacleAhead)
            {
                enemyMoving = false;
                enemyAttacking = true;
                //AttackPlayer();
            }
            else if (distance <= chaseRadius && distance > attackRadius && !obstacleAhead)
            {
                enemyAttacking = false;
                enemyMoving = true;
                MoveEnemy();
            }
            else
            {
                enemyMoving = false;
                enemyAttacking = false;
                ChangeState(EnemyState.Idle);
            }
        }

        else if (currentEnemyType == EnemyType.Patrolling)
        {
            obstacleAhead = CastRayPatrolling(rayDir);

            if (distance < attackRadius && !obstacleAhead)
            {
                enemyMoving = false;
                enemyAttacking = true;
                //AttackPlayer();
            }

            else if (distance <= chaseRadius && distance > attackRadius && !obstacleAhead)
            {
                enemyAttacking = false;
                enemyMoving = true;
                MoveEnemy();
            }

            else if ((distance <= chaseRadius && distance > attackRadius && obstacleAhead && !enemyAttacking) ||
            (distance > chaseRadius && !enemyAttacking))
            {
                enemyAttacking = false;
                enemyMoving = true;
                MoveEnemyOnPatrollingPoints();
            }
        }
        //CheckDistance();
        ChangeAnimation(dir);
        SetMoveEnemy(enemyMoving);
        SetAttackPlayer(enemyAttacking);
    }

    public virtual void CheckDistance()
    {
        //     rayDir = playerTransform.position - transform.position;
        //     obstacleAhead = CastRay(rayDir);
        //     distance = Vector3.Distance(playerTransform.position, transform.position);

        //     if (distance <= attackRadius)
        //     {
        //         AttackPlayer();
        //         //enemyAttacking = true;
        //         //enemyMoving = false;
        //     }

        //     else if (distance <= chaseRadius && distance > attackRadius && !obstacleAhead)
        //     {
        //         MoveEnemy();
        //     }

        //     SetMoveEnemy(enemyMoving);
        //     SetAttackPlayer(enemyAttacking);
    }

    protected void MoveEnemy()
    {
        move = Vector3.MoveTowards(transform.position, playerTransform.position, enemySpeed * Time.fixedDeltaTime);
        dir = (move - (Vector2)transform.position).normalized;
        _rigidb.MovePosition(move);
    }

    private void MoveEnemyOnPatrollingPoints()
    {
        if (Vector3.Distance(transform.position, path[currentPoint].position) > roundingDistance)
        {
            move = Vector3.MoveTowards(transform.position, path[currentPoint].position, enemySpeed * Time.deltaTime);
            dir = (move - (Vector2)transform.position).normalized;
            //ChangeAnimation(dir);
            _rigidb.MovePosition(move);
        }
        else if (Vector3.Distance(transform.position, path[currentPoint].position) <= roundingDistance)
        {
            ChangeGoal();
        }
    }

    protected void ChangeAnimation(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction.y = 0;
            if (direction.x > 0)
            {
                direction.x = 1;
            }
            else if (direction.x < 0)
            {
                direction.x = -1;
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            direction.x = 0;
            if (direction.y > 0)
            {
                direction.y = 1;
            }
            else if (direction.y < 0)
            {
                direction.y = -1;
            }
        }
        //direction = direction.normalized;
        _anim.SetFloat("xDir", direction.x);
        _anim.SetFloat("yDir", direction.y);

        #region Longer Method
        //if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        //{
        //    if (direction.x > 0)
        //    {
        //        _anim.SetFloat("xDir", 1);
        //        _anim.SetFloat("yDir", 0);
        //    }
        //    else if (direction.x < 0)
        //    {
        //        _anim.SetFloat("xDir", -1);
        //        _anim.SetFloat("yDir", 0);
        //    }
        //}
        //else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        //{
        //    if (direction.y > 0)
        //    {
        //        _anim.SetFloat("yDir", 1);
        //        _anim.SetFloat("xDir", 0);
        //    }
        //    else if (direction.y < 0)
        //    {
        //        _anim.SetFloat("yDir", -1);
        //        _anim.SetFloat("xDir", 0);
        //    }
        //}
        #endregion
    }

    protected void ChangeState(EnemyState state)
    {
        if (state != currentState)
        {
            currentState = state;
        }
    }

    private void ChangeGoal()
    {
        if (currentPoint == path.Length - 1)
        {
            currentPoint = 0;
            currentGoal = path[0];
        }
        else
        {
            currentPoint++;
            currentGoal = path[currentPoint];
        }
    }

    protected void SetAttackPlayer(bool isAttacking)
    {
        _anim.SetBool("IsEnemyAttacking", isAttacking);
    }
    protected void SetMoveEnemy(bool isMoving)
    {
        _anim.SetBool("IsEnemyMoving", isMoving);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        _rigidb.velocity = Vector2.zero;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 1;
        }
    }

    public bool CastRayNormal(Vector2 rayNormal)
    {
        #region Old Raydirection
        // if (Mathf.Abs(rayDirection.x) > Mathf.Abs(rayDirection.y))
        // {
        //     rayDirection.y = 0;
        //     if(rayDirection.x > 0)
        //     {
        //         rayDirection.x = 1;
        //     }
        //     else if(rayDirection.x < 0)
        //     {
        //         rayDirection.x = -1;
        //     }
        // }
        // else if(Mathf.Abs(rayDirection.x) < Mathf.Abs(rayDirection.y))
        // {
        //     rayDirection.x = 0;
        //     if(rayDirection.y > 0)
        //     {
        //         rayDirection.y = 1;
        //     }
        //     else if(rayDirection.y < 0)
        //     {
        //         rayDirection.y = -1;
        //     }
        // }
        #endregion

        rayHit = Physics2D.Raycast(transform.position, rayNormal);

        if (!rayHit.collider.gameObject.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, rayNormal, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, rayNormal, Color.green);
            return false;
        }

        // if(move == Vector2.zero)
        // {
        //     return true;
        // }
        // else
        // {
        //     rayHit = Physics2D.Raycast(transform.position, ray);

        //     if (!rayHit.collider.gameObject.CompareTag("Player"))
        //     {
        //         Debug.Log(rayHit.collider.gameObject.name + " was hit");
        //         Debug.DrawRay(transform.position, ray, Color.red);
        //         return true;
        //     }
        //     else
        //     {
        //         Debug.DrawRay(transform.position, ray, Color.green);
        //         return false;
        //     }
        // }
    }

    public bool CastRayPatrolling(Vector2 rayPatrolling)
    {
        if (move == Vector2.zero)
        {
            return true;
        }
        else
        {
            rayHit = Physics2D.Raycast(transform.position, rayPatrolling);

            if (!rayHit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, rayPatrolling, Color.red);
                return true;
            }
            else
            {
                Debug.DrawRay(transform.position, rayPatrolling, Color.green);
                return false;
            }
        }
    }

    IEnumerator Death()
    {
        _rigidb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}