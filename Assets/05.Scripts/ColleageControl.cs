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
        //target = GameObject.FindWithTag("Enemy"); // 더 이상 특정 Enemy를 찾지 않음
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
        FindClosestEnemy(); // 매 프레임마다 가장 가까운 적을 찾도록 업데이트

        if (player == null) return; // 플레이어가 없으면 더 이상 진행하지 않음
        if (target == null) // 가장 가까운 적이 없으면 플레이어 추적
        {
            state = ColleageState.PLAYERTRACE;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerTraceDistance) //플레이어와의 거리가 playerTraceDistance 이내이고,
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance) //enemy와의 거리가 stopDistance 이내면 enemy 공격 시작
            {
                state = ColleageState.ATTACK;
            }
            else if (Vector3.Distance(transform.position, target.transform.position) <= enemyTraceDistance) //enemy와의 거리가 enemyTraceDistance 이내면 enemy 추적
            {
                state = ColleageState.ENEMYTRACE;
            }
            else //enemy가 멀리 있고, player와의 거리가 stopDistance 이내면 idle 상태
            {
                if (Vector3.Distance(transform.position, player.transform.position) >= stopDistance)
                {
                    state = ColleageState.PLAYERTRACE;
                }
                else state = ColleageState.IDLE;
            }

        }
        else //플레이어와의 거리가 playerTraceDistance를 초과하는 경우
        {
            //플레이어 따라감.
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
                    state = ColleageState.PLAYERTRACE; // 적이 없어지면 플레이어 추적
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
                    state = ColleageState.PLAYERTRACE; // 적이 없어지면 플레이어 추적
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

    // 가장 가까운 적을 찾는 함수
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