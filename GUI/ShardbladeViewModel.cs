using TaleWorlds.Library;

namespace MountandShardblade.GUI
{
    public class ShardbladeViewModel : ViewModel
    {
        private float _currentTime;
        private float _maxTime;
        private string _shardbladeSummonText;

        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                if (value != _currentTime)
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
                if (value != _maxTime)
                {
                    _maxTime = value;
                    OnPropertyChanged(nameof(MaxTime));
                }
            }
        }

        public string ShardbladeSummonText
        {
            get => _shardbladeSummonText;
            set
            {
                if (value != _shardbladeSummonText)
                {
                    _shardbladeSummonText = value;
                    OnPropertyChanged(nameof(ShardbladeSummonText));
                }
            }
        }

        public ShardbladeViewModel()
        {
            MaxTime = 5.0f;  // Example max time
            CurrentTime = 0.0f;  // Initial summon progress
            ShardbladeSummonText = "Summoning Shardblade...";
        }

        // Call this to update the summoning progress slider
        public void UpdateSlider(float currentTime)
        {
            CurrentTime = currentTime;
        }

        // Call this when the summoning completes
        public void CompleteSummon()
        {
            ShardbladeSummonText = "Shardblade Summoned!";
            CurrentTime = MaxTime;
        }

        // Reset the summoning UI in case of cancellation
        public void ResetSummon()
        {
            ShardbladeSummonText = "Summoning Cancelled.";
            CurrentTime = 0.0f;
        }
    }
}
