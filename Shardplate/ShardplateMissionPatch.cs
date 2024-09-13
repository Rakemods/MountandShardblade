using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace MountandShardblade.Shardplate
{
    // Update patch target to Agent.OnMeleeHit
    [HarmonyPatch(typeof(Agent), "OnMeleeHit")]
    public static class ShardbladeMissionPatch
    {
        private const float ShardbladeBaseMomentum = 1000f;

        // Prefix to modify the momentum when certain conditions are met (before the hit)
        [HarmonyPrefix]
        public static bool MeleeHitCallbackForShardbladeWielderPrefix(ref AttackCollisionData collisionData, Agent attacker, Agent victim, ref float inOutMomentumRemaining)
        {
            if (attacker == null || victim == null)
            {
                return true;  // Skip patch if attacker or victim is null
            }

            // Apply shardblade-specific logic
            if (ShardPatchLogic.ShouldSliceThrough(attacker, victim, collisionData, null))
            {
                inOutMomentumRemaining = ShardbladeBaseMomentum;
            }

            return true;  // Continue with the original method.
        }

        // Postfix to modify hit particles and momentum after the hit
        [HarmonyPostfix]
        public static void MeleeHitCallbackForShardbladeWielderPostfix(ref AttackCollisionData collisionData, Agent attacker, Agent victim, ref float inOutMomentumRemaining, ref HitParticleResultData hitParticleResultData)
        {
            if (attacker == null || victim == null)
            {
                return;
            }

            // If a slice-through event occurs, reset particle effects and set momentum
            if (ShardPatchLogic.ShouldSliceThrough(attacker, victim, collisionData, null))
            {
                hitParticleResultData.Reset();  // Reset particle effects
                inOutMomentumRemaining = ShardbladeBaseMomentum;  // Set base momentum
            }
        }

        // Postfix to update collision reactions during weapon collisions
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Agent), "DecideWeaponCollisionReaction")]
        public static void DecideWeaponCollisionReactionForShardbladeWielder(ref AttackCollisionData collisionData, Agent attacker, Agent defender, ref MeleeCollisionReaction colReaction)
        {
            // If the attacker should slice through, update the collision reaction to SlicedThrough
            if (ShardPatchLogic.ShouldSliceThrough(attacker, defender, collisionData, null))
            {
                colReaction = MeleeCollisionReaction.SlicedThrough;
            }
        }

        // Struct to handle hit particle results
        public struct HitParticleResultData
        {
            public int StartHitParticleIndex;
            public int ContinueHitParticleIndex;
            public int EndHitParticleIndex;

            // Method to reset the particle data
            public void Reset()
            {
                StartHitParticleIndex = -1;
                ContinueHitParticleIndex = -1;
                EndHitParticleIndex = -1;
            }
        }
    }
}
