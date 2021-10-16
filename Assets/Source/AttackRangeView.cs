using UnityEngine;

public class AttackRangeView : MonoBehaviour
{
    void Update()
    {
        transform.position = Main.Get<Player>().GetPosition() + Vector3.up * 0.01f;
        transform.localScale = Vector3.one * Main.Get<Player>().unit.CalculateAttackRange() * 2f;
    }
}