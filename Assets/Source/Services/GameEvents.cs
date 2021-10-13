using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemySpawnedEvent : UnityEvent<Enemy>
{
}

[System.Serializable]
public class DamageDealtEvent : UnityEvent<Vector3, int>
{
}

public class GameEvents : GameService
{
    public EnemySpawnedEvent EnemySpawned;
    public DamageDealtEvent DamageDealt;
}