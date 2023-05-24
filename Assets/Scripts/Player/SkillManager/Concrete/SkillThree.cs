using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillThree :BaseSkill
{
    public SkillThree(Image image) : base(8f,5f,-1f,image) { }

    private float timer;

    public override void EmitSpecialEffects()
    {
        base.EmitSpecialEffects();
    }

    public override void SkillHit(CharacterStats playerStats, CharacterStats enemyStats)
    {
         //十倍伤害
         enemyStats.SkillTakeDamage(playerStats, enemyStats,10);
    }
}
