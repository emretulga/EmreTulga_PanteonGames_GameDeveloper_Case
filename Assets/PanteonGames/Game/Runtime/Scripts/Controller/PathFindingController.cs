using System;
using System.Collections.Generic;
using PanteonGames.Game.Runtime.Factory;
using PanteonGames.Game.Runtime.Model;

namespace PanteonGames.Game.Runtime.Controller
{
    public class PathFindingController
    {
        public static PathFindingController Instance;

        private readonly PathNodeModel[,] _nodes;

        private readonly List<PathNodeModel> _processingNodes = new ();
        private readonly List<PathNodeModel> _processedNodes = new ();
        private readonly List<PathNodeModel> _allNodes = new ();

        private readonly Stack<Vector2Int> _destination = new ();

        public PathFindingController(Vector2Int mapSize)
        {
            _nodes = new PathNodeModel[mapSize.X, mapSize.Y];
        }

        public Stack<Vector2Int> FindDestination(Vector2Int startPoint, Vector2Int endPoint, out Vector2Int avaiblePosition, out int pathPoint)
        {
            ClearNodes();
            Stack<Vector2Int> destination = null;
            avaiblePosition = endPoint;
            pathPoint = 0;

            CreatePathNodeAtPosition(null, startPoint, 0, 0);

            while(_processingNodes.Count != 0 && destination == null)
            {
                PathNodeModel lowestPathNode = GetLowestPointAndProcessingPathNode();
                pathPoint = lowestPathNode.PathPoint;
                destination = ProcessPathNode(lowestPathNode, endPoint);
            }

            if(destination == null)
            {
                PathNodeModel nearestNodeToEndPoint = GetLowestPointAndNearestPathNodeToPosition(endPoint);

                if(nearestNodeToEndPoint != null)
                {
                    avaiblePosition = nearestNodeToEndPoint.GridPosition;
                    pathPoint = nearestNodeToEndPoint.PathPoint;
                    destination = GetDestination(nearestNodeToEndPoint);
                }
            }

            return destination;
        }

        private PathNodeModel CreatePathNodeAtPosition(PathNodeModel previousNode, Vector2Int position, int distancePointToStart, int DistancePointToEnd)
        {
            PathNodeModel pathNode = IPathNodeFactory.Instance.GetNode(previousNode, position, distancePointToStart, DistancePointToEnd);
            _nodes[position.X, position.Y] = pathNode;
            _processingNodes.Add(pathNode);
            _allNodes.Add(pathNode);

            return pathNode;
        }

        private Stack<Vector2Int> ProcessPathNode(PathNodeModel processingNode, Vector2Int endPosition)
        {
            for(int neighbourPositionX = -1; neighbourPositionX <= 1; neighbourPositionX++)
            {
                for(int neighbourPositionY = -1; neighbourPositionY <= 1; neighbourPositionY++)
                {
                    bool isProcessingNodeEncountered = neighbourPositionX == 0 && neighbourPositionY == 0;
                    if(isProcessingNodeEncountered) continue;
                    
                    int neighbourPositionXOnMap = processingNode.GridPosition.X + neighbourPositionX;
                    int neighbourPositionYOnMap = processingNode.GridPosition.Y +  neighbourPositionY;
                    Vector2Int neighbourPosition = new (neighbourPositionXOnMap, neighbourPositionYOnMap);

                    bool isNeighborPositionXWithinLimits = neighbourPosition.X >= 0 && neighbourPosition.X < _nodes.GetLength(0);
                    bool isNeighborPositionYWithinLimits = neighbourPosition.Y >= 0 && neighbourPosition.Y < _nodes.GetLength(1);
                    bool isNeighbourWithinLimits = isNeighborPositionXWithinLimits && isNeighborPositionYWithinLimits;
                    if(!isNeighbourWithinLimits) continue;

                    bool isGridEmpty = GridController.Instance.IsGridEmpty(neighbourPosition);
                    if(!isGridEmpty) continue;

                    bool isPathFound = endPosition.X == neighbourPosition.X && endPosition.Y == neighbourPosition.Y;
                    if(isPathFound) return GetDestinationWithEndPoint(processingNode, endPosition);

                    PathNodeModel neighbourPathNode = _nodes[neighbourPosition.X, neighbourPosition.Y];
                    
                    if(neighbourPathNode != null)
                    {
                        bool isNeighbourPathNodeProcessable = _processingNodes.Contains(neighbourPathNode);
                        if(!isNeighbourPathNodeProcessable) continue;

                        TryToSetPreviousNodeOfNode(neighbourPathNode, processingNode);
                    }
                    else
                    {
                        int distanceBetweenTwoNodes = GetDistancePointBetweenTwoPoints(neighbourPosition, processingNode.GridPosition);
                        int distancePointToStart = processingNode.DistancePointToStart + distanceBetweenTwoNodes;
                        int distancePointToEnd = GetDistancePointBetweenTwoPoints(neighbourPosition, endPosition);

                        CreatePathNodeAtPosition(processingNode, neighbourPosition, distancePointToStart, distancePointToEnd);
                    }
                }
            }

            _processingNodes.Remove(processingNode);
            _processedNodes.Add(processingNode);

            return null;
        }

