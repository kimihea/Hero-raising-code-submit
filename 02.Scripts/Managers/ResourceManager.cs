using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMapData
{
    public List<AddressableMap> list = new List<AddressableMap>();

    public void AddRange(List<AddressableMap> list)
    {
        this.list.AddRange(list);
    }

    public void Add(AddressableMap data)
    {
        list.Add(data);
    }
}

[Serializable]
public class AddressableMap
{
    public EAddressableType addressableType;
    public string key;
    public string path;
}


public class ResourceManager : Singleton<ResourceManager>
{
    [NonSerialized] public bool isInit = false;

    private Dictionary<EAddressableType, Dictionary<string, AddressableMap>> addressableMap = new Dictionary<EAddressableType, Dictionary<string, AddressableMap>>();

    private readonly string[] localPath =
    {
        "Prefab/",
        "Data/",
        "Audio/"
    };

    public async void Init()
    {
         await LoadAddressable();
    }

    public async Task LoadAddressable()
    {
        var init = await Addressables.InitializeAsync().Task;
        var handle = Addressables.DownloadDependenciesAsync("InitDownload"); // InitDownload label 다운로드
        UILoading.instance.SetProgress(handle, "리소스 로딩 중...");
        //StartCoroutine(SetProgress(handle));
        await handle.Task;
        switch (handle.Status)
        {
            case AsyncOperationStatus.None:
                break;
            case AsyncOperationStatus.Succeeded:
                //Debug.Log("다운로드 성공!");
                break;
            case AsyncOperationStatus.Failed:
                //Debug.Log("다운로드 실패 : " + handle.OperationException.Message);
                Debug.LogError(handle.OperationException.ToString());
                break;
            default:
                break;
        }
        Addressables.Release(handle);
        InitAddressableMap();
    }

    private async void InitAddressableMap()
    {
        // Label : AddressableMap인 에셋들을 로드 > .json 파일들을 불러와서 AddressableMap 형태로 dict에 저장
        await Addressables.LoadAssetsAsync<TextAsset>("AddressableMap", (text) =>
        {
            var map = JsonUtility.FromJson<AddressableMapData>(text.text);
            var key = EAddressableType.PREFAB;
            Dictionary<string, AddressableMap> mapDic = new Dictionary<string, AddressableMap>();
            foreach (var data in map.list) // MapData > Dict<Map> 형태로 매핑
            {
                key = data.addressableType;
                if (!mapDic.ContainsKey(data.key)) // data.key = rcode
                    mapDic.Add(data.key, data);
            }
            if (!addressableMap.ContainsKey(key)) addressableMap.Add(key, mapDic);

        }).Task;
        isInit = true;
    }

    public string GetPath(string key, EAddressableType addressableType)
    {
        var map = addressableMap[addressableType][key.ToLower()];
        return map.path;
    }

    public async Task<T> GetResource<T>(string key, EAddressableType addressableType)
    {
        try
        {
            var path = GetPath(key, addressableType);
            return await LoadAssetAsync<T>(path);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        return default;
    }

    private async Task<T> LoadAssetAsync<T>(string path)
    {
        try
        {
            if (path.Contains(".prefab") && typeof(T) != typeof(GameObject))
            {
                var obj = await Addressables.LoadAssetAsync<GameObject>(path).Task;
                return obj.GetComponent<T>();
            }
            else
                return await Addressables.LoadAssetAsync<T>(path).Task;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return default;
    }

    //private void InitResourceMap()
    //{
    //    foreach(var type in Enum.GetValues(typeof(EResourceType)))
    //    {
    //        Dictionary<string, UnityEngine.Object> rcodeDict = new Dictionary<string, UnityEngine.Object>();
    //        string path = localPath[(int)type];
    //        var obj = Resources.LoadAll(path);
    //        foreach(var item in obj)
    //        {
    //            rcodeDict.Add(item.name, item);
    //        }
    //        ResourceMap.Add((EResourceType)type, rcodeDict);
    //    }
    //}

    //public T GetResource<T>(string key, EResourceType resourceType) where T : UnityEngine.Object
    //{
    //    //string path = localPath[(int)resourceType] + key;
    //    //var obj = Resources.Load(path) as T;        

    //    return ResourceMap[resourceType][key] as T;
    //}     
}
