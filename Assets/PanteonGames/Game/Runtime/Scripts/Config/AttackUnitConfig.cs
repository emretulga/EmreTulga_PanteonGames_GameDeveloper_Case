using UnityEngine;
using System.Collections.Generic;

namespace PanteonGames.Game.Runtime.Config
{
    [CreateAssetMenu(fileName = "AttackUnitConfig", menuName = "PanteonGames/ScriptableObjects/Config/AttackUnitConfig", order = 4)] [System.Serializable]
    public class AttackUnitConfig : ScriptableObject
    {
        public List<AttackUnitObjectConfig> AttackUnits;
    }

    [System.Serializable]
    public class AttackUnitObjectConfig : UnitObjectConfig
    {
        public int Damage;
    }
}