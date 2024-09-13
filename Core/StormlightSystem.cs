using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

namespace MountandShardblade.Core
{
    public class StormlightSystem
    {
        public float CurrentStormlight { get; private set; }
        public float MaxStormlight { get; private set; }
        private float regenerationRate;

        public StormlightSystem(float maxStormlight, float initialStormlight, float regenRate)
        {
            MaxStormlight = maxStormlight;
            CurrentStormlight = initialStormlight;
            regenerationRate = regenRate;
        }

        public void RegenerateStormlight(float deltaTime)
        {
            CurrentStormlight = MathF.Min(MaxStormlight, CurrentStormlight + regenerationRate * deltaTime);
        }

        public bool CanConsumeStormlight(float amount)
        {
            return CurrentStormlight >= amount;
        }

        public bool ConsumeStormlight(float amount)
        {
            if (CanConsumeStormlight(amount))
            {
                CurrentStormlight -= amount;
                return true;
            }
            return false;
        }

        public void IncreaseMaxStormlight(float additionalMax)
        {
            MaxStormlight += additionalMax;
        }

        // Set accessor for CurrentStormlight
        public void SetCurrentStormlight(float stormlight)
        {
            CurrentStormlight = stormlight;
        }
    }
}
