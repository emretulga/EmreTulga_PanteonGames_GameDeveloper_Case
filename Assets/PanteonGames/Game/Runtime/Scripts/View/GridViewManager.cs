using System;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public delegate void OnGridsChanged();

    public interface IGridViewManager
    {
        public void CreateGrids(int width, int height, Action<Model.Vector2Int> onClickAction);
        public void ClearGrids();
        public void ChangeGridsStatus(int x_1, int y_1, int x_2, int y_2, bool isPlacable);
    }

    public class GridViewManager : MonoBehaviour, IGridViewManager
    {
        public static GridViewManager Instance;

        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private float _gridDistance;
        public event OnGridsChanged OnGridsPlaced;
        [HideInInspector]
        public GridView[,] Grids;
        
        private void Awake()
        {
            Instance = this;
        }

        public void CreateGrids(int width, int height, Action<Model.Vector2Int> onRightClickAction)
        {
            Vector2Int mapSize = new(width, height);
            Grids = new GridView[mapSize.x, mapSize.y];

            for(int xIndex = 0; xIndex < mapSize.x; xIndex++)
            {
                for(int yIndex = 0; yIndex < mapSize.y; yIndex++)
                {
                    GameObject createdGrid = Instantiate(_prefab, new Vector3(xIndex * _gridDistance, -yIndex * _gridDistance, 0f), Quaternion.identity);
                    GridView gridView = createdGrid.GetComponent<GridView>();
                    gridView.RightClicked += onRightClickAction;
                    gridView.GridPosition.Set(xIndex, yIndex);
                    Grids[xIndex, yIndex] = gridView;
                }
            }

            OnGridsPlaced?.Invoke();
        }

        public void ClearGrids()
        {
            Vector2Int mapSize = new(Grids.GetLength(0), Grids.GetLength(1));
            for(int xIndex = 0; xIndex < mapSize.x; xIndex++)
            {
                for(int yIndex = 0; yIndex < mapSize.y; yIndex++)
                {
                    Grids[xIndex, yIndex].ShowDefaultGrid();
                }
            }
        }

        public void ChangeGridsStatus(int x_1, int y_1, int x_2, int y_2, bool isPlacable)
        {
            Vector2Int mapSize = new(Grids.GetLength(0), Grids.GetLength(1));
            ClearGrids();
            for(int xIndex = x_1; xIndex < x_2; xIndex++)
            {
                for(int yIndex = y_1; yIndex < y_2; yIndex++)
                {
                    bool isPositionWithinLimits = (xIndex >= 0 && xIndex < mapSize.x) && (yIndex >= 0 && yIndex < mapSize.y);
                    if(!isPositionWithinLimits) continue;

                    GridView grid = Grids[xIndex, yIndex];

                    if(isPlacable)
                    {
                        grid.ShowPlacableGrid();
                    }
                    else
                    {
                        grid.ShowUnplacableGrid();
                    }
                }
            }
        }

        public float GetGridDistance()
        {
            return _gridDistance;
        }

        public Vector3 GetPositionOfGrid(int x, int y)
        {
            return Grids[x, y].transform.position;
        }

        public Vector3 GetGridWorldPosition(Model.Vector2Int gridPosition)
        {
            return Grids[gridPosition.X, gridPosition.Y].transform.position;
        }
    }
}