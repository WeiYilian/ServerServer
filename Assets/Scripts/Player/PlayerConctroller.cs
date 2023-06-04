using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerConctroller : MonoBehaviour
{
    public static PlayerConctroller Instance;
    
    [HideInInspector] public CharacterController characterController;

    [HideInInspector] public Animator animator;

    //摄像机的位置
    [HideInInspector] public CameraController photographer;
    //摄像机跟随的位置
    [HideInInspector] public Transform followingTarget;
    
    //背包
    public Inventory MyBag;

    public GameObject LevelUP;

    public GameObject ReturnBlood;


    private CharacterStats characterStats;

    private MoveController moveController;

    private AttackController attackController;

    private AttribController attribController;

    public MainPanel MainPanel;

    private string playerName;

    private bool isInit;
    

    //判断是否可以移动
    private bool canMove = true;
    //判断是否被击打
    private bool isHit;
    //判断是否攻击
    private bool isAttack;
    //判断是否在空中
    private bool isAir;
    //判断是否死亡
    private bool isDeath;
    public bool isReturnBlood;

    private float Healthtimer = 5f;

    #region 共有属性

    public string PlayerName
    {
        get => playerName;
        private set => playerName = value;
    }

    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }

    public CharacterStats CharacterStats
    {
        get => characterStats;
        private set => characterStats = value;
    }

    public bool IsHit
    {
        get => isHit;
        set => isHit = value;
    }

    public bool IsAttack
    {
        get => isAttack;
        set => isAttack = value;
    }

    public bool IsAir
    {
        get => isAir;
        set => isAir = value;
    }

    #endregion
   

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        Init();
    }

    

    private void Init()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        photographer = GameObject.Find("photograther").GetComponent<CameraController>();
        followingTarget = transform.Find("FollowingTarget");

        characterStats = GetComponent<CharacterStats>();
        attribController = new AttribController();
        moveController = new MoveController();
        attackController = new AttackController();
    }
    
    
    public void Update()
    {
        if (!isInit)
        {
            isInit = true;
            attackController.Init();
            attribController.Init();
        }
        
        attribController.SuperviserNumber();
        
        if (isDeath || GameLoop.Instance.isTimeOut || !GameController.Instance.GameStart) return;
        
        //如果挨打，直接返回
        if (isHit)
        {
            ThreeSkillOut();
            return;
        }

        if (isReturnBlood)
        {
            Healthtimer -= Time.deltaTime;
            float Hplimit = characterStats.CurrentHealth + 20;
            characterStats.CurrentHealth = Mathf.Min(Mathf.Lerp(characterStats.CurrentHealth, characterStats.CurrentHealth + 4,
                Time.deltaTime), characterStats.MaxHealth);
            if(Healthtimer<=0)
                ThreeSkillOut();

            return;
        }
        
        PlayerDeath();
        
        if (IsGround())
            isAir = false;
        else
            isAir = true;

        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Base State") || moveController.isRoll)
            canMove = false;
        else
            canMove = true;

        if (animator.GetCurrentAnimatorStateInfo(3).IsName("Base State"))
            isHit = false;
        
        attackController.Attack();
        moveController.PlayerAction();
    }
    
    //判断玩家是否死亡
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayerDeath()
    {
        if(characterStats.CurrentHealth <= 0)
        {
            isDeath = true;
            AudioManager.Instance.PlayAudio(2,"Die");
            animator.SetTrigger("Die");
            EvenCenter.BroadCast(EventNum.GAMEOVER);
        }
    }
    
    /// <summary>
    /// 判断是否在地面
    /// </summary>
    /// <returns></returns>
    private bool IsGround()
    {
        var raycastAll = Physics.OverlapBox(transform.position,  new Vector3(0.5f,0.5f,0.5f),Quaternion.identity,~3);
        if (raycastAll.Length >= 0.5f)
            return true;// 在地面
        return false;// 离地
    }

    #region 动画事件

     public void Hit()
    {
        attackController.Hit();
    }

    public void AttackEffection(int index)
    {
        attackController.AttackEffection(index);
    }

    /// <summary>
    /// 动画事件，玩家攻击结束
    /// </summary>
    public void AttackOver()
    {
        isAttack = false;
    }
    
    //跳跃攻击开始
    public void AirAttackStart()
    {
        characterController.enabled = true;
    }
    //跳跃攻击结束
    public void AirAttackEnd()
    {
        canMove = true;
        isAttack = false;
    }

    public void HitEnd()
    {
        isHit = false;
    }

    public void ThreeSkill()
    {
        isReturnBlood = true;
        AudioManager.Instance.PlayAudio(4,"Summon");
        ReturnBlood.GetComponent<ParticleSystem>().Play();
    }

    private void ThreeSkillOut()
    {
        characterStats.CurrentHealth = Mathf.FloorToInt(characterStats.CurrentHealth);
        isReturnBlood = false;
        AudioManager.Instance.StopAudio(4);
        ReturnBlood.GetComponent<ParticleSystem>().Stop();
        animator.SetBool("ReturnHP",false);
        Healthtimer = 5;
    }
    

    #endregion
   

}