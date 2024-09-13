using TaleWorlds.CampaignSystem;

namespace MountandShardblade.Core
{
    public class StormlightEconomy
    {
        private readonly StormlightSystem _stormlightSystem;
        private HighstormManager _highstormManager;

        public StormlightEconomy(StormlightSystem stormlightSystem, HighstormManager highstormManager)
        {
            _stormlightSystem = stormlightSystem;
            _highstormManager = highstormManager;
        }

        public void OnHighstormOccur()
        {
            float stormlightGained = _highstormManager.GenerateStormlight();
            _stormlightSystem.IncreaseMaxStormlight(stormlightGained);
        }

        public bool PurchaseItem(float stormlightCost)
        {
            return _stormlightSystem.CanConsumeStormlight(stormlightCost)
                && _stormlightSystem.ConsumeStormlight(stormlightCost);  // Corrected: ConsumeStormlight to return bool
        }
    }
}
