using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using MountandShardblade.Shardplate;

namespace MountandShardblade
{
    public static class ShardPatchLogic
    {
        public static GameEntity CreateEmptyGameEntityAtAgent(Agent agent, float elevateAmount)
        {
            if (agent == null || Mission.Current == null)
            {
                return null;
            }

            GameEntity gameEntity = GameEntity.CreateEmpty(Mission.Current.Scene);
            MatrixFrame matrixFrame = agent.Frame;
            matrixFrame.Elevate(elevateAmount);
            gameEntity.SetFrame(ref matrixFrame);
            return gameEntity;
        }

        public static bool ShouldSliceThrough(Agent attacker, Agent victim, AttackCollisionData? collisionData, WeaponComponentData defendItem)
        {
            if (attacker == null || !IsWieldingShardblade(attacker))
                return false;

            if (victim != null)
            {
                if (attacker.Team != victim.Team)
                {
                    if (defendItem != null && defendItem.IsShield)
                    {
                        EquipmentIndex offHandEquipmentIndex = victim.GetWieldedItemIndex((Agent.HandIndex)EquipmentIndex.Weapon2);
                        if (offHandEquipmentIndex != EquipmentIndex.None && IsHalfShardShield(defendItem, victim.Equipment[offHandEquipmentIndex].Item))
                            return false;
                    }

                    ShardplateAgentComponent shardplate = victim.GetComponent<ShardplateAgentComponent>();
                    if (shardplate == null || shardplate.GetShardplateHealth() <= 0)
                        return true;
                }
            }
            return false;
        }

        public static bool IsHalfShardShield(WeaponComponentData defendItem, ItemObject defendItemObject)
        {
            return defendItem != null && defendItem.IsShield && defendItemObject != null && defendItemObject.StringId == "half_shard_shield";
        }

        public static bool IsShardBlade(WeaponComponentData defendItem, ItemObject defendItemObject)
        {
            return defendItem != null && defendItem.IsMeleeWeapon && defendItemObject != null && defendItemObject.StringId == "shardblade_default";
        }

        public static bool IsWieldingShardblade(Agent agent)
        {
            return agent != null && agent.HasWeapon() && agent.WieldedWeapon.Item.StringId == "shardblade_default";
        }
    }
}
