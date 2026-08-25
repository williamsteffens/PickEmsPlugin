using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin : DeadworksPluginBase
{
    public override string Name => "PickEmsPlugin";

    public override void OnLoad(bool isReload)
    {
        Console.WriteLine(isReload ? "PickEmsPlugin reloaded!" : "PickEmsPlugin loaded!");
        Console.WriteLine("Loading hero ability mapping...");
        LoadHeroAbilityMapping();
    }

    public override void OnUnload()
    {
        Console.WriteLine("PickEmsPlugin unloaded!");
    }

    public override void OnPrecacheResources()
    {
        // precaching all heroes instead of just the ones in the draft pool, 
        // since we don't know which heroes will be in the draft pool at this point

        // this only works if heroes abilities are precached in their own .vpcf files
        // this is a bit of a hack, but it works for now
        // better: only heroes in the draft pool
        // ideal: only abilities?
        var heroes = Enum.GetValues<Heroes>()
            .Where(hero => hero.GetHeroData()?.AvailableInGame == true)
            .ToArray();

        foreach (var hero in heroes)
            Precache.AddHero(hero);

        Precache.AddHero(Heroes.Skyrunner);
    }

    public override HookResult OnClientConCommand(ClientConCommandEvent ev)
    {
        WriteConsole(null, $"client ran {string.Join(' ', ev.Args)}");

        var pawn = ev.Controller?.GetHeroPawn();
        if (pawn == null) return HookResult.Continue;

        WriteConsole(null, $"client hero pawn: {pawn.Name} ({pawn.Handle})");
        
        // pawn.AddItem("upgrade_mystic_reverb", true);
        // ev.Caller.GetHeroPawn()?.AddDependentItemUnchecked("upgrade_mystic_reverb", 1);
        return HookResult.Continue;
    }

    [GameEventHandler("ability_added")]
    public HookResult OnAbilityAdded(GameEvent ev)
    {
        WriteConsole(null, "GameEvent: ability_added");
        return HookResult.Continue;
    }

    [GameEventHandler("player_ability_upgraded")]
    public HookResult OnPlayerAbilityUpgraded(GameEvent ev)
    {
        WriteConsole(null, "GameEvent: player_ability_upgraded");
        return HookResult.Continue;
    }

    private static readonly SchemaAccessor<int> _stackCount = new("CBaseModifier"u8, "m_iStackCount"u8);

    static int GetStacks(CBaseEntity ent, string modifierName) {
        var prop = ent.ModifierProp;
        if (prop is null) 
            return 0;

        foreach (var m in prop.Modifiers)
            if (m.SubclassVData?.Name == modifierName)
                return _stackCount.Get(m.Handle);
        
        return 0;
    }
}
