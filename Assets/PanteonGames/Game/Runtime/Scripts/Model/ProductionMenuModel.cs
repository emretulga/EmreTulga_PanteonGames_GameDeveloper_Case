namespace PanteonGames.Game.Runtime.Model
{
    public class ProductionMenuModel
    {
        public static ProductionMenuModel Instance;

        public UnitModel ChoosenProduct;
        public Vector2Int LastHoveredPosition = new(-1, -1);
    }
}