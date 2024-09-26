using System;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GridViewManager.Instance.OnGridsPlaced += AdjustCameraToMap;
        }

        public void AdjustCameraToMap()
        {
            GridView[,] grids = GridViewManager.Instance.Grids;
            Vector2Int mapSize = new (grids.GetLength(0), grids.GetLength(1));
            float gridDistance = GridViewManager.Instance.GetGridDistance();
            float newX = mapSize.x * gridDistance * 0.5f;
            float newY = -mapSize.y * gridDistance * 0.5f;
            transform.position = new Vector3(newX, newY, -10f);

            int scaleParameter = Math.Max(mapSize.x, mapSize.y);
            Camera.main.orthographicSize = 0.5f + scaleParameter * 0.225f;
        }

        private void OnDestroy()
        {
            GridViewManager.Instance.OnGridsPlaced -= AdjustCameraToMap;
        }
    }
}