using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Wpf.Linting.Rules
{
    /// <summary>
    /// Rule evaluation result for graph linter rules
    /// </summary>
    public class GraphRuleEvaluationResult : IEquatable<GraphRuleEvaluationResult>, IRuleEvaluationResult
    {
        /// <summary>
        /// Id of the rule this evaluation result belongs to
        /// </summary>
        public string RuleId { get; }

        /// <summary>
        /// Evaluation result
        /// </summary>
        public EvaluationRuleResultEnum Result { get; }

        public GraphRuleEvaluationResult(string ruleId, EvaluationRuleResultEnum result)
        {
            RuleId = ruleId ?? throw new ArgumentNullException(nameof(ruleId));
            Result = result;
        }

        public bool Equals(GraphRuleEvaluationResult other)
        {
            if (other is null)
                return false;

            return this.RuleId == other.RuleId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return Equals(obj as GraphRuleEvaluationResult);
        }
    }
}
