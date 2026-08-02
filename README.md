# Deadlock Pick'ems/Drafting 

A **Deadlock** server plugin for creating custom ability kits for  drafting/pick'em gamemodes.

Built using the **Deadworks modding tool**.

## Configuration

> [!NOTE]
> Hero names are based on in-game naming. Refer to the `heroAbilityMapping.json` file, or use the in-game `/draft --list_heroes` command.

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
> Hero names are based on in-game naming. Refer to the `heroAbilityMapping.json` file, or use the in-game `/draft --list_heroes` command.

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

See `/draft_help` in-game for more commands and usage.

## Contributing

Contributions, suggestions, and bug reports are welcome.

If you find an issue or have an idea for a new drafting/pick'em feature, feel free to open an issue or submit a pull request.

## Credits

Built with **Deadworks**, a community modding framework/toolset for Deadlock.

Deadlock is a trademark of Valve Corporation. This project is an unofficial community plugin and is not affiliated with or endorsed by Valve.

## License

This project is licensed under the MIT License.
