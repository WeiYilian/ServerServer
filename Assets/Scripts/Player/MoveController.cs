using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController
{
    private Animator animator;

    private CharacterController characterController;
   
    private CameraController photographer;
    
    private Transform followingTarget;
    
    private PlayerConctroller PlayerConctroller;
    
    public MoveController()
    {
        PlayerConctroller = PlayerConctroller.Instance;
        
        characterController = PlayerConctroller.characterController;
        animator = PlayerConctroller.animator;
        photographer = PlayerConctroller.photographer;
        followingTarget = PlayerConctroller.followingTarget;

        photographer.InitCamera(followingTarget);
    }
    
    
    #region 基础移动参数

    [Header("基础移动参数")]
    //基本速度
    private float speed;
    //行走的速度
    private float mWlkSpeed = 2f; 
    //奔跑时的速度
    private float mRunSpeed = 4f;
    //翻滚速度
    private float mRollSpeed = 6f;
    //跳起来的速度
    private float jumpSpeed = 10f;
    //跳跃时间
    private float jumpTime;
    //设置重力的大小
    private float gravity = 10.0f;
    //镜头旋转角度
    private Quaternion rot;
    //镜头旋转的速度
    private float rotateSpeed = 5.0f;
    //以玩家自身为坐标系的移动，用以移动动画的播放
    private Vector3 animationMove = Vector3.zero;
    //用以承载角色移动向量
    private Vector3 moveDirection = Vector3.zero;
   
    //判断是否在奔跑
    public bool isRun;
    //判断是否在翻滚
    public bool isRoll;

    #endregion

    #region 玩家基础移动

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 玩家行为
    /// </summary>
    public void PlayerAction()
    {
        //如果挨打，直接返回
        if (PlayerConctroller.IsHit) return;

        //改变移动速度
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
            speed = mRollSpeed;
        if (isRun)
        {
            if (Math.Abs(speed - 4) > 0)
            {
                speed = Mathf.Lerp(speed, mRunSpeed, Time.deltaTime);
            }
        }
        else
        {
            if (Math.Abs(speed - 2) > 0)
            {
                speed = Mathf.Lerp(speed, mWlkSpeed, Time.deltaTime);
            }
        }
        
        BaseMove();
        //奔跑
        if (Input.GetKey(KeyCode.LeftShift))
            isRun = true;
        else
            isRun = false;
        //跳跃 翻滚与跑步时均不能跳
        if (Input.GetButtonDown("Jump") && !PlayerConctroller.IsAir && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            isRun = false;
            jumpTime = 0.3f;
            animator.SetBool("Jump",true);
        }
        jumpTime -= Time.deltaTime;//计时
        if (jumpTime >= 0 && PlayerConctroller.CanMove)
            characterController.Move(jumpSpeed * Vector3.up * Time.deltaTime);
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd"))
            animator.SetBool("Jump",false);

        //翻滚
        if (Input.GetMouseButtonDown(1) && moveDirection != Vector3.zero &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !PlayerConctroller.IsAir)
        {
            isRoll = true;
            animator.SetTrigger("Roll");
        }
        else
            isRoll = false;

        //模拟重力
        moveDirection.y -= gravity * Time.deltaTime; 
        
        //运动
        if(PlayerConctroller.CanMove) characterController.Move(moveDirection * (speed * Time.deltaTime));
        
        SwitchAnimation();
    }
    
    /// <summary>
    /// 基础移动
    /// </summary>
    void BaseMove()
    {
        if (!PlayerConctroller.CanMove) return;

        float curX = PlayerConctroller.CanMove ? Input.GetAxis("Vertical") : 0;
        float curY = PlayerConctroller.CanMove ? Input.GetAxis("Horizontal") : 0;

        rot = Quaternion.Euler(0,photographer.Yaw,0);
        moveDirection = rot * Vector3.forward * curX + rot * Vector3.right * curY;
        
        if(moveDirection != Vector3.zero || PlayerConctroller.IsAttack)
            PlayerConctroller.transform.rotation = Quaternion.Slerp(PlayerConctroller.transform.rotation, rot, rotateSpeed * Time.deltaTime);//顺滑转向
        
        animationMove = Vector3.forward * (curX * speed) + Vector3.right * (curY * speed);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 播放动画与声音
    /// </summary>
    private void SwitchAnimation()
    {
        animator.SetFloat("Walk", animationMove.x);
        animator.SetFloat("Turn", animationMove.z);
        animator.SetBool("IsAir", PlayerConctroller.IsAir);
    }

    #endregion
}
