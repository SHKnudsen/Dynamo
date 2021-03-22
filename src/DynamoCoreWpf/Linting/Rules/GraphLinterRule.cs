using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Wpf.Linting.Interfaces;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Wpf.Linting.Rules
{
    /// <summary>
    /// Base class for creating Graph related linter rules
    /// </summary>
    public abstract class GraphLinterRule : LinterRule
    {
        /// <summary>
        /// Method to call when this rule needs to be evaluated.
        /// This will use <see cref="EvalualteFunction(WorkspaceModel)"/> to evaluate the rule.       
        /// </summary>
        /// <param name="workspaceModel"></param>
        public void Evaluate(WorkspaceModel workspaceModel)
        {
            var res = EvalualteFunction(workspaceModel);
            OnRuleEvaluated(res);
        }

        /// <summary>
        /// Function used to evaluate this rule
        /// </summary>
        /// <param name="workspaceModel"></param>
        /// <returns></returns>
        protected abstract IRuleEvaluationResult EvalualteFunction(WorkspaceModel workspaceModel);
    }
}
