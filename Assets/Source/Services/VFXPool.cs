using System.Collections.Generic;
using UnityEngine;

public class VFXPool
{
    GameObject pf;
    List<GameObject> free = new List<GameObject>();
    float releaseAfter;

    public VFXPool(GameObject prefab, float releaseDelay)
    {
        releaseAfter = releaseDelay;
        pf = prefab;
    }

    public GameObject Get()
    {
        if (free.Count > 0)
        {
            var freePf = free[0];
            free.RemoveAt(0);
            freePf.GetComponent<PoolHandle>().Reset();
            return freePf;
        }

        var newGameObject = Object.Instantiate(pf);
        var poolHandle = newGameObject.AddComponent<PoolHandle>();
        poolHandle.pool = this;
        poolHandle.delay = releaseAfter;
        return newGameObject;
    }

    public void Release(GameObject go)
    {
        free.Add(go);
    }
}

public class PoolHandle : MonoBehaviour
{
    public VFXPool pool;
    public float delay;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        
        var particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null)
            particleSystem?.Play();
        gameObject.BroadcastMessage("ResetFromPool", SendMessageOptions.DontRequireReceiver);
        
        Invoke(nameof(Release), delay);
    }

    void Release()
    {
        gameObject.SetActive(false);
        pool.Release(gameObject);
    }
}