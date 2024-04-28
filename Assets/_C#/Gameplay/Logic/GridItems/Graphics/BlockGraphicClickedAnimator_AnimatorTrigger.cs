using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGraphicClickedAnimator_AnimatorTrigger : BlockGraphicClickedAnimator
{
    [SerializeField] Animator animator;
    [SerializeField, NaughtyAttributes.AnimatorParam(nameof(animator), AnimatorControllerParameterType.Trigger)] string trigger;

    protected override void AnimateClicked()
    {
        animator.SetTrigger(trigger);
    }
}
