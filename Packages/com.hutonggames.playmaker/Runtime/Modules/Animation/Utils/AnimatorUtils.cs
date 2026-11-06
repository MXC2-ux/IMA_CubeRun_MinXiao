using UnityEngine;

namespace HutongGames
{
    public class AnimatorUtils
    {
        public static bool IsPlaying(Animator anim, int animLayer, string stateName)
        {
            var stateInfo = anim.GetCurrentAnimatorStateInfo(animLayer);
            return MatchesStateName(stateInfo, stateName) && (stateInfo.loop || stateInfo.normalizedTime < 1.0f);
        }

        public static bool MatchesStateName(AnimatorStateInfo stateInfo, string stateName)
        {
            return !string.IsNullOrEmpty(stateName) &&
                   (stateInfo.IsName(stateName) || stateInfo.shortNameHash == Animator.StringToHash(stateName));
        }
    }
}
