using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float speed = 5f;
    public float sensitivity = 0.5f;
    public float motionDamping = 0.9f;
    public float rotationDamping = 0.25f;

    public float attackRange = 1f;
    public float attackRate = 1f;
    
    float attackCooldown;
    
    public float radius = 0;
    Vector3 origin;
    bool input;
    Vector3 moveVector;

    Animator animator;
    HeroAnimationEvents animationEvents;

    Collider attackTarget;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animationEvents = GetComponentInChildren<HeroAnimationEvents>();
        animationEvents.HitDamage += HitAnimation;
        
        InvokeRepeating(nameof(SlowTick), 0f, 1 / 10f);
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
            else if (IsCloserToMeThan(closest, enemy))
                closest = enemy;
        }

        attackTarget = closest;
    }

    bool IsCloserToMeThan(Collider clsst, Collider enemy)
    {
        return Vector3.Distance(clsst.transform.position, transform.position) <
               Vector3.Distance(enemy.transform.position, transform.position);
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

        transform.position += moveVector * Time.fixedDeltaTime * speed;

        if (attackTarget != null)
        {
            var faceTarget = transform.position-attackTarget.transform.position;
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
        attackCooldown = 1f / attackRate;
        animator.SetTrigger("Hit");
    }

    Collider[] collider = new Collider[32];

    Collider[] FetchTargets()
    {
        for (var i = 0; i < collider.Length; i++)
            collider[i] = null;
        
        Physics.OverlapSphereNonAlloc(transform.position, attackRange, collider, LayerMask.GetMask("Enemy"));
        return collider;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}