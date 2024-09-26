using UnityEngine;
using System.Collections.Generic;

namespace PanteonGames.Game.Runtime.Config
{
    [CreateAssetMenu(fileName = "ProducerUnitConfig", menuName = "PanteonGames/ScriptableObjects/Config/ProducerUnitConfig", order = 5)] [System.Serializable]
    public class ProducerUnitConfig : ScriptableObject
    {
        public List<ProducerUnitObjectConfig> ProducerUnits;
    }

    [System.Serializable]
    public class ProducerUnitObjectConfig : UnitObjectConfig
    {
        public string[] ProducibleUnitKeys;
        public int ProductSpawnPointX, ProductSpawnPointY;
    }
}