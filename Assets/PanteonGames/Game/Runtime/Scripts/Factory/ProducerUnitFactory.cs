using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Model;

namespace PanteonGames.Game.Runtime.Factory
{
    public interface IProducerUnitFactory : IPool<ProducerUnitModel>
    {
        static IProducerUnitFactory Instance;
        ProducerUnitModel GetUnit(string configKey);
    }

    public class ProducerUnitFactory : Pool<ProducerUnitModel>, IProducerUnitFactory
    {
        public ProducerUnitFactory(int instanceAmount) : base(instanceAmount){}

        protected override ProducerUnitModel CreateInstance()
        {
            return new ProducerUnitModel()
            {
                Type = UnitType.Producer
            };
        }

        public ProducerUnitModel GetUnit(string configKey)
        {
            ProducerUnitModel unitModel = PullObject();
            ProducerUnitObjectConfig unitConfig = IConfigLoader.Instance.GetProducerUnitConfig(configKey);

            UnitFactoryHelper.SetBaseUnitModel(unitModel, unitConfig);
            unitModel.ProductSpawnPoint.Set(unitConfig.ProductSpawnPointX, unitConfig.ProductSpawnPointY);
            unitModel.ProducibleUnitKeys = unitConfig.ProducibleUnitKeys;

            return unitModel;
        }
    }
}