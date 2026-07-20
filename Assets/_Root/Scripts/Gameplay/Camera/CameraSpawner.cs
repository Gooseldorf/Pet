using UnityEngine;
namespace Pet.Gameplay
{
    public class CameraSpawner
    {
        private readonly CameraConfig cameraConfig;

        public CameraSpawner(CameraConfig cameraConfig)
        {
            this.cameraConfig = cameraConfig;
        }

        public CameraRig Spawn()
        {
            return Object.Instantiate(cameraConfig.Prefab);
        }
    }
}
