// HighstormManager.cs
using System;

namespace MountandShardblade.Core
{
    public class HighstormManager
    {
        private readonly float stormlightGeneratedPerStorm;

        public HighstormManager(float stormlightPerStorm)
        {
            stormlightGeneratedPerStorm = stormlightPerStorm;
        }
        public bool IsHighstormActive { get; private set; }
        public DateTime HighstormStart { get; private set; }

        public HighstormManager()
        {
            IsHighstormActive = false;
        }

        public void StartHighstorm()
        {
            HighstormStart = DateTime.Now;
            IsHighstormActive = true;
        }

        public void EndHighstorm()
        {
            IsHighstormActive = false;
        }

        public void UpdateHighstorm(float deltaTime)
        {
            if (IsHighstormActive && (DateTime.Now - HighstormStart).TotalMinutes > 10)
            {
                EndHighstorm();
            }
        }
        public float GenerateStormlight()
        {
            // Simulate the stormlight generated during a highstorm
            return stormlightGeneratedPerStorm;
        }
    }
}
