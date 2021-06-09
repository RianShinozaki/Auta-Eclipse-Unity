using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolObject : MonoBehaviour {
    public string PoolName;

    public void RePool() {
        ObjectPool.Instance.RepoolObject(this);
    }
}