using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Core;
using Dynamo.Engine.Linting;
using Dynamo.Engine.Linting.Rules;
using Dynamo.Graph.Nodes;
using Dynamo.LintingViewExtension.Controls;
using Dynamo.LintingViewExtension.TestLinters;

namespace Dynamo.LintingViewExtension
{
    public class LinterViewModel : NotificationObject
    {
        public LinterManager LinterManager { get; }
        public ObservableCollection<NodeRuleIssueDto> NodeIssues { get; set; }
        public ObservableCollection<GraphRuleIssueDto> GraphIssues { get; set; }

        public LinterViewModel(LinterManager linterManager)
        {
            LinterManager = linterManager ?? throw new ArgumentNullException(nameof(linterManager));
            NodeIssues = new ObservableCollection<NodeRuleIssueDto>();
            GraphIssues = new ObservableCollection<GraphRuleIssueDto>();
            LinterManager.RuleEvaluationResults.CollectionChanged += RuleEvaluationResultsCollectionChanged;
        }

        private void AddNewNodeIssue(NodeRuleEvaluationResult item)
        {
            var dto = NodeIssues.Where(x => x.Id == item.RuleId).FirstOrDefault();
            if (!(dto is null))
            {
                if (dto.Results.Contains(item))
                    return;

                dto.Results.Add(item);
                return;
            }

            var newDto = new NodeRuleIssueDto(
                item.RuleId, GetLinterRule(item.RuleId) as NodeLinterRule);
            newDto.AddResult(item);

            NodeIssues.Add(newDto);
        }

        private void AddNewGraphIssue(GraphRuleEvaluationResult item)
        {
            var dto = GraphIssues.Where(x => x.Id == item.RuleId).FirstOrDefault();
            if (!(dto is null))
            {
                if (dto.Results.Contains(item))
                    return;

                dto.Results.Add(item);
                //RaisePropertyChanged(nameof(NodeIssues));
                return;
            }

            var newDto = new GraphRuleIssueDto(
                item.RuleId, GetLinterRule(item.RuleId) as GraphLinterRule);
            newDto.AddResult(item);

            GraphIssues.Add(newDto);
            //RaisePropertyChanged(nameof(GraphIssues));
        }

        private void RemoveNodeIssue(NodeRuleEvaluationResult item)
        {
            var dto = NodeIssues.Where(x => x.Id == item.RuleId).FirstOrDefault();
            if (dto is null ||
                !dto.Results.Contains(item))
                return;

            dto.Results.Remove(item);

            if (dto.Results.Count == 0)
                NodeIssues.Remove(dto);
        }

        private void RemoveGraphIssue(GraphRuleEvaluationResult item)
        {
            var dto = GraphIssues.Where(x => x.Id == item.RuleId).FirstOrDefault();
            if (dto is null ||
                !dto.Results.Contains(item))
                return;

            dto.Results.Remove(item);

            if (dto.Results.Count == 0)
                GraphIssues.Remove(dto);
        }

        private LinterRule GetLinterRule(string id)
        {
            return LinterManager.CurrentLinter.LinterRules.Where(x => x.Id == id).FirstOrDefault();
        }

        private void RuleEvaluationResultsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (item is NodeRuleEvaluationResult nodeRuleEvaluationResult)
                            AddNewNodeIssue(nodeRuleEvaluationResult);
                        else if (item is GraphRuleEvaluationResult graphRuleEvaluationResult)
                            AddNewGraphIssue(graphRuleEvaluationResult);
                    }
                    return;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        if (item is NodeRuleEvaluationResult nodeRuleEvaluationResult)
                            RemoveNodeIssue(nodeRuleEvaluationResult);
                        else if (item is GraphRuleEvaluationResult graphRuleEvaluationResult)
                            RemoveGraphIssue(graphRuleEvaluationResult);
                    }
                    return;

                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
