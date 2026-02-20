using UnityEngine;

namespace Core
{
    public sealed class CellHighlight : MonoBehaviour
    {
        [SerializeField] private Color _highlightColor = Color.green;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Camera _camera = null!;

        public void Setup(Grid grid, Camera camera_)
        {
            _grid = grid;
            _camera = camera_;
        }

        private SpriteRenderer? _highligted;

        private void Update()
        {
            UpdateHighlight();
        }

        private void UpdateHighlight()
        {
            var screenMousePos = Input.mousePosition;
            var worldMousePos = _camera.ScreenToWorldPoint(screenMousePos);
            var gridMousePos = GridHelper.WorldToGridPosition(_grid.Size, worldMousePos);
            var gridIntMousePos = GridHelper.SnapToInt(gridMousePos);
            if (!GridHelper.IsInGrid(_grid.Size, gridIntMousePos))
            {
                DeHighlight();
                return;
            }

            var cell = _grid.FindCell(gridIntMousePos);
            if (cell is null)
            {
                return;
            }

            DeHighlight();
            var r = cell.GetComponentInChildren<SpriteRenderer>();
            r.color = _highlightColor;
            _highligted = r;

            void DeHighlight()
            {
                if (_highligted != null)
                {
                    _highligted.color = Color.white;
                    _highligted = null;
                }
            }
        }
    }
}