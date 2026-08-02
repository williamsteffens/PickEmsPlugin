using System.Text.Json;
using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    private static readonly Dictionary<string, Dictionary<int, string>> heroAbilityMapping =
        LoadHeroAbilityMapping();

    // TODO: all of this should probably be moved to a config class https://docs.deadworks.net/api-reference/configuration
    private static Dictionary<string, Dictionary<int, string>> LoadHeroAbilityMapping()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        
        var path = Directory
        .EnumerateFiles(
            currentDirectory,
             "heroAbilityMapping.json",
            SearchOption.AllDirectories)
        .FirstOrDefault();

        Console.WriteLine($"Loading hero ability mapping from: {path}");

        if (!File.Exists(path))
        {
            Console.WriteLine($"Hero ability mapping not found: {path}");
            return new Dictionary<string, Dictionary<int, string>>(
                // Use case-insensitive comparison for hero names
                StringComparer.OrdinalIgnoreCase
            );
        }

        try
        {
            var json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<
                Dictionary<string, Dictionary<int, string>>
            >(json) ?? new Dictionary<string, Dictionary<int, string>>(
                StringComparer.OrdinalIgnoreCase
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load heroAbilityMapping.json: {ex}");
            return new Dictionary<string, Dictionary<int, string>>(
                StringComparer.OrdinalIgnoreCase
            );
        }
    }

    private static void AddDraftAbility(CCitadelPlayerPawn pawn, int slot, string newAbility)
    {
        CCitadelBaseAbility? oldAbility = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);
        string oldAbilityName = "citadel_ability_void_sphere";
        int oldUpgradeLevel = 0b00001;
        if (oldAbility != null)
        {
            oldAbilityName = oldAbility.AbilityName;
            oldUpgradeLevel = oldAbility.UpgradeBits;
            pawn.RemoveAbility(oldAbilityName);
        }

        try
        {
            var check = pawn.AddAbility(newAbility, (ushort)slot)
                ?? throw new InvalidOperationException($"Failed to add ability '{newAbility}' to slot {slot}.");
            pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot)!.UpgradeBits |= oldUpgradeLevel;
        }
        catch
        {
            pawn.AddAbility(oldAbilityName, (ushort)slot);
            pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot)!.UpgradeBits |= oldUpgradeLevel;
            throw;
        }
    }

    private static void ProgressAbility(CCitadelPlayerPawn pawn, int slot, int upgrades)
    {
        CCitadelBaseAbility? ability = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);
        if (ability == null)
        {
            Console.WriteLine($"No ability found in slot {slot} for pawn {pawn.Name}");
            return;
        }

        int updatedUpgradeBits = ability.UpgradeBits;
        updatedUpgradeBits = (updatedUpgradeBits << 1) | 1;

        ability.UpgradeBits = updatedUpgradeBits;
    }
}
