using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using MountandShardblade.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using Bannerlord.UIExtenderEx;
using System;
using MountandShardblade;
using MountandShardblade.Util;
using SubModule = MountandShardblade.SubModule;
using Logger = MountandShardblade.Util.Logger;

namespace MountandShardblade.Shardblade
{
    public class ShardbladeAgentComponent : BaseAgentComponent
    {
        public bool BladeSummoned { get; set; }
        public ItemObject BladeItemObject { get; set; }
        public MissionWeapon? BladeMissionWeapon { get; set; }
        private readonly Agent _agent;
        private readonly float _wealth;

        // Constructor for the ShardbladeAgentComponent
        public ShardbladeAgentComponent(Agent agent, float wealth) : base(agent, wealth)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent), "Agent cannot be null.");
            _wealth = wealth;

            BladeSummoned = false;

            // Check if ShardbladeID is valid
            if (string.IsNullOrEmpty(SubModule.ShardbladeID))
            {
                Logger.Instance().Log("ShardbladeID is null or empty. Unable to fetch ItemObject.", LogSeverity.Error);
            }
            else
            {
                BladeItemObject = MBObjectManager.Instance.GetObject<ItemObject>(SubModule.ShardbladeID);

                // Check if BladeItemObject was successfully retrieved
                if (BladeItemObject == null)
                {
                    Logger.Instance().Log($"Shardblade ItemObject with ID '{SubModule.ShardbladeID}' not found!", LogSeverity.Error);
                }
            }

            BladeMissionWeapon = null; // Initialize to null, as it's nullable
        }

        // Start summoning the Shardblade
        public void StartSummoningShardblade()
        {
            if (!BladeSummoned)
            {
                Logger.Instance().Log("Started summoning shardblade.", LogSeverity.Info);
                UpdateParticleEffects(true);
            }
            else
            {
                Logger.Instance().Log("Shardblade already summoned.", LogSeverity.Warning);
            }
        }

        // Spawn the Shardblade into the agent's hands
        public void SpawnBladeInHands(bool inHands)
        {
            if (inHands)
            {
                if (BladeItemObject != null)
                {
                    // Initialize BladeMissionWeapon if it's still null
                    if (BladeMissionWeapon == null)
                    {
                        BladeMissionWeapon = new MissionWeapon(BladeItemObject, null, null, 0);
                        Logger.Instance().Log("Shardblade weapon created and ready for summoning.", LogSeverity.Info);
                    }

                    Agent.EquipItemsFromSpawnEquipment(true);
                    Agent.TryToWieldWeaponInSlot(EquipmentIndex.Weapon1, Agent.WeaponWieldActionType.WithAnimation, true);
                    BladeSummoned = true;
                    Logger.Instance().Log("Shardblade equipped and wielded by the agent.", LogSeverity.Info);

                    // Stop particle effects after summoning is done
                    UpdateParticleEffects(false);
                }
                else
                {
                    Logger.Instance().Log("Failed to spawn shardblade. BladeItemObject is null.", LogSeverity.Error);
                }
            }
            else
            {
                Logger.Instance().Log("Shardblade not summoned in hand as 'inHands' is false.", LogSeverity.Warning);
            }
        }

        // AI-controlled tick method
        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);
            try
            {
                MissionTick(dt);
            }
            catch (Exception ex)
            {
                Logger.Instance().LogException(ex, nameof(OnTickAsAI));
            }
        }

        // Main mission tick method
        private void MissionTick(float dt)
        {
            HandleDash();
            UpdateParticleEffects();
        }

        // Update particle effects based on the summoning status
        private void UpdateParticleEffects(bool isSummoning = false)
        {
            if (isSummoning)
            {
                Logger.Instance().Log("Summoning particle effects initiated.", LogSeverity.Info);

                GameEntity summonEntity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(Agent, 1f);
                MatrixFrame localFrame = MatrixFrame.Identity;
                ParticleSystem.CreateParticleSystemAttachedToEntity("summon_particle_fx", summonEntity, ref localFrame);

                Logger.Instance().Log("Particle effect 'summon_particle_fx' created successfully.", LogSeverity.Info);
            }
            else
            {
                Logger.Instance().Log("Removing particle effects.", LogSeverity.Info);

                GameEntity entity = Agent.AgentVisuals.GetEntity();
                entity.RemoveAllParticleSystems();

                Logger.Instance().Log("Particle effects cleaned up after summoning.", LogSeverity.Info);
            }
        }

        // Placeholder method for handling dash functionality
        private void HandleDash()
        {
            // Dash logic can be added later when required
        }

        internal void UpdateParticleEffects()
        {
            throw new NotImplementedException();
        }
    }
}
