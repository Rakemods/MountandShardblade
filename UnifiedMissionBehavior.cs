using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using MountandShardblade.Shardblade;
using MountandShardblade.Shardplate;
using TaleWorlds.Core;
using MountandShardblade.Util;
using TaleWorlds.ScreenSystem;
using TaleWorlds.GauntletUI;
using MountandShardblade.GUI;
using TaleWorlds.Library;
using System;
using TaleWorlds.GauntletUI.Data;
using Logger = MountandShardblade.Util.Logger;
using TaleWorlds.Engine.GauntletUI;

namespace MountandShardblade
{
    public class UnifiedMissionBehavior : MissionBehavior
    {
        private static bool shardbladeSummoning;
        private static DateTime summonStartTime;
        private ShardbladeViewModel _shardbladeViewModel;
        private static ShardplateHealthVM shardplateHealthVM;
        private static GameEntity? summonParticleEntity;
        private GauntletLayer _gauntletLayer;
        private IGauntletMovie _shardbladeMovie;
        private GauntletMovie _shardbladeSummonMovie;
        private bool _shardbladeSummoning;
        private DateTime _summonStartTime;

        public UnifiedMissionBehavior()
        {
            _shardbladeSummoning = false;
            _gauntletLayer = null;  // Ensure it starts as null
        }

