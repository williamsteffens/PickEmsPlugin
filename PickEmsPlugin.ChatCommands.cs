using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    [Command("draft")]
    public void CmdDraft(
        CCitadelPlayerController caller,
        int slot,
        string hero,
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

        if (!heroAbilityMapping.TryGetValue(hero, out var abilities))
        {
            Console.WriteLine($"No ability mapping found for hero {hero}.");
            return;
        }

        if (!abilities.TryGetValue(abilitySlot, out var ability))
        {
            Console.WriteLine($"No ability mapping found for slot {abilitySlot} for hero {hero}.");
            return;
        }

        AddDraftAbility(
            pawn,
            slot,
            ability
        );

        Console.WriteLine($"Drafting {hero}'s ability {ability} to slot {slot} for player {caller?.PlayerSteamId}.");
    }
}
