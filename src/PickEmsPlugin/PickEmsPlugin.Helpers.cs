using System.Text.Json;
using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    private const int MinAbilitySlot = 1;
    private const int MaxAbilitySlot = 4;

    private bool IsValidAbilitySlot(CCitadelPlayerController? caller, int slot, string slotName)
    {
        if (slot >= MinAbilitySlot && slot <= MaxAbilitySlot)
            return true;

        WriteConsole(caller, $"Invalid {slotName} slot {slot}. Must be between {MinAbilitySlot} and {MaxAbilitySlot}.");
        
        return false;
    }

    private bool IsValidAbilitySlotCombination(CCitadelPlayerController? caller, int draftSlot, int abilitySlot)
    {
        var draftIsUltimate = draftSlot == MaxAbilitySlot;
        var abilityIsUltimate = abilitySlot == MaxAbilitySlot;

        if (draftIsUltimate == abilityIsUltimate)
            return true;

        WriteConsole(
            caller, 
            $"Invalid ability slot {abilitySlot} for draft slot {draftSlot}. Ultimate abilities can only be selected for slot {MaxAbilitySlot}."
        );

        return false;
    }

    private bool TryGetHeroAbilities(CCitadelPlayerController? caller, string heroName, out Heroes hero, out Dictionary<int, string> abilities)
    {
        hero = default;
        abilities = null!;
        if (!_heroLookup.TryGetValue(heroName, out hero))
        {
            WriteConsole(caller, $"Invalid hero name '{heroName}'. Must be a valid hero name.");
            return false;
        }

        if (!_heroAbilityMapping.TryGetValue(hero.ToString(), out abilities!))
        {
            WriteConsole(caller, $"No ability mapping found for hero {hero.ToDisplayName()}.");
            return false;
        }

        return true;
    }

    private void PrintAbilities(CCitadelPlayerController? caller, Dictionary<int, string> abilities)
    {
        foreach (var ability in abilities.OrderBy(x => x.Key))
            WriteConsole(caller, $"    {ability.Key}: {ability.Value}");
    }
}
