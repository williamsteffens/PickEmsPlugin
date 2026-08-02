using DeadworksManaged.Api;

namespace PickEmsPlugin;

public partial class PickEmsPlugin
{
    private static void WriteConsole(CCitadelPlayerController? caller, string msg)
    {
        if (caller != null)
            caller.PrintToConsole("[PickEmsPlugin] "+msg);
        else
            Console.WriteLine("[PickEmsPlugin] "+msg);
    }
}
