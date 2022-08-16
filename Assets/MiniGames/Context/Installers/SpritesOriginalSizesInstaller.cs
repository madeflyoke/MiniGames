using MiniGames.Modules.Level.Utils;
using UnityEngine;
using Zenject;

public class SpritesOriginalSizesInstaller : MonoInstaller
{
    [SerializeField] private SpritesOriginalSizes spritesOriginalSizes;
    public override void InstallBindings()
    {
        Container.BindInstance(spritesOriginalSizes).AsSingle().NonLazy();
    }
}