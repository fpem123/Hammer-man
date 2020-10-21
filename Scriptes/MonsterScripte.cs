using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScripte : MonoBehaviour
{
    int maxDamage = 10;
    int maxDefense = 10;
    int maxHealth = 1;
    float maxSpeed = 10.0f;
    float maxAttackSpeed = 10.0f;

    public int health { get { return currentHealth; } }
    public int defense { get { return currentDefense; } }
    public int damge { get { return currentDamage; } }
    public float speed { get { return currentSpeed; } }
    public float AttackSpeed { get { return currentAttackSpeed; } }

    int currentHealth;
    int currentDefense;
    int currentDamage;
    int destroyTimer = 3;
    float currentSpeed;
    float currentAttackSpeed;
    float changeTime = 0.3f;
    float directionTimer;
    float xdirection = 0.0f;
    float ydirection = 0.0f;
    bool yesIamDead = false;        // Die 함수를 한번만 호출하기 위함

    GameObject myRoom;  // 자신이 종속된 방

    Rigidbody2D rigidbody2d;
    HeroController player;
    RoomManager room;           // 자신이 종속된 방의 스크립트 정보

    // Start is called before the first frame update
    void Start()
    {
        myRoom = this.gameObject.transform.parent.gameObject;   

        rigidbody2d = GetComponent<Rigidbody2D>();
        room = myRoom.GetComponent<RoomManager>();

        currentHealth = 10;
        currentDefense = 0;
        currentDamage = 1;
        currentSpeed = 1.0f;
        currentAttackSpeed = 5.0f;
        directionTimer = changeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0 && !yesIamDead)
        {
            yesIamDead = true;

            Die();

            return;
        }
    }

    private void LateUpdate()
    {
        if (currentHealth <= 0 && !yesIamDead)
        {
            yesIamDead = true;

            Die();

            return;
        }

        directionTimer -= Time.deltaTime;

        if (directionTimer < 0)
        {
            xdirection = Random.Range(-1, 2);
            ydirection = Random.Range(-1, 2);

            directionTimer = changeTime;
        }

        Move();

    }

    private void Move()
    {
        Vector2 position = rigidbody2d.position;

        position.x += Time.deltaTime * currentSpeed * xdirection;
        position.y += Time.deltaTime * currentSpeed * ydirection;

        rigidbody2d.MovePosition(position);
    }

    private void Attack()
    {
        // 플레이어에게 데미지를 주는 공격을 한다.

    }

    // 체력이 0이 되면 호출하는 함수, 이 몬스터는 죽었다
    private void Die()
    {
        // 아이템과 시체를 남기고 파괴시킨다.
        // 먼저 콜라이더를 모두 제거시키고 일정 시간 지나면 오브젝트도 파괴시키는 걸로

        rigidbody2d.simulated = false;

        room.ChangeNumberOfMonster(-1);

        Destroy(gameObject, destroyTimer);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.GetComponent<HeroController>();

            if (player != null)
            {
                player.setHealth(-damge);
            }
        }

        if (other.gameObject.tag == "Wall")
        {
            // 벽이면 방향을 바꾸자
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.GetComponent<HeroController>();

            if (player != null)
            {
                player.setHealth(-damge);
            }
        }

        if (other.gameObject.tag == "Wall")
        {
            // 벽이면 방향을 바꾸자
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        player = other.gameObject.GetComponent<HeroController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        player = other.gameObject.GetComponent<HeroController>();

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player = other.gameObject.GetComponent<HeroController>();
    }


    // setter 들
    public void setHealth(int amount)
    {
        if (amount < -1)
            amount = Mathf.Clamp(amount + currentDefense, amount, -1);

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Monster Health: " + currentHealth);
    }
    public void setDefense(int amount) { currentDefense = Mathf.Clamp(currentDefense + amount, 0, maxDefense); }
    public void setDamage(int amount) { currentDamage = Mathf.Clamp(currentDamage + amount, 0, maxDamage); }
    public void setSpeed(float amount) { currentSpeed = Mathf.Clamp(currentSpeed + amount, 0, maxSpeed); }
    public void setAttackSpeed(float amount) { currentAttackSpeed = Mathf.Clamp(currentAttackSpeed + amount, 0, maxAttackSpeed); }
}
