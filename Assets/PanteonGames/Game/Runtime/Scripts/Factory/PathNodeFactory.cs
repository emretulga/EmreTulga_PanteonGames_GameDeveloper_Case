using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Model;

namespace PanteonGames.Game.Runtime.Factory
{
    public interface IPathNodeFactory : IPool<PathNodeModel>
    {
        static IPathNodeFactory Instance;
        PathNodeModel GetNode(PathNodeModel previousNode, Vector2Int gridPosition, int distancePointToStart, int distancePointToEnd);
    }

    public class PathNodeFactory : Pool<PathNodeModel>, IPathNodeFactory
    {
        public PathNodeFactory(int instanceAmount) : base(instanceAmount){}

        protected override PathNodeModel CreateInstance()
        {
            return new PathNodeModel();
        }

        public PathNodeModel GetNode(PathNodeModel previousNode, Vector2Int gridPosition, int distancePointToStart, int distancePointToEnd)
        {
            PathNodeModel pathNode = PullObject();
            pathNode.PreviousNode = previousNode;
            pathNode.GridPosition = gridPosition;
            pathNode.DistancePointToStart = distancePointToStart;
            pathNode.DistancePointToEnd = distancePointToEnd;

            return pathNode;
        }
    }
}