using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    // TODO:
    // I want a short hand version of this that just takes 
    // slot and hero name 
    // makes it cleaner for ult as well and will still allow for 
    // ult and signature 0-2 swap later if i figure that out

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
        if (caller?.GetHeroPawn() is not { } pawn)
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
        if (caller?.GetHeroPawn() is not { } pawn)
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
        if (caller?.GetHeroPawn() is not { })
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

        if (caller?.GetHeroPawn() is not { } pawn)
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
        if (caller?.GetHeroPawn() is not { } pawn)
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
        if (caller?.GetHeroPawn() is not { } pawn)
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
