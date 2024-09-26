using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames.Game.Runtime.Config
{
    public interface IConfigLoader
    {
        static IConfigLoader Instance;
        MapConfig GetMapConfig();
        UnitObjectConfig GetUnitConfig(string key);
        AttackUnitObjectConfig GetAttackUnitConfig(string key);
        ProducerUnitObjectConfig GetProducerUnitConfig(string key);
    }

    public class ConfigLoader : IConfigLoader
    {
        public const string MapConfigPath = "Game/Configs/MapConfig";
        public const string UnitConfigPath = "Game/Configs/UnitConfig";
        public const string AttackUnitConfigPath = "Game/Configs/AttackUnitConfig";
        public const string ProducerUnitConfigPath = "Game/Configs/ProducerUnitConfig";

        private MapConfig _mapConfig;
        private Dictionary<string, UnitObjectConfig> _unitDictionary;
        private Dictionary<string, AttackUnitObjectConfig> _attackUnitDictionary;
        private Dictionary<string, ProducerUnitObjectConfig> _producerUnitDictionary;

        public ConfigLoader()
        {
            LoadMapConfig();
            LoadUnitConfig();
            LoadAttackUnitConfig();
            LoadProducerUnitConfig();
        }

        private void LoadUnitConfig()
        {
            UnitConfig unitConfig = Resources.Load<UnitConfig>(UnitConfigPath);
            _unitDictionary = new();

            foreach(UnitObjectConfig unitObjectConfig in unitConfig.Units)
            {
                _unitDictionary.Add(unitObjectConfig.Key, unitObjectConfig);
            }
        }

        private void LoadMapConfig()
        {
            _mapConfig = Resources.Load<MapConfig>(MapConfigPath);
        }

        public MapConfig GetMapConfig()
        {
            return _mapConfig;
        }

        public UnitObjectConfig GetUnitConfig(string key)
        {
            return _unitDictionary[key];
        }

        private void LoadAttackUnitConfig()
        {
            AttackUnitConfig attackUnitConfig = Resources.Load<AttackUnitConfig>(AttackUnitConfigPath);
            _attackUnitDictionary = new();

            foreach(AttackUnitObjectConfig attackUnitObjectConfig in attackUnitConfig.AttackUnits)
            {
                _attackUnitDictionary.Add(attackUnitObjectConfig.Key, attackUnitObjectConfig);
            }
        }

        public AttackUnitObjectConfig GetAttackUnitConfig(string key)
        {
            return _attackUnitDictionary[key];
        }

        private void LoadProducerUnitConfig()
        {
            ProducerUnitConfig producerUnitConfig = Resources.Load<ProducerUnitConfig>(ProducerUnitConfigPath);
            _producerUnitDictionary = new();

            foreach(ProducerUnitObjectConfig producerUnitObjectConfig in producerUnitConfig.ProducerUnits)
            {
                _producerUnitDictionary.Add(producerUnitObjectConfig.Key, producerUnitObjectConfig);
            }
        }

        public ProducerUnitObjectConfig GetProducerUnitConfig(string key)
        {
            return _producerUnitDictionary[key];
        }
    }
}