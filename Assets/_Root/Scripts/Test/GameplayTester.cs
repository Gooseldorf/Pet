using Pet.Gameplay;
using UnityEngine;

namespace Pet.Gameplay
{
    public class GameplayTester : MonoBehaviour
    {
        private SpiderPlayerController spiderPlayerController;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad1))
            {
                LogSpiderSurfaceState();
            }
        }

        private void LogSpiderSurfaceState()
        {
            spiderPlayerController ??= FindAnyObjectByType<SpiderPlayerController>();

            if (spiderPlayerController == null)
            {
                Debug.Log("GameplayTester: SpiderPlayerController was not found in the scene.");
                return;
            }

            SpiderSurfaceState surfaceState = spiderPlayerController.CurrentSurfaceState;
            SpiderSurfaceHit primaryHit = surfaceState.PrimaryHit;

            Debug.Log(
                $"Spider surface state | " +
                $"HasSurface: {surfaceState.HasSurface} | " +
                $"IsStableSurface: {surfaceState.IsStableSurface} | " +
                $"HitCount: {surfaceState.HitCount} | " +
                $"SurfaceDistance: {surfaceState.SurfaceDistance:F3} | " +
                $"SurfacePoint: {surfaceState.SurfacePoint} | " +
                $"SurfaceNormal: {surfaceState.SurfaceNormal} | " +
                $"PrimaryCollider: {(primaryHit.Collider != null ? primaryHit.Collider.name : "None")} | " +
                $"PrimaryPoint: {primaryHit.Point} | " +
                $"PrimaryNormal: {primaryHit.Normal}");
        }
    }
}
