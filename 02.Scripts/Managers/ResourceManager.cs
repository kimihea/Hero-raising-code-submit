using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<EResourceType, Dictionary<string, UnityEngine.Object>> ResourceMap = new Dictionary<EResourceType, Dictionary<string, UnityEngine.Object>>();
    private readonly string[] localPath =
    {
        "Prefab/",
        "Data/",
        "Audio/"
    };

    protected override void Awake()
    {
        base.Awake();
        InitResourceMap();
    }

    private void Start()
    {
        //InitResourceMap();
    }

    private void InitResourceMap()
    {
        foreach(var type in Enum.GetValues(typeof(EResourceType)))
        {
            Dictionary<string, UnityEngine.Object> rcodeDict = new Dictionary<string, UnityEngine.Object>();
            string path = localPath[(int)type];
            var obj = Resources.LoadAll(path);
            foreach(var item in obj)
            {
                rcodeDict.Add(item.name, item);
            }
            ResourceMap.Add((EResourceType)type, rcodeDict);
        }
    }

    public T GetResource<T>(string key, EResourceType resourceType) where T : UnityEngine.Object
    {
        //string path = localPath[(int)resourceType] + key;
        //var obj = Resources.Load(path) as T;        

        return ResourceMap[resourceType][key] as T;
    }     
}
