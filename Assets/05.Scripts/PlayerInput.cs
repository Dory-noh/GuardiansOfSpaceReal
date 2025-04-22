using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerMovement movement;
    public float posX { get; private set; }
    public float posY { get; private set; }
    public float rotate { get; private set; }
    public bool fire = false;
    public bool reload = false;
    public bool jump = false;
    public bool isRun = false;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) isRun = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isRun = false;
    }
    void OnMove(InputValue value)
    {
        //Debug.Log("Move");
        Vector2 pos = value.Get<Vector2>();
        posX = pos.x;
        posY = pos.y;
    }

    void OnLook(InputValue value)
    {
       // Debug.Log("RotateToMouseX");
        Vector2 dir = value.Get<Vector2>();
        rotate = dir.x;
    }

    void OnJump(InputValue value)
    {
        //Debug.Log("Jump");
        if(jump == false)
        {
            jump = true;
            movement.Jump();
            StartCoroutine(isJumpReset());

        }
    }

    IEnumerator isJumpReset()
    {
        yield return null;
        jump = false;
    }
    //void OnRotate(InputValue value)
    //{
    //    Debug.Log("Rotate");
    //    Vector2 dir = value.Get<Vector2>();
    //    rotate = dir.x;
    //}

    void OnFire2(InputValue value)
    {
        //Debug.Log("Fire1");
        if(fire == false)
        {
            fire = true;
            StartCoroutine(isFireReset());
            movement.Attack();
        }
    }

    IEnumerator isFireReset()
    {
        yield return null;
        fire = false;
    }

    //void OnFire2(InputValue value)
    //{
    //    //Debug.Log("Fire1");
    //    if (fire == false)
    //    {
    //        fire = true;
    //        StartCoroutine(isFireReset());
    //        movement.Attack2();
    //    }
    //}

    void OnReload(InputValue value)
    {
        //Debug.Log("Reload");
        reload = true;
    }

    IEnumerator isReloadReset()
    {
        yield return null;
        reload = false;
    }
}

