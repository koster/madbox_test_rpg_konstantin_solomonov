using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Weapon weapon;
    public Unit unit;

    public GameObject alert;

    public Unit engagedTarget;

    List<BehaviourNode> enemyBehaviour;

    void Start()
    {
        alert.SetActive(false);

        unit.OnKilled.AddListener(OnEnemyKilled);
        unit.Equip(weapon);

        enemyBehaviour = new List<BehaviourNode>();
        enemyBehaviour.Add(new EnemyIdling());
        enemyBehaviour.Add(new EnemyEngaging());
        enemyBehaviour.Add(new EnemyChasing());
        enemyBehaviour.Add(new EnemyAttack());

        StartCoroutine(AI());
    }

    IEnumerator AI()
    {
        var n = 0;

        while (true)
        {
            var node = enemyBehaviour[n % enemyBehaviour.Count];

            // skip nodes that are already complete to avoid any kind of flickering
            if (!node.Interrupt(this))
            {
                var nodeExecution = StartCoroutine(node.Execute(this));
                while (!node.Interrupt(this) && !node.IsExecuted())
                {
                    if (unit.IsDead())
                        yield break;

                    yield return new WaitForEndOfFrame();
                }

                StopCoroutine(nodeExecution);
            }

            n++;
        }
    }

    void OnDestroy()
    {
        unit.OnKilled.RemoveListener(OnEnemyKilled);
    }

    void OnEnemyKilled()
    {
        Main.Get<GameEvents>().EnemyKilled?.Invoke(this);
    }
}


public abstract class BehaviourNode
{
    bool isExecuted = false;

    public bool IsExecuted()
    {
        return isExecuted;
    }

    public IEnumerator Execute(Enemy me)
    {
        isExecuted = false;
        yield return Logic(me);
        isExecuted = true;
    }

    protected virtual IEnumerator Logic(Enemy me)
    {
        yield return null;
    }

    public virtual bool Interrupt(Enemy me)
    {
        return false;
    }
}

public class EnemyIdling : BehaviourNode
{
    protected override IEnumerator Logic(Enemy me)
    {
        while (true)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                me.unit.moveDirection = Random.insideUnitSphere;
                me.unit.moveDirection.y = 0;
            }
            else
            {
                me.unit.moveDirection = Vector3.zero;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    public override bool Interrupt(Enemy me)
    {
        return me.unit.IsValidTarget(me.unit.attackTarget) || me.engagedTarget != null;
    }
}

public class EnemyEngaging : BehaviourNode
{
    bool isEngaged;

    protected override IEnumerator Logic(Enemy me)
    {
        me.engagedTarget = me.unit.attackTarget;
        
        me.unit.moveDirection = Vector3.zero;
        me.unit.facingPoint = me.engagedTarget.transform.position;

        me.alert.SetActive(true);
        yield return new WaitForSeconds(1f);
        me.alert.SetActive(false);

        isEngaged = true;
    }

    public override bool Interrupt(Enemy me)
    {
        return isEngaged;
    }
}

public class EnemyChasing : BehaviourNode
{
    protected override IEnumerator Logic(Enemy me)
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            var direction = me.engagedTarget.transform.position - me.transform.position;
            me.unit.moveDirection = direction.normalized;
            me.unit.facingPoint = me.engagedTarget.transform.position;

            yield return waitForFixedUpdate;
        }
    }

    public override bool Interrupt(Enemy me)
    {
        return me.unit.IsValidTarget(me.engagedTarget) && me.unit.IsInAttackRange(me.engagedTarget);
    }
}

public class EnemyAttack : BehaviourNode
{
    protected override IEnumerator Logic(Enemy me)
    {
        var unit = me.unit;

        unit.facingPoint = me.engagedTarget.transform.position;
        unit.moveDirection = Vector3.zero;
        unit.Attacking();

        me.alert.SetActive(true);

        var singleAttackTime = 1f / unit.CalculateAttackRate();
        yield return new WaitForSeconds(singleAttackTime);

        me.alert.SetActive(false);

        unit.NotAttacking();

        yield return new WaitForSeconds(1f);
    }
}