using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public static ObjectPool Instance;

    public Pool[] Pools;

    Dictionary<string, Stack<ObjectPoolObject>> Objects;

    private void Awake() {
        if(Instance) {
            return;
        }
        Instance = this;

        foreach(Pool pool in Pools) {
            Objects.Add(pool.Name, new Stack<ObjectPoolObject>());

            int i = 0;
            while(i < pool.InitialCount) {
                GameObject obj = Instantiate(pool.Obj, transform);
                obj.SetActive(false);
                Objects[pool.Name].Push(obj.GetComponent<ObjectPoolObject>());
                Objects[pool.Name].Peek().PoolName = pool.Name;
                i++;
            }
        }
    }

    public void SpawnObject(string Name, Transform Parent) {
        SpawnObject(Name, Parent.position, Parent.rotation, Parent);
    }

    public void SpawnObject(string Name, Vector3 Position, Quaternion Rotation, Transform Parent = null) {
        if(Objects[Name].Count == 1) {
            Instantiate(Objects[Name].Peek().gameObject, Position, Rotation, Parent);
            return;
        }

        ObjectPoolObject obj = Objects[Name].Pop();

        obj.transform.position = Position;
        obj.transform.rotation = Rotation;
        obj.transform.parent = Parent;

        obj.gameObject.SetActive(true);
    }

    public void RepoolObject(ObjectPoolObject obj) {
        obj.gameObject.SetActive(false);
        obj.transform.position = Vector2.zero;
        obj.transform.parent = transform;
        Objects[obj.PoolName].Push(obj);
    }
}

[System.Serializable]
public struct Pool {
    public string Name;
    public GameObject Obj;
    public int InitialCount;
}