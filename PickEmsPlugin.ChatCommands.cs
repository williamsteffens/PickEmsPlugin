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
            Console.WriteLine("Player does not have a hero pawn.");
            return;
        }

        if (slot < 1 || slot > 4) // (ushort)EAbilitySlot.Signature4 used this before, but it was changed to 4 to match the abilitySlot check below
        {
            Console.WriteLine($"Invalid ability slot {slot}. Must be between 1 and 4.");
            return;
        }

        if (abilitySlot < 1 || abilitySlot > 4)
        {
            Console.WriteLine($"Invalid ability slot {abilitySlot}. Must be between 1 and 4.");
            return;
        }

        if (slot == 4 && abilitySlot != 4 || slot != 4 && abilitySlot == 4)
        {
            Console.WriteLine($"Invalid ability slot {abilitySlot} for ultimate ability. Ultimate ability can only be selected for slot 4.");
            return;
        }

        if (!heroAbilityMapping.TryGetValue(heroName, out var abilities))
        {
            Console.WriteLine($"No ability mapping found for hero {heroName}.");
            return;
        }

        if (!abilities.TryGetValue(abilitySlot, out var ability))
        {
            Console.WriteLine($"No ability mapping found for slot {abilitySlot} for hero {heroName}.");
            return;
        }

        AddDraftAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            ability
        );

        Console.WriteLine($"Drafting {heroName}'s ability {ability} to slot {slot} for player {caller?.PlayerSteamId}.");
    }

    [Command("draft_list")]
    public void CmdDraftList(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            Console.WriteLine("Player does not have a hero pawn.");
            return;
        }

        foreach (var hero in heroAbilityMapping)
        {
            Console.WriteLine($"{hero.Key}:");

            foreach (var ability in hero.Value.OrderBy(x => x.Key))
            {
                Console.WriteLine(
                    $"  {ability.Key}: {ability.Value}");
            }
        }
    }


    [Command("draft_all_from_hero")]
    public void CmdDraftAllFromHero(CCitadelPlayerController caller, string heroName)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            Console.WriteLine("Player does not have a hero pawn.");
            return;
        }

        if (!heroAbilityMapping.TryGetValue(heroName, out var abilities))
        {
            Console.WriteLine($"No ability mapping found for hero {heroName}.");
            return;
        }

        foreach (var ability in abilities)
        {
            AddDraftAbility(
                pawn,
                ability.Key - 1, // Convert to 0-based index
                ability.Value
            );

            Console.WriteLine($"Drafting {heroName}'s ability {ability.Value} to slot {ability.Key} for player {caller?.PlayerSteamId}.");
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
            Console.WriteLine("Player does not have a hero pawn.");
            return;
        }

        ProgressAbility(
            pawn,
            slot - 1, // Convert to 0-based index
            upgrades
        );
    }
}
