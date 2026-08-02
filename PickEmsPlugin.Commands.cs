using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
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

        if (slot < 1 || slot > 4) // (ushort)EAbilitySlot.Signature4 used this before, but it was changed to 4 to match the abilitySlot check below
        {
            WriteConsole(caller, $"Invalid ability slot {slot}. Must be between 1 and 4.");
            return;
        }

        if (abilitySlot < 1 || abilitySlot > 4)
        {
            WriteConsole(caller, $"Invalid ability slot {abilitySlot}. Must be between 1 and 4.");
            return;
        }

        if (slot == 4 && abilitySlot != 4 || slot != 4 && abilitySlot == 4)
        {
            WriteConsole(caller, $"Invalid ability slot {abilitySlot} for ultimate ability. Ultimate ability can only be selected for slot 4.");
            return;
        }

        if (!_heroLookup.TryGetValue(heroName, out var hero))
        {
            WriteConsole(caller, $"Invalid hero name {heroName}. Must be a valid hero name.");
            return;
        }

        if (!_heroAbilityMapping.TryGetValue(hero.ToString(), out var abilities))
        {
            WriteConsole(caller, $"No ability mapping found for hero {heroName}.");
            return;
        }

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

    [Command("draft_list")]
    public void CmdDraftList(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        foreach (var hero in _heroAbilityMapping)
        {
            if (Enum.TryParse<Heroes>(hero.Key, true, out var heroEnum))
            {
                WriteConsole(caller, $"{heroEnum.ToDisplayName()} / \"{hero.Key}\":");
            }
            else
            {
                WriteConsole(caller, $"{hero.Key}:");
            }

            foreach (var ability in hero.Value.OrderBy(x => x.Key))
            {
                WriteConsole(caller, $"  {ability.Key}: {ability.Value}");
            }
        }
    }


    [Command("draft_random")]
    public void CmdDraftRandom(CCitadelPlayerController caller)
    {
        // this that could be added: 
        //     1. random hero selection 
        //     2. the same ability for all slots

        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        for (int slot = 1; slot <= 4; slot++)
        {
            var heroes = Enum.GetValues<Heroes>()
                 .Where(h => h.GetHeroData()?.AvailableInGame == true)
                 .ToArray();
            var randomHero = heroes[Random.Shared.Next(heroes.Length)];

            if (!_heroLookup.TryGetValue(randomHero.ToString(), out var hero))
            {
                WriteConsole(caller, $"Invalid hero name {randomHero}. Must be a valid hero name.");
                return;
            }

            if (!_heroAbilityMapping.TryGetValue(hero.ToString(), out var abilities))
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


    [Command("draft_all_from_hero")]
    public void CmdDraftAllFromHero(CCitadelPlayerController caller, string heroName)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        if (!_heroLookup.TryGetValue(heroName, out var hero))
        {
            WriteConsole(caller, $"Invalid hero name {heroName}. Must be a valid hero name.");
            return;
        }

        if (!_heroAbilityMapping.TryGetValue(hero.ToString(), out var abilities))
        {
            WriteConsole(caller, $"No ability mapping found for hero {heroName}.");
            return;
        }

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

        ProgressAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            upgrades
        );
    }
}
