using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Wpf.Linting.Rules
{
    /// <summary>
    /// Base class for creating Node related linter rules
    /// </summary>
    public abstract class NodeLinterRule : LinterRule
    {

        /// <summary>
        /// Method to call when this rule needs to be evaluated.
        /// This will use <see cref="EvalualteFunction(NodeModel)"/> to evaluate the rule.
        /// </summary>
        /// <param name="nodeModel"></param>
        public void Evaluate(NodeModel nodeModel)        
        {
            var res = EvalualteFunction(nodeModel);
            OnRuleEvaluated(res);
        }

        /// <summary>
        /// Function used to evaluate this rule
        /// </summary>
        /// <param name="nodeModel">Node to evaluate</param>
        /// <returns></returns>
        protected abstract IRuleEvaluationResult EvalualteFunction(NodeModel nodeModel);

    }
}
