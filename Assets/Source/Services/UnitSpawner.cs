using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : GameService
{
    public List<GameObject> enemies;
    public int count = 10;

    public float width;
    public float height;

    public override void GameStarted()
    {
        for (var i = 0; i < count; i++)
        {
            var randomEnemy = enemies[Random.Range(0, enemies.Count)];
            var instance = Instantiate(randomEnemy);
            instance.transform.position = new Vector3(
                Random.Range(-width / 2f, width / 2f),
                0,
                Random.Range(-height / 2f, height / 2f)
            );

            Main.Get<GameEvents>().EnemySpawned.Invoke(instance.GetComponent<Enemy>());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0.1f, height));
    }
}