using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public enum EnemyStates {GUARD, PATROL, CHASE ,DEAD }
public enum EnemySort{MAGE,WARRIOR}
[RequireComponent(typeof(NavMeshAgent))]//要求脚本挂载的物体上必须要有某个组件
[RequireComponent(typeof(CharacterStats))]
public class EnemyConcroller : MonoBehaviour,IEnemy
{
    protected EnemyStates enemyStates;
    protected EnemySort enemySort;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected new Collider collider;
    
    protected CharacterStats characterStats;
    
    [Header("Basic Settings")]
    public float sightRadius;//攻击范围
    public bool isGuard;
    protected float speed;
    protected GameObject AttackTarget;
        
    public float lookAtTime;//巡逻停留时间
    protected float remainLookAtTime;//巡逻计时器
    protected float lastAttackTime;//攻击计时器

    [Header("Patrol State")] 
    public float patrolRange;//巡逻范围
    protected Vector3 wayPoint;//随机巡逻点
    protected Vector3 guardPos;//初始坐标
    protected Quaternion guardRotation;//初始角度(初始面向方向)

    //bool配合动画
    protected bool isWalk;
    protected bool isChase;
    protected bool isFollow;
    protected bool isDead;
    protected bool playerDead;
    private bool isFall;

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        collider = GetComponent<Collider>();
        
        speed = agent.speed;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    protected IEnumerator Start()
    {
        EvenCenter.AddListener(EventNum.GAMEOVER,EndNotify);
        
        //判断是否为站桩怪
        yield return null;
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
            guardPos = transform.position;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            wayPoint = transform.position;
        }
    }

    private void OnDisable()
    {
        EvenCenter.RemoveListener(EventNum.GAMEOVER,EndNotify);
    }

    private void Update()
    {
        if (playerDead || GameLoop.Instance.isTimeOut || !GameController.Instance.GameStart) return;

        if (GameController.Instance.FirstOver && transform.CompareTag("Zombie"))
            characterStats.CurrentHealth = 0;
        if (characterStats.CurrentHealth <= 0)
            isDead = true;
        
        if (animator.GetCurrentAnimatorStateInfo(2).IsName("Damage"))
            return;
       

        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
        
    }

    void SwitchAnimation()
    {
        animator.SetBool("Walk",isWalk);
        animator.SetBool("Chase",isChase);
        animator.SetBool("Follow",isFollow);
        animator.SetBool("Death", isDead);
    }
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 状态切换
    /// </summary>
    protected virtual void SwitchStates()
    {
        //如果生命值等于0，切换到DEAD
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        //如果发现Player，切换到CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
            

        switch (enemyStates)
        {
            case EnemyStates.GUARD://站桩模式的敌人
                
                if(GameController.Instance.IsFinalStage)
                {
                    AttackTarget = PlayerConctroller.Instance.gameObject;
                    enemyStates = EnemyStates.CHASE;
                }

                isWalk = false;
                isChase = false;
                agent.isStopped = true;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;//回到原点

                    //SqrMagnitude计算两个三维坐标之间的差值，与disdance作用类似，但开销比distance小
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.1f);//最后一个参数越小，旋转的越慢
                    }
                }
                break;
            case EnemyStates.PATROL://巡逻模式的敌人
                
                if(GameController.Instance.IsFinalStage)
                {
                    AttackTarget = PlayerConctroller.Instance.gameObject;
                    enemyStates = EnemyStates.CHASE;
                }
                
                isChase = false;
                agent.speed = speed;
                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE://追击模式
                isWalk = false;
                isChase = true;

                agent.speed = speed * 7;
                
                if (!FoundPlayer() && !GameController.Instance.IsFinalStage)
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    if(GameController.Instance.IsFinalStage)
                        AttackTarget = PlayerConctroller.Instance.gameObject;
                    
                    
                    isFollow = true;
                    if(animator.GetCurrentAnimatorStateInfo(1).IsName("Run"))
                    {
                        agent.isStopped = false;
                        agent.destination = AttackTarget.transform.position;
                    }
                }
                //在攻击范围内则攻击
                if (TargetInAttackRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    transform.LookAt(AttackTarget.transform);
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.characterData.coolDown;
                        
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.characterData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD://死亡模式
                //掉落回血包
                if (Random.Range(0f, 1f) <= 0.3f && !isFall)
                {
                    isFall = true;
                    Instantiate(GameFacade.Instance.LoadGameObject("RestoreDrug"), transform.position,
                        Quaternion.identity);
                }
                Destroy(gameObject,2f);
                agent.isStopped = true;
                collider.enabled = false;//关闭collider
                agent.radius = 0;
                break;
        }
    }

    protected virtual void Attack()
    {
        int index = Random.Range(1, 3);
        if (index == 1 && TargetInAttackRange())
            animator.SetTrigger("Skill");//技能攻击动画
        else if (index == 2 && TargetInAttackRange())
            animator.SetTrigger("Attack");//近身攻击动画

    }

    //检测敌人sightRadius内是否有Player
    protected bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                AttackTarget = target.gameObject;
                return true;
            }
        }

        AttackTarget = null;
        return false;
    }

    //判断是否进入基础攻击距离
    protected bool TargetInAttackRange()
    {
        if (AttackTarget != null)
            return Vector3.Distance(AttackTarget.transform.position, transform.position) <=
                   characterStats.characterData.attackRange;
        else
            return false;
    }
    
    //判断是否进入技能攻击距离
    protected bool TargetInSkillRange()
    {
        if (AttackTarget != null)
            return Vector3.Distance(AttackTarget.transform.position, transform.position) <=
                   characterStats.characterData.skillRange;
        else
            return false;
    }

    //获取随机巡逻点
    protected void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }


    #region 动画事件

    //Animation Event
    public virtual void Hit()
    {
        if (enemyStates == EnemyStates.DEAD) return;
        //如果攻击目标不为空，攻击目标在前方，没有被打就可以执行
        if (TargetInAttackRange() && transform.IsFacingTarget(AttackTarget.transform)/*扩展方法*/)
        {
            var targetStats = AttackTarget.GetComponentInChildren<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    
    //Animation Event
    public virtual void KickOff()
    {
        if (enemyStates == EnemyStates.DEAD) return;
        
        if(TargetInSkillRange() && transform.IsFacingTarget(AttackTarget.transform)/*扩展方法*/)
        {
            var targetStats = AttackTarget.GetComponentInChildren<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats,true);
        }
    }

    #endregion
    
    
    //怪物胜利，游戏结束
    public void EndNotify()
    {
        //停止所有移动
        playerDead = true;
        isChase = false;
        isWalk = false;
        AttackTarget = null;
        //停止Agent

    }
    
    //将各种范围可视化
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
