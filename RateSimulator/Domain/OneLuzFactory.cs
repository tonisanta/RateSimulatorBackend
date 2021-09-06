namespace RateSimulator.Domain
{
    public class OneLuzFactory : IRateFactory
    {
        private readonly ConfigurationPeriods config;
        private readonly PriceConfiguration priceConfig;

        public OneLuzFactory(ConfigurationPeriods config, PriceConfiguration priceConfig)
        {
            this.config = config;
            this.priceConfig = priceConfig;
        }

        public IRate GetInstance()
        {
            return new OneLuzRate(config, priceConfig);
        }
    }
}
