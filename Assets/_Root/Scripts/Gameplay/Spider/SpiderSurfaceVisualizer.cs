using UnityEngine;
using UnityEngine.Rendering;

namespace Pet.Gameplay
{
    public sealed class SpiderSurfaceVisualizer : MonoBehaviour
    {
        private const float CONTACT_RADIUS = 0.025f;
        private const float CONTACT_NORMAL_LENGTH = 0.25f;
        private const float SELECTED_CONTACT_RADIUS = 0.04f;
        private const float SURFACE_RADIUS = 0.05f;
        private const float SURFACE_NORMAL_LENGTH = 0.5f;
        private const float NORMAL_RADIUS = 0.005f;

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
            SpiderSurfaceDetector detector = playerController.SurfaceDetector;

            if (!detector.HasSample)
            {
                return;
            }

            DrawSphere(detector.BodyCenter, detector.SearchRadius, ref searchRadiusRenderParams);

            for (int contactIndex = 0; contactIndex < detector.ContactCount; contactIndex++)
            {
                SpiderSurfaceContact contact = detector.GetContact(contactIndex);
                DrawSphere(contact.Point, CONTACT_RADIUS, ref contactRenderParams);
                DrawNormal(contact.Point, contact.Normal, CONTACT_NORMAL_LENGTH, ref contactRenderParams);
            }

            for (int contactIndex = 0; contactIndex < detector.SelectedContactCount; contactIndex++)
            {
                SpiderSurfaceContact contact = detector.GetSelectedContact(contactIndex);
                DrawSphere(contact.Point, SELECTED_CONTACT_RADIUS, ref contactRenderParams);
            }

            SpiderSurfaceState state = detector.State;

            if (state.IsAttached)
            {
                DrawSphere(state.Point, SURFACE_RADIUS, ref contactRenderParams);
                DrawNormal(state.Point, state.Normal, SURFACE_NORMAL_LENGTH, ref contactRenderParams);
            }
        }

        private void DrawSphere(Vector3 position, float radius, ref RenderParams renderParams)
        {
            float diameter = radius * 2f;
            Vector3 scale = Vector3.one * diameter;
            renderParams.worldBounds = new Bounds(position, scale);
            Graphics.RenderMesh(in renderParams, sphereMesh, 0, Matrix4x4.TRS(position, Quaternion.identity, scale));
        }

        private void DrawNormal(Vector3 start, Vector3 normal, float length, ref RenderParams renderParams)
        {
            Vector3 center = start + normal * (length * 0.5f);
            Vector3 scale = new(NORMAL_RADIUS * 2f, length * 0.5f, NORMAL_RADIUS * 2f);
            renderParams.worldBounds = new Bounds(center, Vector3.one * (length + NORMAL_RADIUS * 2f));

            Graphics.RenderMesh(
                in renderParams,
                cylinderMesh,
                0,
                Matrix4x4.TRS(center, Quaternion.FromToRotation(Vector3.up, normal), scale));
        }
    }
}
