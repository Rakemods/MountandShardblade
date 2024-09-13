using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace MountandShardblade.Shardplate
{
    public class ShardplateAgentStatCalculateModelPatch : AgentStatCalculateModel
    {
        private readonly AgentStatCalculateModel _originalModel;

        public ShardplateAgentStatCalculateModelPatch(AgentStatCalculateModel originalModel)
        {
            _originalModel = originalModel;
        }

        public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            _originalModel.UpdateAgentStats(agent, agentDrivenProperties);

            if (agent.GetComponent<ShardplateAgentComponent>() != null)
            {
                var shardplateComponent = agent.GetComponent<ShardplateAgentComponent>();
                float baseSpeed = agentDrivenProperties.MaxSpeedMultiplier;
                agentDrivenProperties.MaxSpeedMultiplier = shardplateComponent.GetSpeedBasedOnHealth(baseSpeed);
            }
        }

        public override bool CanAgentRideMount(Agent agent, Agent targetMount)
        {
            return _originalModel.CanAgentRideMount(agent, targetMount);
        }

        public override float GetDifficultyModifier() => _originalModel.GetDifficultyModifier();
        public override float GetDismountResistance(Agent agent) => _originalModel.GetDismountResistance(agent);
        public override float GetKnockBackResistance(Agent agent) => _originalModel.GetKnockBackResistance(agent);
        public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = StrikeType.Invalid) => _originalModel.GetKnockDownResistance(agent, strikeType);
        public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon) => _originalModel.GetWeaponDamageMultiplier(agent, weapon);
        public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
        {
            _originalModel.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
        }
    }
}
