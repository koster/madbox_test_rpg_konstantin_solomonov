using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData")]
[Serializable]
public class UnitData : ScriptableObject
{
    public GameObject prefab;

    public int maxHealth;

    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public float attackRate = 1f;

    public float rotationDamping = 0.25f;
    public float height = 1f;

    public AnimationClip attackAnimation;
}