using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Model;
using Unity.VisualScripting;

namespace PanteonGames.Game.Runtime.Factory
{
    public interface IUnitFactory : IPool<UnitModel>
    {
        static IUnitFactory Instance;
        UnitModel GetUnit(string configKey);
    }

    public class UnitFactory : Pool<UnitModel>, IUnitFactory
    {
        public UnitFactory(int instanceAmount) : base(instanceAmount){}

        protected override UnitModel CreateInstance()
        {
            return new UnitModel()
            {
                Type = UnitType.Default
            };
        }

        public UnitModel GetUnit(string configKey)
        {
            UnitModel unitModel = PullObject();
            UnitObjectConfig unitConfig = IConfigLoader.Instance.GetUnitConfig(configKey);

            UnitFactoryHelper.SetBaseUnitModel(unitModel, unitConfig);

            return unitModel;
        }
    }
}