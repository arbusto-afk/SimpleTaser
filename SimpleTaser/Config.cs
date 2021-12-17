using Rocket.API;

namespace SimpleTaser
{
    public class Config : IRocketPluginConfiguration
    {
        public int TaserId;
        public int TaserTime;

        public void LoadDefaults()
        {
            TaserId = 7471;
            TaserTime = 6;
        }
    }
}