using TaleWorlds.Library;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using TaleWorlds.GauntletUI.Data;

namespace MountandShardblade.Shardplate
{
    public class ShardplateHealthVM : ViewModel
    {
        private float _shardplateHealth;
        private float _shardplateHealthMax;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _healthMovie;

        public float ShardplateHealth
        {
            get => _shardplateHealth;
            set
            {
                if (_shardplateHealth != value)
                {
                    _shardplateHealth = value;
                    OnPropertyChanged(nameof(ShardplateHealth));
                }
            }
        }

        public float ShardplateHealthMax
        {
            get => _shardplateHealthMax;
            set
            {
                if (_shardplateHealthMax != value)
                {
                    _shardplateHealthMax = value;
                    OnPropertyChanged(nameof(ShardplateHealthMax));
                }
            }
        }

        public void Activate(ScreenBase screen, float healthMax)
        {
            ShardplateHealthMax = healthMax;
            ShardplateHealth = ShardplateHealthMax;

            _gauntletLayer = new GauntletLayer(100);
            _healthMovie = (GauntletMovie)_gauntletLayer.LoadMovie("ShardplateHealthUI", this);
            screen.AddLayer(_gauntletLayer);
        }

        public void Deactivate(ScreenBase screen)
        {
            screen.RemoveLayer(_gauntletLayer);
            ScreenManager.TryLoseFocus(_gauntletLayer);
            _gauntletLayer.ReleaseMovie(_healthMovie);
        }
    }
}
