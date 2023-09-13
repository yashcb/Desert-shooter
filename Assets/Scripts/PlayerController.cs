using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public float playerRunSpeed;
    public float playerJumpForce;
    public float bulletSpeed;
    public float playerHealth;

    public Transform[] firePoints;
    public GameObject bullet_hor;
    public GameObject bullet_ver;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private Vector2 movement;
    private Vector2 direction;

    private bool IsMoving;
    private bool IsRunning;
    private bool IsJumping;
    private bool IsAttacking;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerHealth <= 0)
        {
            PlayerDeath();
        }
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Check for attack input
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                Fire();
                _animator.SetBool("Attacking", IsAttacking);
            }
        }
        else
        {
            if (IsAttacking)
            {
                IsAttacking = false;
                _animator.SetBool("Attacking", IsAttacking);
            }
        }

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!IsJumping)
            {
                IsJumping = true;
                _animator.SetBool("IsJumping", IsJumping);
            }
        }
        else
        {
            if (IsJumping)
            {
                IsJumping = false;
                _animator.SetBool("IsJumping", IsJumping);
            }
        }

        // Check for running
        if (IsMoving && Input.GetKey(KeyCode.LeftShift))
        {
            //movement = new Vector2(moveX, moveY).normalized * playerRunSpeed;
            IsRunning = true;
            _animator.SetBool("IsRunning", IsRunning);
        }
        else
        {
            IsRunning = false;
            _animator.SetBool("IsRunning", IsRunning);
        }

        // If not doing anything
        if (!IsMoving && !IsJumping)
        {
            movement = Vector3.zero;
        }

        //if (moveY != 0)
        //{
        //    if(moveX == 0)
        //    {
        //        MovingX = false;
        //        MovingY = true;
        //        //movement = Vector2.up * moveY;
        //        movement = new Vector2(moveX, moveY);
        //    }
        //    else if(moveX != 0)
        //    {
        //        if (MovingX)
        //        {
        //            movement = new Vector2(0, moveY);
        //        }
        //        else if(!MovingX)
        //        {
        //            movement = new Vector2(moveX, 0);
        //        }
        //    }
        //}

        //else if(moveX != 0)
        //{
        //    if(moveY == 0)
        //    {
        //        MovingX = true;
        //        MovingY = false;
        //        //movement = Vector2.right * moveX;
        //        movement = new Vector2(moveX, moveY);
        //    }
        //    else if(moveY != 0)
        //    {
        //        if (MovingY)
        //        {
        //            movement = new Vector2(moveX, 0);
        //        }
        //        else if(!MovingY)
        //        {
        //            movement = new Vector2(0, moveY);
        //        }
        //    }
        //}

        if (moveX != 0)
        {
            if (moveY != 0)
            { moveY = 0; }
        }
        else if (moveY != 0)
        {
            if (moveX != 0)
            { moveY = 0; }
        }

        // Determine if player is moving or not
        if (moveX != 0 || moveY != 0)
        {
            _animator.SetFloat("X", moveX);
            _animator.SetFloat("Y", moveY);

            IsMoving = true;
            _animator.SetBool("IsMoving", IsMoving);
        }
        else
        {
            IsMoving = false;
            _animator.SetBool("IsMoving", IsMoving);
        }

        movement = new Vector2(moveX, moveY);

        if (movement != Vector2.zero)
        {
            direction = movement.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (IsAttacking)
        {
            _rigidbody.velocity = Vector2.zero;
        }
        else if (IsRunning)
        {
            _rigidbody.velocity = playerRunSpeed * playerSpeed * Time.fixedDeltaTime * movement;
        }
        else
            _rigidbody.velocity = playerSpeed * Time.fixedDeltaTime * movement;
    }

    private void Fire()
    {
        if (direction == Vector2.right)
        {
            GameObject proj = Instantiate(bullet_hor, firePoints[0].position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (direction == Vector2.left)
        {
            GameObject proj = Instantiate(bullet_hor, firePoints[1].position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(Vector2.left * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (direction == Vector2.up)
        {
            GameObject proj = Instantiate(bullet_ver, firePoints[2].position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (direction == Vector2.down || direction == Vector2.zero)
        {
            GameObject proj = Instantiate(bullet_ver, firePoints[3].position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(Vector2.down * bulletSpeed, ForceMode2D.Impulse);
            Debug.Log("Down done!");
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            playerHealth -= 1;
        }
    }

    private void PlayerDeath()
    {

    }
}
