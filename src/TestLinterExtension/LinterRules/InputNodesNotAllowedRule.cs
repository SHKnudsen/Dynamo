using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Interfaces;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.TestLinterExtension.LinterRules
{
    public class InputNodesNotAllowedRule : NodeLinterRule
    {
        public override string Id => "ebdabd96-4b7a-46bf-930f-6feca33c53b2";
        public override SeverityCodesEnum SeverityCode => SeverityCodesEnum.Error;
        public override string Description => "Nodes are not allowed to be set as input in this graph";
        public override string CallToAction => "Set the above nodes to not be inputs";


        protected override IRuleEvaluationResult EvalualteFunction(NodeModel nodeModel)
        {
            EvaluationRuleResultEnum result = EvaluationRuleResultEnum.Failed;
            if (!nodeModel.IsSetAsInput)
                result = EvaluationRuleResultEnum.Passed;

            return new NodeRuleEvaluationResult(Id, result, nodeModel.GUID.ToString());
        }

        protected override List<IRuleEvaluationResult> InitFunction(WorkspaceModel workspaceModel)
        {
            var results = new List<IRuleEvaluationResult>();
            foreach (var node in workspaceModel.Nodes)
            {
                var result = EvalualteFunction(node);

                if (result.Result == EvaluationRuleResultEnum.Passed)
                    continue;

                results.Add(new NodeRuleEvaluationResult(Id, result.Result, node.GUID.ToString()));
            }
            return results;
        }

        public override void Dispose()
        {
        }
    }
}
