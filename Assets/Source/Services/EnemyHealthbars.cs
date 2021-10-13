public class EnemyHealthbars : GameService
{
    public HealthbarUI healthbarPrefab;

    public override void Init()
    {
        Main.Get<GameEvents>().EnemySpawned.AddListener(AttachHealthbar);
    }

    void OnDestroy()
    {
        Main.Get<GameEvents>().EnemySpawned.RemoveListener(AttachHealthbar);
    }

    void AttachHealthbar(Enemy enemy)
    {
        var hbp = Instantiate(healthbarPrefab);
        hbp.enemy = enemy;
        hbp.transform.parent = transform;
    }
}