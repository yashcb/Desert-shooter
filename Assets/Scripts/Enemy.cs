using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float enemySpeed;

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking
    }
    public EnemyState currentState;

    public enum EnemyType{
        Chasing,
        Patrolling
    }
    public EnemyType currentEnemyType;
}