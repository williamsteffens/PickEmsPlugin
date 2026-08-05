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
        return HookResult.Continue;
    }    
}
