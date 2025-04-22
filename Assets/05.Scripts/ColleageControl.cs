using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public enum ColleageState
{
    IDLE,
    PLAYERTRACE,
    ENEMYTRACE,
    ATTACK,
    DAMAGE,
    DIE
}
public class ColleageControl : LivingEntity, IPlayer
{
    public NavMeshAgent agent;

    public GameObject player;
    public GameObject target;

    public ColleageState state;
    public float Movespeed = 6f;
    public float stopDistance = 4f;
    public float enemyTraceDistance = 6f;
    public float playerTraceDistance = 10f;
    public float attackTime = 1.5f;
    private bool canAttack = true;

    private readonly int hashIsMove = Animator.StringToHash("IsMove");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //target = GameObject.FindWithTag("Enemy"); // �� �̻� Ư�� Enemy�� ã�� ����
        agent = GetComponent<NavMeshAgent>();
        agent.destination = player.transform.position;
        agent.stoppingDistance = stopDistance;
        agent.speed = Movespeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        state = ColleageState.IDLE;
        UpdateHpBar();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameover) return;
        FindClosestEnemy(); // �� �����Ӹ��� ���� ����� ���� ã���� ������Ʈ

        if (player == null) return; // �÷��̾ ������ �� �̻� �������� ����
        if (target == null) // ���� ����� ���� ������ �÷��̾� ����
        {
            state = ColleageState.PLAYERTRACE;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerTraceDistance) //�÷��̾���� �Ÿ��� playerTraceDistance �̳��̰�,
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance) //enemy���� �Ÿ��� stopDistance �̳��� enemy ���� ����
            {
                state = ColleageState.ATTACK;
            }
            else if (Vector3.Distance(transform.position, target.transform.position) <= enemyTraceDistance) //enemy���� �Ÿ��� enemyTraceDistance �̳��� enemy ����
            {
                state = ColleageState.ENEMYTRACE;
            }
            else //enemy�� �ָ� �ְ�, player���� �Ÿ��� stopDistance �̳��� idle ����
            {
                if (Vector3.Distance(transform.position, player.transform.position) >= stopDistance)
                {
                    state = ColleageState.PLAYERTRACE;
                }
                else state = ColleageState.IDLE;
            }

        }
        else //�÷��̾���� �Ÿ��� playerTraceDistance�� �ʰ��ϴ� ���
        {
            //�÷��̾� ����.
            state = ColleageState.PLAYERTRACE;
        }

        switch (state)
        {
            case ColleageState.IDLE:
                agent.destination = player.transform.position;
                animator.ResetTrigger(hashAttack);
                animator.SetBool(hashIsMove, false);
                agent.isStopped = true;
                break;

            case ColleageState.PLAYERTRACE:
                agent.destination = player.transform.position;
                animator.SetBool(hashIsMove, true);
                agent.isStopped = false;
                break;

            case ColleageState.ENEMYTRACE:
                if (target != null)
                {
                    agent.destination = target.transform.position;
                    animator.SetBool(hashIsMove, true);
                    agent.isStopped = false;
                }
                else
                {
                    state = ColleageState.PLAYERTRACE; // ���� �������� �÷��̾� ����
                }
                break;

            case ColleageState.ATTACK:
                if (target != null)
                {
                    agent.destination = target.transform.position;
                    agent.isStopped = true;
                    animator.SetBool(hashIsMove, false);
                    if (canAttack == true)
                    {
                        animator.SetTrigger(hashAttack);
                        canAttack = false;
                        StartCoroutine(EnableAttack());
                    }
                    
                    
                }
                else
                {
                    state = ColleageState.PLAYERTRACE; // ���� �������� �÷��̾� ����
                }
                break;
            case ColleageState.DAMAGE:
                animator.SetBool(hashIsMove, false);
                animator.SetTrigger(hashDamage);
                agent.isStopped = true;
                break;
        }
    }

    IEnumerator EnableAttack()
    {
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }

    // ���� ����� ���� ã�� �Լ�
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in enemies)
        {
            float distanceToTarget = Vector3.Distance(currentPosition, potentialTarget.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestEnemy = potentialTarget;
            }
        }

        target = closestEnemy;
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        UpdateHpBar();
        state = ColleageState.DAMAGE;
    }

    public override void Die()
    {
        base.Die();

        agent.isStopped = true;
        animator.SetBool(hashIsMove, false);
        animator.SetTrigger(hashDie);
        state = ColleageState.DIE;
        Invoke("DisableCharacter", 3f);
    }
}