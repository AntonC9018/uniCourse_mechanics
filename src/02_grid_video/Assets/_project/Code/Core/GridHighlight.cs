using UnityEngine;

namespace Core
{
    public sealed class GridHighlight : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Color _highlightColor = Color.green;

        private SpriteRenderer? _highlightedCell = null;

        private void Update()
        {
            Highlight();
        }

        private void Highlight()
        {
            var cell = _cellTargeting.FindCellUnderMouse();
            DeHighlight();
            if (cell == null)
            {
                return;
            }

            var spriteRenderer = CellHelper.GetSpriteRenderer(cell.gameObject);
            spriteRenderer.color = _highlightColor;
            _highlightedCell = spriteRenderer;


            void DeHighlight()
            {
                if (_highlightedCell != null)
                {
                    _highlightedCell.color = Color.white;
                }
                _highlightedCell = null;
            }
        }
    }
}
