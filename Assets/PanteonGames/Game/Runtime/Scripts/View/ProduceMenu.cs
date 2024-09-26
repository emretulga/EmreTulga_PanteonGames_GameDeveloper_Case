using System;
using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public interface IProduceMenu
    {
        static IProduceMenu Instance;
        event Action<string> OnMenuButtonClicked;
        void Open(List<string> productKeys);
        void ClickOnButton(string productKey);
    }

    public class ProduceMenu : MonoBehaviour, IProduceMenu
    {
        public event Action<string> OnMenuButtonClicked;

        private List<ProduceMenuObject> _produceMenuObjects;

        public RectTransform ContentRect;
        public float ButtonsStartPositionX, ButtonsDistanceX;
        public float ContentStartWidth, ContentWidthEachButton;

        private void Awake()
        {
            IProduceMenu.Instance = this;
            _produceMenuObjects = new ();
        }

        public void Open(List<string> productKeys)
        {
            Clear();

            for(int keyIndex = 0; keyIndex < productKeys.Count; keyIndex++)
            {
                ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x + ContentWidthEachButton, ContentRect.sizeDelta.y);
                Vector2 createPosition = new Vector2(ButtonsStartPositionX + ButtonsDistanceX * keyIndex, 0f);
                
                CreateButton(productKeys[keyIndex], createPosition);
            }

            gameObject.SetActive(true);
        }

        private void CreateButton(string productKey, Vector2 position)
        {
            ProduceMenuObject produceMenuObject = (ProduceMenuObject)PoolView.Instance.PullObjectFromPool("ProduceMenuObject");
            produceMenuObject.ProductKey = productKey;
            produceMenuObject.transform.SetParent(ContentRect, false);
            produceMenuObject.GetComponent<RectTransform>().anchoredPosition = position;
            _produceMenuObjects.Add(produceMenuObject);
        }

        private void Clear()
        {
            ContentRect.sizeDelta = new Vector2(ContentStartWidth, ContentRect.sizeDelta.y);

            foreach(ProduceMenuObject produceMenuObject in _produceMenuObjects)
            {
                PoolView.Instance.SendObjectToPool(produceMenuObject);
            }

            _produceMenuObjects.Clear();
        }

        public void ClickOnButton(string productKey)
        {
            OnMenuButtonClicked?.Invoke(productKey);
        }
    }
}