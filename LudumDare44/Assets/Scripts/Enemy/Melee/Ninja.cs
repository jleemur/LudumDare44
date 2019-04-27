using System.Collections;
using System.Collections.Generic;
using Enemy.Interface;
using UnityEngine;

public class Ninja: MonoBehaviour, IDamageable<float>, IKillable, IEnemy
{
    
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private float health = 10f;
    [SerializeField] private float shootCooldown = 5f;
    public Animator animator;

    public GameObject explosion;
    public GameObject poofEffect;
	
    private AudioSource audio;
    private Rigidbody2D enemyBody;
    private Vector3 velocity = Vector3.zero;
    private float currentCooldown;
    private bool hasShot = true;
    private GameManager gameManager;

    private float currentHealth;
    SpriteRenderer spriteRenderer;

    private bool isDead = false;

    private Transform shootPosition;
    
    private void Awake() {
        enemyBody = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        currentCooldown = 2f;
    }
    
    private void Start()
    {
        
    }

    private void Update()
    {
      
    }

    public void Damage(float damageTaken)
    {
        health -= damageTaken;
        if (health <= 0f)
            Kill();
    }
    
    public void Kill()
    {
        isDead = true;
        Instantiate(explosion, transform.position, Quaternion.identity);
        gameManager.DecreaseEnemyCount();
        Destroy(gameObject);
    }

    public void Move(float tarX, float tarY)
    {
        Vector3 targetVelocity = new Vector2(tarX * 10f, tarY * 10f);
        enemyBody.velocity = Vector3.SmoothDamp(enemyBody.velocity, targetVelocity, ref velocity, movementSmoothing);
    }
    
    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void Attack(float tarX, float tarY)
    {
       
    }
}
