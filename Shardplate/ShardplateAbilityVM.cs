using TaleWorlds.Library;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.ScreenSystem;

namespace MountandShardblade.Shardplate
{
    /*
     * This class handles the UI for the Shardplate Abilities (Leap, Dash).
     * Refactored to include modularity and extensibility for additional abilities.
     */
    class ShardplateAbilityVM : ViewModel
    {
        private const string ShardplateAbilityXMLName = "ShardplateAbilityUI";

        private float _maxTime;
        private float _currentTime;
        private string _abilityText;
        private GauntletMovie _currentMovie;
        private ScreenBase _screen;
        private GauntletLayer _gauntletLayer;

        public enum ShardplateAbility
        {
            Leap,
            Dash,
            // Future abilities can be added here
        }

        // Constructor
        public ShardplateAbilityVM()
        {
            CurrentTime = 0.0f;
            MaxTime = 0.0f;
            AbilityText = "";
            _gauntletLayer = new GauntletLayer(100, "GauntletLayer");
        }

        [DataSourceProperty]
        public float MaxTime
        {
            get => _maxTime;
            set
            {
                if (value != _maxTime)
                {
                    _maxTime = value;
                    OnPropertyChangedWithValue(value, nameof(MaxTime));
                }
            }
        }

        [DataSourceProperty]
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                if (value != _currentTime)
                {
                    _currentTime = value;
                    OnPropertyChangedWithValue(value, nameof(CurrentTime));
                }
            }
        }

        [DataSourceProperty]
        public string AbilityText
        {
            get => _abilityText;
            set
            {
                if (value != null && !value.Equals(_abilityText))
                {
                    _abilityText = value;
                    OnPropertyChangedWithValue(value, nameof(AbilityText));
                }
            }
        }

        // Activate the UI for an ability with its duration
        public void Activate(ScreenBase screen, ShardplateAbility ability, double abilityTimeSeconds)
        {
            CurrentTime = 0.0f;
            AbilityText = ability == ShardplateAbility.Leap ? "Charging Leap" : "Charging Dash";
            MaxTime = (float)abilityTimeSeconds;

            _currentMovie = (GauntletMovie)_gauntletLayer.LoadMovie(ShardplateAbilityXMLName, this);
            _screen = screen;
            _screen.AddLayer(_gauntletLayer);
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        // Deactivate the UI for the current ability
        public void Deactivate()
        {
            if (_screen != null)
            {
                ScreenManager.TryLoseFocus(_gauntletLayer);
                _gauntletLayer.ReleaseMovie(_currentMovie);
                _screen.RemoveLayer(_gauntletLayer);
                _screen = null;
            }
        }

        // Update the UI during the mission tick
        public void MissionTick(float summonCurrentTime)
        {
            if (_screen != null)
            {
                CurrentTime = summonCurrentTime;
            }
        }
    }
}