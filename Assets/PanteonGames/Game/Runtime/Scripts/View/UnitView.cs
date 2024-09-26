using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public interface IUnitView
    {
        event Action<IUnitView> OnPlacedUnitClick;
        event Action<IUnitView, Model.Vector2Int> ArrivedPosition;

        void ShowInformation(string name, int maxHealth, int health);
        void PlaceOnPosition(int x, int y);
        void ProceedToDestination(Stack<Model.Vector2Int> destination);
        void StopProceedingToDestination();
        void Remove();
    }

    public class UnitView : PoolObject, IUnitView
    {
        const float MovingTime = 0.15f;
        const float DiagonalTimeMultipier = 1.4f;

        public event Action<IUnitView> OnPlacedUnitClick;
        public event Action<IUnitView, Model.Vector2Int> ArrivedPosition;

        public bool IsFollowingMouse { get; set; }

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private int _defaultSortingOrder;

        [SerializeField]
        private LayerMask _gridLayerMask;

        [HideInInspector]
        public bool IsPlaced;
        
        private Coroutine _destinationCoroutine;
        private bool _isMoving;
        private List<IEnumerator> _destination = new ();

        private void Awake()
        {
            _defaultSortingOrder = _spriteRenderer.sortingOrder;
        }

        private void Update()
        {
            FollowMouse();
        }

        public override void OnPooled()
        {
            base.OnPooled();
            IsFollowingMouse = false;
            Color mockColor = _spriteRenderer.color;
            mockColor.a = 1f;
            _spriteRenderer.color = mockColor;
            _spriteRenderer.sortingOrder = _defaultSortingOrder;
            IsPlaced = false;
            _isMoving = false;
        }

        public void PlaceOnPosition(int x, int y)
        {
            IsFollowingMouse = false;
            _spriteRenderer.color = Color.white;
            _spriteRenderer.sortingOrder = _defaultSortingOrder;
            Vector3 gridPosition = GridViewManager.Instance.GetPositionOfGrid(x, y);
            transform.position = new Vector3(gridPosition.x, gridPosition.y, transform.position.z);
            IsPlaced = true;
            ChoosePlacedUnit();
        }

        public void StartFollowingMouse()
        {
            IsFollowingMouse = true;
            Color mockColor = _spriteRenderer.color;
            mockColor.a = 0.5f;
            _spriteRenderer.color = mockColor;
            _spriteRenderer.sortingOrder = 10;
        }

        public void FollowMouse()
        {
            if(!IsFollowingMouse) return;
            Vector3 worldPositionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(worldPositionOfMouse.x, worldPositionOfMouse.y, transform.position.z);
            Vector2Int desiredPosition = new(-1, -1);
            RaycastHit2D hit = Physics2D.Raycast(worldPositionOfMouse, Vector2.zero, Mathf.Infinity, _gridLayerMask);
            if(hit.collider != null)
            {
                Color mockColor = Color.white;
                mockColor.a = 0.5f;
                _spriteRenderer.color = mockColor;

                GridView hitGrid = hit.collider.gameObject.GetComponent<GridView>();
                Model.Vector2Int gridPosition = hitGrid.GridPosition;
                ProductionMenuView.Instance.CheckChoosenProductIsFitOnPosition(gridPosition.X, gridPosition.Y);
                desiredPosition = new(gridPosition.X, gridPosition.Y);
            }
            else
            {
                Color mockColor = Color.red;
                mockColor.a = 0.5f;
                _spriteRenderer.color = mockColor;
                ProductionMenuView.Instance.CheckChoosenProductIsFitOnPosition(-1, -1);
            }

            if(Input.GetMouseButtonUp(0))
            {
                IsFollowingMouse = false;
                ProductionMenuView.Instance.TryToPlaceChoosenProductOnPosition(desiredPosition.x, desiredPosition.y);
            }
        }

        public void ChoosePlacedUnit()
        {
            if(!IsPlaced) return;

            _spriteRenderer.color = Color.blue;

            OnPlacedUnitClick?.Invoke(this);
        }

        public void DeselectUnit()
        {
            _spriteRenderer.color = Color.white;
        }

        public void ShowInformation(string unitName, int maxHealth, int health)
        {
            InformationView.Instance.Open(this, _spriteRenderer.sprite, unitName, maxHealth, health);
        }

        public void ProceedToDestination(Stack<Model.Vector2Int> destination)
        {
            StopProceedingToDestination();
            _destination.Clear();
            
            int destinationCount = destination.Count;

            for(int destinationIndex = 0; destinationIndex < destinationCount; destinationIndex++)
            {
                _destination.Add(GoToPosition(destination.Pop()));
            }

            _destinationCoroutine = StartCoroutine(StartDestination());
        }

        public void StopProceedingToDestination()
        {
            if(_destinationCoroutine != null)
            {
                StopCoroutine(_destinationCoroutine);
                _destinationCoroutine = null;
                _destination.Clear();
            }
        }

        private IEnumerator StartDestination()
        {
            while(_isMoving)
            {
                yield return null;
            }

            foreach(IEnumerator movement in _destination)
            {
                yield return StartCoroutine(movement);
            }
        }

        public void Remove()
        {
            StopAllCoroutines();
            _destination.Clear();
            _destinationCoroutine = null;
            
            PoolView.Instance.SendObjectToPool(this);
        }

        private IEnumerator GoToPosition(Model.Vector2Int gridPosition)
        {
            _isMoving = true;
            ArrivedPosition?.Invoke(this, gridPosition);

            Vector3 unitPosition = transform.position;
            Vector3 gridWorldPosition = GridViewManager.Instance.GetGridWorldPosition(gridPosition);
            float movingTimer = 0f;
            float timeMultipier = 1f;

            float dx = Math.Abs(gridWorldPosition.x - unitPosition.x);
            float dy = Math.Abs(gridWorldPosition.y - unitPosition.y);

            bool isMovingDiagonal = dx != 0f && dy != 0f;
            if(isMovingDiagonal) timeMultipier = DiagonalTimeMultipier;

            while(movingTimer < MovingTime * timeMultipier)
            {
                transform.position = Vector3.Lerp(unitPosition, gridWorldPosition, movingTimer / (MovingTime * timeMultipier));
                movingTimer += Time.deltaTime;

                yield return null;
            }

            transform.position = gridWorldPosition;
            _isMoving = false;
        }
    }
}