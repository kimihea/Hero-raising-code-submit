using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

[System.Serializable]
public class Pool
{    
    public string rcode; // key value by rcode
    public int size; // initial size
    //public Transform parentTransform; // parent object
    public GameObject prefab;
}
public class PoolManager : Singleton<PoolManager>
{       
    [Header("# Pool Info")]
    [SerializeField] private List<Pool> pools = new List<Pool>();
    
    private Dictionary<string, List<GameObject>> poolDictionary;
    [NonSerialized] public bool IsInit;

    protected override void Awake()
    {
        base.Awake();        
        StartCoroutine(InitCoroutine());
    }

    public static IEnumerator TaskAsIEnumerator(Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.Exception != null)
        {
            throw task.Exception;
        }
    }

    private async Task InitAsync()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        // pools에 있는 모든 오브젝트를 탐색하고 정해놓은 size만큼 프리팹을 미리 만들어 놓음
        foreach (Pool pool in pools)
        {
            //Debug.Log(pool.rcode);
            List<GameObject> list = new List<GameObject>();
            poolDictionary.Add(pool.rcode, list);
            pool.prefab = await ResourceManager.Instance.GetResource<GameObject>(pool.rcode, EAddressableType.PREFAB);
            AddPoolObject(pool);
            //var path = ResourceManager.Instance.GetPath(pool.rcode, EAddressableType.PREFAB);
            //pool.prefab = await Addressables.InstantiateAsync(path, parent: pool.parentTransform).Task;
            //yield return TaskAsIEnumerator(AddPoolObject(pool));
        }
        IsInit = true;
    }

    private IEnumerator InitCoroutine()
    {
        #region NonIntro
        // TODO : 배포 시 삭제
        if (!ResourceManager.Instance.isInit)
        {
            ResourceManager.Instance.Init();
            yield return new WaitUntil(() => ResourceManager.Instance.isInit);
        }
        #endregion
        yield return TaskAsIEnumerator(InitAsync()); 
    }

    //private IEnumerator InitCoroutine()
    //{
    //    poolDictionary = new Dictionary<string, List<GameObject>>();
    //    // pools에 있는 모든 오브젝트를 탐색하고 정해놓은 size만큼 프리팹을 미리 만들어 놓음
    //    foreach (Pool pool in pools)
    //    {
    //        Debug.Log(pool.rcode);
    //        List<GameObject> list = new List<GameObject>();
    //        poolDictionary.Add(pool.rcode, list);
    //        Task task = AddPoolObject(pool);
    //        yield return new WaitUntil(() => task.IsCompleted);
    //    }
    //    IsInit = true;
    //}

    private void AddPoolObject(Pool pool) // 프리팹 생성
    {        
        for (int i = 0; i < pool.size; i++)
        {
            GameObject poolObj = Instantiate(pool.prefab);        
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