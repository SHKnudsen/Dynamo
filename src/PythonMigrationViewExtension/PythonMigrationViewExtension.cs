using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Logging;
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
        private const string NOTIFICATION_TITLE = "IronPythonNotification";

        private ViewLoadedParams LoadedParams { get; set; }
        private WorkspaceModel CurrentWorkspace { get; set; }
        private DynamoViewModel DynamoViewModel { get; set; }
        private DynamoLogger DynamoLogger { get { return DynamoViewModel.Model.Logger; } }
        private NotificationMessage IronPythonNotification { get; set; }

        /// <summary>
        /// Extension GUID
        /// </summary>
        public string UniqueId { get { return EXTENSION_GUID; } }

        /// <summary>
        /// Extension Name
        /// </summary>
        public string Name { get { return EXTENSION_NAME; } }

        public void Shutdown()
        {
        }

        public void Startup(ViewStartupParams p)
        {
        }

        public void Dispose()
        {
        }

        public void Loaded(ViewLoadedParams p)
        {
            LoadedParams = p;
            DynamoViewModel = LoadedParams.DynamoWindow.DataContext as DynamoViewModel;
            DynamoViewModel.CurrentSpaceViewModel.Model.NodeAdded += Model_NodeAdded;
            DynamoViewModel.Model.PropertyChanged += Model_PropertyChanged;
            DynamoLogger.NotificationLogged += DynamoLogger_NotificationLogged;
            
        }

        private void DynamoLogger_NotificationLogged(NotificationMessage obj)
        {
            if (obj.Title == NOTIFICATION_TITLE)
            {
                IronPythonNotification = obj;
            }
        }

        private void Model_NodeAdded(Graph.Nodes.NodeModel obj)
        {
            if (IronPythonNotification == null && obj.NodeType == "PythonScriptNode" && ((PythonNode)obj).Engine == PythonEngineVersion.IronPython2)
            {
                LogIronPythonNotification();
            }
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentWorkspace")
            {
                IronPythonNotification = null;
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
            {
                LogIronPythonNotification();
                DisplayIronPythonDialog();
            }                  
        }

        private void DisplayIronPythonDialog()
        {
            string summary = Dynamo.Properties.Resources.IronPythonDialogSummary;
            var description = Dynamo.Properties.Resources.IronPythonDialogDescription;

            var dialog = new IronPythonDialog();
            dialog.Title = Dynamo.Properties.Resources.IronPythonDialogTitle;
            dialog.SummaryText.Text = summary;
            dialog.DescriptionText.Text = description;
            dialog.Owner = LoadedParams.DynamoWindow;
            dialog.Show();
        }

        private void LogIronPythonNotification()
        {
            DynamoViewModel.Model.Logger.LogNotification(
                this.GetType().Name,
                NOTIFICATION_TITLE,
                PythonNodeModels.Properties.Resources.IronPythonNotificationShortMessage,
                PythonNodeModels.Properties.Resources.IronPythonNotificationDetailedMessage);
        }
    }
}
