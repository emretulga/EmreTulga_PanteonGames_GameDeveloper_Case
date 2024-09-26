using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PanteonGames.Game.Runtime.View
{
    public interface IInputView
    {
        static IInputView Instance;
        event Action<IUnitView> OnRightClickedUnit;
    }

    public class InputView : MonoBehaviour, IInputView
    {
        [SerializeField]
        private LayerMask _unitsLayerMask, _gridsLayerMask;

        public event Action<IUnitView> OnRightClickedUnit;

        private void Awake()
        {
            IInputView.Instance = this;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Collider2D hitCollider = GetHitByPointerPosition(_unitsLayerMask);

                if(hitCollider != null)
                {
                    UnitView clickedUnitView = hitCollider.GetComponent<UnitView>();
                    clickedUnitView.ChoosePlacedUnit();
                }
                else
                {
                    InformationView.Instance.Close();
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Collider2D hitCollider = GetHitByPointerPosition(_unitsLayerMask + _gridsLayerMask);
                
                if(hitCollider != null)
                {
                    UnitView clickedUnitView = hitCollider.GetComponent<UnitView>();
                    if(clickedUnitView != null)
                    {
                        OnRightClickedUnit?.Invoke(clickedUnitView);
                        return;
                    }

                    GridView clickedGridView = hitCollider.GetComponent<GridView>();
                    clickedGridView.OnRightClicked();
                }
            }
        }

        private Collider2D GetHitByPointerPosition(LayerMask mask)
        {
            Vector3 worldPositionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPositionOfMouse, Vector2.zero, Mathf.Infinity, mask);

            return hit.collider;
        }
    }
}