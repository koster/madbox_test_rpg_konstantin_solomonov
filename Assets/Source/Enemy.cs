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

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}