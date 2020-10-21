using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScripte : MonoBehaviour
{
    int weaponIndex = 0;
    int damage = 1;
    int maxBullet = 2;
    float shotRange = 1.5f;
    float shotDelay = 1.0f;        // 공격속도
    float bulletSpeed = 1.0f;       // 탄환 속도
   
    GameObject gamePlayer;
    Rigidbody2D rigidbody2d;
    HeroController hero;
    MonsterScripte monster;

    int currentBullet;

    public int Index { get { return weaponIndex; } } // Index는 set 불가
    public int Damage 
    { 
        get { return damage; } 
        set { damage = Mathf.Clamp(damage + value, 0, 99); }
    }
    public int Bullet
    { 
        get { return maxBullet; }
        set { maxBullet = Mathf.Clamp(maxBullet + value, 0, 10); }
    }
    public float Range
    {
        get { return shotRange; }
        set { shotRange = Mathf.Clamp(shotRange + value, 0, 10.0f); }
    }
    public float Delay
    { 
        get { return shotDelay; }
        set { shotDelay = Mathf.Clamp(shotDelay + value, 0, 10.0f); }
    }

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        gamePlayer = GameObject.FindWithTag("Player");
        currentBullet = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(this.transform.position, gamePlayer.transform.position) > shotRange)
        {
            currentBullet = Mathf.Clamp(currentBullet + 1, 0, maxBullet);
            Destroy(gameObject);
        }
    }

    public void Shotting(Vector2 direction, float force)
    {
        currentBullet = Mathf.Clamp(currentBullet -1, 0, maxBullet);
        rigidbody2d.AddForce(direction * force * bulletSpeed);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        // 몬스터만 공격
        if (other.gameObject.tag == "Monster")
        {
            monster = other.gameObject.GetComponent<MonsterScripte>();
            monster.setHealth(-damage);
        }

        currentBullet = Mathf.Clamp(currentBullet + 1, 0, maxBullet);
        Destroy(gameObject);
    }

}
