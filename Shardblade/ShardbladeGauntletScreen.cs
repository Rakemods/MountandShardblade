using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.GauntletUI;
using TaleWorlds.Engine.GauntletUI;
using System;
using TaleWorlds.ScreenSystem;
using TaleWorlds.GauntletUI.Data;
using MountandShardblade.GUI;

namespace MountandShardblade.Shardblade
{
    public class ShardbladeGauntletScreen : ScreenBase
    {
        private ShardbladeViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _dataSource = new ShardbladeViewModel();  // Replace with your actual ViewModel
            _gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            AddLayer(_gauntletLayer);
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            _movie = (GauntletMovie)_gauntletLayer.LoadMovie("ShardbladeGauntletMovie", _dataSource);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_gauntletLayer);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(_gauntletLayer);
            _dataSource = null;
            _gauntletLayer = null;
        }
    }
}
