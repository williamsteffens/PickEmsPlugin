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















    // public class CPlayer_WeaponServices(nint handle) : NativeEntity(handle)
    // {
    //     private static ReadOnlySpan<byte> Class => "CPlayer_WeaponServices"u8;

    //     private static readonly SchemaAccessor<ushort> _iAmmo = new(Class, "m_iAmmo"u8);
    //     public ushort Ammo { get => _iAmmo.Get(Handle); set => _iAmmo.Set(Handle, value); }


    //     private static readonly SchemaAccessor<nint> _pWeaponServices =
    //         new("CBasePlayerPawn"u8, "m_pWeaponServices"u8);

    //     public CPlayer_WeaponServices? WeaponServices
    //     {
    //         get
    //         {
    //             nint ptr = _pWeaponServices.Get(Handle);
    //             return ptr != 0 ? new CPlayer_WeaponServices(ptr) : null;
    //         }
    //     }
    // }


    private static readonly SchemaAccessor<nint> _pWeaponServices =
        new("CCitadelBasePlayer"u8, "m_pWeaponServices"u8);

    private static readonly SchemaAccessor<ushort> _iAmmo =
        new("CPlayer_WeaponServices"u8, "m_iAmmo"u8);



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

        // CPlayer_WeaponServices? weaponServices = new(pawn.Handle);
        // ushort ammo = weaponServices.Ammo;
        // weaponServices.Ammo = 120;

        // Plugin version not working:

        nint weaponServices = _pWeaponServices.Get(pawn.Handle);
        if (weaponServices == nint.Zero)
        {
            WriteConsole(caller, "Weapon services pointer is null.");
            return;
        }

        ushort ammo = _iAmmo.Get(weaponServices);
        // WriteConsole(caller, $"Weapon services pointer: {weaponServices}");

        WriteConsole(caller, $"Ammo: {ammo}");
    }



    
    private static readonly SchemaAccessor<int> _weaponAccessorTest =
        new("CCitadel_Ability_PrimaryWeapon"u8, "m_iClip"u8);

    // private static readonly SchemaAccessor<nint> _pWeaponInfo = 
    //     new("CCitadelBasePlayer"u8, "m_pWeaponInfo"u8);


    private static readonly SchemaAccessor<int> _weaponInfo =
        new("CitadelAbilityVData"u8, "m_WeaponInfo"u8);

    private static readonly SchemaAccessor<float> m_flBulletDamage =
        new("CCitadelWeaponInfo"u8, "m_flBulletDamage"u8);


    // ------------------------------------------------------------------------
    // dd_wpn
    // ------------------------------------------------------------------------
    [Command(
        "dd_wpn",
        Description = "Prints the current weapon abilities for a hero pawn",
        Hidden = true
    )]
    public unsafe void CmdDraftDebugWpn(CCitadelPlayerController caller)
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

        var vdata = weapon.SubclassVData;

        // 0x158 is the offset for m_WeaponInfo in CitadelAbilityVData, but we can use the SchemaAccessor instead of hardcoding the offset.

        if (vdata is null)
            return;

        nint weaponInfoHandle = vdata.Handle + 0x158;

        // 0x30 is the clipsize offset in CCitadelWeaponInfo
        *(int*)((byte*)weaponInfoHandle + 0x30) = 45;
        int clipSize =  *(int*)((byte*)weaponInfoHandle + 0x30);

        WriteConsole(caller, $"Weapon Clip Size: {clipSize}");

        // burst shot
        *(int*)((byte*)weaponInfoHandle + 0x3C) = 1;

        // gravity scale
        *(float*)((byte*)weaponInfoHandle + 0xBC) = 1.0f;
        float gravityScale = *(float*)((byte*)weaponInfoHandle + 0xBC);
        WriteConsole(caller, $"Weapon Gravity Scale: {gravityScale}");

        // bullet speed
        *(float*)((byte*)weaponInfoHandle + 0xB4) = 1000.0f;
        float bulletSpeed = *(float*)((byte*)weaponInfoHandle + 0xB4);
        WriteConsole(caller, $"Weapon Bullet Speed: {bulletSpeed}");

        // bullet radius
        *(float*)((byte*)weaponInfoHandle + 0xC0) = 200000.0f;
        float bulletRadius = *(float*)((byte*)weaponInfoHandle + 0xC0);
        WriteConsole(caller, $"Weapon Bullet Radius: {bulletRadius}");

        // builduprate
        *(float*)((byte*)weaponInfoHandle + 0xC4) = 1.0f;
        float buildUpRate = *(float*)((byte*)weaponInfoHandle + 0xAC);
        WriteConsole(caller, $"Weapon Build Up Rate: {buildUpRate}");

        // spinsup
        *(bool*)((byte*)weaponInfoHandle + 0x9C) = true;

        // canzoom
        *(bool*)((byte*)weaponInfoHandle + 0xD4) = false;


        // nint weaponInfo = _weaponInfo.Get(weapon.SubclassVData!.Handle);

        // if (weaponInfo == nint.Zero)
        // {
        //     WriteConsole(caller, "Weapon info pointer is null.");
        //     return;
        // }

        // WriteConsole(caller, $"Weapon info pointer: {weaponInfo}");

        // var bulletDamage = m_flBulletDamage.Get(weaponInfo);
        // WriteConsole(caller, $"Weapon bullet damage: {bulletDamage}");

        // WriteConsole(caller, $"    Weapon: {weapon.DesignerName}");
        // WriteConsole(caller, $"    Weapon Vdata name: {weapon.SubclassVData?.Name}");
        // WriteConsole(caller, $"    Weapon Handle: {weapon.Handle}");
        // WriteConsole(caller, $"    Weapon Body Component: {weapon.Classname}");

        // WriteConsole(caller, $"    Weapon VDATA class?: {weapon.SubclassVData}");




        // var clip = _weaponAccessorTest.Get(weapon.Handle);
        // WriteConsole(caller, $"    Weapon Clip: {clip}");
        // _weaponAccessorTest.Set(weapon.Handle, 120);

        // weapon.SetScale(10.0f);
    }
}
