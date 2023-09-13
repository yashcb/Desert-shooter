using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Enemy
{
    public Transform playerTransform;
    public float chaseRadius;
    public float attackRadius;

    public Rigidbody2D _rigidb;
    public Animator _anim;
    public Vector2 move;
    public Vector2 dir;
    public float distance;
    //private bool enemyMoving;
    private bool enemyAttacking;
    private bool obstacleAhead;
    private RaycastHit2D rayHit;

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
        //decrease = (health / 4);
    }

    #region Move Animation
    //private void Update()
    //{
    //if (enemyMoving)
    //{
    //    _anim.SetBool("IsEnemyMoving", enemyMoving);
    //}
    //else
    //_anim.SetBool("IsEnemyMoving", enemyMoving);

    //if(distance <= attackRadius)
    //{
    //    AttackPlayer();
    //}

    //move = move.normalized;
    //if (move.y != 0)
    //{
    //    if (move.x == 0)
    //    {
    //        MovingX = false;
    //        MovingY = true;
    //        //movement = Vector2.up * moveY;
    //        move = new Vector2(move.x, move.y);
    //    }
    //    else if (move.x != 0)
    //    {
    //        if (MovingX)
    //        {
    //            move = new Vector2(0, move.y);
    //        }
    //        else
    //        {
    //            move = new Vector2(move.x, 0);
    //        }
    //    }
    //}
    //else if (move.x != 0)
    //{
    //    if (move.y == 0)
    //    {
    //        MovingX = true;
    //        MovingY = false;
    //        move = new Vector2(move.x, move.y);
    //    }
    //    else if (move.y != 0)
    //    {
    //        if (MovingY)
    //        {
    //            move = new Vector2(move.x, 0);
    //        }
    //        else
    //        {
    //            move = new Vector2(0, move.y);
    //        }
    //    }
    //}
    //}
    #endregion

    //private void Update()
    //{
    //    obstacleAhead = CastRay(move);
    //}

    private void FixedUpdate()
    {
        //Debug.DrawLine(_rigidb.transform.position, Vector2.up, Color.red);
        distance = Vector3.Distance(playerTransform.position, transform.position);
        CheckDistance();
    }

    public virtual void CheckDistance()
    {
        obstacleAhead = CastRay(move);
        //Debug.Log(obstacleAhead);
        if (obstacleAhead)
        {
            if (distance <= chaseRadius)
            {
                if (distance > attackRadius)
                {
                    enemyAttacking = false;
                    //enemyMoving = true;
                    ChangeState(EnemyState.Moving);
                    _anim.SetBool("IsEnemyMoving", true);
                    ChangeAnimation(dir);
                    move = Vector3.MoveTowards(transform.position, playerTransform.position, enemySpeed * Time.fixedDeltaTime);
                    dir = move - (Vector2)transform.position;
                    _rigidb.MovePosition(move);
                    AttackPlayer(enemyAttacking);
                }
                else if (distance <= attackRadius)
                {
                    enemyAttacking = true;

                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        dir.y = 0;
                    }
                    else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                        dir.x = 0;

                    ChangeAnimation(dir);
                    AttackPlayer(enemyAttacking);
                }
            }
            else
            {
                //enemyMoving = false;
                ChangeState(EnemyState.Idle);
                _anim.SetBool("IsEnemyMoving", false);
                _rigidb.MovePosition(transform.position);
            }
        }
    }

    public void ChangeAnimation(Vector2 direction)
    {
        direction = direction.normalized;
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

    private void ChangeState(EnemyState state)
    {
        if(state != currentState)
        {
            currentState = state;
        }
    }

    private void AttackPlayer(bool isAttacking)
    {
        _anim.SetBool("IsEnemyAttacking", isAttacking);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rigidb.velocity = Vector2.zero;

        if(collision.gameObject.CompareTag("Bullet"))
        {
            //_rigidb.AddForce(move.normalized , ForceMode2D.Impulse);
            health -= 1;
            //health -= decrese;
        }
        if (health <= 0)
        {
            _anim.SetTrigger("Died");
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        _rigidb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    private bool CastRay(Vector3 mov)
    {
        //mov = mov.normalized;
        Debug.Log(transform.forward);
        rayHit = Physics2D.Raycast(transform.position, transform.position - mov, 1f);

        if (rayHit.collider != null)
        {
            //Debug.Log(rayHit.collider.name + " was hit");
            Debug.DrawLine(transform.position, transform.position - mov, Color.red);
            return true;
        }
        else
            return false;
    }
}
