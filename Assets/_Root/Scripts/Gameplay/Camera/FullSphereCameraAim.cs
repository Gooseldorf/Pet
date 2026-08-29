using Unity.Cinemachine;
using UnityEngine;

namespace Pet.Gameplay
{
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [RequiredTarget(RequiredTargetAttribute.RequiredTargets.LookAt)]
    [DisallowMultipleComponent]
    public sealed class FullSphereCameraAim : CinemachineComponentBase
    {
        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

        public override bool IsValid => enabled && orbitalFollow != null && LookAtTarget != null;

        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        public override void MutateCameraState(ref CameraState state, float deltaTime)
        {
            Quaternion orbitRotation = Quaternion.Euler(
                orbitalFollow.VerticalAxis.Value,
                orbitalFollow.HorizontalAxis.Value,
                0f);
            Vector3 targetDirection = state.ReferenceLookAt - state.GetCorrectedPosition();

            if (targetDirection.sqrMagnitude <= Epsilon * Epsilon)
            {
                return;
            }

            Vector3 forward = targetDirection.normalized;
            Vector3 up = Vector3.ProjectOnPlane(orbitRotation * Vector3.up, forward);

            if (up.sqrMagnitude <= Epsilon * Epsilon)
            {
                Vector3 right = Vector3.ProjectOnPlane(orbitRotation * Vector3.right, forward).normalized;
                up = Vector3.Cross(forward, right);
            }

            state.RawOrientation = Quaternion.LookRotation(forward, up.normalized);
            state.ReferenceUp = state.RawOrientation * Vector3.up;
        }
    }
}
