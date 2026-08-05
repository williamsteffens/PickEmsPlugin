# Deadlock Pick'ems/Drafting 

A **Deadlock** server plugin for creating custom ability kits for drafting/pick'em gamemodes.

Built using the **Deadworks modding tool**.

## Configuration

> [!NOTE]
> Hero names are based on internal naming and "displayNames". Refer to the `heroAbilityMapping.json` file, and/or use the `dw_draft_list [hero]` (the hero option is optional and will only list name and abilities of the given hero) command.

Abilities are defined in `heroAbilityMapping.json`:

```json
{
  "Inferno": {
    "1": "ability_incendiary_projectile",
    "2": "ability_flame_dash",
    "3": "ability_afterburn",
    "4": "ability_fire_bomb"
  }
}
```

The first number represents the **hero's ability slot**, while the value is the ability's internal name.

## Main Usage

> [!NOTE]
> Hero names are based on internal naming and "displayNames". Refer to the `heroAbilityMapping.json` file, and/or use the `dw_draft_list [hero]` (the hero option is optional and will only list name and abilities of the given hero) command.

Draft an ability using:

```text
/draft <draft_slot> <hero> <ability_slot>
```

Example:

```text
/draft 1 inferno 1
```

This places Inferno's ability from slot `1` into your current hero's slot `1`.

Another example:

```text
/draft 2 Hornet 1
```

This places Vindicta's ability from slot `4` into your current hero's slot `2`.

For the `/draft` command the hero names are case-insensitive, so the following are equivalent:

```text
/draft 1 inferno 1
/draft 1 Inferno 1
/draft 1 INFERNO 1
```

Use `dw_help` in the in-game console for more commands.

## Known Bugs

- Imbued items are not working: Trying to purchase a imbued item will fail without any loss of currency. 
- Some abilities depend on additional abilities (such as Viper's slide or Sinclair's Assistant) and are currently not able to be drafted.
- Some bugs are isolated to duplicated abilities:
    - Upgrading duplicated abilities result in only one ability getting upgraded, with a visual bug causing none of the tiers to be marked as upgraded. Currently, `dw_progress <slot> <tier>` can be used to upgrade the ability as a workaround.
    - Some abilities have one or more trigger/cancel abilities coupled to them (like Pocket's Flying Cloack or McGinnis' Spectral Wall) and upon adding abilities, the coupled abilities are only added on the first instance of the ability. This causes the coupled abilities to not function for later instances of the ability drafted and in some cases crashing may occur.


## Contributing

Contributions, suggestions, and bug reports are welcome.

If you find an issue or have an idea for a new drafting/pick'em feature, feel free to open an issue or submit a pull request.

## Credits

Built with **Deadworks**, a community modding framework/toolset for Deadlock.

Deadlock is a trademark of Valve Corporation. This project is an unofficial community plugin and is not affiliated with or endorsed by Valve.

## License

This project is licensed under the MIT License.
