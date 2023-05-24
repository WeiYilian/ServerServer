using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class EnemySpawn : MonoBehaviour
{
    public bool canSpawn;
    
    //刷新范围
    public float Range = 5;
    
    //刷新间隔
    public float intervalTime = 10f;

    private void OnEnable()
    {
        InvokeRepeating(nameof(RandomSpawnEnemy),1f,intervalTime);
    }

    public void RandomSpawnEnemy()
    {
        if (!canSpawn) return;
        
        
        int zombieIndex = Random.Range(1, 10);
        GameObject go = GameFacade.Instance.LoadGameObject("Zombie" + zombieIndex);
        Instantiate(go, transform);
        go.transform.localPosition = RandomSpawnPoint();
        //canSpawn = false;
    }
    
    /// <summary>
    /// 随机刷新点
    /// </summary>
    /// <returns></returns>
    public Vector3 RandomSpawnPoint()
    {
        Vector3 randomSpawnPoint = Vector3.zero;
       
        float randomX = Random.Range(-Range, Range + 1);
        float randomZ = Random.Range(-Range, Range + 1);
        
        Vector3 randomPoint = new Vector3(randomX, transform.position.y + 2, randomZ);
        
        NavMeshHit hit;
        randomSpawnPoint =  NavMesh.SamplePosition(randomPoint, out hit, Range, 1) ? hit.position : Vector3.zero;
        
        return randomSpawnPoint;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
