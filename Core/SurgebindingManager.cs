namespace MountandShardblade.Core
{
    public class SurgebindingManager
    {
        private StormlightSystem _stormlightSystem;

        public SurgebindingManager(float maxStormlight, float initialStormlight, float regenRate)
        {
            _stormlightSystem = new StormlightSystem(maxStormlight, initialStormlight, regenRate);
        }

        public void RegenerateStormlight(float deltaTime)
        {
            _stormlightSystem.RegenerateStormlight(deltaTime);
        }

        public bool UseSurgebindingPower(float stormlightCost)
        {
            if (_stormlightSystem.CanConsumeStormlight(stormlightCost))
            {
                _stormlightSystem.ConsumeStormlight(stormlightCost);
                return true;
            }
            return false;
        }
    }
}
