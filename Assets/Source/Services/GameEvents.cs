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

[System.Serializable]
public class WeaponChangedEvent : UnityEvent<Weapon>
{
}

[System.Serializable]
public class EnemyKilledEvent : UnityEvent<Enemy>
{
}
[System.Serializable]
public class LootPickedUpEvent : UnityEvent<Drop>
{
}

public class GameEvents : GameService
{
    public EnemySpawnedEvent EnemySpawned;
    public DamageDealtEvent DamageDealt;
    public WeaponChangedEvent PlayerWeaponChanged;
    public EnemyKilledEvent EnemyKilled;
    public LootPickedUpEvent LootPickedUp;
}