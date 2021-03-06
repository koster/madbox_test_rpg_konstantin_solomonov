using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitAnimationEvents : MonoBehaviour
{
    public event UnityAction HitDamage;

    public void OnHitDamage()
    {
        HitDamage?.Invoke();
    }
}
