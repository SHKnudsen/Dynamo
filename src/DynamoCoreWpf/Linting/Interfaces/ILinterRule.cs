using System;
using System.Collections.Generic;
using Dynamo.Wpf.Linting.Interfaces;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Wpf.Linting
{
    /// <summary>
    /// Represents the method that will handle rule evaluated related events.
    /// </summary>
    /// <param name="workspace"></param>
    /// <param name="saveContext"></param>
    public delegate void RuleEvaluatedHandler(IRuleEvaluationResult result);

    public interface ILinterRule
    {
        /// <summary>
        /// Unique id of the rule
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Rules severity code -> Error, Warning
        /// </summary>
        SeverityCodesEnum SeverityCode { get; }

        /// <summary>
        /// Description of what the rule is checking
        /// </summary>
        string Description { get; }

        /// <summary>
        /// How to fix this rule
        /// </summary>
        string CallToAction { get; }

        /// <summary>
        /// This event should be called whenever the rule is evaluated
        /// </summary>
        event RuleEvaluatedHandler RuleEvaluated;
    }
}