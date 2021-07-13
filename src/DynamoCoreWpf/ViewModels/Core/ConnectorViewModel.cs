using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Dynamo.UI.Commands;
using Newtonsoft.Json;

using Point = System.Windows.Point;
using Dynamo.Selection;
using Dynamo.Engine;
using System.ComponentModel;
using Dynamo.ViewModels;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Dynamo.Graph;
using Dynamo.Nodes;
using HelixToolkit.Wpf.SharpDX;

namespace Dynamo.ViewModels
{
    public enum PreviewState{Selection, ExecutionPreview, None}

    public partial class ConnectorViewModel:ViewModelBase
    {

        #region Properties

        public ObservableCollection<ConnectorPinViewModel> ConnectorPinViewCollection { get; set; }

        public List<Point[]> BezierControlPoints { get; set; }

        private double _panelX;
        private double _panelY;

        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                _panelX = value;
                RaisePropertyChanged(nameof(PanelX));
            }
        }

        public double PanelY
        {
            get { return _panelY; }
            set
            {
                if (value.Equals(_panelY)) return;
                _panelY = value;
                RaisePropertyChanged(nameof(PanelY));
            }
        }

        private Point _mousePosition;
        Point MousePosition
        {
            get
            {
                return _mousePosition;
            }
            set
            {
                _mousePosition = value;
                RaisePropertyChanged(nameof(MousePosition));
            }
        }

        /// <summary>
        /// Required timer for 'watch placement' button desirable behaviour.
        /// </summary>
        private System.Windows.Threading.DispatcherTimer timer;

        private WatchHoverIconViewModel watchHoverViewModel;
        public WatchHoverIconViewModel WatchHoverViewModel
        {
            get { return watchHoverViewModel; }
            private set { watchHoverViewModel = value; RaisePropertyChanged(nameof(WatchHoverViewModel)); }
        }

        private readonly WorkspaceViewModel workspaceViewModel;
        private PortModel _activeStartPort;
        public PortModel ActiveStartPort { get { return _activeStartPort; } internal set { _activeStartPort = value; } }

        private ConnectorModel _model;

        public ConnectorModel ConnectorModel
        {
            get { return _model; }
        }

        private bool _isConnecting = false;
        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                _isConnecting = value;
                RaisePropertyChanged("IsConnecting");
            }
        }

        /// <summary>
        /// Controls wire visibility: on/off. When wire is off, additional styling xaml turns off tooltips.
        /// </summary>
        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                SetVisibilityOfPins(_isVisible);
                RaisePropertyChanged(nameof(IsVisible));
            }
        }

        private void SetVisibilityOfPins(bool visibility)
        {
            if (ConnectorPinViewCollection is null) return;

            foreach (var pin in ConnectorPinViewCollection)
            {
                pin.IsHalftone = !visibility;
            }
        }

        /// <summary>
        /// Property which overrides 'isVisible==false' condition. When this prop is set to true, wires are set to 
        /// 40% opacity.
        /// </summary>
        private bool _isPartlyVisible = false;
        public bool IsPartlyVisible
        {
            get { return _isPartlyVisible; }
            set
            {
                _isPartlyVisible = value;
                RaisePropertyChanged(nameof(IsPartlyVisible));
            }
        }

        /// <summary>
        /// Contains up-to-date tooltip corresponding to wire you are hovering over.
        /// </summary>
        private string connectorDataToolTip;
        public string ConnectorDataTooltip 
        {
            get
            {
                return connectorDataToolTip;
            }
            set
            {
                connectorDataToolTip = value;
                RaisePropertyChanged(nameof(ConnectorDataTooltip));
            }
        }

        /// <summary>
        /// Property to determine whether the data corresponding to this wire hold a collection or a single value.
        /// </summary>
        private bool isHoveredACollection;
        public bool IsHoveredACollection
        {
            get
            {
                return isHoveredACollection;
            }
            set
            {
                isHoveredACollection = value;
                RaisePropertyChanged(nameof(IsHoveredACollection));
            }
        }

        private bool _mouseHoverOn;
        public bool MouseHoverOn
        {
            get
            {
                return _mouseHoverOn;
            }
            set
            {
                _mouseHoverOn = value;
                RaisePropertyChanged(nameof(MouseHoverOn));
            }
        }

        public double Left
        {
            get { return 0; }
        }

        public double Top
        {
            get { return 0; }
        }

        //Changed the connectors ZIndex to 2. Groups have ZIndex of 1.
        public double ZIndex
        {
            get { return 2; }
        }

        /// <summary>
        ///     The start point of the path pulled from the port's center
        /// </summary>
        public Point CurvePoint0
        {
            get
            {
                if (_model == null)
                    return _activeStartPort.Center.AsWindowsType();
                else if (_model.Start != null)
                    return _model.Start.Center.AsWindowsType();
                else
                    return new Point();
            }
        }

        private Point _curvePoint1;
        public Point CurvePoint1
        {
            get
            {
                return _curvePoint1;
            }
            set
            {
                _curvePoint1 = value;
                RaisePropertyChanged("CurvePoint1");
            }
        }

        private Point _curvePoint2;
        public Point CurvePoint2
        {
            get { return _curvePoint2; }
            set
            {
                _curvePoint2 = value;
                RaisePropertyChanged("CurvePoint2");
            }
        }

        private Point _curvePoint3;
        public Point CurvePoint3
        {
            get { return _curvePoint3; }
            set
            {
                _curvePoint3 = value;
                RaisePropertyChanged("CurvePoint3");
            }
        }

        private double _dotTop;
        public double DotTop
        {
            get { return _dotTop; }
            set
            {
                _dotTop = value;
                RaisePropertyChanged("DotTop");
            }
        }

        private double _dotLeft;
        public double DotLeft
        {
            get { return _dotLeft; }
            set
            {
                _dotLeft = value;
                RaisePropertyChanged("DotLeft");
            }
        }

        private double _endDotSize = 6;
        public double EndDotSize
        {
            get { return _endDotSize; }
            set
            {
                _endDotSize = value;
                RaisePropertyChanged("EndDotSize");
            }
        }

        /// <summary>
        /// Returns visible if the connectors is in the current space and the 
        /// model's current connector type is BEZIER
        /// </summary>
        public bool BezVisibility
        {
            get
            {
                //if (workspaceViewModel.DynamoViewModel.ConnectorType == ConnectorType.BEZIER &&
                //    workspaceViewModel.DynamoViewModel.IsShowingConnectors)
                    if (workspaceViewModel.DynamoViewModel.ConnectorType == ConnectorType.BEZIER)
                    return true;
                return false;
            }
            set
            {
                RaisePropertyChanged("BezVisibility");
            }
        }

        /// <summary>
        /// Returns visible if the connectors is in the current space and the 
        /// model's current connector type is POLYLINE
        /// </summary>
        public bool PlineVisibility
        {
            get
            {
                //if (workspaceViewModel.DynamoViewModel.ConnectorType == ConnectorType.POLYLINE &&
                //    workspaceViewModel.DynamoViewModel.IsShowingConnectors)
                    if (workspaceViewModel.DynamoViewModel.ConnectorType == ConnectorType.POLYLINE)
                    return true;
                return false;
            }
            set
            {
                RaisePropertyChanged("PlineVisibility");
            }
        }

        public NodeViewModel Nodevm
        {
            get
            {
                return workspaceViewModel.Nodes.FirstOrDefault(x => x.NodeLogic.GUID == _model.Start.Owner.GUID);
            }
        }

        public PreviewState PreviewState
        {
            get
            {               
                if (_model == null)
                {
                    return PreviewState.None;
                }
              
                if (Nodevm.ShowExecutionPreview)
                {                  
                    return PreviewState.ExecutionPreview;
                }

                if (_model.Start.Owner.IsSelected ||
                    _model.End.Owner.IsSelected)
                {
                    return PreviewState.Selection;
                }

                return PreviewState.None;
            }
        }
         
        public bool IsFrozen
        {
            get { return _model == null ? _activeStartPort.Owner.IsFrozen : Nodevm.IsFrozen; }
        }

        #endregion

        /// <summary>
        /// Updates 'ConnectorDataTooltip' to reflect data of wire being hovered over.
        /// </summary>
        private void UpdateConnectorDataToolTip()
        {
            bool isCollectionofFiveorMore = false;

            if (_model != null)
            {
                var portValue = _model.Start.Owner.GetValue(_model.Start.Index, workspaceViewModel.DynamoViewModel.EngineController);
                if (portValue is null)
                {
                    ConnectorDataTooltip = "N/A";
                    return;
                }

                var isColl = portValue.IsCollection;
                if (isColl)
                {
                    var counter = portValue.GetElements().Count();
                    if (isColl && portValue.GetElements().Count() > 5)
                    {
                        ///only sets 'is a collection' to true if the collection meets a size of 5
                        isCollectionofFiveorMore = true;
                        string formatted = string.Empty;
                        for (int i = 0; i < 5; i++)
                        {
                            formatted += portValue.GetElements().ElementAt(i).StringData;
                            formatted += Environment.NewLine;
                        }
                        formatted += "...";
                        formatted += Environment.NewLine;
                        formatted += portValue.GetElements().Last().StringData;
                        ConnectorDataTooltip = $"{_model.Start.Owner.Name} -> {_model.End.Owner.Name}" + Environment.NewLine +
                      formatted;
                    }
                    else
                    {
                        string formatted = string.Empty;
                        for (int i = 0; i < portValue.GetElements().Count(); i++)
                        {
                            formatted += portValue.GetElements().ElementAt(i).StringData;
                            if (i != portValue.GetElements().Count() - 1)
                                formatted += Environment.NewLine;
                        }
                        ConnectorDataTooltip = $"{_model.Start.Owner.Name} -> {_model.End.Owner.Name}" + Environment.NewLine +
                      formatted;
                    }
                }
                else
                {
                    ConnectorDataTooltip = $"{_model.Start.Owner.Name} -> {_model.End.Owner.Name}" + Environment.NewLine + portValue.StringData;
                }
                isHoveredACollection = isCollectionofFiveorMore;
            }
        }
        #region Commands

        public DelegateCommand BreakConnectionCommand { get; set; }
        public DelegateCommand HideConnectorCommand { get; set; }
        public DelegateCommand SelectConnectedCommand { get; set; }
        public DelegateCommand MouseHoverCommand { get; set; }
        public DelegateCommand MouseUnhoverCommand { get; set; }
        public DelegateCommand PinConnectorCommand { get; set; }

        public void MouseHoverCommandExecute(object parameter)
        {
            var pX = PanelX;
            var pY = PanelY;
            if (WatchHoverViewModel == null && isHoveredACollection && timer == null)
            {
                MouseHoverOn = true;
                WatchHoverViewModel = new WatchHoverIconViewModel(this, workspaceViewModel.DynamoViewModel);
                WatchHoverViewModel.IsHalftone = !IsVisible;
                RaisePropertyChanged(nameof(WatchHoverIconViewModel));
            }

        }
        public void MouseUnhoverCommandExecute(object parameter)
        {
            if (WatchHoverViewModel != null && timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                timer.Tick += TimerDone;
            }
        }

        /// <summary>
        /// Turns timer off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerDone(object sender, EventArgs e)
        {
            timer.Stop();
            timer = null;
            WatchHoverViewModel = null;
            RaisePropertyChanged(nameof(WatchHoverIconViewModel));
        }


        /// <summary>
        /// Breaks connections between node models it is connected to.
        /// </summary>
        /// <param name="parameter"></param>
        private void BreakConnectionCommandExecute(object parameter)
        {
            this.Dispose();
            ConnectorModel.Delete();
        }
        /// <summary>
        /// Toggles wire viz on/off. This can be overwritten when a node is selected in hidden mode.
        /// </summary>
        /// <param name="parameter"></param>
        private void HideConnectorCommandExecute(object parameter)
        {
            IsVisible = !IsVisible;
        }
        /// <summary>
        /// Selects nodes connected to this wire.
        /// </summary>
        /// <param name="parameter"></param>
        private void SelectConnectedCommandExecute(object parameter)
        {
            var leftSideNode = _model.Start.Owner;
            var rightSideNode = _model.End.Owner;

            DynamoSelection.Instance.Selection.Add(leftSideNode);
            DynamoSelection.Instance.Selection.Add(rightSideNode);
        }

        private void PinConnectorCommandExecute(object parameters)
        {
            MousePosition = new Point(PanelX, PanelY);
            if (MousePosition == new Point(0, 0)) return;
            var connectorPinModel = new ConnectorPinModel(PanelX, PanelY, Guid.NewGuid(), _model.GUID);
            AddConnectorPinModel(connectorPinModel);
        }

        private void HandlerRedrawRequest(object sender, EventArgs e)
        {
            Redraw();
        }

        bool CanRun(object parameter)
        {
            return true;
        }
        bool CanRunMouseHover(object parameter)
        {
            if (IsConnecting == false)
                return true;
            else
                return false;
        }
        bool CanRunMouseUnhover(object parameter)
        {
            if (MouseHoverOn == true)
                return true;
            else
                return false;
        }

        private void InitializeCommands()
        {
            BreakConnectionCommand = new DelegateCommand(BreakConnectionCommandExecute, CanRun);
            HideConnectorCommand = new DelegateCommand(HideConnectorCommandExecute, CanRun);
            SelectConnectedCommand = new DelegateCommand(SelectConnectedCommandExecute, CanRun);
            MouseHoverCommand = new DelegateCommand(MouseHoverCommandExecute, CanRunMouseHover);
            MouseUnhoverCommand = new DelegateCommand(MouseUnhoverCommandExecute, CanRunMouseUnhover);
            PinConnectorCommand = new DelegateCommand(PinConnectorCommandExecute, CanRun);
        }

        #endregion

        /// <summary>
        /// Construct a view and start drawing.
        /// </summary>
        /// <param name="port"></param>
        public ConnectorViewModel(WorkspaceViewModel workspace, PortModel port)
        {
            this.workspaceViewModel = workspace;
            ConnectorPinViewCollection = new ObservableCollection<ConnectorPinViewModel>();
            ConnectorPinViewCollection.CollectionChanged += HandleCollectionChanged;

            IsVisible = workspaceViewModel.DynamoViewModel.IsShowingConnectors;
            IsConnecting = true;
            MouseHoverOn = false;
            _activeStartPort = port;

            Redraw(port.Center);

            InitializeCommands();
        }


        /// <summary>
        /// Construct a view and respond to property changes on the model. 
        /// </summary>
        /// <param name="model"></param>
        public ConnectorViewModel(WorkspaceViewModel workspace, ConnectorModel model)
        {
            this.workspaceViewModel = workspace;
            _model = model;
            _model.ConnectorPinModels.CollectionChanged += ConnectorPinModelCollectionChanged;

            ConnectorPinViewCollection = new ObservableCollection<ConnectorPinViewModel>();
            ConnectorPinViewCollection.CollectionChanged += HandleCollectionChanged;

            IsVisible = workspaceViewModel.DynamoViewModel.IsShowingConnectors;
            MouseHoverOn = false;

            if (_model.ConnectorPinModels != null)
            {
                foreach (var p in model.ConnectorPinModels)
                {
                    AddConnectorPinViewModel(p);
                }
            }

            _model.PropertyChanged += Model_PropertyChanged;
            _model.Start.Owner.PropertyChanged += StartOwner_PropertyChanged;
            _model.End.Owner.PropertyChanged += EndOwner_PropertyChanged;

            workspaceViewModel.DynamoViewModel.PropertyChanged += DynamoViewModel_PropertyChanged;
            Nodevm.PropertyChanged += nodeViewModel_PropertyChanged;
            Redraw();
            InitializeCommands();

            UpdateConnectorDataToolTip();
        }

        private void ConnectorPinModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is null) return;
            foreach (ConnectorPinModel newItem in e.NewItems)
            {
                AddConnectorPinViewModel(newItem);
            }
        }

        /// <summary>
        /// Adding a pinModel to a collection of pinModels stored in the ConnectorModel
        /// </summary>
        /// <param name="pinModel"></param>
        private void AddConnectorPinModel(ConnectorPinModel pinModel)
        {
            _model.ConnectorPinModels.Add(pinModel);
        }
        /// <summary>
        /// View model adding method only- given a model
        /// </summary>
        /// <param name="pinModel"></param>
        private void AddConnectorPinViewModel(ConnectorPinModel pinModel)
        {
            var pinViewModel = new ConnectorPinViewModel(this.workspaceViewModel, pinModel);

            pinViewModel.RequestSelect += HandleRequestSelected;
            pinViewModel.RequestRedraw += HandlerRedrawRequest;
            pinViewModel.RequestRemove += HandleConnectorPinViewModelRemove;

            workspaceViewModel.Pins.Add(pinViewModel);
            ConnectorPinViewCollection.Add(pinViewModel);
        }

        private void HandleRequestSelected(object sender, EventArgs e)
        {
            ConnectorPinViewModel pinViewModel = sender as ConnectorPinViewModel;
            IsPartlyVisible = pinViewModel.IsSelected && IsVisible == false ? true : false;
        }

        private void HandleConnectorPinViewModelRemove(object sender, EventArgs e)
        {
            var viewModelSender = sender as ConnectorPinViewModel;
            if (viewModelSender is null) return;

            var matchingPin = workspaceViewModel.Pins.First(x => x == viewModelSender);
            matchingPin.RequestSelect -= HandleRequestSelected;
            matchingPin.RequestRedraw -= HandlerRedrawRequest;
            workspaceViewModel.Pins.Remove(matchingPin);
            ConnectorPinViewCollection.Remove(matchingPin);

            _model.ConnectorPinModels.Remove(viewModelSender.Model);

            if (ConnectorPinViewCollection.Count == 0)
                BezierControlPoints = null;

            matchingPin.Dispose();
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Redraw();
        }
        public virtual void Dispose()
        {
            _model.PropertyChanged -= Model_PropertyChanged;
            _model.Start.Owner.PropertyChanged -= StartOwner_PropertyChanged;
            _model.End.Owner.PropertyChanged -= EndOwner_PropertyChanged;
            _model.ConnectorPinModels.CollectionChanged -= ConnectorPinModelCollectionChanged;

            workspaceViewModel.DynamoViewModel.Model.PreferenceSettings.PropertyChanged -= DynamoViewModel_PropertyChanged;
            Nodevm.PropertyChanged -= nodeViewModel_PropertyChanged;

            foreach (var pin in ConnectorPinViewCollection)
            {
                pin.RequestRedraw -= HandlerRedrawRequest;
                pin.RequestSelect -= HandleRequestSelected;
            }

            DiscardAllConnectorPins();
        }

        private void nodeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowExecutionPreview":
                    RaisePropertyChanged("PreviewState");
                    break;
                case "IsFrozen":
                    RaisePropertyChanged("IsFrozen");
                    break;
            }
        }

        /// <summary>
        /// If the start owner changes position or size, redraw the connector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartOwner_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           
            switch (e.PropertyName)
            {
                case "IsSelected":
                    RaisePropertyChanged("PreviewState");
                    IsPartlyVisible = _model.Start.Owner.IsSelected && IsVisible == false? true : false;
                    break;
                case "Position":
                    RaisePropertyChanged("CurvePoint0");
                    Redraw();
                    break;
                case "Width":
                    RaisePropertyChanged("CurvePoint0");
                    Redraw();
                    break;
                case "ShowExecutionPreview":
                    RaisePropertyChanged("PreviewState");
                    break;
                case "CachedValue":
                    UpdateConnectorDataToolTip();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// If the end owner changes position or size, redraw the connector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EndOwner_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            switch (e.PropertyName)
            {
                case "IsSelected":
                    RaisePropertyChanged("PreviewState");
                    IsPartlyVisible = _model.End.Owner.IsSelected && IsVisible == false? true : false;
                    break;
                case "Position":
                    RaisePropertyChanged("CurvePoint0");
                    Redraw();
                    break;
                case "Width":
                    RaisePropertyChanged("CurvePoint0");
                    Redraw();
                    break;
                case "ShowExecutionPreview":
                    RaisePropertyChanged("PreviewState");
                    break;
            }
        }

        void DynamoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ConnectorType":
                    if (workspaceViewModel.DynamoViewModel.ConnectorType == ConnectorType.BEZIER)
                    {
                        BezVisibility = true;
                        PlineVisibility = false;
                    }
                    else
                    {
                        BezVisibility = false;
                        PlineVisibility = true;
                    }
                    Redraw();
                    break;
                case "IsShowingConnectors":
                    RaisePropertyChanged("BezVisibility");
                    RaisePropertyChanged("PlineVisibility");
                    var dynModel = sender as DynamoViewModel;
                    IsVisible = dynModel.IsShowingConnectors;
                    break;               
            }
        }

        void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentWorkspace":
                    RaisePropertyChanged("BezVisibility");
                    RaisePropertyChanged("PlineVisibility");
                    var dynModel = sender as DynamoViewModel;
                    IsVisible = dynModel.IsShowingConnectors;
                    break;
            }
        }

        /// <summary>
        ///     Recalculate the path points using the internal model.
        /// </summary>
        public void Redraw()
        {
            if (this.ConnectorModel.End != null && ConnectorPinViewCollection.Count > 0)
            {
                RedrawBezierManyPoints();
            }
            else if (this.ConnectorModel.End != null)
                this.Redraw(this.ConnectorModel.End.Center);
        }

       

        /// <summary>
        /// Recalculate the connector's points given the end point
        /// </summary>
        /// <param name="p2">The position of the end point</param>
        public void Redraw(object parameter)
        {
            var p2 = new Point();

            if (parameter is Point)
            {
                p2 = (Point) parameter;
            } else if (parameter is Point2D)
            {
                p2 = ((Point2D)parameter).AsWindowsType();
            }

            CurvePoint3 = p2;

            var offset = 0.0;
            double distance = 0;
            if ( this.BezVisibility == true)
            {
                distance = Math.Sqrt(Math.Pow(CurvePoint3.X - CurvePoint0.X, 2) + Math.Pow(CurvePoint3.Y - CurvePoint0.Y, 2));
                offset = .45 * distance;
            }
            else
            {
                distance = CurvePoint3.X - CurvePoint0.X;
                offset = distance / 2;
            }
            
            CurvePoint1 = new Point(CurvePoint0.X + offset, CurvePoint0.Y);
            CurvePoint2 = new Point(p2.X - offset, p2.Y);

            //if connector is dragged from an input port
            if (ActiveStartPort != null && ActiveStartPort.PortType == PortType.Input)
            {
                CurvePoint1 = new Point(CurvePoint0.X - offset, CurvePoint1.Y); ;
                CurvePoint2 = new Point(p2.X + offset, p2.Y);
            }

            _dotTop = CurvePoint3.Y - EndDotSize / 2;
            _dotLeft = CurvePoint3.X - EndDotSize / 2;

            //Update all the bindings at once.
            //http://stackoverflow.com/questions/4651466/good-way-to-refresh-databinding-on-all-properties-of-a-viewmodel-when-model-chan
            //RaisePropertyChanged(string.Empty);

         
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = CurvePoint0;

            BezierSegment segment = new BezierSegment(CurvePoint1, CurvePoint2, CurvePoint3, true);
            var segmentCollection = new PathSegmentCollection(1);
            segmentCollection.Add(segment);
            pathFigure.Segments = segmentCollection;
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);

            ComputedBezierPathGeometry = new PathGeometry();
            ComputedBezierPathGeometry.Figures = pathFigureCollection;
            ComputedBezierPath = new Path();
            ComputedBezierPath.Data = ComputedBezierPathGeometry;
        }

        public Path ComputedBezierPath { get; set; }
        private PathGeometry _computedPathGeometry;
        public PathGeometry ComputedBezierPathGeometry { 
            get
            {
                return _computedPathGeometry;
            }
            set
            {
                _computedPathGeometry = value;
                RaisePropertyChanged(nameof(ComputedBezierPathGeometry));
            }
        }

        public PathFigure DrawSegmentBetweenPointPairs(Point startPt, Point endPt, ref List<Point[]> controlPointList)
        {
            var offset = 0.0;
            double distance = 0;
            if (this.BezVisibility == true)
            {
                distance = Math.Sqrt(Math.Pow(endPt.X - startPt.X, 2) + Math.Pow(endPt.Y - startPt.Y, 2));
                offset = .45 * distance;
            }
            else
            {
                distance = endPt.X - startPt.X;
                offset = distance / 2;
            }

            var pt1 = new Point(startPt.X + offset, startPt.Y);
            var pt2 = new Point(endPt.X - offset, endPt.Y);


            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPt;

            BezierSegment segment = new BezierSegment(pt1, pt2, endPt, true);
            var segmentCollection = new PathSegmentCollection(1);
            segmentCollection.Add(segment);
            pathFigure.Segments = segmentCollection;

            controlPointList.Add(new Point[]{startPt, pt1, pt2, endPt});

            return pathFigure;
        }

        public void RedrawBezierManyPoints()
        {
            var parameter = this.ConnectorModel.End.Center;
            var param = parameter as object;

            var controlPoints = new List<Point[]>();
            try
            {
                var p2 = new Point();

                if (parameter is Point)
                {
                    p2 = (Point)param;
                }
                else if (parameter is Point2D)
                {
                    p2 = ((Point2D)param).AsWindowsType();
                }

                CurvePoint3 = p2;

                var offset = 0.0;
                double distance = 0;
                if (this.BezVisibility == true)
                {
                    distance = Math.Sqrt(Math.Pow(CurvePoint3.X - CurvePoint0.X, 2) + Math.Pow(CurvePoint3.Y - CurvePoint0.Y, 2));
                    offset = .45 * distance;
                }
                else
                {
                    distance = CurvePoint3.X - CurvePoint0.X;
                    offset = distance / 2;
                }

                CurvePoint1 = new Point(CurvePoint0.X + offset, CurvePoint0.Y);
                CurvePoint2 = new Point(p2.X - offset, p2.Y);

                //if connector is dragged from an input port
                if (ActiveStartPort != null && ActiveStartPort.PortType == PortType.Input)
                {
                    CurvePoint1 = new Point(CurvePoint0.X - offset, CurvePoint1.Y); ;
                    CurvePoint2 = new Point(p2.X + offset, p2.Y);
                }

                _dotTop = CurvePoint3.Y - EndDotSize / 2;
                _dotLeft = CurvePoint3.X - EndDotSize / 2;


                ///Add chain of points including start/end
                
                Point[] points = new Point[ConnectorPinViewCollection.Count];
                int count = 0;
                foreach (var wirePin in ConnectorPinViewCollection)
                {
                    points[count] = new Point(wirePin.Left, wirePin.Top);
                    count++;
                }

                var orderedPoints = points.OrderBy(p => p.X).ToList();

                orderedPoints.Insert(0, CurvePoint0);
                orderedPoints.Insert(orderedPoints.Count, CurvePoint3);

                Point[,] pointPairs = BreakIntoPointPairs(orderedPoints);

                PathFigureCollection pathFigureCollection = new PathFigureCollection();

                for (int i = 0; i < pointPairs.GetLength(0); i++)
                {
                    //each segment starts here
                    var segmentList = new List<Point>();

                    for (int j = 0; j < pointPairs.GetLength(1); j++)
                    {
                        segmentList.Add(pointPairs[i, j]);
                    }

                    var pathFigure = DrawSegmentBetweenPointPairs(segmentList[0], segmentList[1], ref controlPoints);
                    pathFigureCollection.Add(pathFigure);
                }

                BezierControlPoints = new List<Point[]>();
                BezierControlPoints = controlPoints;

                ComputedBezierPathGeometry = new PathGeometry();
                ComputedBezierPathGeometry.Figures = pathFigureCollection;
                ComputedBezierPath = new Path();
                ComputedBezierPath.Data = ComputedBezierPathGeometry;
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
        }

        /// <summary>
        /// Point pairs from a chain of sorted points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private Point[,] BreakIntoPointPairs(List<Point> points)
        {
            Point[,] outPointPairs = new Point[points.Count - 1, 2];

            for (int i = 0; i < points.Count-1; i++)
                for (int j = 0; j < 2; j++)
                    outPointPairs[i, j] = points[i + j];
            return outPointPairs;
        }

        public void DiscardAllConnectorPins()
        {
            foreach (var pin in ConnectorPinViewCollection)
            {
                pin.Model.Dispose();
                pin.Dispose();
            }
            ConnectorPinViewCollection.Clear();
            workspaceViewModel.Pins.Clear();
        }


        private bool CanRedraw(object parameter)
        {
            return true;
        }
    }
}
