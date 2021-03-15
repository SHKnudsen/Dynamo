using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Engine.Linting.Interfaces;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Engine.Linting.Rules
{
    /// <summary>
    /// Base class for all linting rules
    /// </summary>
    public abstract class LinterRule
    {
        /// <summary>
        /// Unique id of this rule
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Severity code of this rule.
        /// This code will define how the rule is displayed in the UI
        /// </summary>
        public abstract SeverityCodesEnum SeverityCode { get; }

        /// <summary>
        /// Description of the rule
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Description of how to solve issues related to this rule
        /// </summary>
        public abstract string CallToAction { get; }

        /// <summary>
        /// The init function is used when the Linter extension implementing this Rule is initialized.
        /// </summary>
        /// <param name="workspaceModel"></param>
        /// <returns></returns>
        protected abstract List<IRuleEvaluationResult> InitFunction(WorkspaceModel workspaceModel);

        /// <summary>
        /// Initializes this rule using the <see cref="InitFunction(WorkspaceModel)"/>
        /// </summary>
        /// <param name="workspaceModel"></param>
        /// <returns></returns>
        internal void Initialize(WorkspaceModel workspaceModel)
        {
            var initResults = InitFunction(workspaceModel).
                Where(r => r.Result == EvaluationRuleResultEnum.Failed);

            foreach (var result in initResults)
            {
                OnRuleEvaluated(result);
            }
        }

        internal static event RuleEvaluatedHandler RuleEvaluated;

        public static void OnRuleEvaluated(IRuleEvaluationResult result)
        {
            RuleEvaluated?.Invoke(result);
        }

        public virtual void Dispose() { }
    }


}
