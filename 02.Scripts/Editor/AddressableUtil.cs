using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEditor.AddressableAssets;
using UnityEngine;
using System.IO;
using System.Linq;

public class AddressableUtil : Editor
{
    [InitializeOnEnterPlayMode]
    [MenuItem("EditorUtil/Addressable/Mapping")]
    internal static void Mapping()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                // Label이 addressableMap인 경우를 제외하고 진행
                if (!entry.AssetPath.Contains("Assets") || entry.AssetPath.Contains("addressableMap")) continue;
                var type = (EAddressableType)Enum.Parse(typeof(EAddressableType), group.Name);
                var dir = Application.dataPath + entry.AssetPath.Replace("Assets", ""); // 에셋 경로
                var mapData = SetMapping(dir, type); // path와 type으로 AddressableMapData로 매칭
                var newPath = entry.AssetPath + "/addressableMap.json";
                var dt = JsonUtility.ToJson(mapData);
                File.WriteAllText(newPath, dt);
                AssetDatabase.ImportAsset(newPath);
            }
        }
    }

    static AddressableMapData SetMapping(string dir, EAddressableType type)
    {
        var files = Directory.GetFiles(dir).ToList();
        files.RemoveAll(obj => obj.Contains(".meta")); // meta파일 제외
        AddressableMapData mapData = new AddressableMapData();
        var dirs = Directory.GetDirectories(dir).ToList();
        foreach (var d in dirs) // 폴더링
        {
            if (d.Contains("Atlas")) continue; // Atlas 제외, 이유 몰?루
            var res = SetMapping(d, type);
            mapData.AddRange(res.list);
        }
        foreach (var file in files)
        {
            var path = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            var data = new AddressableMap();
            data.addressableType = type;
            data.key = path.Split('/').Last().Split('.')[0].ToLower().Split('.').First(); // 확장자 제거 = 파일명 = rcode
            data.path = path; // 경로
            mapData.Add(data);
        }
        return mapData;
    }
}
