using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; //시작 체력
    public float health {  get; set; } //현재 체력
    public bool dead { get; private set; } //사망 상태

    public event Action onDeath; //사망시 발동할 이벤트

    public int power = 10;

    //일정 시간 동안 데미지를 입지 않았을 때 hp 회복을 위한 변수
    protected float lastDamageTime = 0f;
    protected float regenerationInterval = 5f; // 1분 (60초)

    [SerializeField] protected Animator animator;
    [SerializeField] protected Image hpBar;
    
    protected readonly int hashDamage = Animator.StringToHash("Damage");
    protected readonly int hashDie = Animator.StringToHash("Die");

   

    //생명체가 활성화될 때 상태 리셋
    protected virtual void OnEnable()
    {
        //사망하지 않은 상태로 시작
        dead = false;
        //체력을 시작 체력으로 초기화
        health = startingHealth;
        animator = GetComponent<Animator>();
        lastDamageTime = Time.time; // 활성화 시 초기화
    }

    public virtual void FixedUpdate()
    {
        if (GameManager.Instance.GameClear == true || GameManager.Instance.IsGameover == true) return;
        // HP 자동 회복 로직
        if (Time.time - lastDamageTime >= regenerationInterval && health < startingHealth)
        {
            Debug.Log($"{gameObject.name} Hp회복합니다.");
            RecoverHP();
        }
    }
    public void RecoverHP()
    {
        lastDamageTime = Time.time; // 데미지 타임 초기화
        health+=5;
        health = Mathf.Clamp(health, 0, startingHealth);
        hpBar.fillAmount = (float)health / startingHealth;
    }

    //데미지를 잃는 메서드
    public virtual void OnDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, startingHealth);
        lastDamageTime = Time.time; // 데미지 받은 시간 업데이트
        if (health <= 0 && !dead)
        {
            Debug.Log($"{gameObject.name} 죽음!");
            Die();
        }
    }

    //사망 처리
    public virtual void Die()
    {
        //onDeath 이벤트에 등록된 메서드가 있다면 실행한다.
        if(onDeath != null)
        {
            onDeath();
        }
        //사망 상태를 참으로 변경한다.
        dead = true;
    }
    protected void DisableCharacter()
    {
        gameObject.SetActive(false);
    }

    protected void UpdateHpBar()
    {
        hpBar.fillAmount = health / startingHealth;
    }
}
