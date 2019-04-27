﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Interface;

public class KillJane : MonoBehaviour, IKillable, IDamageable<float>
{
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private float health = 10f;
    [SerializeField] private float shootCooldown = 5f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float maxRangeToLandAttack = 7f;
    public Animator animator;

    public GameObject explosion;
    public GameObject poofEffect;
    public GameObject hitEffect;
    public AudioClip attackSound;

    public RoomController roomController;
	
    private AudioSource audio;
    private Rigidbody2D enemyBody;
    private Vector3 velocity = Vector3.zero;
    private GameManager gameManager;
    private GameObject player;

    private float currentHealth;
    private float attackDuration;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool isAttacking = false;

    private Transform shootPosition;

    //Enemy controller stuff
    //public float maxDistance = 20f;
    //public float attackDistance = 40f;
    //public float speed = 10f;
    public float attackSpeed = 20f;
    public float dashSpeed = 20f;
    public float attackCooldown = 5f;
    public float dashTime = 1f;

    private Transform playerTransform;
    private Vector3 moveDir = Vector3.zero;

    private float currentAttackCooldown;
    private float currentCooldown;

    private bool dash = false;
    private bool attack = false;
    private bool isDashing = false;
    private Vector2 dashVelocity;

    private bool dashNext = false;
    private bool attackNext = true;
    
    private void Awake() {
        enemyBody = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentCooldown = 2f;
    }
    
    private void Start()
    {
        Instantiate(poofEffect, transform.position, Quaternion.identity);
        currentHealth = health;
        player = GameObject.FindGameObjectWithTag("Player");
        //this.controller = this.gameObject.GetComponent<IEnemy>();
        currentAttackCooldown = attackCooldown;
    }

    private void Update()
    {
        currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0f)
        {
            if(attackNext) {
                Debug.Log("ATTACKING");
                attack = true;
                attackNext = false;
                dashNext = true;
            } else if(dashNext) {
                Debug.Log("DASHING");
                dash = true;
                dashNext = false;
                attackNext = true;
            }
            currentAttackCooldown = attackCooldown;
        }
        // float dist = Vector3.Distance(player.transform.position, transform.position);
        // if (dist > maxDistance) {
        //     move = true;
        // }
    }

    private void FixedUpdate() {
        Vector3 playerNormal = (player.transform.position - transform.position).normalized;

        if(isDashing) {
            //Vector3 targetVelocity = new Vector2(tarX * 10f, tarY * 10f);
            enemyBody.velocity = dashVelocity; //Vector3.SmoothDamp(enemyBody.velocity, dashVelocity, ref velocity, movementSmoothing);
        } else {
            enemyBody.velocity = Vector2.zero;
        }

        if (dash) {
            Rotate(playerNormal);
            Dash(playerNormal.x * dashSpeed, playerNormal.y * dashSpeed);
            ResetAttack();
        }
        if (attack) {
            //Do some special stuff
            Rotate(playerNormal);
            Attack(playerNormal.x * attackSpeed, playerNormal.y * attackSpeed);
            ResetAttack();
        }
    }

    public void Damage(float damageTaken)
    {
        currentHealth -= damageTaken;
        if (currentHealth <= 0f && !isDead)
            Kill();
        if (health > currentHealth) 
        {
            var healthPercentage = currentHealth/health;
            spriteRenderer.color = new Color(1f, healthPercentage, healthPercentage);
        }
    }
    
    public void Kill()
    {
        if(!isDead) {
            isDead = true;
            Instantiate(explosion, transform.position, Quaternion.identity);
            if(roomController) {
                roomController.DecrementAliveEnemyCount();
            }
            Destroy(gameObject);
        }
    }

    // public void Move(float tarX, float tarY)
    // {
    //     Vector3 targetVelocity = new Vector2(tarX * 10f, tarY * 10f);
    //     enemyBody.velocity = Vector3.SmoothDamp(enemyBody.velocity, targetVelocity, ref velocity, movementSmoothing);
    // }
    
    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void Attack(float tarX, float tarY)
    {
        animator.SetTrigger("Attack");
        audio.PlayOneShot(attackSound);
    }

    public void Dash(float tarX, float tarY)
    {
        animator.SetBool("Dashing", true);
        StartCoroutine("StopDashing");
        dashVelocity = new Vector2(tarX, tarY);
        isDashing = true;
        //audio.PlayOneShot(attackSound);
    }

    public void FinishPunch()
    {
        if(Vector2.Distance(player.transform.position, transform.position) <= maxRangeToLandAttack) {
            //TODO: Do damage to player
            var craigController = player.gameObject.GetComponent<CraigController>();
            craigController.Damage(attackDamage);
            Instantiate(hitEffect, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 0.1f), Quaternion.identity);
        }
    }


    private void ResetAttack()
    {
        attack = false;
        dash = false;
    }

    // public float GetAttackRange()
    // {
    //     return attackDistance;
    // }

    public IEnumerator StopDashing() {
        yield return new WaitForSeconds(dashTime);
        animator.SetBool("Dashing", false);
        isDashing = false;
    }
}