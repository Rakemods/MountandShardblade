using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using System.Collections.Generic;
using System;
using TaleWorlds.ObjectSystem;
using MountandShardblade;
using Bannerlord.UIExtenderEx;
using SubModule = MountandShardblade.SubModule;
using MountandShardblade.Util;
using Logger = MountandShardblade.Util.Logger;

namespace MountandShardblade.Shardblade
{
    class ShardbladeMissionBehavior : MissionLogic, IMissionBehavior
    {
        private const double ShardbladeSummonParticleTimeSeconds = 1f;
        private const double ShardbladeKillParticleTimeSeconds = 0.5f;

        private static DateTime _summonStart;
        private static double _summonTimeSeconds;
        private static ShardbladeSummonVM _shardbladeVM;
        private static bool _activelySummoning;
        private static ItemObject _bladeItemObject;
        private static Dictionary<GameEntity, DateTime> _eyeBurnParticleDictionary = new();
        private static GameEntity _summonParticleEntity;
        private static DateTime _summonParticleEntityStart;

        public ShardbladeMissionBehavior()
        {
            _summonParticleEntity = null;
            _activelySummoning = false;
        }

        public override void OnPreMissionTick(float dt)
        {
            base.OnPreMissionTick(dt);
            if (Agent.Main != null)
            {
                InitializeShardblade();
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (Agent.Main != null)
            {
                ShardbladeAgentComponent shardbladeAgentComponent = Agent.Main.GetComponent<ShardbladeAgentComponent>();
                if (Input.IsKeyPressed(SubModule.ShardbladeSummonKey))
                {
                    Logger.Instance().Log("Shardblade summoning initiated.", LogSeverity.Info);
                    HandleShardbladeSummon(shardbladeAgentComponent);
                }

                HandleSummonedParticleEffects();
                HandleEyeBurnParticleEffects();
            }
        }

        public override void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnEarlyAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
            if (affectedAgent != null && affectorAgent != null && agentState == AgentState.Killed && ShardPatchLogic.IsWieldingShardblade(affectorAgent))
            {
                GameEntity gameEntity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(affectedAgent, 1f);
                MatrixFrame localFrame = MatrixFrame.Identity;
                ParticleSystem.CreateParticleSystemAttachedToEntity(ShardParticleContainer.ShardbladeKillParticleEffect, gameEntity, ref localFrame);
                _eyeBurnParticleDictionary.Add(gameEntity, DateTime.Now);
            }
        }

        protected override void OnEndMission()
        {
            _eyeBurnParticleDictionary.Clear();
            _summonParticleEntity = null;
        }

        private void InitializeShardblade()
        {
            _bladeItemObject = MBObjectManager.Instance.GetObject<ItemObject>(SubModule.ShardbladeID);

            foreach (Agent agent in Mission.Current.Agents)
            {
                if (IsBladeBearer(agent))
                {
                    var shardbladeAgentComponent = new ShardbladeAgentComponent(agent, 1000); // Example wealth
                    agent.AddComponent(shardbladeAgentComponent);
                }
            }
        }

        private static bool IsBladeBearer(Agent agent)
        {
            return agent != null && agent.IsMainAgent;
        }

        private void HandleShardbladeSummon(ShardbladeAgentComponent shardbladeAgentComponent)
        {
            if (!shardbladeAgentComponent.BladeSummoned && !_activelySummoning)
            {
                _activelySummoning = true;
                _summonStart = DateTime.Now;
                _shardbladeVM.Activate(ScreenManager.TopScreen as ScreenBase, (float)_summonTimeSeconds);
            }

            if (_activelySummoning && (DateTime.Now - _summonStart).TotalSeconds >= SubModule.SummonTimeSeconds)
            {
                shardbladeAgentComponent.SpawnBladeInHands(true);
                _shardbladeVM.Deactivate(ScreenManager.TopScreen as ScreenBase);
                _activelySummoning = false;

                GameEntity gameEntity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(Agent.Main, 1f);
                MatrixFrame localFrame = MatrixFrame.Identity;
                _summonParticleEntity = gameEntity;
                ParticleSystem.CreateParticleSystemAttachedToEntity(ShardParticleContainer.ShardbladeSummonParticleEffect, gameEntity, ref localFrame);
                _summonParticleEntityStart = DateTime.Now;
            }
            else if (!Input.IsKeyDown(SubModule.ShardbladeSummonKey))
            {
                _shardbladeVM.Deactivate(ScreenManager.TopScreen as ScreenBase);
                _activelySummoning = false;
            }
        }

        private void HandleSummonedParticleEffects()
        {
            if (_summonParticleEntity != null && (DateTime.Now - _summonParticleEntityStart).TotalSeconds >= ShardbladeSummonParticleTimeSeconds)
            {
                _summonParticleEntity.RemoveAllParticleSystems();
                _summonParticleEntity = null;
            }
        }

        private void HandleEyeBurnParticleEffects()
        {
            foreach (var entity in _eyeBurnParticleDictionary.Keys)
            {
                if ((DateTime.Now - _eyeBurnParticleDictionary[entity]).TotalSeconds >= ShardbladeKillParticleTimeSeconds)
                {
                    entity.RemoveAllParticleSystems();
                }
            }
        }
    }
}
