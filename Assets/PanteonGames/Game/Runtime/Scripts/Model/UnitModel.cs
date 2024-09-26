using System;

namespace PanteonGames.Game.Runtime.Model
{
    public class UnitModel
    {
        public string Name;
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public Vector2Int Size;
        public Vector2Int Position;
        public UnitType Type;

        public void SetMaxHealth(int newMaxHealth)
        {
            int oldMaxHealth = MaxHealth;
            MaxHealth = Math.Max(1, newMaxHealth);
            
            if(oldMaxHealth == Health) Health = MaxHealth;
        }

        public void SetHealth(int newHealth)
        {
            Health = Math.Max(0, Math.Min(newHealth, MaxHealth));
        }
    }
}