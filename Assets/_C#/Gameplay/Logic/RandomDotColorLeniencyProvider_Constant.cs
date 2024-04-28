using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDotColorLeniencyProvider_Constant : RandomDotColorLeniencyProvider
{
    [SerializeField] float leniency;
    
    public override float Leniency => leniency;
}
