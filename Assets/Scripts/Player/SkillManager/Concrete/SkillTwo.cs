using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTwo : BaseSkill
{
    public SkillTwo(Image image):base(10f,8f,0.9f,image) { }

    
    
    public override void EmitSpecialEffects()
    {

        base.EmitSpecialEffects();
    }

    public override void SkillHit(CharacterStats playerStats, CharacterStats enemyStats)
    {
        enemyStats.TakeDamage(playerStats, enemyStats);
        //技能四，眩晕敌人1.5秒
        enemyStats.GetComponent<Animator>().SetTrigger("Dizzy");//播放敌人眩晕动画
        
    }
}
