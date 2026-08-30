using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public class SpiderConfig : ScriptableObject
    {
        [Header("Prefab")]
        [field: SerializeField] public SpiderPlayerController Prefab { get; private set; }

        [Header("Surface Detection")]
        [SerializeField] private LayerMask traversableSurfaceMask;
        [SerializeField] private float surfaceSearchDistance = 0.3f;
        [SerializeField, Range(0f, 180f)] private float maxSurfaceBlendAngle = 100f;
        [SerializeField, Min(1)] private int attachConfirmationFrames = 2;

        [Header("Movement")]
        [SerializeField] private float maxMoveSpeed = 5f;
        [SerializeField] private float moveAcceleration = 25f;

        [Header("Adhesion")]
        [SerializeField] private float surfaceHoverOffset = 0.1f;
        [SerializeField] private float adhesionForce = 30f;
        [SerializeField] private float surfaceStickSpeed = 3f;
        [SerializeField] private float adhesionDeadZone = 0.05f;

        [Header("Orientation")]
        [SerializeField] private float surfaceAlignmentSharpness = 12f;
        [SerializeField] private float headingAlignmentSharpness = 15f;

        [Header("Airborne")]
        [SerializeField] private float airborneGravity = 18f;
        [SerializeField] private float jumpSpeed = 5f;
        [SerializeField, Range(0f, 1f)] private float airControlCoefficient = 0.5f;

        public LayerMask TraversableSurfaceMask => traversableSurfaceMask;
        public float SurfaceSearchDistance => surfaceSearchDistance;
        public float MaxSurfaceBlendAngle => maxSurfaceBlendAngle;
        public int AttachConfirmationFrames => attachConfirmationFrames;
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MoveAcceleration => moveAcceleration;
        public float SurfaceHoverOffset => surfaceHoverOffset;
        public float AdhesionForce => adhesionForce;
        public float SurfaceStickSpeed => surfaceStickSpeed;
        public float AdhesionDeadZone => adhesionDeadZone;
        public float SurfaceAlignmentSharpness => surfaceAlignmentSharpness;
        public float HeadingAlignmentSharpness => headingAlignmentSharpness;
        public float AirborneGravity => airborneGravity;
        public float JumpSpeed => jumpSpeed;
        public float AirControlCoefficient => airControlCoefficient;
    }
}
