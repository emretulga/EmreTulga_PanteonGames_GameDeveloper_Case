using System;
using UnityEngine;

namespace PanteonGames.Game.Runtime.View
{
    public interface IGridView
    {
        event Action<Model.Vector2Int> RightClicked;
    }

    public class GridView : MonoBehaviour, IGridView
    {
        public event Action<Model.Vector2Int> RightClicked;

        [HideInInspector]
        public Model.Vector2Int GridPosition;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        public Color DefaultColor, UnplacableColor, PlacableColor;

        private void Start()
        {
            ShowDefaultGrid();
        }

        public void ShowUnplacableGrid()
        {
            _spriteRenderer.sortingOrder = 6;
            _spriteRenderer.color = UnplacableColor;
        }

        public void ShowPlacableGrid()
        {
            _spriteRenderer.sortingOrder = 6;
            _spriteRenderer.color = PlacableColor;
        }

        public void ShowDefaultGrid()
        {
            _spriteRenderer.sortingOrder = 0;
            _spriteRenderer.color = DefaultColor;
        }

        public void OnRightClicked()
        {
            RightClicked?.Invoke(GridPosition);
        }
    }
}