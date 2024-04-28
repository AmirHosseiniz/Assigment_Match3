using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemGraphicGravityAnimator_AnimatorTrigger : GridItemGraphicGravityAnimator
{
    [SerializeField] Animator animator;
    [SerializeField, NaughtyAttributes.AnimatorParam(nameof(animator), AnimatorControllerParameterType.Trigger)] string trigger;

    protected override void AnimateGravity()
    {
        animator.SetTrigger(trigger);
    }
}
