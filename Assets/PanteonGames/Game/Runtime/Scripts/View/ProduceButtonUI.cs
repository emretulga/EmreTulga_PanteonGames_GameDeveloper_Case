using UnityEngine;
using UnityEngine.UI;

namespace PanteonGames.Game.Runtime.View
{
    public class ProduceButtonUI : PoolObject
    {
        [HideInInspector]
        public RectTransform Rect;
        
        protected Image _image;
        protected InfiniteObjectInformation _information;

        protected void Awake()
        {
            Rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        public void Set(InfiniteObjectInformation package, Vector2 newPosition)
        {
            _image.sprite = package.Sprite;
            _information = package;
            Rect.anchoredPosition = newPosition;
        }

        public void OnPointerDown()
        {
            ProductionMenuView.Instance.ChooseProduct(_information.UnitType, _information.ProduceKey, _information.PoolKey);
        }
    }
}