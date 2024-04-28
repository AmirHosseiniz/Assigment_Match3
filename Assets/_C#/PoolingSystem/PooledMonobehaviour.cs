using System;
using UnityEngine;

public class PooledMonobehaviour : MonoBehaviour, IPooledObject
{

    [SerializeField]
    private int initialPoolSize = 1;
    public bool OneTimeUse = false;
    [SerializeField] bool disableOnParentDisable = true;

    public bool DisableOnParentDisable { get => disableOnParentDisable; set => disableOnParentDisable = value; }

    public int InitialPoolSize { get { return initialPoolSize; } }

    public Action OnDestroyEvent;

    public virtual void ObjectInitiated()
    {

    }
    public virtual void PreLoad()
    {
        Pool.GetPool(this);
    }
    public virtual void PreLoad(int Count)
    {
        Pool.GetPool(this, Count);
    }

    public virtual void OnDisable()
    {
        if (disableOnParentDisable && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        if (!gameObject.activeSelf && OnDestroyEvent != null)
            OnDestroyEvent();
    }

    public T Get<T>(bool enable = true) where T : PooledMonobehaviour
    {
        var pool = Pool.GetPool(this);
        var pooledObject = pool.Get<T>();

        if (enable)
        {
            pooledObject.gameObject.SetActive(true);
        }

        return pooledObject;
    }

    public T Get<T>(Transform parent, bool resetTransform = false) where T : PooledMonobehaviour
    {
        var pooledObject = Get<T>(true);
        pooledObject.transform.SetParent(parent, false);

        if (resetTransform)
        {
            pooledObject.transform.localPosition = Vector3.zero;
            pooledObject.transform.localRotation = Quaternion.identity;
        }

        return pooledObject;
    }

    public T Get<T>(Transform parent, Vector3 relativePosition, Quaternion relativeRotation, bool enable = true) where T : PooledMonobehaviour
    {
        var pooledObject = Get<T>(enable);
        pooledObject.transform.SetParent(parent, false);
        pooledObject.transform.localPosition = relativePosition;
        pooledObject.transform.localRotation = relativeRotation;

        return pooledObject;
    }

}