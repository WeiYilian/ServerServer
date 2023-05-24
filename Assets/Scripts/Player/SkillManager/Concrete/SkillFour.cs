using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFour : BaseSkill
{
    public SkillFour():base(0f,3f,0.5f,null) { }
        
    public override void EmitSpecialEffects()
    {
        base.EmitSpecialEffects();
    }
    
    public override void SkillHit(CharacterStats playerStats, CharacterStats enemyStats)
    {
        //双倍伤害
        enemyStats.SkillTakeDamage(playerStats, enemyStats,2);
    }
}