        private void InitializeComponents()
        {
            var agentMain = Agent.Main;
            if (agentMain == null)
            {
                Logger.Instance().Log("Agent.Main is null. Aborting component initialization.", LogSeverity.Error);
                return;
            }

            var shardbladeAgent = agentMain.GetComponent<ShardbladeAgentComponent>();
            if (shardbladeAgent == null)
            {
                Logger.Instance().Log("ShardbladeAgentComponent is missing, attempting to add it.", LogSeverity.Warning);
                shardbladeAgent = new ShardbladeAgentComponent(agentMain, 10000); // Placeholder wealth
                agentMain.Components.Add(shardbladeAgent);
                Logger.Instance().Log("Added ShardbladeAgentComponent.", LogSeverity.Info);
            }

            var shardplateAgent = agentMain.GetComponent<ShardplateAgentComponent>();
            if (shardplateAgent == null)
            {
                shardplateAgent = new ShardplateAgentComponent(agentMain, 500f);  // Placeholder for shardplate functionality
                agentMain.Components.Add(shardplateAgent);
                Logger.Instance().Log("Added ShardplateAgentComponent.", LogSeverity.Info);
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            if (Mission.Current == null || Mission.Current.MissionEnded || Agent.Main == null)
            {
                Logger.Instance().Log("Mission ended or Agent.Main is null, skipping further updates.", LogSeverity.Warning);
                return;
            }

            try
            {
                InitializeComponents();
                HandleShardbladeSummon();
                UpdateParticleEffects();
                HandleShardplateHealthUI();
            }
            catch (Exception ex)
            {
                Logger.Instance().Log($"Exception during OnMissionTick: {ex.Message}", LogSeverity.Error);
            }
        }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private void UpdateParticleEffects()
        {
            var shardbladeAgent = Agent.Main?.GetComponent<ShardbladeAgentComponent>();
            if (shardbladeAgent != null && !shardbladeAgent.BladeSummoned)
            {
                Logger.Instance().Log("Starting particle effects for shardblade summoning.", LogSeverity.Info);
                var particleEffect = ShardParticleContainer.ShardbladeSummonParticleEffect ?? "default_particle_effect";
                summonParticleEntity = ShardPatchLogic.CreateEmptyGameEntityAtAgent(Agent.Main, 1f);
                if (summonParticleEntity != null)
                {
                    MatrixFrame localFrame = MatrixFrame.Identity;
                    ParticleSystem.CreateParticleSystemAttachedToEntity(particleEffect, summonParticleEntity, ref localFrame);
                    Logger.Instance().Log($"Particle effect {particleEffect} created successfully.", LogSeverity.Info);
                }
                else
                {
                    Logger.Instance().Log("Failed to create summonParticleEntity.", LogSeverity.Error);
                }
            }
        }

        private void HandleShardbladeSummon()
        {
            Agent playerAgent = Agent.Main;
            if (playerAgent == null)
            {
                Logger.Instance().Log("Player Agent is null.", LogSeverity.Error);
                return;
            }

            var shardbladeAgent = playerAgent.GetComponent<ShardbladeAgentComponent>();
            if (shardbladeAgent == null)
            {
                Logger.Instance().Log("ShardbladeAgentComponent is null. Cannot summon shardblade.", LogSeverity.Error);
                return;
            }

            if (Input.IsKeyPressed(SubModule.ShardbladeSummonKey))
            {
                Logger.Instance().Log("Q key pressed. Starting shardblade summoning.", LogSeverity.Info);

                if (!_shardbladeSummoning && !shardbladeAgent.BladeSummoned)
                {
                    StartSummoningShardblade();
                }
                else if (_shardbladeSummoning && (DateTime.Now - _summonStartTime).TotalSeconds >= SubModule.SummonTimeSeconds)
                {
                    CompleteShardbladeSummoning(shardbladeAgent);
                }
                else
                {
                    // Update the summoning progress on the UI slider
                    float currentSummonTime = (float)(DateTime.Now - _summonStartTime).TotalSeconds;
                    _shardbladeViewModel.UpdateSlider(currentSummonTime);
                }
            }
            else if (_shardbladeSummoning)
            {
                Logger.Instance().Log("Summon key released. Cancelling summoning.", LogSeverity.Info);
                CancelShardbladeSummoning();
            }
        }

        private void StartSummoningShardblade()
        {
            _shardbladeSummoning = true;
            _summonStartTime = DateTime.Now;

            // Initialize the UI Layer and ViewModel
            if (_gauntletLayer == null)
            {
                _shardbladeViewModel = new ShardbladeViewModel();
                _gauntletLayer = new GauntletLayer(1, "GauntletLayer");
                _shardbladeSummonMovie = (GauntletMovie)_gauntletLayer.LoadMovie("ShardbladeSummonUI", _shardbladeViewModel);
                ScreenManager.TopScreen.AddLayer(_gauntletLayer);
            }

            _shardbladeViewModel.ShardbladeSummonText = "Summoning Shardblade...";
            _shardbladeViewModel.MaxTime = (float)SubModule.SummonTimeSeconds;
        }

        private void CompleteShardbladeSummoning(ShardbladeAgentComponent shardbladeAgent)
        {
            _shardbladeSummoning = false;
            shardbladeAgent.SpawnBladeInHands(true);
            Logger.Instance().Log("Shardblade summoned successfully.", LogSeverity.Info);

            _shardbladeViewModel.CompleteSummon();

            // Remove UI Layer after summoning is complete
            if (_gauntletLayer != null)
            {
                ScreenManager.TopScreen.RemoveLayer(_gauntletLayer);
                _gauntletLayer = null;
            }
        }

        private void CancelShardbladeSummoning()
        {
            _shardbladeSummoning = false;
            _shardbladeViewModel.ResetSummon();

            // Remove the UI Layer after canceling
            if (_gauntletLayer != null)
            {
                ScreenManager.TopScreen.RemoveLayer(_gauntletLayer);
                _gauntletLayer = null;
            }
        }

        private void HandleShardplateHealthUI()
        {
            var shardplateAgent = Agent.Main?.GetComponent<ShardplateAgentComponent>();
            if (shardplateAgent != null)
            {
                if (shardplateHealthVM == null)
                {
                    shardplateHealthVM = new ShardplateHealthVM();
                    shardplateHealthVM.Activate(ScreenManager.TopScreen as ScreenBase, shardplateAgent.GetMaxShardplateHealth());
                }

                shardplateHealthVM.ShardplateHealth = shardplateAgent.GetShardplateHealth();
            }
        }
    }
}
