using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class Player : MonoBehaviour
    {
        public const int ExecutionOrder = 100;

        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private CellHighlighting _cellHigh = null!;
        [SerializeField] private Color _availableMoveColor = Color.coral;
        [SerializeField] private PlayerMovesService _playerMoves = null!;

        [SerializeField] private int _maxDistance = 3;

        private MoveSet _moveSet = MoveSet.Orthogonal;
        private ValidMoves _currentValidMoves;

        public Vector2Int GridPosition
        {
            get
            {
                var t = transform;
                var playerPosInGridSpace = _grid.WorldToGridOfCellOrigin(t.position);
                return playerPosInGridSpace;
            }
        }

        private MoveDerivationData MoveData => new(
            MoveSet: _moveSet,
            MaxDistance: _maxDistance);

        private void Start()
        {
            #if UNITY_EDITOR
            _started = true;
            #endif
            HighlightMoves();
        }

        private void Update()
        {
            SwitchMoveSet();
            DoPlayerMouseMovement();
        }

        #if UNITY_EDITOR
        private bool _started = false;

        private void OnValidate()
        {
            if (!_started)
            {
                return;
            }
            if (Application.isPlaying)
            {
                HighlightMoves();
            }
        }
        #endif

        private void SwitchMoveSet()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_moveSet == MoveSet.Orthogonal)
                {
                    _moveSet = MoveSet.Diagonal;
                }
                else
                {
                    _moveSet = MoveSet.Orthogonal;
                }

                HighlightMoves();
            }
        }

        private void HighlightMoves()
        {
            var availableMovesInGridSpace = SaveValidMoves().Positions;

            var layer = _cellHigh.ModifyLayer(HighlightLayer.AvailableMoves);
            layer.SetColor(_availableMoveColor);
            layer.Clear();
            foreach (var h in availableMovesInGridSpace)
            {
               layer.AddHigh(h);
            }
        }

        private ValidMoves SaveValidMoves()
        {
            var playerPosInGridSpace = _grid.WorldToGridOfCellOrigin(transform.position);
            _currentValidMoves = _playerMoves.GetValidMoves(_currentValidMoves, playerPosInGridSpace, MoveData);
            return _currentValidMoves;
        }

        private void DoPlayerMouseMovement()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var cellPositionGridSpace = _cellTargeting.FindCellPositionUnderMouse();
            if (!_grid.IsInGrid(cellPositionGridSpace))
            {
                return;
            }
            if (!_playerMoves.CheckValidMove(_currentValidMoves, MoveData, cellPositionGridSpace))
            {
                return;
            }

            var t = transform;
            Vector3 playerNewWorldPos = _grid.GridToWorld(cellPositionGridSpace);
            playerNewWorldPos.z = t.position.z;
            t.position = playerNewWorldPos;

            HighlightMoves();
        }
    }
}
