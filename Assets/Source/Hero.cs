using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public float attackRate = 1f;

    public float rotationDamping = 0.25f;

    public List<Weapon> startingWeapons = new List<Weapon>();

    public Transform weaponSlot;

    public AnimationClip attackAnimation;

    Weapon equippedWeapon;

    float attackCooldown;

    JoystickInput joystick;

    Animator animator;
    HeroAnimationEvents animationEvents;

    Collider attackTarget;

    void Start()
    {
        joystick = Main.Get<JoystickInput>();

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
        var desiredAttackDuration = 1f / CalculateAttackRate();
        animator.SetFloat("AttackSpeedMul", attackAnimation.length / desiredAttackDuration);
        animator.SetFloat("MoveSpeedMul", 1f + (equippedWeapon.movementSpeedModifier - 1f) / 4f);
        animator.SetFloat("MoveInput", joystick.GetMoveVector().magnitude);
    }

    void FixedUpdate()
    {
        attackCooldown -= Time.fixedDeltaTime;

        if (!joystick.IsDown())
        {
            if (attackTarget != null)
            {
                if (attackCooldown < 0)
                {
                    Attacking();
                }
            }
            else
            {
                NotAttacking();
            }
        }
        else
        {
            NotAttacking();
        }

        var moveVector = joystick.GetMoveVector();
        transform.position += moveVector * Time.fixedDeltaTime * CalculateMoveSpeed();

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

    void Attacking()
    {
        attackCooldown = 1f / CalculateAttackRate();
        animator.SetBool("Hit", true);
    }

    void NotAttacking()
    {
        animator.SetBool("Hit", false);
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

    float CalculateMoveSpeed()
    {
        return moveSpeed * equippedWeapon.movementSpeedModifier;
    }

    float CalculateAttackRate()
    {
        return attackRate * equippedWeapon.attackSpeedModifier;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}