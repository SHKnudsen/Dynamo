using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Workspaces;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Interfaces;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.TestLinterExtension.LinterRules
{
    public class GraphNeedsOutputNodesRule : GraphLinterRule
    {
        public override string Id => "a9ecd7fc-ac33-4a2d-b1c5-cf225e740d47";

        public override SeverityCodesEnum SeverityCode => SeverityCodesEnum.Error;

        public override string Description => "There are currently no nodes in the graph set as output";

        public override string CallToAction => "Set an appropriate node as 'IsOutput'";

        protected override IRuleEvaluationResult EvalualteFunction(WorkspaceModel workspaceModel)
        {
           
            EvaluationRuleResultEnum result = EvaluationRuleResultEnum.Failed;
            if (workspaceModel.Nodes.Any(x => x.IsSetAsOutput))
                result = EvaluationRuleResultEnum.Passed;

            return new GraphRuleEvaluationResult(Id, result);
        }

        protected override List<IRuleEvaluationResult> InitFunction(WorkspaceModel workspaceModel)
        {
            return new List<IRuleEvaluationResult> { EvalualteFunction(workspaceModel) };
        }
    }
}
