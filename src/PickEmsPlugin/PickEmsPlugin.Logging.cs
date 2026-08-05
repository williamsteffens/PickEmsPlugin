using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    private void WriteConsole(CCitadelPlayerController? caller, string msg)
    {
        if (caller != null)
            caller.PrintToConsole($"[{Name}] "+msg);
        else
            Console.WriteLine($"[{Name}] "+msg);
    }
}
