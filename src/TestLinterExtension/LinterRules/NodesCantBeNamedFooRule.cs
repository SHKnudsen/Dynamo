using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Interfaces;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.TestLinterExtension.LinterRules
{
    public class NodesCantBeNamedFooRule : NodeLinterRule
    {
        public override string Id => "01963dcb-4cbe-41f8-adb0-cc101c5dc2e5";

        public override SeverityCodesEnum SeverityCode => SeverityCodesEnum.Error;

        public override string Description => "Nodes are not allowed to be named 'Foo'";

        public override string CallToAction => "Rename the nodes listed above to something else than 'Foo'";

        protected override IRuleEvaluationResult EvalualteFunction(NodeModel nodeModel)
        {
            EvaluationRuleResultEnum result = EvaluationRuleResultEnum.Failed;
            if (nodeModel.Name != "Foo")
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
    }
}
