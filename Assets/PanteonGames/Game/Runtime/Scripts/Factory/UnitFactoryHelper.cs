using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Model;

namespace PanteonGames.Game.Runtime.Factory
{
    public static class UnitFactoryHelper
    {
        public static void SetBaseUnitModel(UnitModel unitModel, UnitObjectConfig unitConfig)
        {
            unitModel.Name = unitConfig.Name;
            unitModel.Size.Set(unitConfig.SizeX, unitConfig.SizeY);
            unitModel.SetMaxHealth(unitConfig.Health);
            unitModel.SetHealth(unitConfig.Health);
        }

        public static UnitModel GetUnit(UnitType unitType, string productKey)
        {
            return unitType switch
            {
                UnitType.Default => IUnitFactory.Instance.GetUnit(productKey),
                UnitType.Attacker => IAttackerUnitFactory.Instance.GetUnit(productKey),
                UnitType.Producer => IProducerUnitFactory.Instance.GetUnit(productKey),
                _ => IUnitFactory.Instance.GetUnit(productKey)
            };
        }

        public static void PoolObject(this UnitModel unitModel)
        {
            switch(unitModel.Type)
            {
                case UnitType.Default:
                    IUnitFactory.Instance.PoolObject(unitModel);
                    break;
                case UnitType.Attacker:
                    IAttackerUnitFactory.Instance.PoolObject((AttackerUnitModel)unitModel);
                    break;
                case UnitType.Producer:
                    IProducerUnitFactory.Instance.PoolObject((ProducerUnitModel)unitModel);
                    break;
            }
        }
    }
}