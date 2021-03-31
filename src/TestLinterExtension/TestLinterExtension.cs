using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Dynamo.TestLinterExtension.LinterRules;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.TestLinterExtension
{
    public class TestLinterExtension : IViewExtension, ILinterExtension
    {
        private NodesCantBeNamedFooRule fooRule;
        private InputNodesNotAllowedRule inputNodesRule;
        private GraphNeedsOutputNodesRule outputsInGraphRule;

        private ViewLoadedParams viewLoadedParams;
        private readonly List<NodeModel> outputNodes = new List<NodeModel>();

        public string UniqueId => "a7ad5249-10ea-4fbf-b2f6-7f9658773850";

        public string Name => "Test Linter ViewExtension";

        public void Loaded(ViewLoadedParams viewLoadedParams)
        {
            this.viewLoadedParams = viewLoadedParams;
            var linterManager = (viewLoadedParams.DynamoWindow.DataContext as DynamoViewModel).LinterManager;
            this.viewLoadedParams.CurrentWorkspaceChanged += OnCurrentWorkspaceChanged;
        }


        private void OnCurrentWorkspaceChanged(Dynamo.Graph.Workspaces.IWorkspaceModel obj)
        {
            foreach (var node in obj.Nodes)
            {
                OnNodeAdded(node);
            }
  
            obj.NodeAdded += OnNodeAdded;
            obj.NodeRemoved += Obj_NodeRemoved;
        }

        private void Obj_NodeRemoved(NodeModel obj)
        {
            if (outputsInGraphRule is null)
                return;

            if (outputNodes.Contains(obj))
                outputNodes.Remove(obj);

            if (outputNodes.Count == 0)
            {
                var result = new GraphRuleEvaluationResult(outputsInGraphRule.Id, EvaluationRuleResultEnum.Failed);
                outputsInGraphRule.OnRuleEvaluated(result);
            }
        }

        private void OnNodeAdded(Dynamo.Graph.Nodes.NodeModel obj)
        {
            if(obj.Name == "Foo")
                EvaluateFooRule(obj);
            if(obj.IsSetAsInput)
                EvaluateInputRule(obj);
            if (obj.IsSetAsOutput)
                outputNodes.Add(obj);
            obj.PropertyChanged += OnNodePropertyChanged;
        }

        private void OnNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var node = sender as NodeModel;
            switch (e.PropertyName)
            {
                case nameof(NodeModel.Name):
                    EvaluateFooRule(node);
                    return;
                case nameof(NodeModel.IsSetAsInput):
                    EvaluateInputRule(node);
                    return;
                case nameof(NodeModel.IsSetAsOutput):
                    EvaluateOutputNodesInGraphRule(node);
                    return;
                default:
                    break;
            }
        }

        private void EvaluateInputRule(NodeModel node)
        {
            if (inputNodesRule is null)
                return;

            inputNodesRule.Evaluate(node);
        }

        private void EvaluateFooRule(NodeModel node)
        {
            if (fooRule is null)
                return;

            fooRule.Evaluate(node);
        }

        private void EvaluateOutputNodesInGraphRule(NodeModel node)
        {
            if (node.IsSetAsOutput)
            {
                if (outputNodes.Contains(node))
                    return;

                if (outputNodes.Count == 0)
                {
                    var result = new GraphRuleEvaluationResult(outputsInGraphRule.Id, EvaluationRuleResultEnum.Passed);
                    outputsInGraphRule.OnRuleEvaluated(result);
                }
                outputNodes.Add(node);
                return;
            }

            if (outputNodes.Contains(node))
            {
                outputNodes.Remove(node);
                if (outputNodes.Count == 0)
                {
                    var result = new GraphRuleEvaluationResult(outputsInGraphRule.Id, EvaluationRuleResultEnum.Failed);
                    outputsInGraphRule.OnRuleEvaluated(result);
                }
            }
        }

        public ILinterRuleSet RegisterRuleSet()
        {
            fooRule = new NodesCantBeNamedFooRule();
            inputNodesRule = new InputNodesNotAllowedRule();
            outputsInGraphRule = new GraphNeedsOutputNodesRule();

            var ruleset = new TestLinterRuleSet();
            ruleset.LinterRules.Add(fooRule);
            ruleset.LinterRules.Add(inputNodesRule);
            ruleset.LinterRules.Add(outputsInGraphRule);

            return ruleset;
        }

        public void Dispose() { }
        public void Shutdown() { }

        public void Startup(ViewStartupParams viewStartupParams) { }
    }
}
