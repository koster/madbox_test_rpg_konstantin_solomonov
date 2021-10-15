using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Weapon weapon;
    public Unit unit;
    Vector3 random;

    void Start()
    {
        unit.OnKilled.AddListener(OnEnemyKilled);
        unit.Equip(weapon);
        
        InvokeRepeating(nameof(Randomize), 0f,5f);
    }

    void Randomize()
    {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            random = Random.insideUnitSphere;
            random.y = 0;
        }
        else
        {
            random = Vector3.zero;
        }
    }

    void Update()
    {
        unit.moveDirection = random;
    }

    void OnEnemyKilled()
    {
        Main.Get<GameEvents>().EnemyKilled?.Invoke(this);
    }

    public Unit GetUnit()
    {
        return unit;
    }
}