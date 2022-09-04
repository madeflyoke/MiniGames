using MiniGames.Managers;
using UnityEngine;
using Zenject;

public class ModulesControllerInstaller : MonoInstaller
{
    [SerializeField] private ModulesController modulesController;
    public override void InstallBindings()
    {
        Container.BindInstance(modulesController).AsSingle().NonLazy();
    }
}