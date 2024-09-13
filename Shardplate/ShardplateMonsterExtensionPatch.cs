using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using MountandShardblade.Core;
using MountandShardblade.Shardplate;
using TaleWorlds.CampaignSystem;

namespace MountandShardblade.Shardplate
{
    // Patch for the "CreateAgent" method in MissionLogic
    [HarmonyPatch(typeof(MissionLogic), "CreateAgent")]
    public static class ShardplateCreateAgentPatch
    {
        [HarmonyPrefix]
        private static void UpdateAgentStatsForShardplateBearer(ref Monster monster, ref float stepSize, BasicCharacterObject characterObject)
        {
            if (ShardplateMissionBehavior.IsPlateBearer(characterObject))
            {
                stepSize = 30f;

                if (characterObject is CharacterObject heroCharacter && heroCharacter.IsHero)
                {
                    float wealth = heroCharacter.HeroObject.Gold;
                    Agent agent = Mission.Current.SpawnAgent(new AgentBuildData(characterObject).Monster(monster));
                    ShardplateAgentComponent shardplateComponent = new(agent, wealth);
                    agent.AddComponent(shardplateComponent);

                    shardplateComponent.StormlightSystem.SetCurrentStormlight(shardplateComponent.StormlightSystem.MaxStormlight);

                    if (ShardplateMissionBehavior.IsHighstormActive())
                    {
                        shardplateComponent.ApplyHighstormEffect();
                    }
                }
            }
        }
    }

    // Patch for the "SpawnAgent" method in MissionLogic
    [HarmonyPatch(typeof(MissionLogic), "SpawnAgent")]
    public static class ShardplateSpawnAgentPatch
    {
        [HarmonyPrefix]
        private static void SpawnAgentPatch(ref AgentBuildData agentBuildData)
        {
            BasicCharacterObject agentCharacter = agentBuildData.AgentCharacter;

            if (agentCharacter != null && ShardplateMissionBehavior.IsPlateBearer(agentCharacter))
            {
                Monster shardplateBearerMonster = MBObjectManager.Instance.GetObject<Monster>("shardplate");
                agentBuildData = agentBuildData.Monster(shardplateBearerMonster);

                Agent agent = Mission.Current.SpawnAgent(agentBuildData);

                if (agentCharacter is CharacterObject heroCharacter && heroCharacter.IsHero)
                {
                    float wealth = heroCharacter.HeroObject.Gold;
                    ShardplateAgentComponent shardplateComponent = new(agent, wealth);
                    agent.AddComponent(shardplateComponent);

                    shardplateComponent.StormlightSystem.SetCurrentStormlight(shardplateComponent.StormlightSystem.MaxStormlight);

                    if (ShardplateMissionBehavior.IsHighstormActive())
                    {
                        shardplateComponent.ApplyHighstormEffect();
                    }

                    if (ShardplateMissionBehavior.ShouldSummonShardblade(agentCharacter))
                    {
                        shardplateComponent.InitiateShardbladeSummon();
                    }
                }
            }
        }
    }
}
