using UnityEngine;

namespace VisualTests
{
    public sealed class BallSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _ballPrefab = null!;

        public Transform SpawnBall()
        {
            var ret = Instantiate(_ballPrefab);
            return ret;
        }
    }
}