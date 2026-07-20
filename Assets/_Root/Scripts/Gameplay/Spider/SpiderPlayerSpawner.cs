using UnityEngine;
using VContainer;

namespace Pet.Gameplay
{
    public class SpiderPlayerSpawner
    {
        private readonly SpiderConfig spiderConfig;
        private readonly PlayerSpawnPoint spiderSpawnPoint;
        private readonly IObjectResolver objectResolver;

        public SpiderPlayerSpawner(SpiderConfig spiderConfig, PlayerSpawnPoint spiderSpawnPoint, IObjectResolver objectResolver)
        {
            this.spiderConfig = spiderConfig;
            this.spiderSpawnPoint = spiderSpawnPoint;
            this.objectResolver = objectResolver;
        }

        public SpiderPlayerController Spawn()
        {
            SpiderPlayerController instance = Object.Instantiate(
                spiderConfig.Prefab,
                spiderSpawnPoint.transform.position,
                spiderSpawnPoint.transform.rotation);

            objectResolver.Inject(instance);
            instance.Initialize();
            spiderSpawnPoint.gameObject.SetActive(false);
            return instance;
        }
    }
}
