using System;
using PanteonGames.Game.Runtime.Model;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public interface IProductViewManager
    {
        event Action<UnitType, string> OnProductChoosen;
        event Action<int, int, bool> OnProductChecksIsFitOnPosition; 
        event Action<int, int> OnProductTryToPlaceOnPosition;

        UnitView GetChoosenUnitView();

        void ChooseProduct(UnitType unitType, string key, string poolKey);
        
        void RemoveProduct();

        void PlaceProduct(int x, int y);

        UnitView GetNewUnit(string poolKey);
    }

    public class ProductionMenuView : MonoBehaviour, IProductViewManager
    {
        public static ProductionMenuView Instance;
        public event Action<UnitType, string> OnProductChoosen;
        public event Action<int, int, bool> OnProductChecksIsFitOnPosition; 
        public event Action<int, int> OnProductTryToPlaceOnPosition;
        [HideInInspector]
        public UnitView ChoosenUnitView;

        private void Awake()
        {
            Instance = this;
        }

        public void ChooseProduct(UnitType unitType, string key, string poolKey)
        {
            if(ChoosenUnitView != null)
            {
                RemoveProduct();
            }

            Vector3 worldPositionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UnitView unit = (UnitView)PoolView.Instance.PullObjectFromPool(poolKey);
            unit.transform.position = new Vector3(worldPositionOfMouse.x, worldPositionOfMouse.y, unit.transform.position.z);
            unit.StartFollowingMouse();
            ChoosenUnitView = unit;

            OnProductChoosen?.Invoke(unitType, key);
        }

        public void CheckChoosenProductIsFitOnPosition(int x, int y)
        {
            OnProductChecksIsFitOnPosition?.Invoke(x, y, false);
        }

        public void TryToPlaceChoosenProductOnPosition(int x, int y)
        {
            OnProductTryToPlaceOnPosition?.Invoke(x, y);
        }

        public void RemoveProduct()
        {
            PoolView.Instance.SendObjectToPool(ChoosenUnitView);
            ChoosenUnitView = null;
        }

        public void PlaceProduct(int x, int y)
        {
            ChoosenUnitView.PlaceOnPosition(x, y);
            ChoosenUnitView = null;
        }

        public UnitView GetChoosenUnitView()
        {
            return ChoosenUnitView;
        }

        public UnitView GetNewUnit(string poolKey)
        {
            return (UnitView)PoolView.Instance.PullObjectFromPool("Soldier");
        }
    }
}