using UnityEngine;

public class FloatingTextService : GameService
{
    public FloatingTextUI floatingTextDamage;

    VFXPool poolFTDamage;

    public override void Init()
    {
        poolFTDamage = new VFXPool(floatingTextDamage.gameObject, floatingTextDamage.lifetime);

        Main.Get<GameEvents>().DamageDealt.AddListener(ShowFloatingDamage);
    }

    void OnDestroy()
    {
        Main.Get<GameEvents>().DamageDealt.RemoveListener(ShowFloatingDamage);
    }

    void ShowFloatingDamage(Vector3 pos, int damage)
    {
        var instance = poolFTDamage.Get();

        var ftui = instance.GetComponent<FloatingTextUI>();
        ftui.transform.position = pos;
        ftui.textMesh.text = damage.ToString();

        var facingCamera = (pos - Camera.main.transform.position).normalized;
        facingCamera.y = 0;
        ftui.transform.forward = facingCamera;
    }
}