        private void TryToSetPreviousNodeOfNode(PathNodeModel pathNode, PathNodeModel newPreviousPathNode)
        {
            int distanceBetweenTwoNodes = GetDistancePointBetweenTwoPoints(newPreviousPathNode.GridPosition, pathNode.GridPosition);

            int desiredPathPoint = newPreviousPathNode.DistancePointToStart + distanceBetweenTwoNodes + pathNode.DistancePointToEnd;
            
            if(pathNode.PathPoint <= desiredPathPoint) return;

            pathNode.PreviousNode = newPreviousPathNode;
            pathNode.DistancePointToStart = newPreviousPathNode.DistancePointToStart + distanceBetweenTwoNodes;
        }

        private int GetDistancePointBetweenTwoPoints(Vector2Int nodePoint, Vector2Int endPoint)
        {
            int dx = Math.Abs(endPoint.X - nodePoint.X);
            int dy = Math.Abs(endPoint.Y - nodePoint.Y);

            int diagonalMoves = Math.Min(dx, dy);
            int straightMoves = Math.Abs(dx - dy);
            
            int distance = (diagonalMoves * 14) + (straightMoves * 10);

            return distance;
        }

        private int GetDistanceBetweenTwoPoints(Vector2Int nodePoint, Vector2Int endPoint)
        {
            int dx = Math.Abs(endPoint.X - nodePoint.X);
            int dy = Math.Abs(endPoint.Y - nodePoint.Y);

            int distance = Math.Max(dx, dy);

            return distance;
        }

        private PathNodeModel GetLowestPointAndProcessingPathNode()
        {
            PathNodeModel lowestPointNode = null;

            foreach(PathNodeModel node in _processingNodes)
            {                    
                if(lowestPointNode == null)
                {
                    lowestPointNode = node;
                }
                else if(node.PathPoint < lowestPointNode.PathPoint)
                {
                    lowestPointNode = node;
                }
            }

            return lowestPointNode;
        }

        private PathNodeModel GetLowestPointAndNearestPathNodeToPosition(Vector2Int pointedPosition)
        {
            PathNodeModel lowestPointNode = null;
            int nearestDistance = Math.Max(_nodes.GetLength(0), _nodes.GetLength(1));
            int lowestPathPoint = Math.Max(_nodes.GetLength(0), _nodes.GetLength(1)) * 14;

            foreach(PathNodeModel node in _allNodes)
            {
                int distance = GetDistanceBetweenTwoPoints(node.GridPosition, pointedPosition);

                if(lowestPointNode == null)
                {
                    nearestDistance = distance;
                    lowestPathPoint = node.PathPoint;
                    lowestPointNode = node;
                    continue;
                }

                if(distance < nearestDistance)
                {
                    nearestDistance = distance;
                    lowestPathPoint = node.PathPoint;
                    lowestPointNode = node;
                }

                if(distance > nearestDistance) continue;
                if(node.PathPoint >= lowestPathPoint) continue;

                nearestDistance = distance;
                lowestPathPoint = node.PathPoint;
                lowestPointNode = node;
            }

            return lowestPointNode;
        }

        private Stack<Vector2Int> GetDestination(PathNodeModel lastPathNode)
        {
            while(lastPathNode.PreviousNode != null)
            {
                _destination.Push(lastPathNode.GridPosition);

                lastPathNode = lastPathNode.PreviousNode;
            }
            
            return _destination;
        }

        private Stack<Vector2Int> GetDestinationWithEndPoint(PathNodeModel lastPathNode, Vector2Int endPosition)
        {
            _destination.Push(endPosition);
            
            return GetDestination(lastPathNode);
        }

        private void ClearNodes()
        {
            for(int nodesXIndex = 0; nodesXIndex < _nodes.GetLength(0); nodesXIndex++)
            {
                for(int nodesYIndex = 0; nodesYIndex < _nodes.GetLength(1); nodesYIndex++)
                {
                    _nodes[nodesXIndex, nodesYIndex] = null;
                }
            }

            foreach(PathNodeModel processingNode in _processingNodes)
            {
                IPathNodeFactory.Instance.PoolObject(processingNode);
            }
            _processingNodes.Clear();

            foreach(PathNodeModel processedNode in _processedNodes)
            {
                IPathNodeFactory.Instance.PoolObject(processedNode);
            }
            _processedNodes.Clear();

            _allNodes.Clear();
            _destination.Clear();
        }
    }
}