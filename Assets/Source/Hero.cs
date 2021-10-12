using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public float attackRate = 1f;
    
    public float sensitivity = 0.5f;
    public float motionDamping = 0.9f;
    public float rotationDamping = 0.25f;

    public List<Weapon> startingWeapons = new List<Weapon>();

    public Transform weaponSlot;

    Weapon equippedWeapon;

    float attackCooldown;

    Vector3 origin;
    bool input;
    Vector3 moveVector;

    Animator animator;
    HeroAnimationEvents animationEvents;

    Collider attackTarget;

    void Start()
    {
        var range = Random.Range(0, startingWeapons.Count);
        var startingWeapon = startingWeapons[range];
        Equip(startingWeapon);

        animator = GetComponentInChildren<Animator>();

        animationEvents = GetComponentInChildren<HeroAnimationEvents>();
        animationEvents.HitDamage += HitAnimation;

        InvokeRepeating(nameof(SlowTick), 0f, 1 / 10f);
    }

    public void Equip(Weapon weapon)
    {
        equippedWeapon = Instantiate(weapon, Vector3.zero, Quaternion.identity);
        equippedWeapon.transform.SetParent(weaponSlot, false);
    }

    void OnDestroy()
    {
        animationEvents.HitDamage -= HitAnimation;
    }

    public void HitAnimation()
    {
        if (attackTarget == null)
            return;

        Destroy(attackTarget.gameObject);
    }

    void SlowTick()
    {
        var enemies = FetchTargets();

        Collider closest = null;
        foreach (var enemy in enemies)
        {
            if (enemy == null)
                break;

            if (closest == null)
                closest = enemy;
            else if (IsCloserToMeThan(closest.transform, enemy.transform))
                closest = enemy;
        }

        attackTarget = closest;
    }

    bool IsCloserToMeThan(Transform clsst, Transform enemy)
    {
        return Vector3.Distance(clsst.position, transform.position) <
               Vector3.Distance(enemy.position, transform.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            input = true;
            origin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            input = false;
        }

        animator.SetFloat("MoveInput", moveVector.magnitude);
    }

    void FixedUpdate()
    {
        attackCooldown -= Time.fixedDeltaTime;

        if (input)
        {
            var axis = origin - Input.mousePosition;
            axis = Vector3.ClampMagnitude(axis * sensitivity, 1f);
            axis = new Vector3(axis.x, 0, axis.y);

            moveVector = Vector3.Lerp(moveVector, axis, motionDamping);
        }
        else
        {
            moveVector *= motionDamping;

            if (attackTarget != null)
            {
                if (attackCooldown < 0)
                {
                    AttackOnce();
                }
            }
        }

        var finalMoveSpeed = moveSpeed * equippedWeapon.movementSpeedModifier;
        transform.position += moveVector * Time.fixedDeltaTime * finalMoveSpeed;

        if (attackTarget != null)
        {
            var faceTarget = transform.position - attackTarget.transform.position;
            faceTarget.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, faceTarget, rotationDamping);
        }
        else
        {
            var rotationThreshold = 0.1f;
            if (moveVector.magnitude > rotationThreshold)
                transform.forward = Vector3.Lerp(transform.forward, -moveVector.normalized, rotationDamping);
        }
    }

    void AttackOnce()
    {
        var finalAttackRate = attackRate * equippedWeapon.attackSpeedModifier;
        attackCooldown = 1f / finalAttackRate;
        animator.SetTrigger("Hit");
    }

    Collider[] collider = new Collider[32];

    Collider[] FetchTargets()
    {
        for (var i = 0; i < collider.Length; i++)
            collider[i] = null;

        Physics.OverlapSphereNonAlloc(transform.position, CalculateAttackRange(), collider, LayerMask.GetMask("Enemy"));
        return collider;
    }

    float CalculateAttackRange()
    {
        return attackRange * equippedWeapon.attackRadiusModifier;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}