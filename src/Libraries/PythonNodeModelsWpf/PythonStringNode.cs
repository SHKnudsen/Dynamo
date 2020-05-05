using Dynamo.Controls;
using Dynamo.Graph.Workspaces;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using PythonNodeModels;
using PythonNodeModelsWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PythonNodeModelsWpf
{
    class PythonStringNodeViewCustomization : VariableInputNodeViewCustomization, INodeViewCustomization<PythonStringNode>
    {
        private PythonStringNode pythonStringNodeModel;
        private NodeView pythonStringNodeView;

        public void CustomizeView(PythonStringNode nodeModel, NodeView nodeView)
        {
            base.CustomizeView(nodeModel, nodeView);

            pythonStringNodeModel = nodeModel;
            pythonStringNodeView = nodeView;

            nodeView.PresentationGrid.Visibility = Visibility.Visible;
            nodeView.PresentationGrid.DataContext = this.pythonStringNodeModel;
            nodeView.PresentationGrid.Children.Add(new EngineLabel());
        }
    }
}
