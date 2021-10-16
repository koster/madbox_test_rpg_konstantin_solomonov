using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public UnitData data;

    public int health;

    public Vector3? facingPoint;
    public Vector3 moveDirection;

    public Unit attackTarget;

    public Transform weaponSlot;

    public LayerMask enemyMask;

    public Weapon equippedWeapon;

    public UnityEvent OnKilled;
    public UnityEvent OnHurt;

    public bool isAttacking;

    //

    void Awake()
    {
        health = data.maxHealth;

        gameObject.AddComponent<UnitMotor>().unit = this;
        gameObject.AddComponent<UnitHealth>().unit = this;
        gameObject.AddComponent<UnitWeapon>().unit = this;
        gameObject.AddComponent<UnitAnimator>().unit = this;
    }

    public float CalculateAttackRange()
    {
        return data.attackRange * equippedWeapon.attackRadiusModifier;
    }

    public float CalculateMoveSpeed()
    {
        return data.moveSpeed * equippedWeapon.movementSpeedModifier;
    }

    public float CalculateAttackRate()
    {
        return data.attackRate * equippedWeapon.attackSpeedModifier;
    }

    public bool IsEquipped(Weapon to)
    {
        return equippedWeapon == to;
    }

    public void Equip(Weapon wpn)
    {
        GetComponent<UnitWeapon>().Equip(wpn);
    }

    public void Attacking()
    {
        GetComponent<UnitWeapon>().Attacking();
    }

    public void NotAttacking()
    {
        GetComponent<UnitWeapon>().NotAttacking();
    }

    public bool IsValidTarget(Unit unit)
    {
        return unit != null && unit.IsAlive();
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public bool IsInAttackRange(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position) < CalculateAttackRange();
    }
}

public class UnitHealth : MonoBehaviour
{
    public Unit unit;

    public void Hurt(int damage)
    {
        if (unit.IsDead())
            return;

        unit.health -= damage;

        var damagePoint = transform.position + Vector3.up * unit.data.height / 2f;
        Main.Get<GameEvents>().DamageDealt?.Invoke(damagePoint, damage);

        unit.OnHurt?.Invoke();

        if (unit.health <= 0)
            unit.OnKilled?.Invoke();
    }
}

public class UnitWeapon : MonoBehaviour
{
    public Unit unit;

    UnitAnimationEvents animationEvents;
    Weapon equippedWeaponInstance;

    void Start()
    {
        animationEvents = GetComponentInChildren<UnitAnimationEvents>();
        animationEvents.HitDamage += OnDamageMomentDuringHitAnimation;

        InvokeRepeating(nameof(SlowTick), Random.Range(0f, 1f), 1 / 10f);
    }

    void OnDestroy()
    {
        animationEvents.HitDamage -= OnDamageMomentDuringHitAnimation;
    }

    public void Equip(Weapon weapon)
    {
        unit.equippedWeapon = weapon;

        if (equippedWeaponInstance != null)
            Destroy(equippedWeaponInstance.gameObject);

        equippedWeaponInstance = Instantiate(weapon, unit.weaponSlot);
    }

    public void OnDamageMomentDuringHitAnimation()
    {
        if (unit.attackTarget == null)
            return;

        var delta = unit.attackTarget.transform.position - transform.position;
        var angle = Vector3.Angle(transform.forward, delta);
        var distance = delta.magnitude;

        if (Mathf.Abs(angle) < unit.equippedWeapon.arc && distance < unit.CalculateAttackRange())
            unit.attackTarget.GetComponent<UnitHealth>().Hurt(unit.equippedWeapon.damage);
    }

    public void Attacking()
    {
        unit.isAttacking = true;
    }

    public void NotAttacking()
    {
        unit.isAttacking = false;
    }

    void SlowTick()
    {
        if (unit.IsDead())
        {
            unit.attackTarget = null;
            return;
        }

        UpdateClosestTarget();
    }

    void UpdateClosestTarget()
    {
        var enemies = FetchTargets();

        Unit closest = null;
        foreach (var collider in enemies)
        {
            var unitTarget = collider?.GetComponent<Unit>();

            if (!unit.IsValidTarget(unitTarget))
                break;

            if (closest == null)
                closest = unitTarget;
            else if (IsCloserToMeThan(closest.transform, collider.transform))
                closest = unitTarget;
        }

        unit.attackTarget = closest;
    }

    Collider[] colliders = new Collider[32];

    Collider[] FetchTargets()
    {
        for (var i = 0; i < colliders.Length; i++)
            colliders[i] = null;

        Physics.OverlapSphereNonAlloc(transform.position, unit.CalculateAttackRange(), colliders, unit.enemyMask);
        return colliders;
    }

    bool IsCloserToMeThan(Transform clsst, Transform enemy)
    {
        return Vector3.Distance(clsst.position, transform.position) <
               Vector3.Distance(enemy.position, transform.position);
    }
}

public class UnitAnimator : MonoBehaviour
{
    public Unit unit;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        unit.OnHurt.AddListener(UnitHurt);
        unit.OnKilled.AddListener(UnitKilled);
    }

    void UnitHurt()
    {
        animator.PlayInFixedTime("Hurt", animator.GetLayerIndex("HurtLayer"), 0);
    }

    void UnitKilled()
    {
        animator.SetBool("Dead", true);
        animator.SetBool("Attacking", false);
    }

    void Update()
    {
        if (unit.IsDead())
            return;

        var desiredAttackDuration = 1f / unit.CalculateAttackRate();
        animator.SetFloat("AttackSpeedMul", unit.data.attackAnimation.length / desiredAttackDuration);
        animator.SetFloat("MoveSpeedMul", 1f + (unit.equippedWeapon.movementSpeedModifier - 1f) / 3f);
        animator.SetFloat("MoveInput", unit.moveDirection.magnitude);
        animator.SetBool("Attacking", unit.isAttacking);
    }

    void FixedUpdate()
    {
        if (unit.IsDead())
            return;

        if (unit.facingPoint.HasValue)
        {
            var faceTarget = unit.facingPoint.Value - transform.position;
            faceTarget.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, faceTarget, unit.data.rotationDamping);
        }
        else
        {
            var rotationThreshold = 0.1f;
            if (unit.moveDirection.magnitude > rotationThreshold)
                transform.forward = Vector3.Lerp(transform.forward, unit.moveDirection.normalized, unit.data.rotationDamping);
        }
    }
}

public class UnitMotor : MonoBehaviour
{
    public Unit unit;

    void FixedUpdate()
    {
        if (unit.IsDead())
            return;

        transform.position += unit.moveDirection * Time.fixedDeltaTime * unit.CalculateMoveSpeed();
    }
}