using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Core;
using Dynamo.Wpf.Linting.Rules;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;

namespace Dynamo.Wpf.Linting
{
    public class LinterManager : NotificationObject
    {
        #region Private fields
        private ILinterExtension currentLinter;
        private ILinterRuleSet currentRuleSet;
        private readonly DynamoViewModel dynamoViewModel;
        #endregion

        #region Public properties
        /// <summary>
        /// Results from evaluated rules
        /// </summary>
        public ObservableCollection<IRuleEvaluationResult> RuleEvaluationResults { get; set; }

        /// <summary>
        /// Available linters
        /// </summary>
        public List<ILinterExtension> AvailableLinters { get; internal set; }

        /// <summary>
        /// The linter currently selected
        /// </summary>
        public ILinterExtension CurrentLinter
        {
            get { return currentLinter; }
            set
            {
                currentLinter = value;
                var ruleset = value.RegisterRuleSet();
                ActiveRuleSet = ruleset;
            }
        }

        /// <summary>
        /// The Ruleset currently being used to validate graphs
        /// </summary>
        public ILinterRuleSet ActiveRuleSet
        {
            get { return currentRuleSet; }
            set 
            {
                if (currentRuleSet == value)
                    return;

                if (currentRuleSet != null && currentRuleSet != value)
                    DisposeCurrentLinter(currentRuleSet);

                currentRuleSet = value;
                InitializeCurrentLinter(currentRuleSet);
                RaisePropertyChanged(nameof(ActiveRuleSet));
            }
        }


        public WorkspaceModel CurrentWorkspace { get; private set; }
        #endregion

        public LinterManager(DynamoViewModel dynamoViewModel)
        {
            this.dynamoViewModel = dynamoViewModel;
            AvailableLinters = new List<ILinterExtension>();
            RuleEvaluationResults = new ObservableCollection<IRuleEvaluationResult>();
            dynamoViewModel.PropertyChanged += OnCurrentWorkspaceChanged;
        }



        /// <summary>
        /// Add new linter to the LinterManager
        /// </summary>
        /// <param name="linter"></param>
        public void AddLinter(ILinterExtension linter)
        {
            if (AvailableLinters is null)
                return;

            if (AvailableLinters.Find(x => x.UniqueId == linter.UniqueId) is null)
            {
                AvailableLinters.Add(linter);
                RaisePropertyChanged(nameof(AvailableLinters));
            }
        }

        /// <summary>
        /// Remove linter if it already exists
        /// </summary>
        /// <param name="linter"></param>
        public void RemoveLinter(ILinterExtension linter)
        {
            if (AvailableLinters is null)
                return;

            if (AvailableLinters.Find(x => x.UniqueId == linter.UniqueId) != null)
            {
                AvailableLinters.Remove(linter);
                RaisePropertyChanged(nameof(AvailableLinters));
            }
        }

        #region Private methods
        private void OnCurrentWorkspaceChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DynamoViewModel.CurrentSpace))
            {
                if (this.CurrentWorkspace != null)
                    this.CurrentWorkspace.NodeRemoved -= OnNodeRemoved;

                this.CurrentWorkspace = dynamoViewModel.CurrentSpace;
                this.CurrentWorkspace.NodeRemoved += OnNodeRemoved;
            }
        }

        private void OnNodeRemoved(Graph.Nodes.NodeModel obj)
        {
            var nodeRuleEvaluations = RuleEvaluationResults.
                Where(x => x is NodeRuleEvaluationResult).
                Cast<NodeRuleEvaluationResult>().
                ToList().
                Where(x => x.NodeId == obj.GUID.ToString()).
                ToList();

            foreach (var item in nodeRuleEvaluations)
            {
                RuleEvaluationResults.Remove(item);
            }
        }

        private void DisposeCurrentLinter(ILinterRuleSet currentRuleSet)
        {
            RuleEvaluationResults.Clear();

            if (currentRuleSet.LinterRules is null || currentRuleSet.LinterRules.Count() <= 0)
                return;

            currentRuleSet.LinterRules.ToList().ForEach(x => x.Dispose());
            foreach (var rule in currentRuleSet.LinterRules)
            {
                rule.RuleEvaluated -= OnLinterRuleEvaluated;
            }
        }

        private void InitializeCurrentLinter(ILinterRuleSet currentLinter)
        {
            if (currentLinter.LinterRules is null || currentLinter.LinterRules.Count() <= 0)
                return;

            foreach (var rule in currentLinter.LinterRules)
            {
                rule.RuleEvaluated += OnLinterRuleEvaluated;
                rule.Initialize(dynamoViewModel.CurrentSpace);
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

        #endregion
    }
}