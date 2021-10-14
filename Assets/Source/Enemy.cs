using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public float height = 1f;

    int health;

    void Awake()
    {
        health = maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public void Hurt(int damage)
    {
        health -= damage;
        
        var damagePoint = transform.position + Vector3.up * height / 2f;
        Main.Get<GameEvents>().DamageDealt?.Invoke(damagePoint, damage);

        if (health <= 0)
        {
            Main.Get<GameEvents>().EnemyKilled?.Invoke(this);
            Destroy(gameObject);
        }
    }
}