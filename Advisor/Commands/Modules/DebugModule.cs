﻿using System;
using Advisor.Commands.Attributes;
using Advisor.Commands.Entities;
using Advisor.Enums;

namespace Advisor.Commands.Modules
{
    public class DebugModule : CommandModule
    {
        [Command("test", SandboxRealm.Shared)]
        public void TestCommand(CommandContext ctx)
        {
            Console.WriteLine("Hello, world!");
        }

        [Command("fuck-this-player", SandboxRealm.Shared)]
        public void FuckThisPlayer(CommandContext ctx, string lePlayer)
        {
            
        }
    }
}