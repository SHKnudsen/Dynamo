namespace Dynamo.Wpf.Linting
{
    public interface IRuleEvaluationResult
    {
        /// <summary>
        /// Id of the rule that create this result
        /// </summary>
        string RuleId { get; }

        EvaluationRuleResultEnum Result { get; }
    }
}