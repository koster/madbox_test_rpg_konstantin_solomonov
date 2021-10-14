using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public float attackRate = 1f;

    public float rotationDamping = 0.25f;

    public Transform weaponSlot;

    public AnimationClip attackAnimation;

    Weapon equippedWeapon;

    float attackCooldown;
    
    Animator animator;
    UnitAnimationEvents animationEvents;

    Enemy attackTarget;
    Vector3 moveDirection;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        animationEvents = GetComponentInChildren<UnitAnimationEvents>();
        animationEvents.HitDamage += HitAnimation;

        InvokeRepeating(nameof(SlowTick), 0f, 1 / 10f);
    }

    public void Equip(Weapon weapon)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);
        
        equippedWeapon = Instantiate(weapon, weaponSlot);
    }

    void OnDestroy()
    {
        animationEvents.HitDamage -= HitAnimation;
    }

    public void HitAnimation()
    {
        if (attackTarget == null)
            return;

        attackTarget.Hurt(equippedWeapon.damage);
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

        attackTarget = closest?.GetComponent<Enemy>();
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
        animator.SetFloat("MoveInput", moveDirection.magnitude);
    }

    void FixedUpdate()
    {
        attackCooldown -= Time.fixedDeltaTime;

        if (!Main.Get<JoystickInput>().IsDown())
        {
            if (IsValidTarget(attackTarget))
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

        transform.position += moveDirection * Time.fixedDeltaTime * CalculateMoveSpeed();

        if (IsValidTarget(attackTarget))
        {
            var faceTarget = transform.position - attackTarget.transform.position;
            faceTarget.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, faceTarget, rotationDamping);
        }
        else
        {
            var rotationThreshold = 0.1f;
            if (moveDirection.magnitude > rotationThreshold)
                transform.forward = Vector3.Lerp(transform.forward, -moveDirection.normalized, rotationDamping);
        }
    }

    bool IsValidTarget(Enemy enemy)
    {
        return enemy != null && enemy.IsAlive();
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
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