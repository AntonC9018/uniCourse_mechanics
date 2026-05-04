using System;
using UnityEngine;

namespace Core
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

    [Serializable]
    public struct Ball
    {
        public Transform Transform;
        public BallData Data;
    }
}