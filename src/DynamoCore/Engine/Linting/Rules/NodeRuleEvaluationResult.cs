using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Engine.Linting
{
    /// <summary>
    /// Rule evaluation result for node linter rules
    /// </summary>
    public class NodeRuleEvaluationResult : IEquatable<NodeRuleEvaluationResult>, IRuleEvaluationResult
    {
        /// <summary>
        /// Id of the rule this evaluation result belongs to
        /// </summary>
        public string RuleId { get; }

        /// <summary>
        /// Evaluation result
        /// </summary>
        public EvaluationRuleResultEnum Result { get; }

        /// <summary>
        /// Unique id of the node that has been evaluated
        /// </summary>
        public string NodeId { get; }

        public NodeRuleEvaluationResult(string ruleId, EvaluationRuleResultEnum result, string nodeId)
        {
            RuleId = ruleId;
            Result = result;
            NodeId = nodeId;
        }

        public bool Equals(NodeRuleEvaluationResult other)
        {
            if (other is null)
                return false;

            return this.NodeId == other.NodeId && this.RuleId == other.RuleId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return Equals(obj as NodeRuleEvaluationResult);
        }
    }
}
