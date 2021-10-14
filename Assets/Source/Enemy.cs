using UnityEngine;

public enum EnemyState
{
    ALIVE,
    DEAD
}

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public float height = 1f;

    EnemyState state;

    int health;
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
        state = EnemyState.ALIVE;
    }

    public EnemyState GetState()
    {
        return state;
    }

    public int GetHealth()
    {
        return health;
    }

    public void Hurt(int damage)
    {
        if (state == EnemyState.DEAD)
            return;

        health -= damage;

        var damagePoint = transform.position + Vector3.up * height / 2f;
        Main.Get<GameEvents>().DamageDealt?.Invoke(damagePoint, damage);

        if (health <= 0)
        {
            Main.Get<GameEvents>().EnemyKilled?.Invoke(this);
            state = EnemyState.DEAD;
            animator.SetBool("Dead", true);
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}