using Pet.Input;
using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderPlayerSpawner
    {
        private readonly SpiderConfig spiderConfig;
        private readonly PlayerSpawnPoint spiderSpawnPoint;
        private readonly IPlayerInputStreams inputStreams;
        private readonly Camera movementCamera;

        public SpiderPlayerSpawner(
            SpiderConfig spiderConfig,
            PlayerSpawnPoint spiderSpawnPoint,
            IPlayerInputStreams inputStreams,
            Camera movementCamera)
        {
            this.spiderConfig = spiderConfig;
            this.spiderSpawnPoint = spiderSpawnPoint;
            this.inputStreams = inputStreams;
            this.movementCamera = movementCamera;
        }

        public SpiderPlayerController Spawn()
        {
            SpiderPlayerController instance = Object.Instantiate(
                spiderConfig.Prefab,
                spiderSpawnPoint.transform.position,
                spiderSpawnPoint.transform.rotation);

            instance.Initialize(spiderConfig, inputStreams, movementCamera);
            spiderSpawnPoint.gameObject.SetActive(false);
            return instance;
        }
    }
}
