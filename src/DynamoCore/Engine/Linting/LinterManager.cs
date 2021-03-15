using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Core;
using Dynamo.Engine.Linting.Rules;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;

namespace Dynamo.Engine.Linting
{
    public class LinterManager : NotificationObject
    {
        public ObservableCollection<IRuleEvaluationResult> RuleEvaluationResults { get; set; }

        public List<ILinterRuleSet> AvailableLinters { get; internal set; }

        private ILinterRuleSet currentLinter;
        private readonly DynamoModel dynamoModel;

        public ILinterRuleSet CurrentLinter 
        {
            get { return currentLinter; }
            set
            {
                if (currentLinter == value)
                    return;

                if (currentLinter != null && currentLinter != value)
                    DisposeCurrentLinter(currentLinter);
                
                currentLinter = value;
                InitializeCurrentLinter(currentLinter);
                RaisePropertyChanged(nameof(CurrentLinter));
            } 
        }

        public WorkspaceModel CurrentWorkspace { get; private set; }

        public LinterManager(DynamoModel dynamoModel)
        {
            this.dynamoModel = dynamoModel;
            AvailableLinters = new List<ILinterRuleSet>();
            RuleEvaluationResults = new ObservableCollection<IRuleEvaluationResult>();

            dynamoModel.PropertyChanged += OnCurrentWorkspaceChanged;
        }

        private void OnCurrentWorkspaceChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DynamoModel.CurrentWorkspace))
            {
                if (this.CurrentWorkspace != null)
                    this.CurrentWorkspace.NodeRemoved -= OnNodeRemoved;

                this.CurrentWorkspace = dynamoModel.CurrentWorkspace;
                this.CurrentWorkspace.NodeRemoved += OnNodeRemoved;
            }
        }

        private void OnNodeRemoved(Graph.Nodes.NodeModel obj)
        {
            var nodeRuleEvaluations = RuleEvaluationResults.
                Where(x => x is NodeRuleEvaluationResult).
                Cast<NodeRuleEvaluationResult>().
                ToList().
                Where(x=>x.NodeId == obj.GUID.ToString()).
                ToList();

            foreach (var item in nodeRuleEvaluations)
            {
                RuleEvaluationResults.Remove(item);
            }

        }

        public void AddLinter(ILinterRuleSet linter)
        {
            if (AvailableLinters is null)
                return;

            if (AvailableLinters.Find(x => x.Id == linter.Id) is null)
            {
                AvailableLinters.Add(linter);
                RaisePropertyChanged(nameof(AvailableLinters));
            }
        }

        public void RemoveLinter(ILinterRuleSet linter)
        {
            if (AvailableLinters is null)
                return;

            if (AvailableLinters.Find(x => x.Id == linter.Id) != null)
            {
                AvailableLinters.Remove(linter);
                RaisePropertyChanged(nameof(AvailableLinters));
            }
        }

        private void DisposeCurrentLinter(ILinterRuleSet currentLinter)
        {
            RuleEvaluationResults.Clear();

            if (currentLinter.LinterRules is null || currentLinter.LinterRules.Count() <= 0)
                return;

            currentLinter.LinterRules.ToList().ForEach(x => x.Dispose());
            foreach (var rule in currentLinter.LinterRules)
            {
                LinterRule.RuleEvaluated -= OnLinterRuleEvaluated;
            }
        }

        private void InitializeCurrentLinter(ILinterRuleSet currentLinter)
        {
            if (currentLinter.LinterRules is null || currentLinter.LinterRules.Count() <= 0)
                return;

            foreach (var rule in currentLinter.LinterRules)
            {
                LinterRule.RuleEvaluated += OnLinterRuleEvaluated;
                rule.Initialize(dynamoModel.CurrentWorkspace);
            }
        }

        private void OnLinterRuleEvaluated(IRuleEvaluationResult result)
        {
            if (result is null)
                return;

            if (result.Result == EvaluationRuleResultEnum.Passed)
            {
                if (!RuleEvaluationResults.Contains(result))
                    return;
                RuleEvaluationResults.Remove(result);
            }

            else
            {
                if (RuleEvaluationResults.Contains(result))
                    return;
                RuleEvaluationResults.Add(result);
            }
        }
    }
}
