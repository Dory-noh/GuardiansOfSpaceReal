using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayer
{
    public enum WeaponMode
    {
        NONE,
        GUN
    }
    public WeaponMode m_WeaponMode;
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float moveSpeed = 0f;
    public float rotateSpeed = 60f;
    public float gravity = -20f; // 중력 값. 값을 크게 해서 허공에 떠 있는 시간을 줄인다.
    Vector3 moveDirection;
    //float damping = 5f;
    private Vector3 velocity;
    public bool isMoving = false;
    PlayerInput input;
    public Vector3 dir;
    bool isJump = false;
    bool isStop = false;
    public bool isGun = true;
    private Vector3 jumpEndPosition; //점프 종료 위치 저장
    //private Rigidbody rb;
    private CharacterController cc;
    private Animator animator;
    public GameObject Gun;
    public Transform gunRightHandAttachment;
    public Transform gunBackAttachment;
    IItem item;
    CinemachineVirtualCamera virtualCamera;
    CinemachineTransposer VCTransposer;

    // 부드러운 전환을 위한 변수
    private float currentOffsetX = 0f;
    private float targetOffsetX = 0f;
    public float offsetSmoothTime = 0.1f; // 전환에 걸리는 시간 (초)
    private float velocityOffsetX;

    private int hashPosX = Animator.StringToHash("PosX");
    private int hashPosY = Animator.StringToHash("PosY");
    private int hashSpeed = Animator.StringToHash("Speed");
    private int hashStopJump = Animator.StringToHash("StopJump");
    private int hashMoveJump = Animator.StringToHash("MoveJump");
    private int hashAttack = Animator.StringToHash("Attack");
    private int hashAttack2 = Animator.StringToHash("Attack2");
    private int hashIsGun = Animator.StringToHash("IsGun");

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            VCTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (VCTransposer != null)
            {
                currentOffsetX = VCTransposer.m_FollowOffset.x; // 초기 값 설정
            }
        }
        else
        {
            Debug.LogError("Virtual Camera를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //아이템과 충돌한 경우 해당 아이템을 가방에 넣는다.
        item = other.GetComponent<IItem>();
    }

    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    GameManager.Instance.GameClear = true;
        //}

        if (GameManager.Instance.IsGameover) return;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            isGun = !isGun;
        }
        GetItem();

        if (isGun) m_WeaponMode = WeaponMode.GUN;
        else m_WeaponMode = WeaponMode.NONE;


        switch (m_WeaponMode)
        {
            case WeaponMode.NONE:
                targetOffsetX = 0f;
                Gun.transform.position = gunBackAttachment.transform.position;
                Gun.transform.rotation = gunBackAttachment.rotation;
                animator.SetBool(hashIsGun, isGun);
                break;
            case WeaponMode.GUN:
                targetOffsetX = 1f;
                Gun.transform.position = gunRightHandAttachment.transform.position;
                Gun.transform.rotation = gunRightHandAttachment.rotation;
                animator.SetBool(hashIsGun, isGun);
                break;
        }

        // 부드러운 Offset X 값 전환
        if (VCTransposer != null)
        {
            currentOffsetX = Mathf.SmoothDamp(currentOffsetX, targetOffsetX, ref velocityOffsetX, offsetSmoothTime);
            VCTransposer.m_FollowOffset = new Vector3(currentOffsetX, VCTransposer.m_FollowOffset.y, VCTransposer.m_FollowOffset.z);
        }

        if (isStop == false)
        {
            isMoving = (input.posX == 0 && input.posY == 0) ? false : true;
            moveSpeed = isMoving ? (input.isRun ? runSpeed : walkSpeed) : 0f;
            dir = new Vector3(input.posX, 0f, input.posY);
            Rotate();
            Move();
            animator.SetFloat(hashPosX, input.posX);
            animator.SetFloat(hashPosY, input.posY);
            animator.SetFloat(hashSpeed, moveSpeed);
        }
    }

    private void GetItem()
    {
        if (UIManager.Instance.UI[0].activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            //item로 부터 IItem 가저오는데 성공했다면(item이 null이 아니라면)
            if (item != null)
            {
                item.Use(gameObject);
            }
            //소리 재생
        }

    }

    private void OnAnimatorMove()
    {
        if (isJump)
        {
            Vector3 rootPosition = animator.rootPosition;
            Vector3 deltaPosition = rootPosition - transform.position;
            velocity.y = deltaPosition.y;
            cc.Move(new Vector3(deltaPosition.x, 0f, deltaPosition.z));
        }
    }

    private void Rotate()
    {
        //Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y * input.rotate, 0);
        transform.rotation *= Quaternion.Euler(0, input.rotate * rotateSpeed * Time.deltaTime, 0);
    }
    private void Move()
    {
        moveDirection = (input.posX * transform.right + input.posY * transform.forward).normalized;
        //rb.MovePosition(rb.position + moveDistance);
        //if (isJump) return;
        if (cc.isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        cc.Move(moveDirection * Time.deltaTime * moveSpeed + velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (cc.isGrounded == false || isJump == true) return;
        isJump = true;
        if (!isMoving)
        {
            animator.SetTrigger(hashStopJump);
            isStop = true;
        }
        else
        {
            animator.SetTrigger(hashMoveJump);
        }
    }

    public void ResetMoveJump()
    {
        isJump = false;
    }

    public void Attack()
    {
        if (isStop == false)
        {
            isStop = true;
            animator.SetTrigger(hashAttack);
            animator.SetFloat(hashSpeed, 0f);
            if (isGun == true)
            {
                Gun.GetComponent<LaserGun>().Shoot();
            }
        }
    }
    //public void Attack2()
    //{
    //    Debug.Log("발사");
    //    if (isStop == false)
    //    {
    //        isStop = true;
    //        animator.SetTrigger(hashAttack2);
    //    }
    //}

    public void AllowMove() //공격 끝나면 isAttack false로 만드는 메서드
    {
        isStop = false;
        if (isJump == true) isJump = false;
    }
}