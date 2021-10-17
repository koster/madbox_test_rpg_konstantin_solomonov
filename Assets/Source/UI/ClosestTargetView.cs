using UnityEngine;

public class ClosestTargetView : MonoBehaviour
{
    void Update()
    {
        var allEnemies = Main.Get<UnitSpawner>().allEnemies;
        if (allEnemies.Count == 0)
            return;

        var player = Main.Get<Player>();
        var playerPosition = player.GetPosition();

        var closest = allEnemies[0];
        foreach (var e in allEnemies)
        {
            if (e.unit.IsAlive())
            {
                if (Vector3.Distance(playerPosition, e.transform.position) <
                    Vector3.Distance(playerPosition, closest.transform.position))
                {
                    closest = e;
                }
            }
        }

        var direction = playerPosition - closest.transform.position;
        transform.right = direction.normalized;
        transform.position = playerPosition;
    }
}