using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.ViewModels;
using DSCore;
using CoreNodeModels;
using Dynamo.UI.Commands;

namespace Dynamo.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class WatchHoverIconViewModel: NotificationObject
    {
        private ConnectorViewModel ViewModel { get; set; }
        private DynamoViewModel DynamoViewModel { get; set; }
        private Dispatcher Dispatcher { get; set; }

        public Point MidPoint { get; private set; }
        public double MarkerSize { get; private set; } = 30;

        public DelegateCommand PlaceWatchNodeCommand { get; set; }

        private void PlaceWatchNodeCommandExecute(object param)
        {
            PlaceWatchNode();
        }

        bool CanExecute(object parameter)
        {
            return true;
        }

        void InitCommands()
        {
            PlaceWatchNodeCommand = new DelegateCommand(PlaceWatchNodeCommandExecute,CanExecute);
        }

        public WatchHoverIconViewModel(ConnectorViewModel connectorViewModel, DynamoViewModel dynamoViewModel)
        {
            ViewModel = connectorViewModel;
            DynamoViewModel = dynamoViewModel;
            InitCommands();

            Dispatcher = Dispatcher.CurrentDispatcher;
            MidPoint = ConnectorBezierMidpoint();
            connectorViewModel.PropertyChanged += OnConnectorViewModelPropertyChanged;
           
        }

        private void OnConnectorViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Contains("CurvePoint")) { return; }
            MidPoint = ConnectorBezierMidpoint();
            RaisePropertyChanged(nameof(MidPoint));
        }

        private Point ConnectorBezierMidpoint()
        {
            // formula to get bezier curve midtpoint
            // https://stackoverflow.com/questions/5634460/quadratic-b%c3%a9zier-curve-calculate-points?rq=1
            var parameter = 0.5;
            var x = ((1 - parameter) * (1 - parameter) * (1 - parameter) *
                ViewModel.CurvePoint0.X + 3 * (1 - parameter) * (1 - parameter)
                * parameter *
                ViewModel.CurvePoint1.X + 3 * (1 - parameter)
                * parameter * parameter *
                ViewModel.CurvePoint2.X + parameter * parameter * parameter *
                ViewModel.CurvePoint3.X) - (MarkerSize / 2);

            var y = ((1 - parameter) * (1 - parameter) * (1 - parameter) *
                ViewModel.CurvePoint0.Y + 3 * (1 - parameter) * (1 - parameter)
                * parameter *
                ViewModel.CurvePoint1.Y + 3 * (1 - parameter)
                * parameter * parameter *
                ViewModel.CurvePoint2.Y + parameter * parameter * parameter *
                ViewModel.CurvePoint3.Y) - (MarkerSize / 2);

            return new Point(x, y);
        }

        internal void PlaceWatchNode()
        {
            NodeModel startNode = ViewModel.ConnectorModel.Start.Owner;
            NodeModel endNode = ViewModel.ConnectorModel.End.Owner;
            var dynamoModel = DynamoViewModel.Model;
            this.Dispatcher.Invoke(() =>
            {
                var watchNode = new Watch();
                var nodeX = MidPoint.X - (watchNode.Width / 2);
                var nodeY = MidPoint.Y - (watchNode.Height / 2);
                dynamoModel.ExecuteCommand(new DynamoModel.CreateNodeCommand(watchNode, nodeX, nodeY, false, false));
                WireNewNode(dynamoModel, startNode, endNode, watchNode);
            });
        }

        private static void WireNewNode(DynamoModel dynamoModel, NodeModel startNode, NodeModel endNode, NodeModel watchNodeModel)
        {
            (List<int> startIndex, List<int> endIndex) = GetPortIndex(startNode, endNode);

            // Connect startNode and remember node
            foreach (var idx in startIndex)
            {
                dynamoModel.ExecuteCommand(new DynamoModel.MakeConnectionCommand(startNode.GUID, idx, PortType.Output, DynamoModel.MakeConnectionCommand.Mode.Begin));
                dynamoModel.ExecuteCommand(new DynamoModel.MakeConnectionCommand(watchNodeModel.GUID, 0, PortType.Input, DynamoModel.MakeConnectionCommand.Mode.End));
            }

            // Connect remember node and endNode
            foreach (var idx in endIndex)
            {
                dynamoModel.ExecuteCommand(new DynamoModel.MakeConnectionCommand(watchNodeModel.GUID, 0, PortType.Output, DynamoModel.MakeConnectionCommand.Mode.Begin));
                dynamoModel.ExecuteCommand(new DynamoModel.MakeConnectionCommand(endNode.GUID, idx, PortType.Input, DynamoModel.MakeConnectionCommand.Mode.End));
            }
        }

        private static (List<int> StartIndex, List<int> EndIndex) GetPortIndex(NodeModel startNode, NodeModel endNode)
        {
            var connectors = startNode.AllConnectors;
            var filter = connectors.Where(c => c.End.Owner.GUID == endNode.GUID);

            var startIndex = filter
                .Select(c => c.Start.Index)
                .Distinct()
                .ToList();

            var endIndex = filter
                .Select(c => c.End.Index)
                .Distinct()
                .ToList();

            return (startIndex, endIndex);
        }
    }
}
