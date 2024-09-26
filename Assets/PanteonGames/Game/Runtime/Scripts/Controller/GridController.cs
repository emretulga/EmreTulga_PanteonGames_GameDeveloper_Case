using System.Collections.Generic;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.View;

namespace PanteonGames.Game.Runtime.Controller
{

    public class GridController
    {
        public static GridController Instance;
    
        public IGridViewManager GridView;

        private List<Vector2Int> _emptyGridsAroundUnit = new ();

        public GridController()
        {
            Vector2Int mapSize = MapModel.Instance.MapSize;

            MapModel.Instance.Grids = new GridModel[mapSize.X, mapSize.Y];

            for(int xIndex = 0; xIndex < mapSize.X; xIndex++)
            {
                for(int yIndex = 0; yIndex < mapSize.Y; yIndex++)
                {
                    GridModel createdGrid = new(xIndex, yIndex);
                    MapModel.Instance.Grids[xIndex, yIndex] = createdGrid;
                }
            }
        }

        public bool IsPositionWithinLimits(Vector2Int position)
        {
            GridModel[,] grids = MapModel.Instance.Grids;
            int mapWidth = grids.GetLength(0);
            int mapHeight = grids.GetLength(1);

            bool isPositionXWithinLimits = position.X >= 0 && position.X < mapWidth;
            bool isPositionYWithinLimits = position.Y >= 0 && position.Y < mapHeight;
            bool isPositionWithinLimits = isPositionXWithinLimits && isPositionYWithinLimits;

            return isPositionWithinLimits;
        }

        public bool IsSizeFitForPosition(Vector2Int size, Vector2Int position)
        {
            if(!IsPositionWithinLimits(position)) return false;

            GridModel[,] grids = MapModel.Instance.Grids;
            int mapWidth = grids.GetLength(0);
            int mapHeight = grids.GetLength(1);

            int sizeEndX = position.X + size.X;
            int sizeEndY = position.Y + size.Y;
            Vector2Int sizeEndPosition = new(sizeEndX - 1, sizeEndY - 1);

            if(!IsPositionWithinLimits(sizeEndPosition)) return false;

            for(int xIndex = position.X; xIndex < sizeEndX; xIndex++)
            {
                for(int yIndex = position.Y; yIndex < sizeEndY; yIndex++)
                {
                    GridModel grid = grids[xIndex, yIndex];
                    bool isGridEmpty = grid.PlacedUnit == null;
                    if(!isGridEmpty) return false;
                }
            }

            return true;
        }

        public void PlaceUnitOnPosition(UnitModel unit, Vector2Int position)
        {
            GridModel[,] grids = MapModel.Instance.Grids;
            unit.Position.Set(position);

            int bodyEndX = position.X + unit.Size.X;
            int bodyEndY = position.Y + unit.Size.Y;

            for(int xIndex = position.X; xIndex < bodyEndX; xIndex++)
            {
                for(int yIndex = position.Y; yIndex < bodyEndY; yIndex++)
                {
                    GridModel grid = grids[xIndex, yIndex];
                    grid.PlacedUnit = unit;
                }
            }
        }

        public void RemoveUnit(UnitModel unit)
        {
            GridModel[,] grids = MapModel.Instance.Grids;
            int bodyEndX = unit.Position.X + unit.Size.X;
            int bodyEndY = unit.Position.Y + unit.Size.Y;

            for(int xIndex = unit.Position.X; xIndex < bodyEndX; xIndex++)
            {
                for(int yIndex = unit.Position.Y; yIndex < bodyEndY; yIndex++)
                {
                    GridModel grid = grids[xIndex, yIndex];
                    grid.PlacedUnit = null;
                }
            }
        }

        public void ChangeGridsStatus(Vector2Int position_1, Vector2Int position_2, bool isPlacable)
        {
            GridView.ChangeGridsStatus(position_1.X, position_1.Y, position_2.X, position_2.Y, isPlacable);
        }

        public void SetDefaultGridsStatus()
        {
            GridView.ClearGrids();
        }

        public bool IsGridEmpty(Vector2Int gridPosition)
        {
            return MapModel.Instance.Grids[gridPosition.X, gridPosition.Y].PlacedUnit == null;
        }
    
        public void UpdateUnitPosition(UnitModel unitModel, Vector2Int newPosition)
        {
            RemoveUnit(unitModel);
            PlaceUnitOnPosition(unitModel, newPosition);
        }
    
        public List<Vector2Int> GetEmptyGridsAroundUnit(UnitModel unitModel)
        {
            _emptyGridsAroundUnit.Clear();

            for(int xIndex = -1; xIndex < unitModel.Size.X + 1; xIndex++)
            {
                for(int yIndex = -1; yIndex < unitModel.Size.Y + 1; yIndex++)
                {
                    Vector2Int gridPosition = new(unitModel.Position.X + xIndex, unitModel.Position.Y + yIndex);

                    if(!IsPositionWithinLimits(gridPosition)) continue;
                    if(!IsGridEmpty(gridPosition)) continue;

                    _emptyGridsAroundUnit.Add(gridPosition);
                }
            }

            return _emptyGridsAroundUnit;
        }
    
        public bool AreUnitsAroundEachOther(UnitModel unit_1, UnitModel unit_2)
        {
            for(int xIndex = -1; xIndex < unit_2.Size.X + 1; xIndex++)
            {
                for(int yIndex = -1; yIndex < unit_2.Size.Y + 1; yIndex++)
                {
                    Vector2Int gridPosition = new(unit_2.Position.X + xIndex, unit_2.Position.Y + yIndex);

                    if(!IsPositionWithinLimits(gridPosition)) continue;
                    
                    bool isPositionSame = gridPosition.X == unit_1.Position.X && gridPosition.Y == unit_1.Position.Y;
                    if(!isPositionSame) continue;

                    return true;
                }
            }

            return false;
        }
    }
}