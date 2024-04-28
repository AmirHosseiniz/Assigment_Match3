using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledParticleSystem : PooledMonobehaviour
{
    public ParticleSystem MyParticleSystem { get; protected set; }

    protected virtual void Awake()
    {
        MyParticleSystem = GetComponent<ParticleSystem>();
    }

    public PooledParticleSystem Create()
    {
        PooledParticleSystem output = Get<PooledParticleSystem>();
        output.MyParticleSystem.Clear();
        return output;
    }

    public PooledParticleSystem Create(Vector3 position)
    {
        PooledParticleSystem output = Create();
        output.transform.position = position;
        output.MyParticleSystem.Clear();
        return output;
    }

    public PooledParticleSystem Create(Vector3 position, Quaternion rotation)
    {
        PooledParticleSystem output = Create(position);
        output.transform.rotation = rotation;
        output.MyParticleSystem.Clear();
        return output;
    }

    public PooledParticleSystem Create(Transform position) => Create(position.position);

    public PooledParticleSystem Create_Rotation(Transform positionAndRotation) => Create(positionAndRotation.position, positionAndRotation.rotation);

    public PooledParticleSystem Create_Parented(Transform parent)
    {
        var instance = Create_Rotation(parent);
        instance.transform.SetParent(parent);
        instance.transform.localScale = Vector3.one;
        return instance;
    }

    public void Create_Void(Transform position) => Create(position);

    public void Create_Rotation_Void(Transform positionAndRotation) => Create_Rotation(positionAndRotation);

    public void Create_Parented_Void(Transform parent) => Create_Parented(parent);
}
