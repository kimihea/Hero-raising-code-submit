using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            // get instance of singleton
            if (instance == null)
            {
                // �Ҵ縸 �Ǿ� ���� ���� ��� 
                instance = (T)FindObjectOfType(typeof(T));

                // ������ ������Ʈ ��ü�� ���� ���
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).ToString());
                    instance = singletonObject.AddComponent<T>();
                }
            }

            return instance;
        }
    }    

    protected virtual void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            if (instance != this)
            {
                //Debug.Log(gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }
}
