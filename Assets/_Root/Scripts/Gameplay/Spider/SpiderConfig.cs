using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public class SpiderConfig : ScriptableObject
    {
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
        [SerializeField] private float probeDistance = 1f;
        [SerializeField] private float probeRadius = 0.25f;
        [SerializeField] private float probeOffset = 0.35f;

        [Header("Movement")]
        [SerializeField] private float maxMoveSpeed = 5f;
        [SerializeField] private float moveAcceleration = 25f;
        [SerializeField] private float maxAirMoveSpeed = 3f;
        [SerializeField] private float airMoveAcceleration = 10f;

        [Header("Orientation")]
        [SerializeField] private float orientationSharpness = 15f;

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
        public float ProbeDistance => probeDistance;
        public float ProbeRadius => probeRadius;
        public float ProbeOffset => probeOffset;
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MoveAcceleration => moveAcceleration;
        public float MaxAirMoveSpeed => maxAirMoveSpeed;
        public float AirMoveAcceleration => airMoveAcceleration;
        public float OrientationSharpness => orientationSharpness;
        public float AdhesionForce => adhesionForce;
        public float SurfaceStickSpeed => surfaceStickSpeed;
        public float SurfaceHoverOffset => surfaceHoverOffset;
        public float AdhesionDeadZone => adhesionDeadZone;
        public float AirborneGravity => airborneGravity;
    }
}
