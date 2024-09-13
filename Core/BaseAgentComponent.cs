using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using System;
using System.Collections.Generic;
using MountandShardblade.Shardblade;
using MountandShardblade.Shardplate;
using TaleWorlds.Library;

namespace MountandShardblade.Core
{
    public abstract class BaseAgentComponent : AgentComponent
    {
        protected const float ShardplateBasicDamage = 5f;
        protected const double DashAbilitySeconds = 1.5;
        protected float _shardplateHealth;
        protected float _shardplateHealthMax;
        protected bool _isDashing = false;
        protected DateTime _dashStart = DateTime.MinValue;
        protected HashSet<Agent>? _dashingHitAgents;
        protected ShardplateParticleHandler _particleHandler;
        protected StormlightSystem _stormlightsystem;

        // Constructor to initialize Shardplate and StormlightManager
        public BaseAgentComponent(Agent agent, float wealth) : base(agent)
        {
            _shardplateHealthMax = 100f;
            _shardplateHealth = _shardplateHealthMax;
            _stormlightsystem = new StormlightSystem(100f, wealth * 0.01f, 0.1f);  // regenRate provided here
        }

        // Method to apply damage to the Shardplate
        protected void ApplyDamageToShardplate(float damage)
        {
            _shardplateHealth -= damage;
            if (_shardplateHealth <= 0f)
            {
                _shardplateHealth = 0f;
                _particleHandler?.TriggerBreakEffect();
                Mission.Current.MakeSoundOnlyOnRelatedPeer(ShardSoundContainer.SoundCodeShardPlateBreak(), Agent.Frame.origin, Agent.Index);
            }
        }

        // Method to handle dash ability logic
        protected void HandleDash()
        {
            if (_dashStart != DateTime.MinValue)
            {
                if (DateTime.Now - _dashStart >= TimeSpan.FromSeconds(DashAbilitySeconds))
                {
                    _isDashing = false;
                    _dashStart = DateTime.MinValue;
                    _dashingHitAgents = null;
                }
                else
                {
                    _isDashing = true;
                    _dashingHitAgents ??= new HashSet<Agent>();
                    ApplyBlowToNearbyAgents(2f, 10);  // Apply blow while dashing
                }
            }
        }

        // Method to apply blow to nearby agents
        protected void ApplyBlowToNearbyAgents(float radius, int damage)
        {
            MBList<Agent> nearbyAgents = new();
            Mission.Current.GetNearbyAgents(Agent.Position.AsVec2, radius, nearbyAgents);
            foreach (Agent nearbyAgent in nearbyAgents)
            {
                if (nearbyAgent == Agent || nearbyAgent.Team == Agent.Team || _dashingHitAgents?.Contains(nearbyAgent) == true)
                    continue;

                // Create a blow and register it with the nearby agent
                Blow blow = new(Agent.Index)
                {
                    BlowFlag = BlowFlags.KnockDown,
                    Direction = (nearbyAgent.Position - Agent.Position).NormalizedCopy(),
                    InflictedDamage = damage,
                    BaseMagnitude = 10f,
                    DamageCalculated = true
                };
                nearbyAgent.RegisterBlow(blow, new AttackCollisionData());
                _dashingHitAgents?.Add(nearbyAgent);
            }
        }

        // Get speed based on Shardplate health
        public float GetSpeedBasedOnHealth(float baseSpeed)
        {
            if (_shardplateHealth > _shardplateHealthMax * 0.5f)
            {
                return baseSpeed;
            }
            else if (_shardplateHealth > _shardplateHealthMax * 0.25f)
            {
                return baseSpeed * 0.8f;
            }
            else if (_shardplateHealth > 0f)
            {
                return baseSpeed * 0.66f;
            }
            else
            {
                return baseSpeed * 0.5f;
            }
        }

        // Mission tick logic for stormlight regeneration and handling dash
        public virtual void MissionTick(float dt)
        {
            _stormlightsystem.RegenerateStormlight(dt);  // Fixed _stormlightManager to _stormlightsystem
            HandleDash();
            _particleHandler?.UpdateParticles();
        }
    }
}
