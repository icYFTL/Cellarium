﻿using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using Cellarium.Utils;

if (Utils.PriorProcess() != null)
{
    Console.WriteLine("Another instance of the app is already running.");
    Environment.Exit(-1);
}

if (!File.Exists(".token"))
{
    Console.WriteLine(".token file must exists");
    Environment.Exit(-1);
}

var token = File.ReadAllText(".token");

if (token.Length == 0)
{
    Console.WriteLine(".token file must contains token");
    Environment.Exit(-1);
}

Environment.SetEnvironmentVariable("token", token);

var parsedArgs = ArgsHandler.Parse();

if (parsedArgs.Item1 == null)
{
    Console.WriteLine("Type -h or --help for help.");
    Environment.Exit(0);
}

parsedArgs.Item1.Run(parsedArgs.Item2 ?? Array.Empty<BaseParameter>());
