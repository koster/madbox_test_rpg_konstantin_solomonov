using UnityEngine;

public class GameOverSystem : GameService
{
    public Canvas ui;

    Player player;

    public override void GameStarted()
    {
        player = Main.Get<Player>();
    }

    void Update()
    {
        ui.enabled = player.unit.IsDead();
    }
}