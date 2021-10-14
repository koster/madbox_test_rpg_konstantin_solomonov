using System.Collections.Generic;
using UnityEngine;

public class LootSystem : GameService
{
    public Drop lootObject;
    public List<Weapon> lootOptions;
    public float lootChance = 0.1f;

    public override void Init()
    {
        var events = Main.Get<GameEvents>();
        events.EnemyKilled.AddListener(DropLoot);
        events.LootPickedUp.AddListener(PickupLoot);
    }

    void OnDestroy()
    {
        var events = Main.Get<GameEvents>();
        events.EnemyKilled.RemoveListener(DropLoot);
        events.LootPickedUp.RemoveListener(PickupLoot);
    }

    void DropLoot(Enemy e)
    {
        if (Random.Range(0f, 1f) < lootChance)
        {
            var drop = Instantiate(lootObject, e.transform.position, Quaternion.identity);
            drop.Set(lootOptions[Random.Range(0, lootOptions.Count)]);
        }
    }

    void PickupLoot(Drop drop)
    {
        Main.Get<Player>().ChangeWeapon(drop.droppedWeapon);
    }
}