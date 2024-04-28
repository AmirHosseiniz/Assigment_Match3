using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Match3InputDetector : MonoBehaviour
{
    public abstract bool IsClickStart();
    
    public abstract bool IsClickEnd();

    public abstract Ray GetRay();
}
