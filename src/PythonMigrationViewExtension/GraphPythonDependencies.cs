using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Dynamo.Graph.Nodes;
using Dynamo.Wpf.Extensions;
using PythonNodeModels;

namespace Dynamo.PythonMigration
{
    public class GraphPythonDependencies
    {
        private ViewLoadedParams ViewLoaded { get; set; }
        public List<PythonNodeBase> GraphPythonNodes { get; set; }

        internal GraphPythonDependencies(ViewLoadedParams viewLoadedParams)
        {
            this.ViewLoaded = viewLoadedParams;

            GraphPythonNodes = ViewLoaded.CurrentWorkspaceModel.Nodes
                .Where(n => n.GetType().IsSubclassOf(typeof(PythonNodeBase)))
                .Select(n => n as PythonNodeBase)
                .ToList();
        }

        internal bool ContainsPythonDependencies()
        {
            return GraphPythonNodes.Any();
        }

        internal bool ContainsIronPythonDependencies()
        {
            var workspace = ViewLoaded.CurrentWorkspaceModel;
            if (workspace == null)
                throw new ArgumentNullException(nameof(workspace));

            return GraphPythonNodes.Any(n => IsIronPythonNode(n));
        }

        internal bool ContainsCPythonDependencies()
        {
            var workspace = ViewLoaded.CurrentWorkspaceModel;
            if (workspace == null)
                throw new ArgumentNullException(nameof(workspace));

            return GraphPythonNodes.Any(n => IsCPythonNode(n));
        }

        internal bool IsIronPythonNode(NodeModel obj)
        {
            if (!(obj is PythonNodeBase pythonNode))
                return false;

            if (!GraphPythonNodes.Contains(pythonNode))
                GraphPythonNodes.Add(pythonNode);

            return pythonNode.Engine == PythonEngineVersion.IronPython2;
        }

        internal bool IsCPythonNode(NodeModel obj)
        {
            if (!(obj is PythonNodeBase pythonNode))
                return false;

            if (!GraphPythonNodes.Contains(pythonNode))
                GraphPythonNodes.Add(pythonNode);

            return pythonNode.Engine == PythonEngineVersion.CPython3;
        }

        internal void RemovePythonNode(NodeModel obj)
        {
            if (!(obj is PythonNodeBase pythonNode))
                return;

            if (!GraphPythonNodes.Contains(pythonNode))
                return;

            GraphPythonNodes.Remove(pythonNode);
        }
    }
}
