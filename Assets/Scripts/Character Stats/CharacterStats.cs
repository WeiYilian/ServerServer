using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class CharacterStats : MonoBehaviour
{
    //public event Action<float, float> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;//模板数据
    
    public CharacterData_SO characterData;//实际数据

    [HideInInspector]//虽然是public的，但是在inspector窗口中屏蔽了
    public bool isCritical;
     private void OnEnable()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);

        if (transform.CompareTag("Zombie"))
        {
            int playerLevel = PlayerConctroller.Instance.CharacterStats.CurrentLevel;
            float health = Random.Range(5 * playerLevel, 10 * playerLevel);
            CurrentHealth = Mathf.Min(MaxHealth, health);
            
            MinDamage = Random.Range(1 * playerLevel, 2 * playerLevel);
            MaxDamage = Random.Range(2 * playerLevel, 3 * playerLevel);

            CurrentDefence = 1 * playerLevel;

            if (GameFacade.Instance.GameControler.IsFinalStage)
            {
                //最终模式下，全部属性翻倍
                CurrentHealth *= 2;
                MinDamage *= 2;
                MaxDamage *= 2;
                CurrentDefence *= 2;
            }
        }
    }
     
    #region Read from Data_SO

    public float MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth;else return 0; }
        set { characterData.maxHealth = value; }
    }
    
    public float CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth;else return 0; }
        set { characterData.currentHealth = value; }
    }
    
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence;else return 0; }
        set { characterData.baseDefence = value; }
    }
    
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence;else return 0; }
        set { characterData.currentDefence = value; }
    }

    public int CurrentLevel
    {
        get { if (characterData != null) return characterData.currentLevel;else return 0; }
        set { characterData.currentLevel = value; }
    }
    
    public float BaseExp
    {
        get { if (characterData != null) return characterData.baseExp;else return 0; }
        set { characterData.baseExp = value; }
    }
    
    public float CurrentExp
    {
        get { if (characterData != null) return characterData.currentExp;else return 0; }
        set { characterData.currentExp = value; }
    }

    public int MinDamage
    {
        get { if (characterData != null) return characterData.minDamage;else return 0; }
        set { characterData.minDamage = value; }
    }
    public int MaxDamage
    {
        get { if (characterData != null) return characterData.maxDamage;else return 0; }
        set { characterData.maxDamage = value; }
    }

    public int AttributePoints
    {
        get { if (characterData != null) return characterData.attributePoints;else return 0; }
        set { characterData.attributePoints = value; }
    }
    
    #endregion

    #region Character Combat

    /// <summary>
    /// 攻击时数据更迭
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defener"></param>
    public void TakeDamage(CharacterStats attacker, CharacterStats defener, bool isSkill = false)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (defener.CompareTag("Player"))
        {
            if (!PlayerConctroller.Instance.IsHit)
            {
                PlayerConctroller.Instance.IsHit = true;
                defener.GetComponent<Animator>().SetTrigger("Hit");
            }
        }
        else
           defener.GetComponent<Animator>().SetTrigger("Hit");//播放敌人被打动画

        //update UI
        //UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //经验update
        if(CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);
        
    }

    public void SkillTakeDamage(CharacterStats attacker, CharacterStats defener,int Multiple)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() * Multiple - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        defener.GetComponent<Animator>().SetTrigger("Hit");//播放敌人被打动画
        //update UI
        //UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //经验update
        if(CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);
    }
    
    /// <summary>
    /// 法师攻击玩家时计算伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="defener"></param>
    public void TakeDamage(int damage, CharacterStats defener)
    {
        int CurrentDamage = Mathf.Max(damage - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - CurrentDamage, 0);
    }

    /// <summary>
    /// 计算当前伤害，判断是否暴击
    /// </summary>
    /// <returns></returns>
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(characterData.minDamage, characterData.maxDamage);

        if (isCritical)
        {
            coreDamage *= characterData.criticalMultiplier;
            Debug.Log("暴击"+coreDamage);
        }
        return (int)coreDamage;
    }
    
    #endregion
    
    
}
