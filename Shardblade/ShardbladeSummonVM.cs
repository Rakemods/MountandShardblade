using TaleWorlds.Library;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using TaleWorlds.GauntletUI.Data;

namespace MountandShardblade.Shardblade
{
    public class ShardbladeSummonVM : ViewModel
    {
        private float _currentTime;
        private float _maxTime;
        private string _summonText;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _summonMovie;

        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        public float MaxTime
        {
            get => _maxTime;
            set
            {
                if (_maxTime != value)
                {
                    _maxTime = value;
                    OnPropertyChanged(nameof(MaxTime));
                }
            }
        }

        public string SummonText
        {
            get => _summonText;
            set
            {
                if (_summonText != value)
                {
                    _summonText = value;
                    OnPropertyChanged(nameof(SummonText));
                }
            }
        }

        public ShardbladeSummonVM()
        {
            MaxTime = 5.0f;
            CurrentTime = 0.0f;
            SummonText = "Summoning Shardblade...";
            _gauntletLayer = new GauntletLayer(100);
        }

        public void Activate(ScreenBase screen, float summonDuration)
        {
            MaxTime = summonDuration;
            CurrentTime = 0.0f;
            SummonText = "Summoning Blade";

            _summonMovie = (GauntletMovie)_gauntletLayer.LoadMovie("ShardbladeSummonUI", this);
            screen.AddLayer(_gauntletLayer);
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        public void Deactivate(ScreenBase screen)
        {
            screen.RemoveLayer(_gauntletLayer);
            ScreenManager.TryLoseFocus(_gauntletLayer);
            _gauntletLayer.ReleaseMovie(_summonMovie);
        }

        public void UpdateSummonProgress(float currentTime)
        {
            CurrentTime = currentTime;
        }
    }
}
