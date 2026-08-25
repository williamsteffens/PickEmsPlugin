using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin : DeadworksPluginBase
{
    [ConVar("dw_my_plugin_enabled", Description = "Turn the plugin on or off")]
    public bool Enabled { get; set; } = true;

    [ConVar("dw_my_plugin_allow_duplicated_abilities_on_same_hero", Description = "Allow duplicated abilities on the same hero")]
    public bool AllowDuplicatedAbilitiesOnSameHero { get; set; } = true;

    [ConVar("dw_my_plugin_allow_duplicated_abilities_on_different_heroes", Description = "Allow duplicated abilities on different heroes")]
    public bool AllowDuplicatedAbilitiesOnDifferentHeroes { get; set; } = true;

}
