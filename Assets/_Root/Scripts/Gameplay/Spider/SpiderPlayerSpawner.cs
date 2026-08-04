using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderPlayerSpawner
    {
        private readonly SpiderConfig spiderConfig;
        private readonly PlayerSpawnPoint spiderSpawnPoint;

        public SpiderPlayerSpawner(SpiderConfig spiderConfig, PlayerSpawnPoint spiderSpawnPoint)
        {
            this.spiderConfig = spiderConfig;
            this.spiderSpawnPoint = spiderSpawnPoint;
        }

        public SpiderPlayerController Spawn()
        {
            SpiderPlayerController instance = Object.Instantiate(
                spiderConfig.Prefab,
                spiderSpawnPoint.transform.position,
                spiderSpawnPoint.transform.rotation);

            spiderSpawnPoint.gameObject.SetActive(false);
            return instance;
        }
    }
}
