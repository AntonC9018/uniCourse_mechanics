using System;
using UnityEngine;

namespace Core
{
    public enum AnimationState
    {
        Finished,
        NotAnimating,
        IsAnimating,
    }

    [Serializable]
    public struct AnimationService
    {
        private Vector3 _animationStartPos;
        private Vector3 _animationEndPos;
        private float _animationStartTimeInSeconds;
        private bool _isAnimating;
        private Transform _transform;

        [Min(0.0001f)]
        public float AnimationTimeInSeconds;

        public AnimationCurve AnimationCurve;

        public void Start(Transform t, Vector3 startPos, Vector3 endPos)
        {
            _transform = t;
            _isAnimating = true;
            _animationStartPos = startPos;
            _animationEndPos = endPos;
            _animationStartTimeInSeconds = Time.time;
        }

        public AnimationState TryAnimate()
        {
            if (!_isAnimating)
            {
                return AnimationState.NotAnimating;
            }

            var timeSinceAnimationStartInSeconds = Time.time - _animationStartTimeInSeconds;
            var t = timeSinceAnimationStartInSeconds / AnimationTimeInSeconds;
            if (t < 1)
            {
                t = AnimationCurve.Evaluate(t);
                var diff = _animationEndPos - _animationStartPos;
                var pos = _animationStartPos + diff * t;
                _transform.position = pos;
                return AnimationState.IsAnimating;
            }
            else
            {
                _transform.position = _animationEndPos;
                _isAnimating = false;
                return AnimationState.Finished;
            }
        }
    }
}