using System;

namespace PanteonGames.Game.Runtime.Model
{
    public class MapModel
    {
        public static MapModel Instance;

        public Vector2Int MapSize;
        public GridModel[,] Grids;

        public MapModel(Vector2Int mapSize)
        {
            mapSize.X = Math.Max(1, mapSize.X);
            mapSize.Y = Math.Max(1, mapSize.Y);
            MapSize.Set(mapSize);
        }
        
        public MapModel(int x, int y)
        {
            x = Math.Max(1, x);
            y = Math.Max(1, y);
            MapSize.Set(x, y);
        }
    }
}