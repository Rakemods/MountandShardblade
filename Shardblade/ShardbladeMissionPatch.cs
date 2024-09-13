// Falskian, 10/13/20

using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace MountandShardblade.Shardblade
{
    [HarmonyPatch(typeof(Mission), nameof(Mission.OnTick))]
    public static class ShardbladeMissionPatch
    {
        // If a Shardblade slices through an object, that means it met no resistance
        // Thus, it has a constant momentum if it's slicing through multiple opponents
        private const float ShardbladeBaseMomentum = 1000f;

        /*
         * This prefix adds momentum to a Shardblade
         * hit, which in turn increases the damage
         */
        [HarmonyPrefix]
        [HarmonyPatch("MeleeHitCallback")]
        private static void MeleeHitCallbackForShardbladeWielderPrefix(ref AttackCollisionData collisionData, Agent attacker, Agent victim, ref float inOutMomentumRemaining)
        {
            if (!ShardPatchLogic.ShouldSliceThrough(attacker, victim, collisionData, null))
            {
                return;
            }

            // TODO: the momentum could be scaled with collisionData.AttackProgress
            // Altering momentumRemainingToComputeDamage in this prefix is what makes
            // the Shardblade deal so much damage
            float momentumRemainingToComputeDamage = ShardbladeBaseMomentum;
            inOutMomentumRemaining = ShardbladeBaseMomentum;
        }

        /*
         * This postfix modifies the momentum as
         * the Shardblade slices through multiple
         * opponents. It also removes blood particle
         * effects when an Agent is sliced by a Shardblade
         */
        [HarmonyPostfix]
        [HarmonyPatch("MeleeHitCallback")]
        private static void MeleeHitCallbackForShardbladeWielderPostfix(ref AttackCollisionData collisionData, Agent attacker, Agent victim, ref float inOutMomentumRemaining, ref HitParticleResultData hitParticleResultData)
        {
            if (!ShardPatchLogic.ShouldSliceThrough(attacker, victim, collisionData, null))
            {
                return;
            }

            // Removing blood particle effects when an Agent is sliced with a Shardblade
            hitParticleResultData.Reset();

            // TODO: Determine if this bit is really necessary
            // My guess is it's not, since in our case we always
            // apply a flat momentum for each Shardblade callback
            // in the prefix
            inOutMomentumRemaining = ShardbladeBaseMomentum;
        }

        /*
         * This postfix allows a Shardblade to
         * slice through multiple opponents
         */
        [HarmonyPostfix]
        [HarmonyPatch("DecideWeaponCollisionReaction")]
        private static void DecideWeaponCollisionReactionForShardbladeWielder(ref AttackCollisionData collisionData, Agent attacker, Agent defender, ref MeleeCollisionReaction colReaction)
        {
            if (ShardPatchLogic.ShouldSliceThrough(attacker, defender, collisionData, null))
            {
                colReaction = MeleeCollisionReaction.SlicedThrough;
            }
        }

        /*
         * This struct was copied and pasted from
         * the Taleworlds .dll for patching purposes
         */
        internal struct HitParticleResultData
        {
            public void Reset()
            {
                this.StartHitParticleIndex = -1;
                this.ContinueHitParticleIndex = -1;
                this.EndHitParticleIndex = -1;
            }

            public int StartHitParticleIndex;

            public int ContinueHitParticleIndex;

            public int EndHitParticleIndex;
        }
    }
}
