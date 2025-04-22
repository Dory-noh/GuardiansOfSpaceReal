using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

//�÷��̾� ĳ������ ����ü�μ��� ������ ����Ѵ�.
public class PlayerHealth : LivingEntity
{
    public Vector3 originPos;
    private PlayerInput PlayerInput;
    private PlayerMovement playerMovement;

    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        originPos = transform.position;
        playerMovement = GetComponent<PlayerMovement>();
        PlayerInput = GetComponent<PlayerInput>();
        power = 20;
    }

    protected override void OnEnable()
    {
        base.OnEnable(); //dead�� false��, hp���� 100���� ����.

        UIManager.Instance.ResetPlayerHpBar();

        //�÷��̾��� ���۰� ������ �޴� ������Ʈ�� Ȱ��ȭ
        playerMovement.enabled = true;
        PlayerInput.enabled = true;
    }


    //������ ó��
    public override void OnDamage(float damage)
    {
        if (!dead) //������ �Ծ��� �� �۵� �ڵ�
        {
            animator.SetTrigger(hashDamage);
            animator.SetFloat(hashSpeed, 0f);
        }
        //Living Entity�� OnDamage() ����
        base.OnDamage(damage);
        //���ŵ� ü���� ü�� �����̴��� �ݿ��Ѵ�.
        UIManager.Instance.healthSlider.value = health;
    }

    public override void Die()
    {
        base.Die();

        //��� �ִ� ���
        animator.SetTrigger(hashDie);
        animator.SetFloat(hashSpeed, 0f);
        //�÷��̾��� ������ �޴� ������Ʈ, �Է� ��ũ��Ʈ ��Ȱ��ȭ
        PlayerInput.enabled = false;
        playerMovement.enabled = false;
        GameManager.Instance.IsGameover = true;
        UIManager.Instance.ToggleHelpUI(1, true);
        Invoke("DisableCharacter", 3f);
        //3�� �� �ٽ� �������ȴ�.
        //Invoke("Respawn", 3f);
    }


    public void Respawn()
    {
        transform.position = originPos;
        gameObject.SetActive(true);
        PlayerInput.enabled = true;
        playerMovement.enabled = true;
    }
}
