using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : ChasingEnemy
{
    // public Transform currentGoal;
    // public Transform[] path;
    // public int currentPoint;
    // public float roundingDistance;

    public override void CheckDistance()
    {
        if (distance <= chaseRadius && distance > attackRadius && !obstacleAhead && !enemyAttacking)
        {
            enemyAttacking = false;
            enemyMoving = true;
            move = Vector3.MoveTowards(transform.position, playerTransform.position, enemySpeed * Time.fixedDeltaTime);
            dir = (move - (Vector2)transform.position).normalized;

            ChangeAnimation(dir);
            _rigidb.MovePosition(move);
        }

        else if ((distance <= chaseRadius && distance > attackRadius && obstacleAhead && !enemyAttacking) || (distance > chaseRadius && !enemyAttacking))
        {
            enemyAttacking = false;
            enemyMoving = true;

            if (Vector3.Distance(transform.position, path[currentPoint].position) > roundingDistance)
            {
                move = Vector3.MoveTowards(transform.position, path[currentPoint].position, enemySpeed * Time.deltaTime);
                dir = (move - (Vector2)transform.position).normalized;
                ChangeAnimation(dir);
                _rigidb.MovePosition(move);
            }
            else if (Vector3.Distance(transform.position, path[currentPoint].position) <= roundingDistance)
            {
                ChangeGoal();
            }
        }

        else if (distance <= attackRadius && !obstacleAhead)
        {
            enemyMoving = false;
            enemyAttacking = true;

            // if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            // {
            //     dir.y = 0;
            //     if (dir.x > 0)
            //     {
            //         dir.x = 1;
            //     }
            //     else if (dir.x < 0)
            //     {
            //         dir.x = -1;
            //     }
            // }
            // else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
            // {
            //     dir.x = 0;
            //     if (dir.y > 0)
            //     {
            //         dir.y = 1;
            //     }
            //     else if (dir.y < 0)
            //     {
            //         dir.y = -1;
            //     }
            // }

            // ChangeAnimation(dir);
            //SetMoveEnemy(enemyMoving);
        }
        else if (distance > chaseRadius)
        {
            enemyAttacking = false;
            enemyMoving = true;
        }
        SetMoveEnemy(enemyMoving);
        SetAttackPlayer(enemyAttacking);
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

    public bool CastRay(Vector2 ray)
    {
        if (move == Vector2.zero)
        {
            return true;
        }
        else
        {
            rayHit = Physics2D.Raycast(transform.position, ray);

            if (!rayHit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, ray, Color.red);
                return true;
            }
            else
            {
                Debug.DrawRay(transform.position, ray, Color.green);
                return false;
            }
        }
    }
}