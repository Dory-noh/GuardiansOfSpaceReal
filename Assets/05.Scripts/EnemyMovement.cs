using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public enum EnemyState
{
    IDLE,
    TRACE,
    ATTACK,
    DAMAGE,
    DIE
}

public class EnemyMovement : LivingEntity, IEnemy
{
    public EnemyState state = EnemyState.IDLE;
    public GameObject key;
    public NavMeshAgent agent;
    public Transform target;
    public float traceDistance = 30f;
    public float attackDistance = 3f;
    public Vector3 originPos;

    public float attackTime = 1.5f;
    private bool canAttack = true;

    private int hashIsMove = Animator.StringToHash("IsMove");
    private int hashPunch = Animator.StringToHash("Punch");

    public Animator[] animators;

    void Start()
    {
        originPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
        animators = GetComponentsInChildren<Animator>();
        power = 1;
        attackTime = 3f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        state = EnemyState.IDLE;
        UpdateHpBar();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameover) return;
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            state = EnemyState.ATTACK;
        }
        else if (Vector3.Distance(transform.position, target.position) <= traceDistance)
        {
            state = EnemyState.TRACE;
        }
        else
        {
            state = EnemyState.IDLE;
        }

        switch (state)
        {
            case EnemyState.IDLE:
                agent.destination = originPos;
                foreach (Animator anim in animators)
                {
                    if (agent.remainingDistance < 3f)
                    {
                        anim.SetBool(hashIsMove, false);
                        agent.isStopped = true;
                    }
                    //원래 자리로 돌아갈 때 걷는 애니메이션 실행되도록 함.
                    else
                    {
                        anim.SetBool(hashIsMove, true);
                        agent.isStopped = false;
                    }
                }
                break;
            case EnemyState.TRACE:
                agent.isStopped = false;
                agent.destination = target.position;
                foreach (Animator anim in animators) anim.SetBool(hashIsMove, true);
                break;
            case EnemyState.DAMAGE:
                agent.isStopped = true;
                transform.LookAt(target.position);
                foreach (Animator anim in animators)
                {
                    anim.SetBool(hashIsMove, false);
                    anim.SetTrigger(hashDamage);
                }
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Attack()
    {
        agent.isStopped = true;
        agent.destination = target.position;
        transform.LookAt(target.position);
        if (canAttack)
        {
            foreach (Animator anim in animators)
            {
                anim.SetBool(hashIsMove, false);
                anim.SetTrigger(hashPunch);
            }
            canAttack = false;
            StartCoroutine(EnableAttack());
        }
    }

    IEnumerator EnableAttack()
    {
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        UpdateHpBar();
        state = EnemyState.DAMAGE;
    }

    public override void Die()
    {
        base.Die();
        state = EnemyState.DIE;
        agent.isStopped = true;
        foreach (Animator anim in animators)
        {
            anim.SetBool(hashIsMove, false);
            anim.SetTrigger(hashDie);
        }
        Invoke("ChangeKeysParent", 1f);
        Invoke("DisableCharacter", 3f);
    }

    private void ChangeKeysParent()
    {
        if (key != null)
        {
            GameObject keyParent = new GameObject("keyParent");
            keyParent.transform.position = new Vector3(gameObject.transform.position.x, -6f, gameObject.transform.position.z);
            Debug.Log("열쇠 부모 변경");
            // Key 오브젝트의 부모를 MainScene으로 변경
            key.transform.SetParent(keyParent.transform);

            // Key 오브젝트의 로컬 위치 및 회전을 초기화
            key.transform.localPosition = new Vector3(0, 1.5f, 0);
            key.transform.localRotation = Quaternion.identity;
        }
    }

    private void OnDisable()
    {
       
    }
}
