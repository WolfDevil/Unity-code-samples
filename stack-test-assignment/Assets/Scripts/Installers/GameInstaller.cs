using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] Settings settings = null;

    public override void InstallBindings()
    {
        Container.BindFactory<Cube, Cube.Factory>().FromComponentInNewPrefab(settings.CubePrefab).WithGameObjectName("Cube").UnderTransformGroup("Cubes");
    }

    [Serializable]
    public class Settings
    {
        public GameObject CubePrefab;
    }
}