using UnityEngine;

public class VFXSystem : GameService
{
    public GameObject hitFx;
    public GameObject stepFx;

    public override void GameStarted()
    {
        Main.Get<GameEvents>().DamageDealt.AddListener(OnHurt);
        Main.Get<GameEvents>().PlayerMoved.AddListener(OnMoved);
    }

    void OnMoved(Vector3 pos)
    {
        Instantiate(stepFx, pos, Quaternion.identity);
    }
    
    void OnHurt(Vector3 pos, int damage)
    {
        Instantiate(hitFx, pos, Quaternion.identity).transform.localScale = Vector3.one * (damage / 100f);
    }
}