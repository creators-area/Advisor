using System;
using System.Reflection.Metadata;
using Advisor.Commands.Attributes;
using Advisor.Commands.Entities;
using Advisor.Enums;
using Advisor.Utils;
using Sandbox;
using Sandbox.Hooks;

namespace Advisor.Commands.Modules
{
	[Category("Advisor: Debug", "debug")]
    public class DebugModule : CommandModule
    {
        [Command("test", SandboxRealm.Shared)]
        public void TestCommand(CommandContext ctx)
        {
            AdvisorLog.Error("Hello, world!");
        }

        [Command("fuck-this-player", SandboxRealm.Shared, targetLevel: TargetPermission.All)]
        public void FuckThisPlayer(CommandContext ctx, string lePlayer, bool b, int i, float f, long l, char c)
        {
            Console.WriteLine($"Fuck '{lePlayer}'!");
        }

        [Command( "echo", SandboxRealm.Server)]
        public void EchoMessage(CommandContext ctx, [Remainder] string text)
        {
	        var cmd = ConsoleSystem.Build( "chat_add", "Server", text, "ui/icons/advisor.png" );
	        ctx.Caller.SendCommandToClient(cmd);
        }

        [Command( "scale", SandboxRealm.Shared )]
        public void SetScale( CommandContext ctx, float newScale )
        {
	        ctx.Caller.WorldScale = newScale;
        }
    }
}
