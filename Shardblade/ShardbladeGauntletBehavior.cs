using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.GauntletUI;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;

namespace MountandShardblade.Shardblade
{
    public class ShardbladeGauntletBehavior : MissionBehavior
    {
        private readonly ShardbladeGauntletScreen _gauntletScreen;
        private bool _screenActive = false;

        public ShardbladeGauntletBehavior()
        {
            _gauntletScreen = new ShardbladeGauntletScreen();
        }

        public override void OnMissionTick(float dt)
        {
            if (!_screenActive)
            {
                ScreenManager.PushScreen(_gauntletScreen);
                _screenActive = true;
            }
        }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    }
}
