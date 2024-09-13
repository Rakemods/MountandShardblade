using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.InputSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.ScreenSystem;
using System;
using TaleWorlds.Library;
using MountandShardblade;
using Bannerlord.UIExtenderEx;
using SubModule = MountandShardblade.SubModule;

namespace MountandShardblade.Shardplate
{
    public class ShardplateMissionBehavior : MissionLogic
    {
        private static bool _missionBehaviorInitialized = false;
        private static bool _isHighstormActive = false;
        private int _playerKillCount = 0;    // Track player kills
        private int _enemyShardbearerCount = 0;  // Track how many shardbearers have been spawned

        public ShardplateMissionBehavior()
        {
            _missionBehaviorInitialized = false;
        }

        public static bool IsPlateBearer(BasicCharacterObject characterObject)
        {
            if (characterObject == null)
            {
                return false;
            }
            return characterObject.StringId == "plate_bearer";
        }

        public static bool IsHighstormActive()
        {
            return _isHighstormActive;
        }

        public static bool ShouldSummonShardblade(BasicCharacterObject characterObject)
        {
            if (characterObject == null)
            {
                return false;
            }
            return characterObject.StringId == "shardblade_summoner";  // Example condition
        }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Logic;

        public override void OnPreMissionTick(float dt)
        {
            base.OnPreMissionTick(dt);

            if (!_missionBehaviorInitialized && Agent.Main != null)
            {
                // Ensure player is shardbearer
                if (Agent.Main.Character != null)
                {
                    if (IsPlateBearer(Agent.Main.Character as BasicCharacterObject) || ShouldSummonShardblade(Agent.Main.Character as BasicCharacterObject))
                    {
                        var shardplateAgentComponent = new ShardplateAgentComponent(Agent.Main, Agent.Main.Health);
                        Agent.Main.AddComponent(shardplateAgentComponent);
                    }
                }

                InitializeBehavior();
            }
        }

        private void InitializeBehavior()
        {
            foreach (Agent agent in Mission.Current.Agents)
            {
                if (agent.Character != null && IsPlateBearer(agent.Character as BasicCharacterObject))
                {
                    var shardplateAgentComponent = new ShardplateAgentComponent(agent, agent.Health);
                    agent.AddComponent(shardplateAgentComponent);
                }
            }
            _missionBehaviorInitialized = true;
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);

            // Check if player killed the agent
            if (affectorAgent == Agent.Main && affectedAgent.Team != Agent.Main.Team)
            {
                _playerKillCount++;  // Increment player kill count

                if (_playerKillCount >= 50)  // Every 50 kills, spawn an enemy shardbearer
                {
                    _playerKillCount = 0;  // Reset kill count for next wave
                    _enemyShardbearerCount++;  // Increment enemy shardbearer count
                    SpawnEnemyShardbearer(_enemyShardbearerCount);  // Spawn the next shardbearer
                }
            }
        }

        private Team GetEnemyTeam(Team playerTeam)
        {
            foreach (Team team in Mission.Current.Teams)
            {
                if (team != playerTeam)
                {
                    return team;  // Return the first team that is not the player's team
                }
            }
            return null;  // Handle cases where no enemy team is found
        }

        private void SpawnEnemyShardbearer(int shardbearerLevel)
        {
            BasicCharacterObject shardbearerCharacter = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("enemy_shardbearer");

            if (shardbearerCharacter != null && Mission.Current != null)
            {
                Team enemyTeam = GetEnemyTeam(Agent.Main.Team);  // Fetch enemy team

                if (enemyTeam != null)
                {
                    MatrixFrame spawnFrame = new()
                    {
                        origin = Agent.Main.Position + new Vec3(0, 5, 0)  // Position near the player
                    };

                    AgentBuildData agentBuildData = new AgentBuildData(shardbearerCharacter)
                        .Team(enemyTeam)
                        .InitialPosition(spawnFrame.origin)
                        .TroopOrigin(null);  // For a random troop origin

                    Agent enemyShardbearer = Mission.Current.SpawnAgent(agentBuildData);
                    if (enemyShardbearer != null)
                    {
                        // Ensure health is set properly
                        enemyShardbearer.Health = 150f + shardbearerLevel * 25f;

                        // Add any shardplate/shardblade components
                        var shardplateComponent = new ShardplateAgentComponent(enemyShardbearer, enemyShardbearer.Health);
                        enemyShardbearer.AddComponent(shardplateComponent);
                    }

                }
            }
        }


        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            // Check if the key assigned to summoning the shardblade is pressed (Q key in this case)
            if (Input.IsKeyPressed(SubModule.ShardbladeSummonKey))
            {
                // Debug message to confirm that the key was detected
                InformationManager.DisplayMessage(new InformationMessage("Shardblade Summon Key Pressed"));

                // Call the shardblade summon logic
                HandleShardbladeSummon();
            }
        }

        private void HandleShardbladeSummon()
        {
            // Logic to summon the shardblade
        }
    }
}
