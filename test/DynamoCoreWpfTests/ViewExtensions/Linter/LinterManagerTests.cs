using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Wpf.Extensions;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;
using DynamoCoreWpfTests;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DynamoCoreWpfTests.Linter
{
    [TestFixture]
    class LinterManagerTests : DynamoTestUIBase
    {
        const string MOCK_GUID = "358321af-2633-4697-b475-81632582eba0";
        const string MOCK_RULE_ID = "1";

        Mock<IViewExtension> mockExtension = new Mock<IViewExtension>();
        Mock<ILinterRuleSet> mockRuleset = new Mock<ILinterRuleSet>();
        Mock<NodeLinterRule> mockRule = new Mock<NodeLinterRule> { CallBase = true };
        Mock<IRuleEvaluationResult> mockResult = new Mock<IRuleEvaluationResult>();

        ViewExtensionManager ViewExtensionManager { get => View.viewExtensionManager; }
        LinterManager LinterManager { get => this.ViewModel.LinterManager; }

        [SetUp]
        public void Setup()
        {
            // Setup mock result
            mockResult.Setup(r => r.RuleId).Returns(MOCK_RULE_ID);
            mockResult.Setup(r => r.Result).Returns(EvaluationRuleResultEnum.Failed);

            // Setup mock rule
            mockRule.Setup(r => r.Id).Returns(MOCK_RULE_ID);
            mockRule.Protected().Setup<List<IRuleEvaluationResult>>("InitFunction", ItExpr.IsAny<WorkspaceModel>()).
                Returns(new List<IRuleEvaluationResult> { mockResult.Object });


            // Setup mock ruleset
            mockRuleset.Setup(r => r.LinterRules).Returns(new List<LinterRule> { mockRule.Object });

            // Setup mock LinterExtension
            mockExtension.Setup(e => e.UniqueId).Returns(MOCK_GUID);
            mockExtension.As<ILinterExtension>();
            mockExtension.As<ILinterExtension>().Setup(m => m.RegisterRuleSet()).Returns(mockRuleset.Object);
        }

        [Test]
        public void LinterExtensionsGetsAddedToManager()
        {
            // Arrange
            var availableLintersBeforeAdding = LinterManager.AvailableLinters.ToList(); 

            // Act
            ViewExtensionManager.Add(mockExtension.Object);
            var availableLintersAfterAdding = this.ViewModel.LinterManager.AvailableLinters;

            // Assert
            Assert.That(availableLintersAfterAdding.Count() == availableLintersBeforeAdding.Count() + 1);
            Assert.That(availableLintersAfterAdding.Contains(mockExtension.As<ILinterExtension>().Object));
        }


        [Test]
        public void VerifyThatChangingCurrentLinterInitializesLinterRules()
        {
            // Arrange
            var startCurrentLinter = LinterManager.ActiveRuleSet;
            var ruleEvaluationResultsBefore = LinterManager.RuleEvaluationResults.Count();

            // Act
            var newCurrentLinter = LinterManager.ActiveRuleSet = mockRuleset.Object;
            var ruleEvaluationResultsAfter = LinterManager.RuleEvaluationResults.Count();

            // Assert
            Assert.That(startCurrentLinter != newCurrentLinter);
            Assert.That(ruleEvaluationResultsAfter > ruleEvaluationResultsBefore);
            Assert.That(LinterManager.RuleEvaluationResults.Contains(mockResult.Object));
            mockRule.Protected().Verify("InitFunction", Times.Once(), ItExpr.IsAny<WorkspaceModel>());
        }


        [Test]
        public void VerifyThatPreviouslyFailedRulesAreRemovedWhenReEvaluationPasses()
        {
            // Arrange
            LinterManager.ActiveRuleSet = mockRuleset.Object;
            var ruleEvaluationResultsBefore = LinterManager.RuleEvaluationResults.Count();

            mockResult.SetupGet(r => r.Result).Returns(EvaluationRuleResultEnum.Passed);

            // Act
            mockRule.Protected().
                Setup<IRuleEvaluationResult>("EvalualteFunction", ItExpr.IsAny<NodeModel>()).
                Returns(mockResult.Object);

            mockRule.Object.Evaluate(It.IsAny<NodeModel>());
            var ruleEvaluationResultsAfter = LinterManager.RuleEvaluationResults.Count();

            // Assert
            Assert.IsTrue(ruleEvaluationResultsBefore > ruleEvaluationResultsAfter);
        }


        [Test]
        public void VerifyThatDeletingNodesAssociatedWithRuleFailureAlsoDeletesEvaluationResult()
        {
            // Arrange
            Open(@"core\Home.dyn");
            var failureNode = new DummyNode();

            this.ViewModel.UIDispatcher.Invoke(new Action(() =>
            {
                this.Model.ExecuteCommand(new DynamoModel.CreateNodeCommand(failureNode, 0, 0, false, false));
            }));

            var newEvaluationResult = new NodeRuleEvaluationResult(MOCK_RULE_ID, EvaluationRuleResultEnum.Failed, failureNode.GUID.ToString());

            LinterManager.ActiveRuleSet = mockRuleset.Object;
            var ruleEvaluationResultsBefore = LinterManager.RuleEvaluationResults.Count();

            // Act
            mockRule.Protected().
                Setup<IRuleEvaluationResult>("EvalualteFunction", ItExpr.IsAny<NodeModel>()).
                Returns(newEvaluationResult);

            mockRule.Object.Evaluate(It.IsAny<NodeModel>());
            var ruleEvaluationResultsAfterEvaluation = LinterManager.RuleEvaluationResults.ToList();


            DynamoModel.DeleteModelCommand delCommand =
                new DynamoModel.DeleteModelCommand(failureNode.GUID);

            this.ViewModel.ExecuteCommand(delCommand);


            var ruleEvaluationResultsAfterNodeDeletion = LinterManager.RuleEvaluationResults;

            // Assert
            Assert.IsTrue(ruleEvaluationResultsBefore < ruleEvaluationResultsAfterEvaluation.Count());
            Assert.IsTrue(ruleEvaluationResultsAfterEvaluation.Count() > ruleEvaluationResultsAfterNodeDeletion.Count());
            Assert.That(ruleEvaluationResultsAfterEvaluation.Contains(newEvaluationResult));
            Assert.IsTrue(ruleEvaluationResultsAfterNodeDeletion.Count == ruleEvaluationResultsBefore);
            Assert.IsFalse(ruleEvaluationResultsAfterNodeDeletion.Contains(newEvaluationResult));
        }


        [Test]
        public void VerifyThatOnlyRulesFromActiveRuleSetGetsEvaluated()
        {
            // Arrange

            // Act

            // Assert
        }

        // Looks like there is currently no implementation for removing a ViewExtension?
        //[Test]
        //public void RemovedLinterExtensionsGetsRemovedFromManager()
        //{
        //    // Arrange

        //    // Act
        //    ViewExtensionManager.Add(mockExtension.Object);
        //    var availableLintersAfterAdding = LinterManager.AvailableLinters.Count();
        //    Assert.That(LinterManager.AvailableLinters.Contains(mockRuleset.Object));

        //    ViewExtensionManager.Remove(mockExtension.Object);

        //    // Assert
        //    Assert.That(LinterManager.AvailableLinters.Count() == availableLintersAfterAdding - 1);
        //    Assert.IsFalse(LinterManager.AvailableLinters.Contains(mockRuleset.Object));
        //}

    }
}
