namespace PanteonGames.Game.Runtime.Model
{
    public class PathNodeModel
    {
        public PathNodeModel PreviousNode;
        public Vector2Int GridPosition;
        public int DistancePointToStart, DistancePointToEnd;
        public int PathPoint => DistancePointToStart + DistancePointToEnd;
    }
}