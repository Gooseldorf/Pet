using UnityEngine;
namespace Pet.Gameplay
{
    public class CameraSpawner
    {
        private readonly CameraConfig cameraConfig;

        // Получает конфигурацию префаба камеры, чтобы создать отдельный rig для заспавненного паука.
        public CameraSpawner(CameraConfig cameraConfig)
        {
            this.cameraConfig = cameraConfig;
        }

        // Создает rig камеры; привязку к целям паука выполняет владелец порядка запуска отдельным явным шагом.
        public CameraRig Spawn()
        {
            return Object.Instantiate(cameraConfig.Prefab);
        }
    }
}
