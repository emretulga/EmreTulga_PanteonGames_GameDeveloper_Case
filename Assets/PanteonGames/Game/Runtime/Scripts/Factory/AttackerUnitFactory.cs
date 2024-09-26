using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Model;

namespace PanteonGames.Game.Runtime.Factory
{
    public interface IAttackerUnitFactory : IPool<AttackerUnitModel>
    {
        static IAttackerUnitFactory Instance;
        AttackerUnitModel GetUnit(string configKey);
        Vector2Int GetSizeOfUnit(string configKey);
    }

    public class AttackerUnitFactory : Pool<AttackerUnitModel>, IAttackerUnitFactory
    {
        public AttackerUnitFactory(int instanceAmount) : base(instanceAmount){}

        protected override AttackerUnitModel CreateInstance()
        {
            return new AttackerUnitModel()
            {
                Type = UnitType.Attacker
            };
        }

        public AttackerUnitModel GetUnit(string configKey)
        {
            AttackerUnitModel unitModel = PullObject();
            AttackUnitObjectConfig unitConfig = IConfigLoader.Instance.GetAttackUnitConfig(configKey);

            UnitFactoryHelper.SetBaseUnitModel(unitModel, unitConfig);
            unitModel.AttackDamage = unitConfig.Damage;

            return unitModel;
        }

        public Vector2Int GetSizeOfUnit(string configKey)
        {
            AttackUnitObjectConfig unitConfig = IConfigLoader.Instance.GetAttackUnitConfig(configKey);
            Vector2Int size = new (unitConfig.SizeX, unitConfig.SizeY);

            return size;
        }
    }
}