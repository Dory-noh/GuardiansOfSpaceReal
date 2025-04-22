using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

//플레이어 캐릭터의 생명체로서의 동작을 담당한다.
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
        base.OnEnable(); //dead를 false로, hp값을 100으로 만듦.

        UIManager.Instance.ResetPlayerHpBar();

        //플레이어의 조작과 동작을 받는 컴포넌트들 활성화
        playerMovement.enabled = true;
        PlayerInput.enabled = true;
    }


    //데미지 처리
    public override void OnDamage(float damage)
    {
        if (!dead) //데미지 입었을 때 작동 코드
        {
            animator.SetTrigger(hashDamage);
            animator.SetFloat(hashSpeed, 0f);
        }
        //Living Entity의 OnDamage() 실행
        base.OnDamage(damage);
        //갱신된 체력을 체력 슬라이더에 반영한다.
        UIManager.Instance.healthSlider.value = health;
    }

    public override void Die()
    {
        base.Die();

        //사망 애니 재생
        animator.SetTrigger(hashDie);
        animator.SetFloat(hashSpeed, 0f);
        //플레이어의 조작을 받는 컴포넌트, 입력 스크립트 비활성화
        PlayerInput.enabled = false;
        playerMovement.enabled = false;
        GameManager.Instance.IsGameover = true;
        UIManager.Instance.ToggleHelpUI(1, true);
        Invoke("DisableCharacter", 3f);
        //3초 후 다시 리스폰된다.
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
