using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Weapon weapon;
    public Unit unit;
    Vector3 randee;

    void Awake()
    {
        unit.OnKilled.AddListener(OnEnemyKilled);
        unit.Equip(weapon);
        
        InvokeRepeating(nameof(RAndomiz), 0f,5f);
    }

    void RAndomiz()
    {
        if (Random.Range(0f, 1f) < 0.5f)
        {
            randee = Random.insideUnitSphere;
            randee.y = 0;
        }
        else
        {
            randee = Vector3.zero;
        }
    }

    void Update()
    {
        unit.SetMoveDirection(randee);
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