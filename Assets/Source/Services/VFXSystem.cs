using UnityEngine;

public class VFXSystem : GameService
{
    public GameObject hitFx;
    public GameObject stepFx;

    VFXPool poolHit;
    VFXPool poolStep;

    public override void GameStarted()
    {
        Main.Get<GameEvents>().DamageDealt.AddListener(OnHurt);
        Main.Get<GameEvents>().PlayerMoved.AddListener(OnMoved);

        poolHit = new VFXPool(hitFx, 2f);
        poolStep = new VFXPool(stepFx, 2f);
    }

    void OnMoved(Vector3 pos)
    {
        var step = poolStep.Get();
        step.transform.position = pos;
    }

    void OnHurt(Vector3 pos, int damage)
    {
        var hit = poolHit.Get();
        hit.transform.position = pos;
        hit.transform.localScale = Vector3.one * damage / 100f;
    }
}