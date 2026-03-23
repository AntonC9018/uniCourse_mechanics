using UnityEditor;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class Player : MonoBehaviour
    {
        public const int ExecutionOrder = HighController.ExecutionOrder - 1;
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private HighController _highController = null!;
        [SerializeField] private MoveSelection _moveSelection = null!;
        private MoveSet _moveSet = MoveSet.Orthogonal;

        private void Start()
        {
            ResetValidMoves();
        }

        private void Update()
        {
            if (TryChangeMoveSet())
            {
                ResetValidMoves();
            }

            if (TryMove())
            {
                ResetValidMoves();
            }
        }

        private bool TryChangeMoveSet()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                int i = (int) _moveSet;
                i = (i + 1) % (int) MoveSet.Count;
                _moveSet = (MoveSet) i;
                return true;
            }
            return false;
        }

        private void HighValidMoves()
        {
            var group = _highController.Group(HighGroup.ValidMoves);
            group.Clear();
            group.SetColor(Color.crimson);
            group.High(_moveSelection.SelectedValidMoves);
        }

        private void ResetValidMoves()
        {
            var playerTransform = transform;
            var playerGridSpace = _grid.WorldToGrid(playerTransform.position);
            var playerGridSpaceInt = _grid.MakeSureCellOrigin(playerGridSpace);
            _moveSelection.ResetMoves(playerGridSpaceInt, _moveSet);

            HighValidMoves();
        }

        private bool TryMove()
        {
            const int left = 0;
            if (!Input.GetMouseButtonDown(left))
            {
                return false;
            }

            var targetPosInGridSpace = _cellTargeting.GetCellPositionOfMouse();
            if (!_grid.IsInGrid(targetPosInGridSpace))
            {
                return false;
            }

            if (_moveSelection.CheckValidMove(targetPosInGridSpace))
            {
                var playerTransform = transform;
                var pos = playerTransform.position;
                Vector3 playerNewPosInWorldSpace = _grid.GridToWorld(targetPosInGridSpace);
                playerNewPosInWorldSpace.z = pos.z;
                playerTransform.position = playerNewPosInWorldSpace;
                return true;
            }
            return false;
        }
    }
}