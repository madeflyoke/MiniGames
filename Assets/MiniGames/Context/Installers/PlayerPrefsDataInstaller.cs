using MiniGames.Managers;
using Zenject;

namespace MiniGames.Context.Installers
{
    public class PlayerPrefsDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInstance(new PlayerPrefsData()).AsSingle().NonLazy();
        }
    }
}
