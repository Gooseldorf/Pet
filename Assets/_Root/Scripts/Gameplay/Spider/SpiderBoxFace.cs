using UnityEngine;

namespace Pet.Gameplay
{
    internal readonly struct SpiderBoxFace
    {
        public SpiderBoxFace(
            Vector3 center,
            Vector3 normal,
            Vector3 axisU,
            Vector3 axisV,
            float extentU,
            float extentV)
        {
            Center = center;
            Normal = normal;
            AxisU = axisU;
            AxisV = axisV;
            ExtentU = extentU;
            ExtentV = extentV;
        }

        public Vector3 Center { get; }
        public Vector3 Normal { get; }
        public Vector3 AxisU { get; }
        public Vector3 AxisV { get; }
        public float ExtentU { get; }
        public float ExtentV { get; }
    }
}
