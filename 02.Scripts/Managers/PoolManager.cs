using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class Pool
{    
    public string rcode; // key value by rcode
    public int size; // initial size
    public Transform parentTransform; // parent object
}
public class PoolManager : Singleton<PoolManager>
{       
    [Header("# Pool Info")]
    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<string, List<GameObject>> poolDictionary;
     

    private void Start()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        // pools에 있는 모든 오브젝트를 탐색하고 정해놓은 size만큼 프리팹을 미리 만들어 놓음
        foreach (Pool pool in pools)
        {
            List<GameObject> list = new List<GameObject>();
            poolDictionary.Add(pool.rcode, list);

            AddPoolObject(pool);           
        }
    }

    private void AddPoolObject(Pool pool) // 프리팹 생성
    {
        GameObject obj = ResourceManager.Instance.GetResource<GameObject>(pool.rcode, EResourceType.PREFAB);
        for (int i = 0; i < pool.size; i++)
        {
            GameObject poolObj = Instantiate(obj, pool.parentTransform);
            poolObj.name = pool.rcode;
            poolObj.SetActive(false);
            poolDictionary[pool.rcode].Add(poolObj);
        }
    }

    // 이미 생성된 오브젝트 풀에서 프리팹을 가져옴
    public GameObject SpawnFromPool(string rcode)
    {
        if (!poolDictionary.ContainsKey(rcode))
        {
            return null;
        }

        GameObject poolObject = null;

        for (int i = 0; i < poolDictionary[rcode].Count; i++)
        {
            if (!poolDictionary[rcode][i].gameObject.activeSelf) // 비활성화된 오브젝트를 찾았을 때
            {
                poolObject = poolDictionary[rcode][i];
                break;
            }

            if (i == poolDictionary[rcode].Count - 1) // 모든 오브젝트가 활성화 됐을 때 
            {
                Pool pool = pools.Find(x => x.rcode == rcode);
                AddPoolObject(pool);
                poolObject = poolDictionary[rcode][i + 1];
            }
        }

        poolObject.gameObject.SetActive(true); // 활성화

        return poolObject;
    }

    // 이미 생성된 오브젝트 풀에서 프리팹을 가져옴
    public T SpawnFromPool<T>(string rcode) where T : MonoBehaviour 
    {
        if (!poolDictionary.ContainsKey(rcode))
        {
            return null;
        }

        GameObject poolObject = null;

        for (int i = 0; i < poolDictionary[rcode].Count; i++)
        {
            if (!poolDictionary[rcode][i].gameObject.activeSelf) // 비활성화된 오브젝트를 찾았을 때
            {
                poolObject = poolDictionary[rcode][i];
                break;
            }

            if (i == poolDictionary[rcode].Count - 1) // 모든 오브젝트가 활성화 됐을 때 
            {
                Pool pool = pools.Find(x => x.rcode == rcode);
                AddPoolObject(pool);
                poolObject = poolDictionary[rcode][i + 1];
            }
        }

        poolObject.gameObject.SetActive(true); // 활성화

        return poolObject.GetComponent<T>();
    }
}