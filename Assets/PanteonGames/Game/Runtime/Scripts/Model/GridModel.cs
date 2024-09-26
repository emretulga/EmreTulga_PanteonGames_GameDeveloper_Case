namespace PanteonGames.Game.Runtime.Model
{
    public class GridModel
    {
        public Vector2Int Position;
        public UnitModel PlacedUnit = null;

        public GridModel(int x, int y)
        {
            Position.X = x;
            Position.Y = y;
        }

        public GridModel(Vector2Int position)
        {
            Position.X = position.X;
            Position.Y = position.Y;
        }
    }
}