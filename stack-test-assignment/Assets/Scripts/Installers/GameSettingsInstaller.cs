using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public Cube.Settings CubeSettings;
    public CubeManager.Settings CubeManagerSettings;
    public CameraController.Settings CameraSettings;
    public ColorSchemeManager.Settings ColorSchemeSettings;
    public GameInstaller.Settings GameInstallerSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(CubeSettings);
        Container.BindInstance(CubeManagerSettings);
        Container.BindInstance(CameraSettings);
        Container.BindInstance(ColorSchemeSettings);
        Container.BindInstance(GameInstallerSettings);
    }
}