using System;

namespace Dynamo.Engine.Linting
{
    public interface ILinterRule<TEventArgs, TEvaluationResult> where TEventArgs : EventArgs where TEvaluationResult : IRuleEvaluationResult
    {
        string Id { get; }
        string SeverityCode { get; }
        string Description { get; }
        string CallToAction { get; }

        TEvaluationResult Evaluate(TEventArgs e);
    }
}