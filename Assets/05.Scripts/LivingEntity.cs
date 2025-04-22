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
    public float startingHealth = 100f; //���� ü��
    public float health {  get; set; } //���� ü��
    public bool dead { get; private set; } //��� ����

    public event Action onDeath; //����� �ߵ��� �̺�Ʈ

    public int power = 10;

    //���� �ð� ���� �������� ���� �ʾ��� �� hp ȸ���� ���� ����
    protected float lastDamageTime = 0f;
    protected float regenerationInterval = 5f; // 1�� (60��)

    [SerializeField] protected Animator animator;
    [SerializeField] protected Image hpBar;
    
    protected readonly int hashDamage = Animator.StringToHash("Damage");
    protected readonly int hashDie = Animator.StringToHash("Die");

   

    //����ü�� Ȱ��ȭ�� �� ���� ����
    protected virtual void OnEnable()
    {
        //������� ���� ���·� ����
        dead = false;
        //ü���� ���� ü������ �ʱ�ȭ
        health = startingHealth;
        animator = GetComponent<Animator>();
        lastDamageTime = Time.time; // Ȱ��ȭ �� �ʱ�ȭ
    }

    public virtual void FixedUpdate()
    {
        if (GameManager.Instance.GameClear == true || GameManager.Instance.IsGameover == true) return;
        // HP �ڵ� ȸ�� ����
        if (Time.time - lastDamageTime >= regenerationInterval && health < startingHealth)
        {
            Debug.Log($"{gameObject.name} Hpȸ���մϴ�.");
            RecoverHP();
        }
    }
    public void RecoverHP()
    {
        lastDamageTime = Time.time; // ������ Ÿ�� �ʱ�ȭ
        health+=5;
        health = Mathf.Clamp(health, 0, startingHealth);
        hpBar.fillAmount = (float)health / startingHealth;
    }

    //�������� �Ҵ� �޼���
    public virtual void OnDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, startingHealth);
        lastDamageTime = Time.time; // ������ ���� �ð� ������Ʈ
        if (health <= 0 && !dead)
        {
            Debug.Log($"{gameObject.name} ����!");
            Die();
        }
    }

    //��� ó��
    public virtual void Die()
    {
        //onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� �����Ѵ�.
        if(onDeath != null)
        {
            onDeath();
        }
        //��� ���¸� ������ �����Ѵ�.
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
