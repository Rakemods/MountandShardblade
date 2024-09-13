using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using MountandShardblade.Core;
using System;
using MountandShardblade.Util;
using TaleWorlds.Library;
using Logger = MountandShardblade.Util.Logger;

namespace MountandShardblade.Shardplate
{
    public class ShardplateAgentComponent : BaseAgentComponent
    {
        private new float _shardplateHealth;
        private new float _shardplateHealthMax;
        private new ShardplateParticleHandler _particleHandler;
        public StormlightSystem StormlightSystem { get; private set; }

        public ShardplateAgentComponent(Agent agent, float wealth) : base(agent, wealth)
        {
            _shardplateHealthMax = 100f;
            _shardplateHealth = _shardplateHealthMax;
            _stormlightsystem = new StormlightSystem(100f, wealth * 0.01f, 0.1f);  // regenRate provided here
        }

        // This method checks if the current agent is a plate bearer
        public bool IsPlateBearer()
        {
            // We check if the agent has shardplate health greater than zero and is initialized
            return _shardplateHealth > 0;
        }

        // Apply damage to Shardplate
        public void ApplyDamageToShardplate(float damage)
        {
            _shardplateHealth -= damage;
            if (_shardplateHealth <= 0f)
            {
                _shardplateHealth = 0f;
                _particleHandler?.TriggerBreakEffect();
                Mission.Current.MakeSoundOnlyOnRelatedPeer(ShardSoundContainer.SoundCodeShardPlateBreak(), Agent.Frame.origin, Agent.Index);
            }
        }


        public float GetShardplateHealth()
        {
            return _shardplateHealth;  // Inherited from BaseAgentComponent
        }

        public float GetMaxShardplateHealth()
        {
            return _shardplateHealthMax;  // Inherited from BaseAgentComponent
        }

        public void HandleMeleeDamageToShardplate(Agent attacker, float damage)
        {
            if (attacker == null || attacker.Team == Agent.Team) return;
            ApplyDamageToShardplate(damage);
        }

        public void UpdateParticleEffects()
        {
            if (Mission.Current == null || Mission.Current.MissionEnded || Agent == null)
            {
                Logger.Instance().Log("Mission has ended or Agent is null. Skipping particle updates.", LogSeverity.Warning);
                return;
            }

            if (_particleHandler == null)
            {
                Logger.Instance().Log("ParticleHandler is null. Initializing a new ParticleHandler.", LogSeverity.Warning);
                _particleHandler = new ShardplateParticleHandler(Agent);
            }

            if (ShardParticleContainer.ShardplateBreakParticleEffect != null)
            {
                GameEntity particleEntity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(Agent, 1f);
                if (particleEntity != null)
                {
                    MatrixFrame localFrame = MatrixFrame.Identity;
                    ParticleSystem.CreateParticleSystemAttachedToEntity(ShardParticleContainer.ShardplateBreakParticleEffect, particleEntity, ref localFrame);
                }
                else
                {
                    Logger.Instance().Log("Failed to create particle entity for shardplate effect.", LogSeverity.Warning);
                }
            }
            else
            {
                Logger.Instance().Log("ShardplateBreakParticleEffect is missing. No particle effect will be played.", LogSeverity.Warning);
            }

            _particleHandler.UpdateParticles();
        }

        public void ApplyHighstormEffect()
        {
            // Apply highstorm-specific effects here
        }

        public void InitiateShardbladeSummon()  // Fixed: Changed method name for shardblade summoning
        {
            // Logic to initiate shardblade summoning
        }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);
            MissionTick(dt);
        }

        private void MissionTick(float dt)
        {
            StormlightSystem.RegenerateStormlight(dt);  // Regenerates stormlight over time
            HandleDash();  // Handles dash logic from BaseAgentComponent
            UpdateParticleEffects();  // Updates particle effects based on shardplate condition
        }

        public void DebugShardplateStatus()
        {
            Logger.Instance().Log($"[TEST] Agent {Agent.Name} is plate bearer: {IsPlateBearer()}", LogSeverity.Info);
            Logger.Instance().Log($"[TEST] Current Shardplate Health: {_shardplateHealth}", LogSeverity.Info);
        }

        internal void UpdateShardplateHealthUI()
        {
            throw new NotImplementedException();
        }
    }
}
