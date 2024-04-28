using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : PooledMonobehaviour , IPooledObject
{
    public ParticleSystem Particle;
    public float ParticleDuration;

    Pool pool;

#if UNITY_EDITOR
    [ContextMenu("Find ps")]
    void FindParticle()
    {
        Particle = GetComponent<ParticleSystem>();
        ParticleDuration = Particle.main.duration;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
    public override void ObjectInitiated()
    {
        OnDestroyEvent();
    }
    void CountDown()
    {
        Invoke("Finish", ParticleDuration);
    }
    public void Finish()
    {
        Particle.Stop();
        OnDestroyEvent();
    }
    public override void OnDisable()
    {
    }
    public void Attach(Transform target)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public void SetForward(Vector3 Direction)
    {
        transform.forward = Direction;
    }
    public void PlayOneTimeUse()
    {
        CountDown();
        Particle.Play();
    }

    public ParticlePool Play(Vector3 pos,float size = 1, bool Auto = true)
    {
        if (pool == null)
        {
            pool = Pool.GetPool(this);
        }
        ParticlePool newp = Get<ParticlePool>(true);
        Vector3 Size = Vector3.one * size;
        newp.transform.localScale = Size;
        newp.transform.localPosition = pos;
        newp.Particle.Play();
        if (Auto)
            newp.CountDown();
        return newp;
    }
}