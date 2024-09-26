using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public class ProduceMenuObject : PoolObject
    {
        [HideInInspector]
        public string ProductKey;

        public void OnMouseDown()
        {
            IProduceMenu.Instance.ClickOnButton(ProductKey);
        }
    }
}