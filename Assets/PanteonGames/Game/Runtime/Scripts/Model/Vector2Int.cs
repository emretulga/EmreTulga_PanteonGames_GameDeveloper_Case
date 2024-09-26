namespace PanteonGames.Game.Runtime.Model
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(Vector2Int position)
        {
            X = position.X;
            Y = position.Y;
        }
    }
}