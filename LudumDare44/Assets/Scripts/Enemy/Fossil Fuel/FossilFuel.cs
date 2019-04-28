﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enemy.Interface;
using UnityEngine;

public class FossilFuel : MonoBehaviour, IDamageable<float>, IEnemy, IKillable
{
    [Header("Boss Properties")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float howBigABitchMitchIs = 100f;
    [SerializeField] private float movementSmoothing = 0.5f;
    [Header("Attack Properties")]
    [SerializeField] private float attackDuration = 5f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private bool randomizeSpread = false;
    [Range(0f, 1f)][SerializeField] private float projectileSpawnRate = 0.2f;
    [Range(0f, 360f)][SerializeField] private float projectileSpread = 25f;
    [Range(0f, 50f)][SerializeField] private float projectileSpeed = 1f;
    [Range(1f, 100f)] [SerializeField] private float projectileSize = 1f;
    [Range(0, 50)] [SerializeField] private int amountOfBulletsAtATime = 1;
    [Range(0f, 100f)] [SerializeField] private float phaseTransitionPercent = 25f;
    [Header("Game Objects")]
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject gunHoleLocation;

    private Animator animator; 
    private Rigidbody2D enemyBody;
    private Vector3 velocity = Vector3.zero;
    private Transform shotOriginatingLocation;
    private GameObject player;
    
    private float currentHealth;
    private float currentAttackDuration;
    private float currentAttackCooldown;
    private float currentProjectileCooldown;
    private bool isDead = false;
    private bool reloading = false;
    private bool isSpinning = false;

    public RoomController roomController;
    

    private void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        shotOriginatingLocation = gunHoleLocation.GetComponent<Transform>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        currentHealth = health;
        currentAttackCooldown = attackCooldown;
        currentAttackDuration = 0f;
        currentProjectileCooldown = projectileSpawnRate;
    }

    public void Damage(float damageTaken)
    {
        currentHealth -= damageTaken;
        if (currentHealth <= 0f && !isDead)
            Kill();
    }

    public void Move(float tarX, float tarY)
    {
        Vector3 targetVelocity = new Vector2(tarX * 10f, tarY * 10f);
        enemyBody.velocity = Vector3.SmoothDamp(enemyBody.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero && !isSpinning) 
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void Attack(float tarX, float tarY)
    {
        if (!reloading)
        {
            currentAttackDuration += Time.deltaTime;
            currentProjectileCooldown -= Time.deltaTime;
            if (currentAttackDuration >= attackDuration)
            {
                reloading = true;
                animator.SetBool("Attack Cooldown", true);
                currentAttackDuration = 0f;
            }
            if (currentProjectileCooldown <= 0f)
            {
                PhaseController();
            }
        }
        else if (reloading)
        {
            currentAttackCooldown -= Time.deltaTime;
            isSpinning = false;
            if (currentAttackCooldown <= 0f)
            {
                reloading = false;
                animator.SetBool("Attack Cooldown", false);
                currentAttackCooldown = attackCooldown;
            }
        }
        
    }

    public void Kill()
    {
        if(!isDead) {
            isDead = true;
            Instantiate(explosion, transform.position, Quaternion.identity);

            if(roomController) {
                roomController.KillBoss();
            }

            Destroy(gameObject);
        }
    }

    public void StopMove()
    {
       
    }

    private void PhaseController()
    {
        if ((currentHealth / health) > (phaseTransitionPercent / 100))
        {
            if (randomizeSpread)
                RandomSpreadAttack();
            else
                RegularSpreadAttack();
        }
        else
        {
            SpinAttack();
        }
    }

    private void RandomSpreadAttack()
    {
        Quaternion rotation = transform.rotation;
        for (int i = 1; i <= amountOfBulletsAtATime; i++)
        {
            var bullet = Instantiate(projectile, shotOriginatingLocation.position, rotation) as GameObject;
            bullet.transform.localScale = new Vector3(projectileSize, projectileSize);
            bullet.transform.Rotate(0, 0, Random.Range(-projectileSpread, projectileSpread));
            bullet.GetComponent<Rigidbody2D>().velocity = projectileSpeed * bullet.transform.up;
        }
        currentProjectileCooldown = projectileSpawnRate;
    }

    private void RegularSpreadAttack()
    {
        Quaternion rotation = transform.rotation;
        if (amountOfBulletsAtATime % 2 == 0)
        {
            var bulletAngle = projectileSpread / (amountOfBulletsAtATime / 2);
            var currentAngle = -projectileSpread;
            for (int i = 1; i <= amountOfBulletsAtATime; i++)
            {
                var bullet = Instantiate(projectile, shotOriginatingLocation.position, rotation) as GameObject;
                bullet.transform.localScale = new Vector3(projectileSize, projectileSize);
                bullet.transform.Rotate(0, 0, currentAngle);
                bullet.GetComponent<Rigidbody2D>().velocity = projectileSpeed * bullet.transform.up;
                currentAngle += bulletAngle;
            }
            currentProjectileCooldown = projectileSpawnRate;
        }
        else
        {
            var bulletAngleIncrements = projectileSpread / (amountOfBulletsAtATime - 1);
            var currentAngle = -(projectileSpread / 2);
            for (int i = 1; i <= amountOfBulletsAtATime; i++)
            {
                var bullet = Instantiate(projectile, shotOriginatingLocation.position, rotation) as GameObject;
                bullet.transform.localScale = new Vector3(projectileSize, projectileSize);
                bullet.transform.Rotate(0, 0, currentAngle);
                bullet.GetComponent<Rigidbody2D>().velocity = projectileSpeed * bullet.transform.up;
                currentAngle += bulletAngleIncrements;
            }
            currentProjectileCooldown = projectileSpawnRate;
        }
    }

    private void SpinAttack()
    {
        isSpinning = true;
        this.gameObject.transform.Rotate(0, 0, Random.Range(-360, 360));
        Quaternion rotation = transform.rotation;
        var bulletAngleIncrements = projectileSpread / (amountOfBulletsAtATime - 1);
        var currentAngle = -(projectileSpread / 2);
        for (int i = 1; i <= amountOfBulletsAtATime; i++)
        {
            var bullet = Instantiate(projectile, shotOriginatingLocation.position, rotation) as GameObject;
            bullet.transform.localScale = new Vector3(projectileSize, projectileSize);
            bullet.transform.Rotate(0, 0, currentAngle);
            bullet.GetComponent<Rigidbody2D>().velocity = projectileSpeed * bullet.transform.up;
            currentAngle += bulletAngleIncrements;
        }
        currentProjectileCooldown = projectileSpawnRate;
    }
}
