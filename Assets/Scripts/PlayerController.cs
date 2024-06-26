using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    
    //Oyuncu karakter hareketiyle ilgili değişkenler
    private Rigidbody2D rigidbody2D;
    private Vector2 move;
    public float speed = 3.0f;
    public InputAction MoveAction;

    
    //Sağlık sistemine ilişkin değişkenler
    public int health { get { return currentHealth; } }
    private int currentHealth;
    public int maxHealth = 5;

    //Geçici yenilmezlikle ilgili değişkenler
    public float timeInvincible = 2.0f;  //oyuncu karakterinin hasar aldıktan sonra ne kadar süre yenilmez kalacağı
    private bool isInvincible; //oyuncu karakterinin o anda yenilmez olup olmadığını saklar
    private float damageCooldown; //oyuncu karakteri artık yenilmez olmayana ve daha fazla hasar alabilene kadar ne kadar süre kaldığını saklar.

    private Animator animator;
    private Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public InputAction launchAction;
    
        // Start is called before the first frame update
    void Start()
    {

        MoveAction.Enable();
        rigidbody2D = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        


    }

    // Update is called once per frame
    void Update()
    {
        
         move = MoveAction.ReadValue<Vector2>();
        //Debug.Log(move);


        //move.x veya move.y'nin 0'a eşit olup olmadığını kontrol eder
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
            
        }
        
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        
        
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
       
    }

    private void FixedUpdate()
    {
        
        
        
        Vector2 position = (Vector2)rigidbody2D.position + move * speed * Time.deltaTime;
        rigidbody2D.MovePosition(position);
    }

   public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }
        
        
        
        
        
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float) maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2D.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        
        animator.SetTrigger("Launch");
    }
    
   
}


