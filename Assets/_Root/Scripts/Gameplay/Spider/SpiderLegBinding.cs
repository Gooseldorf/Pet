using System;
using UnityEngine;

namespace Pet.Gameplay
{
    [Serializable]
    public sealed class SpiderLegBinding
    {
        [SerializeField] private Transform root;
        [SerializeField] private Transform tip;
        [SerializeField] private Transform target;
        [SerializeField, Range(0, 1)] private int gaitGroup;

        internal Transform Root => root;
        internal Transform Tip => tip;
        internal Transform Target => target;
        internal int GaitGroup => gaitGroup;
        internal float MaxReach { get; set; }
        internal Vector3 RestPositionLocal { get; set; }
        internal Vector3 DesiredPosition { get; set; }
        internal Vector3 SupportPoint { get; set; }
        internal Vector3 SupportNormal { get; set; }
        internal Collider SupportCollider { get; set; }
        internal Vector3 StepStartPosition { get; set; }
        internal Vector3 StepStartNormal { get; set; }
        internal Quaternion StepStartRotation { get; set; }
        internal Vector3 StepTargetPoint { get; set; }
        internal Vector3 StepTargetNormal { get; set; }
        internal Collider StepTargetCollider { get; set; }
        internal float StepProgress { get; set; }
        internal float NextSupportSearchTime { get; set; }
        internal Vector3 RetractStartPosition { get; set; }
        internal Quaternion RetractStartRotation { get; set; }
        internal float RetractProgress { get; set; }
        internal bool IsStepping { get; set; }
        internal bool HasLoggedSupportSearchFailure { get; set; }
        internal bool HasSupport => SupportCollider != null;
    }
}
