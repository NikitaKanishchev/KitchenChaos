using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Loader
{
    public static class Loader
    {
        private static Scene targetScene;
    
        public enum Scene
        {
            MainMenuScene,
            GameScene,
            LoadingScene,
            LobbyScene,
            CharacterSelectScene,
        }

        public static void Load(Scene targetScene)
        {
            Loader.targetScene = targetScene;

            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        public static void LoadNetwork(Scene targetScene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        }

        public static void LoaderCallback() => 
            SceneManager.LoadScene(targetScene.ToString());
    }
}
