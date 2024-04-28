using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using UnityEngine;

public interface IGridItem 
{
    public event System.Action<bool> OnTopChanged;
    public event System.Action<float> HighlightChanged;
    public event System.Action GravityApplied;
    
    public abstract TypeEnum Type { get; }
    
    public Transform transform { get; }
    
    public GameObject gameObject { get; }
     
    public GridCell GridCell { get; set; }
    
    public bool OnTop { get; set; }
    
    public float Highlight { get; set; }
    
    /// <summary>
    /// Call this on items inside GridCells that have matched
    /// </summary>
    /// <returns>Whether the item is finished and can be removed from GridCell</returns>
    public bool Explode();

    /// <summary>
    /// Call this on the item inside a GridCell that has been clicked
    /// </summary>
    /// <returns>Whether the item is finished and can be removed from GridCell</returns>
    public bool Click();

    public void OnGravityApplied();
}
