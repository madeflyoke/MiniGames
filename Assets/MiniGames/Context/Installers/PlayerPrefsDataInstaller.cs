using MiniGames.Managers;
using Zenject;

    public class PlayerPrefsDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInstance(new PlayerPrefsData()).AsSingle().NonLazy();
        }
    }
