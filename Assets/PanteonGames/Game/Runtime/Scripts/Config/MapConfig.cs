using UnityEngine;

namespace PanteonGames.Game.Runtime.Config
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "PanteonGames/ScriptableObjects/Config/MapConfig", order = 2)] [System.Serializable]
    public class MapConfig : ScriptableObject
    {
        public int SizeX, SizeY;
    }
}