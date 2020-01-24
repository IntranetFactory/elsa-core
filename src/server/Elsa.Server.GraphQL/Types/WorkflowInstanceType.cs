﻿using Elsa.Models;
using GraphQL.NodaTime;
using GraphQL.Types;

namespace Elsa.Server.GraphQL.Types
{
    public class WorkflowInstanceType : ObjectGraphType<WorkflowInstance>
    {
        public WorkflowInstanceType()
        {
            Name = "WorkflowInstance";
            Description = "Represents an instance of a workflow definition.";

            Field(x => x.Id).Description("The ID of the workflow instance.");
            Field(x => x.DefinitionId).Description("The ID of the workflow definition this workflow is an instance of.");
            Field(x => x.Version).Description("The version of the workflow definition this workflow is an instance of.");
            Field(x => x.Status).Description("The status of the workflow.");
            Field(x => x.CorrelationId, true).Description("The correlation ID of the workflow.");
            Field(x => x.BlockingActivities, true, typeof(ListGraphType<BlockingActivityType>)).Description("A hash of activities that are blocking workflow execution.");
            Field(x => x.Variables, type: typeof(VariablesType)).Description("Holds workflow variables in a stack of scopes.");
            Field(x => x.ExecutionLog, true, typeof(ListGraphType<ExecutionLogEntryType>)).Description("A log of executed activities.");
            Field(x => x.Fault, true, typeof(WorkflowFaultType)).Description("If the workflow is in a Faulted state, this property holds details about the fault.");
            Field(x => x.CreatedAt, type: typeof(InstantGraphType)).Description("The time stamp at which this workflow instance was created.");
        }
    }
}