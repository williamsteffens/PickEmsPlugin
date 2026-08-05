using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    // ------------------------------------------------------------------------
    // dd_curr
    // ------------------------------------------------------------------------

    [Command(
        "dd_curr",
        Description = "Prints the current hero ability mapping for all heroes or a specific hero if",
        Hidden = true
    )]
    public void CmdDraftDebugCurr(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        foreach (var ability in pawn.AbilityComponent.Abilities)
        {
            WriteConsole(caller, $"{ability.AbilityName}");
            // WriteConsole(caller, $"    Handle: {ability.Handle}");
        }
    }

    // ------------------------------------------------------------------------
    // dd_abi
    // ------------------------------------------------------------------------

    [Command(
        "dd_abi",
        Hidden = true
    )]
    public void CmdDraftDebugAbi(CCitadelPlayerController caller, int slot)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        var ability = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot )slot);

        // neat idea but crashes
        // CCitadelBaseAbility? my_new_ability = (CCitadelBaseAbility)CBaseEntity.CreateByName("citadel_ability_void_sphere");


        WriteConsole(caller, $"Ability for slot {slot}: {ability?.AbilityName}");
        WriteConsole(caller, $"    Handle: {ability?.Handle}");
        // WriteConsole(caller, $"    Body Component: {ability?.BodyComponent}");

    }

    // ------------------------------------------------------------------------
    // dd_wt
    // ------------------------------------------------------------------------

    [Command(
        "dd_wt",
        Description = "Prints the current hero ability mapping for all heroes or a specific hero if",
        Hidden = true
    )]
    public void CmdDraftDebugWeaponTest(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        foreach (var ability in pawn.AbilityComponent.Abilities)
        {
            if (ability.AbilityName.StartsWith("citadel_weapon_"))
            {
                // pawn.RemoveAbility(ability.AbilityName);
                // pawn.AbilityComponent.ToggleActivate(ability, true);
                WriteConsole(caller, $"Executing ability {ability.AbilityName} for player {caller?.PlayerSteamId}.");
                // pawn.AbilityComponent.ExecuteAbility(ability);
            }
            WriteConsole(caller, $"Ability vdata: {ability?.Handle}");

            pawn.ExecuteAbility(ability!);
            WriteConsole(caller, $"Ability Component: {pawn.AbilityComponent}");
        }
    }
}
