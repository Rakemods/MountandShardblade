using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MountandShardblade.Shardplate
{
    public class ShardplateParticleHandler
    {
        private readonly Agent _agent;

        public ShardplateParticleHandler(Agent agent)
        {
            _agent = agent;
        }

        // Trigger the shardplate break particle effect
        public void TriggerBreakEffect()
        {
            GameEntity entity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(_agent, 1f);
            MatrixFrame localFrame = MatrixFrame.Identity;
            ParticleSystem.CreateParticleSystemAttachedToEntity(ShardParticleContainer.ShardplateBreakParticleEffect, entity, ref localFrame);
        }

        // Update any ongoing particle effects if necessary
        public void UpdateParticles()
        {
            // Implement updates to ongoing particle effects, if any.
        }
    }
}
