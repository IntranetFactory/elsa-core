﻿using System;
using System.Threading.Tasks;
using Elsa.Activities.Console;
using Elsa.Builders;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Samples.HelloWorldConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection() 
                .AddElsa()
                .AddConsoleActivities()
                .AddSingleton(Console.In)
                .BuildServiceProvider();
            
            // Build a new workflow.
            var workflow = services.GetService<IWorkflowBuilder>()
                .WriteLine("Hello World!")
                .WriteLine("What's your name?")
                .ReadLine()
                .WriteLine(context => $"Greetings, {context.Input.Value}!")
                .Build();
            
            // Get the workflow host.
            var workflowHost = services.GetService<IWorkflowHost>();

            // Execute the workflow.
            await workflowHost.RunWorkflowAsync(workflow);
        }
    }
}