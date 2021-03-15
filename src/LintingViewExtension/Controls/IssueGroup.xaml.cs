using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dynamo.Engine.Linting;

namespace Dynamo.LintingViewExtension.Controls
{
    /// <summary>
    /// Interaction logic for IssueGroup.xaml
    /// </summary>
    public partial class IssueGroup : UserControl
    {
        #region DependencyProperties

        public IEnumerable<NodeRuleEvaluationResult> Results
        {
            get { return (IEnumerable<NodeRuleEvaluationResult>)GetValue(ResultsProperty); }
            set { SetValue(ResultsProperty, value); }
        }

        public static readonly DependencyProperty ResultsProperty = DependencyProperty.Register(
            nameof(Results),
            typeof(IEnumerable<NodeRuleEvaluationResult>),
            typeof(IssueGroup)
        );

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(IssueGroup)
        );

        public string CallToAction
        {
            get { return (string)GetValue(CallToActionkProperty); }
            set { SetValue(CallToActionkProperty, value); }
        }

        public static readonly DependencyProperty CallToActionkProperty = DependencyProperty.Register(
            nameof(CallToAction),
            typeof(string),
            typeof(IssueGroup)
        );

        #endregion DependencyProperties

        public IssueGroup()
        {
            InitializeComponent();
        }
    }
}
