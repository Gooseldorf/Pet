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

        // Формирует ориентацию камеры на всей сфере вокруг паука, чтобы направление камеры могло служить ориентиром для движения на стенах и потолке.
        public override void MutateCameraState(ref CameraState state, float deltaTime)
        {
            // Углы орбиты превращаются в поворот, из которого берется непрерывный вектор "вверх" камеры.
            Quaternion orbitRotation = Quaternion.Euler(
                orbitalFollow.VerticalAxis.Value,
                orbitalFollow.HorizontalAxis.Value,
                0f);
            // Направление от камеры к цели становится ее будущим вектором вперед.
            Vector3 targetDirection = state.ReferenceLookAt - state.GetCorrectedPosition();

            if (targetDirection.sqrMagnitude <= Epsilon * Epsilon)
            {
                return;
            }

            Vector3 forward = targetDirection.normalized;
            // Проецируем верх орбиты на плоскость взгляда, сохраняя roll при переходе через полюса.
            Vector3 up = Vector3.ProjectOnPlane(orbitRotation * Vector3.up, forward);

            if (up.sqrMagnitude <= Epsilon * Epsilon)
            {
                // В полюсе восстанавливаем базис через правую ось, когда проекция исходного верха вырождается.
                Vector3 right = Vector3.ProjectOnPlane(orbitRotation * Vector3.right, forward).normalized;
                up = Vector3.Cross(forward, right);
            }

            state.RawOrientation = Quaternion.LookRotation(forward, up.normalized);
            state.ReferenceUp = state.RawOrientation * Vector3.up;
        }
    }
}
