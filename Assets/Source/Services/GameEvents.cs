using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemySpawnedEvent : UnityEvent<Enemy>
{
}

public class GameEvents : GameService
{
    public EnemySpawnedEvent EnemySpawned;
    public event UnityAction<Vector3, int> DamageDealt;
}