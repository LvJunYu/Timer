using GameA.Game;
using Spine.Unity;

namespace GameA
{
    public static class JoyFunctionEx
    {
        public static void Reset(this SkeletonAnimation skeletonAnimation)
        {
            if (skeletonAnimation != null && skeletonAnimation.state != null && skeletonAnimation.Skeleton != null)
            {
                skeletonAnimation.state.ClearTracks();
                skeletonAnimation.Skeleton.SetToSetupPose();
                skeletonAnimation.Update(ConstDefineGM2D.FixedDeltaTime);
            }
        }
    }
}