using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameplayEnums.Cells;
using UnityEngine;

public class Dot : GridItem
{
    public event System.Action<Dot> ColorGroupSet;
    public event System.Action<Dot> Exploded;
    public static event System.Action<Vector3, int> DotExploded;
    
    public override TypeEnum Type => TypeEnum.dot;

    public override GridCell GridCell { get; set; }
    
    int _colorGroup;
    public int ColorGroup
    {
        get => _colorGroup;
        private set
        {
            _colorGroup = value;
            ColorGroupSet?.Invoke(this);
        }
    }

    public Dot Create(int colorGroup, Vector3 position, Quaternion rotation, Transform parent)
    {
        var instance = Get<Dot>();
        instance.transform.SetParent(parent);
        instance.transform.position = position;
        instance.transform.localScale = Vector3.one;
        instance.transform.rotation = rotation;
        instance.ColorGroup = colorGroup;
        return instance;
    }

    public override bool Explode()
    {
        Exploded?.Invoke(this);
        DotExploded?.Invoke(transform.position, ColorGroup);
        gameObject.SetActive(false);
        return true;
    }

    public override bool Click()
    {
        return false;
    }
}
