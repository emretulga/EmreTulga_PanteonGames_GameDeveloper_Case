using System;
using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    [Serializable]
    public struct InfiniteObjectInformation
    {
        public Model.UnitType UnitType;
        public Sprite Sprite;
        public string ProduceKey, PoolKey;
    }

    public class InfiniteScrollView : MonoBehaviour
    {
        [SerializeField]
        private Order _contentOrder;

        [SerializeField]
        private RectTransform _contentRectTransform;

        [SerializeField]
        private string _prefabKey;

        [SerializeField]
        private InfiniteObjectInformation[] _objectInformations;
        
        [SerializeField]
        private float _objectDistance, _dissepearDistance;

        [SerializeField]
        private int _shownObjectAmount;

        private float _topDistance, _bottomDistance;
        private int _objectInformationIndex = 0;
        private Vector2 _contentRectLastPosition;
        private List<ProduceButtonUI> _currentObjects;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            _contentRectLastPosition = _contentRectTransform.anchoredPosition;

            float maxDistance = _objectDistance * _shownObjectAmount * 0.5f - _objectDistance * 0.5f;
            _topDistance = maxDistance;
            _bottomDistance = -maxDistance;

            _currentObjects = new();
            _objectInformationIndex = 0;

            for(int shownButtonIndex = 0; shownButtonIndex < _shownObjectAmount; shownButtonIndex++)
            {
                float buttonNewPositionY = _topDistance - shownButtonIndex * _objectDistance;
                Vector2 newButtonPosition = new(0f, buttonNewPositionY);
                ProduceButtonUI button = AddNewButtonIntoList();

                button.Set(_objectInformations[_objectInformationIndex], newButtonPosition);

                _objectInformationIndex++;
                if(_objectInformationIndex >= _objectInformations.Length)
                {
                    _objectInformationIndex = 0;
                }
            }
        }

        public void OnContentRepositioning()
        {
            Vector2 contentRectPosition = _contentRectTransform.anchoredPosition;
            int recreateAmount = 0;
            int buttonsAmount = _currentObjects.Count;
            bool isItScrollingUp = contentRectPosition.y > _contentRectLastPosition.y;

            if(isItScrollingUp)
            {
                float bottomButtonPositionY = _currentObjects[buttonsAmount - 1].Rect.anchoredPosition.y;

                for(int buttonIndex = 0; buttonIndex < buttonsAmount; buttonIndex++)
                {
                    ProduceButtonUI button = _currentObjects[buttonIndex];
                    Vector3 buttonWorldPosition = button.Rect.position;
                    Vector3 buttonScreenPosition = _rectTransform.InverseTransformPoint(buttonWorldPosition);
                    float topDissepearPositionY = _topDistance + _dissepearDistance;
                    bool isButtonPassingTop = buttonScreenPosition.y > topDissepearPositionY;

                    if(isButtonPassingTop)
                    {
                        recreateAmount++;
                    }
                    else break;
                }

                for(int recreateIndex = 0; recreateIndex < recreateAmount; recreateIndex++)
                {
                    ProduceButtonUI button = _currentObjects[0];
                    _currentObjects.Remove(button);
                    PoolView.Instance.SendObjectToPool(button);

                    float buttonNewPositionY = bottomButtonPositionY - _objectDistance;
                    Vector2 newButtonPosition = new(0f, buttonNewPositionY);

                    ProduceButtonUI newButton = AddNewButtonIntoList();

                    newButton.Set(_objectInformations[_objectInformationIndex], newButtonPosition);
                    bottomButtonPositionY = newButtonPosition.y;

                    _objectInformationIndex++;
                    if(_objectInformationIndex >= _objectInformations.Length)
                    {
                        _objectInformationIndex = 0;
                    }
                }
            }
            else
            {
                float topButtonPositionY = _currentObjects[0].Rect.anchoredPosition.y;

                for(int buttonIndex = buttonsAmount - 1; buttonIndex >= 0; buttonIndex--)
                {
                    ProduceButtonUI button = _currentObjects[buttonIndex];
                    Vector3 buttonWorldPosition = button.Rect.position;
                    Vector3 buttonScreenPosition = _rectTransform.InverseTransformPoint(buttonWorldPosition);
                    float bottomDissepearPositionY = _bottomDistance - _dissepearDistance;
                    bool isButtonPassingBottom = buttonScreenPosition.y < bottomDissepearPositionY;

                    if(isButtonPassingBottom)
                    {
                        recreateAmount++;
                    }
                    else break;
                }

                for(int recreateIndex = recreateAmount - 1; recreateIndex >= 0; recreateIndex--)
                {
                    ProduceButtonUI button = _currentObjects[buttonsAmount - 1];
                    _currentObjects.Remove(button);
                    PoolView.Instance.SendObjectToPool(button);

                    float buttonNewPositionY = topButtonPositionY + _objectDistance;
                    Vector2 newButtonPosition = new(0f, buttonNewPositionY);

                    ProduceButtonUI newButton = AddNewButtonIntoList(false);

                    newButton.Set(_objectInformations[_objectInformationIndex], newButtonPosition);
                    topButtonPositionY = newButtonPosition.y;

                    _objectInformationIndex--;
                    if(_objectInformationIndex < 0)
                    {
                        _objectInformationIndex = _objectInformations.Length - 1;
                    }
                }
            }
            
            _contentRectLastPosition = contentRectPosition;
        }

        private ProduceButtonUI AddNewButtonIntoList(bool toEndOfList = true)
        {
            ProduceButtonUI button = (ProduceButtonUI)PoolView.Instance.PullObjectFromPool(_prefabKey);
            
            if(toEndOfList)
            {
                _currentObjects.Add(button);
            }
            else
            {
                _currentObjects.Insert(0, button);
            }

            
            button.transform.SetParent(_contentRectTransform, false);

            return button;
        }
    }

    public enum Order
    {
        Horizontal,
        Vertical
    }
}