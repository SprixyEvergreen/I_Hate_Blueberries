using System;
using MonoMod.Cil;

namespace Celeste.Mod.IHateBlueberries;
public class BlueberryModule : EverestModule
{
    public static BlueberryModule Instance { get; private set; }

    public override Type SettingsType => typeof(BlueberryModuleSettings);
    public static BlueberryModuleSettings Settings => (BlueberryModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(BlueberryModuleSession);
    public static BlueberryModuleSession Session => (BlueberryModuleSession) Instance._Session;

    public BlueberryModule()
    {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(BlueberryModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(BlueberryModule), LogLevel.Info);
#endif
    }

    public override void Load()
    {
        IL.Celeste.Strawberry.ctor += Hook_CheckStrawberry;
        IL.Celeste.Strawberry.Added += Hook_CheckStrawberry;
        IL.Celeste.HeartGem.Awake += Hook_CheckHeartGem;
        IL.Celeste.Cassette.Added += Hook_CheckCassette;
    }

    public override void Unload()
    {
        IL.Celeste.Strawberry.ctor -= Hook_CheckStrawberry;
        IL.Celeste.Strawberry.Added -= Hook_CheckStrawberry;
        IL.Celeste.HeartGem.Awake -= Hook_CheckHeartGem;
        IL.Celeste.Cassette.Added -= Hook_CheckCassette;
    }

    private void Hook_CheckStrawberry(ILContext il)
    {
        ILCursor cursor = new(il);
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SaveData>("CheckStrawberry")))
            cursor.EmitDelegate(Override_CheckStrawberry);
    }
    private bool Override_CheckStrawberry(bool wasBlueberry)
        => !Settings.NoBlueberrys && wasBlueberry;

    private void Hook_CheckHeartGem(ILContext il)
    {
        ILCursor cursor = new(il);
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<AreaModeStats>("HeartGem")))
            cursor.EmitDelegate(Override_CheckHeartGem);
    }
    private bool Override_CheckHeartGem(bool wasBlueheart)
    => !Settings.NoBluehearts && wasBlueheart;

    private void Hook_CheckCassette(ILContext il)
    {
        ILCursor cursor = new(il);
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<AreaStats>("Cassette")))
            cursor.EmitDelegate(Override_Cassette);
    }
    private bool Override_Cassette(bool wasBlueCassette)
    => !Settings.NoBluecassettes && wasBlueCassette;
}
