using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AttackController
{
    private Animator animator;

    private CharacterStats characterStats;

    private PlayerConctroller PlayerConctroller;
    
    public AttackController()
    {
        PlayerConctroller = PlayerConctroller.Instance;
        
        animator = PlayerConctroller.animator;
        characterStats = PlayerConctroller.CharacterStats;
        Skills = new List<BaseSkill>();
    }

    public void Init()
    {
        Skills.Add(new SkillOne(GameFacade.Instance.PanelManager.MainPanel().skill1));
        Skills.Add(new SkillTwo(GameFacade.Instance.PanelManager.MainPanel().skill2));
        Skills.Add(new SkillThree(GameFacade.Instance.PanelManager.MainPanel().skill3));
        Skills.Add(new SkillFour());
    }

    #region 普通攻击参数设置
    
    [Header("攻击参数设置")]
    //连击次数
    public int comboStep;
    //允许连击的时间
    public float interval = 2f;
    //计时器
    private float timer;

    #endregion

    #region 技能攻击参数设置

    //技能列表，存储技能
    private List<BaseSkill> Skills;

    //计时器
    private float cd1Time;
    private float cd2Time;
    private float cd3Time;

    #endregion

    /// <summary>
    /// 玩家攻击
    /// </summary>
    public void Attack()
    {
        SkillTrigger();

        NormalAttack();
    }
    
    #region 普通攻击
    
    /// <summary>
    /// 普通攻击
    /// </summary>
    public void NormalAttack()
    {
        //普通攻击
        if (Input.GetMouseButtonDown(0) && !PlayerConctroller.IsAttack && !PlayerConctroller.IsAir)
        {
            PlayerConctroller.IsAttack = true;
            //连击次数递增
            comboStep++;
            if (comboStep > 4)
                comboStep = 1;
            timer = interval;
            AudioManager.Instance.PlayAudio(2,"SwingEmpty");
            animator.SetTrigger("Attack");
            animator.SetInteger("ComboStep",comboStep);
        }
        //跳跃攻击
        if (Input.GetMouseButtonDown(0) && !PlayerConctroller.IsAttack && PlayerConctroller.IsAir)
        {
            PlayerConctroller.CanMove = false;
            PlayerConctroller.IsAttack = true;
            PlayerConctroller.characterController.enabled = false;
            animator.SetTrigger("JumpAttack");
        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsName("JumpAttackLoop"))
            PlayerConctroller.characterController.Move(Vector3.up * (-100f * Time.deltaTime));
        //连击计时
        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                comboStep = 0;
                animator.SetInteger("ComboStep",comboStep);
            }
        }
    }

    #endregion

    #region 技能攻击

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 检测是否触发技能
    /// </summary>
    public void SkillTrigger()
    {
        if(animator.GetCurrentAnimatorStateInfo(2).IsName("Base State"))
            animator.SetInteger("SkillIndex",0);
        
        if (Input.GetKeyUp(KeyCode.Q) && !PlayerConctroller.IsAttack && !Skills[0].IsCooling)
        {
            PlayerConctroller.IsAttack = true;
            Skills[0].Image.fillAmount = 1;
            animator.SetTrigger("Attack");
            animator.SetInteger("SkillIndex",1);
        }
        if (Input.GetKeyUp(KeyCode.E) && !PlayerConctroller.IsAttack && !Skills[1].IsCooling)
        {
            PlayerConctroller.IsAttack = true;
            Skills[1].Image.fillAmount = 1;
            animator.SetTrigger("Attack");
            animator.SetInteger("SkillIndex",2);
        }
        if (Input.GetKeyUp(KeyCode.R) && !PlayerConctroller.IsAttack && !Skills[2].IsCooling)
        {
            PlayerConctroller.IsAttack = true;
            Skills[2].Image.fillAmount = 1;
            animator.SetTrigger("Attack");
            animator.SetBool("ReturnHP",true);
            animator.SetInteger("SkillIndex",3);
        }
        
        CdDecline(0,ref cd1Time);
        CdDecline(1,ref cd2Time);
        CdDecline(2,ref cd3Time);
    }

    /// <summary>
    /// 发出技能后冷却
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="cd"></param>
    /// <param name="index"></param>
    /// <param name="cdTime"></param>
    public void CdDecline(int index,ref float cdTime)
    {
        //玩家面板技能冷却
        if (Skills[index].Image.fillAmount != 0)
        {
            Skills[index].IsCooling = true;
            cdTime += Time.deltaTime;
            Skills[index].Image.fillAmount = (Skills[index].SkillCd - cdTime) / Skills[index].SkillCd;
        }
        
        if (cdTime >= Skills[index].SkillCd)
        {
            Skills[index].IsCooling = false;
            Skills[index].Image.fillAmount = 0;
            cdTime = 0;
        }
    }
    #endregion

    #region 动画事件
    
    /// <summary>
    /// 动画事件，技能释放
    /// </summary>
    public void AttackEffection(int index)
    {
        Skills[index].EmitSpecialEffects();
    }

    /// <summary>
    /// 普通伤害计算
    /// </summary>
    public void Hit()
    {
        var colliders = Physics.OverlapSphere(PlayerConctroller.Instance.transform.position, characterStats.characterData.attackRange);

        foreach (var enemy in colliders)
        {
            if (enemy.GetComponent<IEnemy>() != null && PlayerConctroller.Instance.transform.IsFacingTarget(enemy.transform)/*扩展方法*/)
            {
                var targetStats = enemy.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
            }
        }
    }


    #endregion
}
