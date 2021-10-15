using UnityEngine;
using UnityEngine.Events;

public enum UnitState
{
    ALIVE,
    DEAD
}

public class Unit : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public float attackRate = 1f;

    public float rotationDamping = 0.25f;

    public int maxHealth;
    public float height = 1f;

    public Transform weaponSlot;

    public AnimationClip attackAnimation;
    public UnityEvent OnKilled;
    public LayerMask enemyMask;

    Weapon equippedWeapon;
    Weapon equippedWeaponInstance;

    float attackCooldown;

    Animator animator;
    UnitAnimationEvents animationEvents;

    Vector3 moveDirection;

    UnitState state;

    int health;
    Unit attackTarget;

    void Start()
    {
        health = maxHealth;
        state = UnitState.ALIVE;

        animator = GetComponentInChildren<Animator>();

        animationEvents = GetComponentInChildren<UnitAnimationEvents>();
        animationEvents.HitDamage += HitAnimation;

        InvokeRepeating(nameof(SlowTick), 0f, 1 / 10f);
    }

    Collider[] collider = new Collider[32];

    Collider[] FetchTargets()
    {
        for (var i = 0; i < collider.Length; i++)
            collider[i] = null;

        Physics.OverlapSphereNonAlloc(transform.position, CalculateAttackRange(), collider, enemyMask);
        return collider;
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

        attackTarget = closest?.GetComponent<Unit>();
    }

    bool IsCloserToMeThan(Transform clsst, Transform enemy)
    {
        return Vector3.Distance(clsst.position, transform.position) <
               Vector3.Distance(enemy.position, transform.position);
    }

    public UnitState GetState()
    {
        return state;
    }

    public int GetHealth()
    {
        return health;
    }

    public void Hurt(int damage)
    {
        if (state == UnitState.DEAD)
            return;

        health -= damage;

        var damagePoint = transform.position + Vector3.up * height / 2f;
        Main.Get<GameEvents>().DamageDealt?.Invoke(damagePoint, damage);
        animator.PlayInFixedTime("Hurt", animator.GetLayerIndex("HurtLayer"), 0);

        if (health <= 0)
        {
            OnKilled?.Invoke();
            state = UnitState.DEAD;
            animator.SetBool("Dead", true);
            animator.SetBool("Attacking", false);
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void Equip(Weapon weapon)
    {
        equippedWeapon = weapon;

        if (equippedWeaponInstance != null)
            Destroy(equippedWeaponInstance.gameObject);

        equippedWeaponInstance = Instantiate(weapon, weaponSlot);
    }

    void OnDestroy()
    {
        animationEvents.HitDamage -= HitAnimation;
    }

    public void HitAnimation()
    {
        if (attackTarget == null)
            return;

        attackTarget.Hurt(equippedWeaponInstance.damage);
    }

    void Update()
    {
        var desiredAttackDuration = 1f / CalculateAttackRate();
        animator.SetFloat("AttackSpeedMul", attackAnimation.length / desiredAttackDuration);
        animator.SetFloat("MoveSpeedMul", 1f + (equippedWeaponInstance.movementSpeedModifier - 1f) / 4f);
        animator.SetFloat("MoveInput", moveDirection.magnitude);
    }

    void FixedUpdate()
    {
        if (state == UnitState.DEAD)
            return;

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
            var faceTarget = attackTarget.transform.position - transform.position;
            faceTarget.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, faceTarget, rotationDamping);
        }
        else
        {
            var rotationThreshold = 0.1f;
            if (moveDirection.magnitude > rotationThreshold)
                transform.forward = Vector3.Lerp(transform.forward, moveDirection.normalized, rotationDamping);
        }
    }

    bool IsValidTarget(Unit unit)
    {
        return unit != null && unit.IsAlive();
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    void Attacking()
    {
        attackCooldown = 1f / CalculateAttackRate();
        animator.SetBool("Attacking", true);
    }

    void NotAttacking()
    {
        animator.SetBool("Attacking", false);
    }

    public float CalculateAttackRange()
    {
        return attackRange * equippedWeaponInstance.attackRadiusModifier;
    }

    float CalculateMoveSpeed()
    {
        return moveSpeed * equippedWeaponInstance.movementSpeedModifier;
    }

    float CalculateAttackRate()
    {
        return attackRate * equippedWeaponInstance.attackSpeedModifier;
    }

    public bool IsEquipped(Weapon to)
    {
        return equippedWeapon == to;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}