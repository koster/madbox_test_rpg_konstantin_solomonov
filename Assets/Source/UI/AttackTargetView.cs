using UnityEngine;

public class AttackTargetView : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var unit = Main.Get<Player>().unit;

        if (unit.attackTarget != null)
        {
            transform.position = unit.attackTarget.transform.position + Vector3.up * 0.01f;
            transform.localScale = Vector3.one * unit.attackTarget.transform.localScale.magnitude;
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.1f);
        }
        else
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0f);
        }
    }
}