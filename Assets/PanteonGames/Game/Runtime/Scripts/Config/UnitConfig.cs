using UnityEngine;
using System.Collections.Generic;

namespace PanteonGames.Game.Runtime.Config
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "PanteonGames/ScriptableObjects/Config/UnitConfig", order = 3)] [System.Serializable]
    public class UnitConfig : ScriptableObject
    {
        public List<UnitObjectConfig> Units;
    }

    [System.Serializable]
    public class UnitObjectConfig
    {
        public string Key, Name;
        public int SizeX, SizeY, Health;
    }
}