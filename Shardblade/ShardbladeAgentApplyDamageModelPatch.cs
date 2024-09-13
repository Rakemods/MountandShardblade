// Falskian, 10/14/20

using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using SandBox;
using SandBox.GameComponents;

namespace MountandShardblade.Shardblade
{
    /*
     * These patches allow a Shardblade to
     * slice through shields and blocks.
     * 
     * Note: We can probably create our own model rather
     * than patching the existing ones. The main advantage
     * of patching is we don't need to constantly reupdate
     * if Taleworlds changes the model in updates.
     * 
     * See: MissionGameModels.Current.AgentApplyDamageModel;
     */
    namespace MountandShardblade
    {
        [HarmonyPatch]
        public static class ShardbladeAgentApplyDamageModelPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(MultiplayerAgentApplyDamageModel), "DecideCrushedThrough")]
            private static void DecideCrushedThroughForShardbladeMultiplayer(Agent attackerAgent, Agent defenderAgent, WeaponComponentData defendItem, ref bool __result)
            {
                if (ShardPatchLogic.ShouldSliceThrough(attackerAgent, defenderAgent, null, defendItem))
                {
                    __result = true;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), "DecideCrushedThrough")]
            private static void DecideCrushedThroughForShardbladeSandbox(Agent attackerAgent, Agent defenderAgent, WeaponComponentData defendItem, ref bool __result)
            {
                if (ShardPatchLogic.ShouldSliceThrough(attackerAgent, defenderAgent, null, defendItem))
                {
                    __result = true;
                }
            }
        }
    }
}