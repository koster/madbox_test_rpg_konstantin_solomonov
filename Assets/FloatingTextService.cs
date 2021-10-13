using UnityEngine;

public class FloatingTextService : GameService
{
    public FloatingTextUI floatingTextDamage;

    public override void Init()
    {
        Main.Get<GameEvents>().DamageDealt.AddListener(ShowFloatingDamage);
    }

    void OnDestroy()
    {
        Main.Get<GameEvents>().DamageDealt.RemoveListener(ShowFloatingDamage);
    }

    void ShowFloatingDamage(Vector3 pos, int damage)
    {
        var instance = Instantiate(floatingTextDamage, transform.parent);
        instance.transform.position = pos;
        instance.textMesh.text = damage.ToString();

        var facingCamera = (pos - Camera.main.transform.position).normalized;
        facingCamera.y = 0;
        instance.transform.forward = facingCamera;
    }
}