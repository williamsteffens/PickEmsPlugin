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
            WriteConsole(caller, $"    Vdata: name: {ability.SubclassVData?.Name}");
            WriteConsole(caller, $"    Body Component: {ability.Classname}");
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

        var ability = pawn.AbilityComponent.GetAbilityBySlot((EAbilitySlot)slot);

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

    [Command(
    "dd_give",
    Description = "Prints the current hero ability mapping for all heroes or a specific hero if",
    Hidden = true
    )]
    public void CmdDraftDebugGive(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }


        // foreach (var ability in pawn.AbilityComponent.Abilities)
        // {
        //     if (ability.AbilityName.StartsWith("citadel_weapon_"))
        //     {
        //         // pawn.RemoveAbility(ability.AbilityName);
        //         // pawn.AbilityComponent.ToggleActivate(ability, true);
        //         WriteConsole(caller, $"Executing ability {ability.AbilityName} for player {caller?.PlayerSteamId}.");
        //         // pawn.AbilityComponent.ExecuteAbility(ability);
        //     }
        // }
    }

    [Command(
    "dd_mods",
    Description = "Prints the current modifiers of a hero pawn",
    Hidden = true
    )]
    public void CmdDraftDebugMods(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        foreach (var modifier in pawn.ModifierProp!.Modifiers)
        {
            WriteConsole(caller, $"Modifier: {modifier?.SubclassVData?.Name}");
            WriteConsole(caller, $"    Handle: {modifier?.Handle}");
        }
    }














    private static readonly SchemaAccessor<int> _weaponAccessorTest =
        new("CCitadel_Ability_PrimaryWeapon"u8, "m_iClip"u8);

    // private static readonly SchemaAccessor<nint> _pWeaponInfo = 
    //     new("CCitadelBasePlayer"u8, "m_pWeaponInfo"u8);





    // private static readonly SchemaAccessor<nint> _pWeaponServices =
    //     new("CCitadelBasePlayer"u8, "m_pWeaponServices"u8);

    // private static readonly SchemaAccessor<ushort> _iAmmo =
    //     new("CPlayer_WeaponServices"u8, "m_iAmmo"u8);

    public class CPlayer_WeaponServices(nint handle) : NativeEntity(handle)
    {
        private static ReadOnlySpan<byte> Class => "CPlayer_WeaponServices"u8;

        private static readonly SchemaAccessor<ushort> _iAmmo = new(Class, "m_iAmmo"u8);
        public ushort Ammo { get => _iAmmo.Get(Handle); set => _iAmmo.Set(Handle, value); }


        private static readonly SchemaAccessor<nint> _pWeaponServices =
            new("CBasePlayerPawn"u8, "m_pWeaponServices"u8);

        public CPlayer_WeaponServices? WeaponServices
        {
            get
            {
                nint ptr = _pWeaponServices.Get(Handle);
                return ptr != 0 ? new CPlayer_WeaponServices(ptr) : null;
            }
        }
    }




    [Command(
        "dd_aids",
        Hidden = true
    )]
    public void CmdAids(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }


        CPlayer_WeaponServices? weaponServices = new(pawn.Handle);
        ushort ammo = weaponServices.Ammo;
        weaponServices.Ammo = 120;







        // Plugin version not working:

        // nint weaponServices = _pWeaponServices.Get(pawn.Handle);
        // if (weaponServices == nint.Zero)
        // {
        //     WriteConsole(caller, "Weapon services pointer is null.");
        //     return;
        // }

        // ushort ammo = _iAmmo.Get(weaponServices);
        // // WriteConsole(caller, $"Weapon services pointer: {weaponServices}");





        WriteConsole(caller, $"Ammo: {ammo}");
    }




    // ------------------------------------------------------------------------
    // dd_wpn
    // ------------------------------------------------------------------------
    [Command(
        "dd_wpn",
        Description = "Prints the current weapon abilities for a hero pawn",
        Hidden = true
    )]
    public void CmdDraftDebugWpn(CCitadelPlayerController caller)
    {
        var pawn = caller?.GetHeroPawn();
        if (pawn == null)
        {
            WriteConsole(caller, "Player does not have a hero pawn.");
            return;
        }

        var weapon = pawn.GetAbilityBySlot(EAbilitySlot.WeaponPrimary);
        if (weapon == null)
            return;

        WriteConsole(caller, $"    Weapon: {weapon.DesignerName}");
        WriteConsole(caller, $"    Weapon Vdata name: {weapon.SubclassVData?.Name}");
        WriteConsole(caller, $"    Weapon Handle: {weapon.Handle}");
        WriteConsole(caller, $"    Weapon Body Component: {weapon.Classname}");

        WriteConsole(caller, $"    Weapon VDATA class?: {weapon.SubclassVData}");




        var clip = _weaponAccessorTest.Get(weapon.Handle);
        WriteConsole(caller, $"    Weapon Clip: {clip}");
        _weaponAccessorTest.Set(weapon.Handle, 120);

        weapon.SetScale(10.0f);
    }
}
