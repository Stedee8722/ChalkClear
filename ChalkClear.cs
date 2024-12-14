using Cove.Server.Plugins;
using Cove.Server;
using Cove.GodotFormat;
using Cove.Server.Chalk;

// Change the namespace and class name!
namespace ChalkClear
{
    public class ChalkClear : CovePlugin
    {
        CoveServer Server { get; set; }
        public ChalkClear(CoveServer server) : base(server) {
            Server = server;
        }

        public override void onInit()
        {
            base.onInit();

            Log("Registering command!");
            RegisterCommand("clear_chalk", (player, args) =>
            {
                // Check for admin
                if (!IsPlayerAdmin(player)) return;
                // Parsing args
                if (args == null || args.Length == 0)
                {
                    SendPlayerChatMessage(player, "!clear_chalk [all:history:help]");
                    return;
                }
                var clearType = args[0].ToLower();
                long canvasID = 0;
                if (clearType == "help" || clearType == "h" || clearType == "-h" || clearType == "")
                {
                    SendPlayerChatMessage(player, "!clear_chalk [all:history:help]");
                    return;
                }
                else if (clearType == "history")
                {
                    SendPlayerChatMessage(player, "Removing old chalk data...");
                    Dictionary<Vector2, int> chalkImage = new Dictionary<Vector2, int>();
                    foreach (ChalkCanvas canvas in Server.chalkCanvas)
                    {
                        Dictionary<int, object> allChalk = canvas.getChalkPacket();
                        foreach (KeyValuePair<int, object> entry in allChalk)
                        {
                            Dictionary<int, object> data = (Dictionary<int, object>)entry.Value;
                            chalkImage[(Vector2)data[0]] = (int)data[1];
                        }
                        canvasID = canvas.canvasID;
                    }
                    Server.chalkCanvas.Clear();
                    ChalkCanvas new_canvas = new ChalkCanvas(canvasID);
                    Server.chalkCanvas.Add(new_canvas);
                    Dictionary<int, object> packet = new Dictionary<int, object>();
                    ulong i = 0;
                    foreach (KeyValuePair<Vector2, int> entry in chalkImage.ToList())
                    {
                        Dictionary<int, object> arr = new();
                        arr[0] = entry.Key;
                        arr[1] = entry.Value;
                        packet[(int)i] = arr;
                        i++;
                    }
                    new_canvas.chalkUpdate(packet);
                }
                else if (clearType == "all")
                {
                    SendPlayerChatMessage(player, "!Removing all chalk data...");
                    Server.chalkCanvas.Clear();
                }
            });
            SetCommandDescription("clear_chalk", "Clears all chalk data in the server");
        }

        public override void onEnd()
        {
            base.onEnd();

            // unregister all commands
            // this is needed to allow for the plugin to be reloaded!
            UnregisterCommand("clear_chalk");
        }
    }
}