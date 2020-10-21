using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    // 주인공이 가지는 변수들의 최대 값
    int maxHealth = 20;
    int maxDefense = 20;
    int maxKey = 50;
    int maxBoom = 99;
    float maxSpeed = 10.0f;
    float defaultAttackSpeed = 1.0f;
    public GameObject AttackType;

    // public 변수들 getter
    public int health { get { return currentHealth; } }
    public int defense { get { return currentDefense; } }
    public int key { get { return currentKey; } }
    public int boom { get { return currentBoom; } }
    public float speed { get { return currentSpeed; } }

    // 현재 주인공이 가지는 변수값
    int currentHealth;
    int currentDefense;
    public int currentKey;
    int currentBoom;
    float currentSpeed;


    float timeInvincible = 2.0f;
    bool isInvincible = false;
    float invincibleTimer;

    bool isattackable = true;
    float attackTimer;

    Rigidbody2D rigidbody2d;
    BulletScripte weapon;

    Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = 5;
        currentDefense = 0;
        currentKey = 1;
        currentBoom = 3;
        currentSpeed = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // 체력이 없으면 사망
        if (currentHealth <= 0)
        {
            Die();

            return;
        }

        if (Input.GetKeyDown(KeyCode.C) && isattackable)
        {
            Attack();
            attackTimer = weapon.Delay;
            isattackable = false;
        } else {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0)
                isattackable = true;
        }

        // 무적 상태 검사기
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
    }

    private void LateUpdate()
    {
        // 체력이 없으면 사망
        if (currentHealth <= 0)
        {
            Die();

            return;
        }


        Move();
    }


    // 주인공의 이동을 정의
    private void Move()
    {
        float horisiontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horisiontal, vertical);
        Vector2 position = rigidbody2d.position;

        if (!Mathf.Approximately(move.x, 0.0f) ||
            !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        position.x += currentSpeed * Time.deltaTime * horisiontal;
        position.y += currentSpeed * Time.deltaTime * vertical;

        rigidbody2d.MovePosition(position);
    }

    public void Die()
    {

    }

    // 주인공의 공격을 정의
    private void Attack()
    {
        GameObject weaponObject = Instantiate(AttackType, rigidbody2d.position, Quaternion.identity);
        weapon = weaponObject.GetComponent<BulletScripte>();
        weapon.Shotting(lookDirection, 300);
    }

    /* 주인공이 가지는 변수들에 대한 setter들 */
    public void setHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        Debug.Log("Health: " + currentHealth);
    }
    public void setDefense(int amount) { currentDefense = Mathf.Clamp(currentDefense + amount, -5, maxDefense); }
    public void setKey(int amount) { currentKey = Mathf.Clamp(currentKey + amount, 0, maxKey); }
    public void setBoom(int amount) { currentBoom = Mathf.Clamp(currentBoom + amount, 0, maxBoom); }
    public void setSpeed(float amount) { currentSpeed = Mathf.Clamp(currentSpeed + amount, 0, maxSpeed); }
}
