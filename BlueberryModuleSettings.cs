namespace Celeste.Mod.IHateBlueberries;
public class BlueberryModuleSettings : EverestModuleSettings
{
    [SettingInGame(false)]
    public bool NoBlueberrys { get; set; } = true;

    [SettingInGame(false)]
    public bool NoBluehearts { get; set; } = true;

    [SettingInGame(false)]
    public bool NoBluecassettes { get; set; } = true;
}
