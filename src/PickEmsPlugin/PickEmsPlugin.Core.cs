using System.Text.Json;
using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    private static readonly Dictionary<string, Dictionary<int, string>> _heroAbilityMapping =
        LoadHeroAbilityMapping();

    private static readonly Dictionary<string, string[]> _abilityDependencyMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["citadel_ability_fissure_wall"] =
            [
                "citadel_ability_fissure_wall_cancel",
                "citadel_ability_fissure_wall_trigger"
            ],
            ["ability_viper_venom"] = ["ability_viper_slide"],
            ["ability_viper_debuffdagger"] = ["ability_viper_slide"]
        };

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

            var raw = JsonSerializer.Deserialize<Dictionary<string, Dictionary<int, string>>>(json);

            var result = new Dictionary<string, Dictionary<int, string>>(
                StringComparer.OrdinalIgnoreCase
            );

            if (raw == null)
            {
                Console.WriteLine($"Hero ability mapping is empty: {path}");
                return result;
            }

            foreach (var (hero, abilities) in raw)
                result[hero] = new Dictionary<int, string>(abilities);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load heroAbilityMapping.json: {ex}");
            return new Dictionary<string, Dictionary<int, string>>(
                StringComparer.OrdinalIgnoreCase
            );
        }
    }

    private static readonly Dictionary<string, Heroes> _heroLookup =
        BuildHeroLookup();

    private static Dictionary<string, Heroes> BuildHeroLookup()
    {
        var lookup = new Dictionary<string, Heroes>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var hero in Enum.GetValues<Heroes>())
        {
            lookup[hero.ToString()] = hero;

            var displayName = hero.ToDisplayName();

            if (!string.IsNullOrWhiteSpace(displayName))
                lookup[displayName] = hero;
        }

        return lookup;
    }

    private static readonly SchemaAccessor<bool> _abilityCanBeImbued =
        new("CCitadelBaseAbility"u8, "m_bCanBeImbued"u8);

    private static readonly SchemaAccessor<int> _abilityRemainingCharges =
        new("CCitadelBaseAbility"u8, "m_iRemainingCharges"u8);

    private void AddDraftAbility(CCitadelPlayerPawn pawn, int slot, string newAbility)
    {
        CCitadelBaseAbility? oldAbility = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);
        string oldAbilityName = "citadel_ability_void_sphere";
        int oldUpgradeLevel = 0b00001;
        if (oldAbility != null)
        {
            oldAbilityName = oldAbility.AbilityName;
            oldUpgradeLevel = oldAbility.UpgradeBits;

            bool oldCanBeImbued = _abilityCanBeImbued.Get(oldAbility.Handle);
            WriteConsole(pawn.Controller, $"old ability: {oldAbility.Classname}");
            WriteConsole(pawn.Controller, $"can be imbued: {oldCanBeImbued}");

            pawn.RemoveAbility(oldAbilityName);
        }

        try
        {
            var check = pawn.AddAbility(newAbility, (ushort)slot)
                ?? throw new InvalidOperationException($"Failed to add ability '{newAbility}' to slot {slot}.");
            CCitadelBaseAbility? newAbilityInstance = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);
            if (newAbilityInstance != null)
            {
                newAbilityInstance.UpgradeBits = oldUpgradeLevel;
                _abilityCanBeImbued.Set(newAbilityInstance.Handle, false);
                WriteConsole(pawn.Controller, $"new ability: {newAbilityInstance.Classname}");
                WriteConsole(pawn.Controller, $"new ability can be imbued: {_abilityCanBeImbued.Get(newAbilityInstance.Handle)}");
                WriteConsole(pawn.Controller, $"new ability remaining charges: {_abilityRemainingCharges.Get(newAbilityInstance.Handle)}");
                _abilityRemainingCharges.Set(newAbilityInstance.Handle, 1);
            }
        }
        catch
        {
            // If adding the new ability fails, restore the old ability and its upgrade level
            pawn.AddAbility(oldAbilityName, (ushort)slot);
            pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot)!.UpgradeBits = oldUpgradeLevel;
            throw;
        }
    }

    public static int ApplyUpgradeProgress(int currentBits, int upgrades)
    {
        if (upgrades <= 0)
            return currentBits | 0b00001;

        return (currentBits << upgrades) | ((1 << upgrades) - 1);
    }

    private void ProgressAbility(CCitadelPlayerPawn pawn, int slot, int upgrades)
    {
        CCitadelBaseAbility? ability = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);
        if (ability == null)
        {
            WriteConsole(pawn.Controller, $"No ability found in slot {slot} for pawn {pawn.Name}");
            return;
        }

        ability.UpgradeBits = ApplyUpgradeProgress(ability.UpgradeBits, upgrades);
    }
}
