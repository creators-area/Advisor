using System;
using Advisor.Commands.Attributes;
using Advisor.Commands.Entities;
using Advisor.Enums;
using Sandbox;

namespace Advisor.Commands.Modules
{
	[Category("Advisor: Debug", "debug")]
    public class DebugModule : CommandModule
    {
        [Command("test", SandboxRealm.Shared)]
        public void TestCommand(CommandContext ctx)
        {
            Console.WriteLine("Hello, world!");
        }

        [Command("fuck-this-player", SandboxRealm.Shared, targetLevel: TargetPermission.All)]
        public void FuckThisPlayer(CommandContext ctx, string lePlayer, bool b, int i, float f, long l, char c)
        {
            Console.WriteLine($"Fuck '{lePlayer}'!");
        }
    }
}
