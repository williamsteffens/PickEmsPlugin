using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    // ------------------------------------------------------------------------ 
    // draft <slot> <hero> <abilitySlot> 
    // ------------------------------------------------------------------------

    [Command("draft")]
    public void CmdDraft(
        CCitadelPlayerController caller,
        int slot,
        string heroName,
        int abilitySlot)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        if (
            !IsValidAbilitySlot(caller, slot, "draft") ||
            !IsValidAbilitySlot(caller, abilitySlot, "ability") ||
            !IsValidAbilitySlotCombination(caller, slot, abilitySlot)
        )
            return;


        if (!TryGetHeroAbilities(caller, heroName, out var hero, out var abilities))
            return;

        if (!abilities.TryGetValue(abilitySlot, out var ability))
        {
            WriteConsole(caller, $"No ability mapping found for slot {abilitySlot} for hero {heroName}.");
            return;
        }

        AddDraftAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            ability
        );

        WriteConsole(caller, $"Drafting {heroName}'s ability {ability} to slot {slot} for player {caller?.PlayerSteamId}.");
    }

    // ------------------------------------------------------------------------ 
    // draft_by_name <slot> <abilityName> 
    // ------------------------------------------------------------------------

    [Command("draft_by_name")]
    public void CmdDraftByName(
        CCitadelPlayerController caller,
        int slot,
        string abilityName)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        if (!IsValidAbilitySlot(caller, slot, "draft"))
            return;


        AddDraftAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            abilityName
        );

        WriteConsole(caller, $"Drafting ability {abilityName} to slot {slot} for player {caller?.PlayerSteamId}.");
    }

    // ------------------------------------------------------------------------
    // draft_list [hero]
    // ------------------------------------------------------------------------

    [Command("draft_list")]
    public void CmdDraftList(CCitadelPlayerController caller, params string[] args)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        var heroName = args.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(heroName))
        {
            if (!TryGetHeroAbilities(caller, heroName, out var hero, out var abilities))
                return;

            WriteConsole(caller, $"{hero.ToDisplayName()} / \"{hero}\":");

            PrintAbilities(caller, abilities);

            return;
        }

        foreach (var entry in _heroAbilityMapping)
        {
            if (_heroLookup.TryGetValue(entry.Key, out var hero))
            {
                WriteConsole(caller, $"{hero.ToDisplayName()} / \"{entry.Key}\":");
            }
            else
            {
                WriteConsole(caller, $"{entry.Key}:");
            }

            PrintAbilities(caller, entry.Value);
        }
    }

    // ------------------------------------------------------------------------ 
    // draft_random 
    // ------------------------------------------------------------------------

    [Command("draft_random")]
    public void CmdDraftRandom(CCitadelPlayerController caller)
    {
        // this that could be added: 
        //     1. random hero selection 
        //     2. the same ability for all slots
        //     3. different abilities not corresponding to the same slot of the random hero

        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        var heroes = Enum.GetValues<Heroes>()
                 .Where(h => h.GetHeroData()?.AvailableInGame == true)
                 .ToArray();

        for (int slot = MinAbilitySlot; slot <= MaxAbilitySlot; slot++)
        {
            var randomHero = heroes[Random.Shared.Next(heroes.Length)];

            if (!_heroAbilityMapping.TryGetValue(randomHero.ToString(), out var abilities))
            {
                WriteConsole(caller, $"No ability mapping found for hero {randomHero}.");
                return;
            }

            AddDraftAbility(
                pawn,
                slot - 1, // Convert to 0-based index
                abilities[slot]
            );

            WriteConsole(caller, $"Drafting {randomHero}'s ability {abilities[slot]} to slot {slot} for player {caller?.PlayerSteamId}.");
        }
    }

    // ------------------------------------------------------------------------
    // draft_all_from_hero <hero> 
    // ------------------------------------------------------------------------

    [Command("draft_all_from_hero")]
    public void CmdDraftAllFromHero(CCitadelPlayerController caller, string heroName)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        if (!TryGetHeroAbilities(caller, heroName, out var hero, out var abilities))
            return;

        foreach (var ability in abilities)
        {
            AddDraftAbility(
                pawn,
                ability.Key - 1, // Convert to 0-based index
                ability.Value
            );

            WriteConsole(caller, $"Drafting {heroName}'s ability {ability.Value} to slot {ability.Key} for player {caller?.PlayerSteamId}.");
        }
    }

    // ------------------------------------------------------------------------
    // progress <slot> <upgrades> 
    // ------------------------------------------------------------------------

    [Command("progress")]
    public void CmdProgress(
        CCitadelPlayerController caller,
        int slot,
        int upgrades)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        if (!IsValidAbilitySlot(caller, slot, "ability"))
            return;

        if (upgrades < 0)
        {
            WriteConsole(caller, $"Invalid upgrades {upgrades}. Must be a non-negative integer.");
            return;
        }

        ProgressAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            upgrades
        );
    }
}
