using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public class SpiderConfig : ScriptableObject
    {
        private const float DEFAULT_FORWARD_PROBE_PRIORITY_ANGLE = 35f;
        private const float DEFAULT_SURFACE_ALIGNMENT_SHARPNESS = 12f;

        [Header("Prefab")]
        [field: SerializeField] public SpiderPlayerController Prefab { get; private set; }

        [Header("Feature Toggles")]
        [SerializeField] private bool enableCeilingTraversal = true;
        [SerializeField] private bool enableSurfaceAdhesion = true;
        [SerializeField] private bool enableJump = true;
        [SerializeField] private bool enableWeb;
        [SerializeField] private bool enableAirControl = true;

        [Header("Surface Probing")]
        [SerializeField] private LayerMask traversableSurfaceMask = ~0;
        [SerializeField] private float downProbeDistance = 0.8f;
        [SerializeField] private float downProbeRadius = 0.25f;
        [SerializeField] private float forwardProbeDistance = 0.75f;
        [SerializeField] private float forwardProbeRadius = 0.3f;
        [SerializeField] private float forwardProbePriorityAngle = DEFAULT_FORWARD_PROBE_PRIORITY_ANGLE;
        [SerializeField] private int forwardProbeConfirmFrames = 2;

        [Header("Legacy Surface Probing")]
        [SerializeField] private float probeDistance = 1f;
        [SerializeField] private float probeRadius = 0.25f;

        [Header("Movement")]
        [SerializeField] private float maxMoveSpeed = 5f;
        [SerializeField] private float moveAcceleration = 25f;
        [SerializeField] private float maxAirMoveSpeed = 3f;
        [SerializeField] private float airMoveAcceleration = 10f;

        [Header("Orientation")]
        [SerializeField] private float orientationSharpness = 15f;
        [SerializeField] private float surfaceAlignmentSharpness = DEFAULT_SURFACE_ALIGNMENT_SHARPNESS;

        [Header("Adhesion")]
        [SerializeField] private float adhesionForce = 30f;
        [SerializeField] private float surfaceStickSpeed = 3f;
        [SerializeField] private float surfaceHoverOffset = 0.25f;
        [SerializeField] private float adhesionDeadZone = 0.05f;

        [Header("Airborne")]
        [SerializeField] private float airborneGravity = 18f;

        public bool EnableCeilingTraversal => enableCeilingTraversal;
        public bool EnableSurfaceAdhesion => enableSurfaceAdhesion;
        public bool EnableJump => enableJump;
        public bool EnableWeb => enableWeb;
        public bool EnableAirControl => enableAirControl;

        public LayerMask TraversableSurfaceMask => traversableSurfaceMask;
        public float DownProbeDistance => downProbeDistance > 0f ? downProbeDistance : probeDistance;
        public float DownProbeRadius => downProbeRadius > 0f ? downProbeRadius : probeRadius;
        public float ForwardProbeDistance => forwardProbeDistance > 0f ? forwardProbeDistance : probeDistance;
        public float ForwardProbeRadius => forwardProbeRadius > 0f ? forwardProbeRadius : probeRadius;
        public float ForwardProbePriorityAngle => forwardProbePriorityAngle > 0f
            ? forwardProbePriorityAngle
            : DEFAULT_FORWARD_PROBE_PRIORITY_ANGLE;
        public int ForwardProbeConfirmFrames => Mathf.Max(1, forwardProbeConfirmFrames);
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MoveAcceleration => moveAcceleration;
        public float MaxAirMoveSpeed => maxAirMoveSpeed;
        public float AirMoveAcceleration => airMoveAcceleration;
        public float OrientationSharpness => orientationSharpness;
        public float SurfaceAlignmentSharpness => surfaceAlignmentSharpness > 0f
            ? surfaceAlignmentSharpness
            : DEFAULT_SURFACE_ALIGNMENT_SHARPNESS;
        public float AdhesionForce => adhesionForce;
        public float SurfaceStickSpeed => surfaceStickSpeed;
        public float SurfaceHoverOffset => surfaceHoverOffset;
        public float AdhesionDeadZone => adhesionDeadZone;
        public float AirborneGravity => airborneGravity;
    }
}
