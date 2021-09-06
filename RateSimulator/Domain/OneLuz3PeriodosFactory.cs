namespace RateSimulator.Domain
{
    public class OneLuz3PeriodosFactory : IRateFactory
    {
        private readonly ConfigurationPeriods periodsConfig;
        private readonly PriceConfiguration priceConfig;

        public OneLuz3PeriodosFactory(ConfigurationPeriods config, PriceConfiguration priceConfig)
        {
            this.periodsConfig = config;
            this.priceConfig = priceConfig;
        }

        public IRate GetInstance()
        {
            return new OneLuz3PeriodosRate(periodsConfig, priceConfig);
        }
    }
}
