using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
	private static Dictionary<PooledMonobehaviour, Pool> pools = new Dictionary<PooledMonobehaviour, Pool> ();

	private Queue<PooledMonobehaviour> objects = new Queue<PooledMonobehaviour>();
	private List<PooledMonobehaviour> disabledObjects = new List<PooledMonobehaviour>();

	private PooledMonobehaviour prefab;

	public PooledMonobehaviour[] PreloadTargets;
	public void Start()
	{
		//for (int i = 0; i < PreloadTargets.Length; i++)
		//{
		//	PreloadTargets[i].PreLoad();
		//}
	}

	public static void InitPool()
	{
		pools = new Dictionary<PooledMonobehaviour, Pool>();
	}
	public static void CleanPool()
	{
		List<PooledMonobehaviour> CleanedPools = new List<PooledMonobehaviour>();
		foreach (var Manager in pools)
		{
			if (Manager.Key.OneTimeUse)
			{
				print("Cleaning : " + Manager.Key.name);
				CleanedPools.Add(Manager.Key);
				Manager.Value.Clean();
			}
		}
		for (int i = 0; i < CleanedPools.Count; i++)
		{
			pools.Remove(CleanedPools[i]);
		}
	}
	public static Pool GetPool (PooledMonobehaviour prefab)
	{
        if (pools.ContainsKey(prefab) && pools[prefab] == null)
            pools.Remove(prefab);

        if (pools.ContainsKey (prefab))
			return pools [prefab];

		var pool = new GameObject ("Pool-" + (prefab as Component).name).AddComponent<Pool> ();
		pool.prefab = prefab;

		pool.GrowPool (prefab.InitialPoolSize);
		pools.Add (prefab, pool);
		return pool;
	}
	public static Pool GetPool(PooledMonobehaviour prefab, int WarmupCount)
	{
		if (pools.ContainsKey(prefab) && pools[prefab] == null)
			pools.Remove(prefab);

		if (pools.ContainsKey(prefab))
			return pools[prefab];

		var pool = new GameObject("Pool-" + (prefab as Component).name).AddComponent<Pool>();
		pool.prefab = prefab;

		pool.GrowPool(WarmupCount);
		pools.Add(prefab, pool);
		return pool;
	}

	public static void MakePool(PooledMonobehaviour prefab, bool IsParticle)
	{
		if (pools.ContainsKey(prefab) && pools[prefab] == null)
			pools.Remove(prefab);
		if (pools.ContainsKey(prefab))
			return;

		var pool = new GameObject("Pool-" + (prefab as Component).name).AddComponent<Pool>();
		pool.prefab = prefab;

		pool.GrowPool(prefab.InitialPoolSize);
		pools.Add(prefab, pool);
	}

	void Clean()
	{
		for (int i = 0; i < disabledObjects.Count; i++)
		{
			if(disabledObjects[i] != null)
				Destroy(disabledObjects[i].gameObject);
		}
		disabledObjects.Clear();
		Destroy(gameObject);
	}
	public T Get<T> () where T : PooledMonobehaviour
	{
		if (objects.Count == 0) {
			GrowPool (prefab.InitialPoolSize);
		}

		var pooledObject = objects.Dequeue ();

		return pooledObject as T;
	}

	public void GrowPool (int Count)
	{
		for (int i = 0; i < Count; i++) {
			var pooledObject = Instantiate (this.prefab) as PooledMonobehaviour;
			(pooledObject as Component).gameObject.name += " " + i;

			pooledObject.OnDestroyEvent = () => AddObjectToAvailable (pooledObject);
            pooledObject.ObjectInitiated();
			(pooledObject as Component).gameObject.SetActive (false);
		}
	}

	private void Update ()
	{
		MakeDisabledObjectsChildren ();
	}

	private void MakeDisabledObjectsChildren ()
	{
		if (disabledObjects.Count > 0) {
			foreach (var pooledObject in disabledObjects) {
				if (pooledObject.gameObject.activeInHierarchy == false) {
					(pooledObject as Component).transform.SetParent (transform, false);
				}
			}
			disabledObjects.Clear ();
		}
	}

	private void AddObjectToAvailable (PooledMonobehaviour pooledObject)
	{
		disabledObjects.Add (pooledObject);
		objects.Enqueue (pooledObject);
	}
}