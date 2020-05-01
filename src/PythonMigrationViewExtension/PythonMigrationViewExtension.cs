using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.UI.Prompts;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using PythonNodeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynamo.PythonMigration
{
    public class PythonMigrationViewExtension : IViewExtension
    {
        private const string EXTENSION_NAME = "Workspace References";
        private const string EXTENSION_GUID = "1f8146d0-58b1-4b3c-82b7-34a3fab5ac5d";

        private ViewLoadedParams LoadedParams { get; set; }
        private WorkspaceModel CurrentWorkspace { get; set; }

        /// <summary>
        /// Extension GUID
        /// </summary>
        public string UniqueId { get { return EXTENSION_GUID; } }

        /// <summary>
        /// Extension Name
        /// </summary>
        public string Name { get { return EXTENSION_NAME; } }

        public void Dispose()
        {
        }

        public void Loaded(ViewLoadedParams p)
        {
            LoadedParams = p;
            var viewModel = LoadedParams.DynamoWindow.DataContext as DynamoViewModel;
            viewModel.Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentWorkspace")
            {
                var dynamoModel = sender as DynamoModel;
                CurrentWorkspace = dynamoModel.CurrentWorkspace;
                CheckForIronPythonDependencies(CurrentWorkspace);
            }
        }

        private void CheckForIronPythonDependencies(WorkspaceModel workspace)
        {
            if (workspace == null)
                return;

            var workspacePythonNodes = workspace.Nodes
                .Where(n => n.NodeType == "PythonScriptNode")
                .Select(n => n as PythonNode);

            if (workspacePythonNodes == null)
                return;

            if (workspacePythonNodes.Any(n => n.Engine == PythonEngineVersion.IronPython2))
                DisplayIronPythonWarning();
                    
        }

        private void DisplayIronPythonWarning()
        {
            string summary = Dynamo.Properties.Resources.IronPythonDialogSummary;
            var description = Dynamo.Properties.Resources.IronPythonDialogDescription;

            var dialog = new IronPythonDialog();
            dialog.SummaryText.Text = summary;
            dialog.DescriptionText.Text = description;
            dialog.Show();
        }

        public void Shutdown()
        {
        }

        public void Startup(ViewStartupParams p)
        {
        }
    }
}
