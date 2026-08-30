using UnityEngine;
using UnityEngine.Rendering;

namespace Pet.Gameplay
{
    public sealed class SpiderLegVisualizer : MonoBehaviour
    {
        private const float DESIRED_POSITION_RADIUS = 0.025f;
        private const float TARGET_RADIUS = 0.035f;
        private const float SUPPORT_RADIUS = 0.045f;
        private const float NORMAL_LENGTH = 0.2f;
        private const float LINE_RADIUS = 0.005f;

        [Header("Required References")]
        [SerializeField] private SpiderPlayerController playerController;
        [SerializeField] private Material searchRadiusMaterial;
        [SerializeField] private Material contactMaterial;

        private Mesh sphereMesh;
        private Mesh cylinderMesh;
        private RenderParams searchRadiusRenderParams;
        private RenderParams contactRenderParams;

        private void Awake()
        {
            sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            cylinderMesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            searchRadiusRenderParams = CreateRenderParams(searchRadiusMaterial);
            contactRenderParams = CreateRenderParams(contactMaterial);
        }

        private RenderParams CreateRenderParams(Material material)
        {
            return new RenderParams(material)
            {
                layer = gameObject.layer,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                lightProbeUsage = LightProbeUsage.Off
            };
        }

        private void LateUpdate()
        {
            SpiderLegController legController = playerController.LegController;

            if (!legController.IsInitialized)
            {
                return;
            }

            for (int legIndex = 0; legIndex < legController.LegCount; legIndex++)
            {
                Transform root = legController.GetRoot(legIndex);
                Transform target = legController.GetTarget(legIndex);
                Vector3 desiredPosition = legController.GetDesiredPosition(legIndex);
                DrawSphere(desiredPosition, legController.SearchRadius, ref searchRadiusRenderParams);
                DrawSphere(desiredPosition, DESIRED_POSITION_RADIUS, ref searchRadiusRenderParams);
                DrawSphere(target.position, TARGET_RADIUS, ref contactRenderParams);
                DrawLine(root.position, target.position, ref contactRenderParams);

                if (legController.TryGetSupport(legIndex, out SpiderLegSurfaceContact support))
                {
                    DrawSphere(support.Point, SUPPORT_RADIUS, ref contactRenderParams);
                    DrawNormal(support.Point, support.Normal, NORMAL_LENGTH, ref contactRenderParams);
                }
            }
        }

        private void DrawSphere(Vector3 position, float radius, ref RenderParams renderParams)
        {
            float diameter = radius * 2f;
            Vector3 scale = Vector3.one * diameter;
            renderParams.worldBounds = new Bounds(position, scale);
            Graphics.RenderMesh(in renderParams, sphereMesh, 0, Matrix4x4.TRS(position, Quaternion.identity, scale));
        }

        private void DrawLine(Vector3 start, Vector3 end, ref RenderParams renderParams)
        {
            Vector3 direction = end - start;
            float length = direction.magnitude;

            if (length <= 0f)
            {
                return;
            }

            Vector3 center = (start + end) * 0.5f;
            Vector3 scale = new(LINE_RADIUS * 2f, length * 0.5f, LINE_RADIUS * 2f);
            renderParams.worldBounds = new Bounds(center, Vector3.one * (length + LINE_RADIUS * 2f));
            Graphics.RenderMesh(
                in renderParams,
                cylinderMesh,
                0,
                Matrix4x4.TRS(center, Quaternion.FromToRotation(Vector3.up, direction), scale));
        }

        private void DrawNormal(Vector3 start, Vector3 normal, float length, ref RenderParams renderParams)
        {
            Vector3 end = start + normal * length;
            DrawLine(start, end, ref renderParams);
        }
    }
}
