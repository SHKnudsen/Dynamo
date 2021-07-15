using System;
using System.Linq;
using Dynamo.Configuration;
using Dynamo.Graph;
using Dynamo.Selection;
using Dynamo.UI.Commands;
using Dynamo.Wpf.ViewModels.Core;
using Newtonsoft.Json;

namespace Dynamo.ViewModels
{
    public partial class ConnectorPinViewModel : ViewModelBase
    {
        #region Events
        public event EventHandler RequestSelect;
        public virtual void OnRequestSelect(Object sender, EventArgs e)
        {
            if (RequestSelect != null)
            {
                RequestSelect(this, e);
            }
        }

        public event EventHandler RequestRedraw;
        public virtual void OnRequestRedraw(Object sender, EventArgs e)
        {
            if (RequestRedraw != null)
            {
                RequestRedraw(this, e);
            }
        }

        public event EventHandler RequestRemove;
        public virtual void OnRequestRemove(Object sender, EventArgs e)
        {
            RequestRemove(this, e);
        }

        #endregion

        #region Properties

        

        [JsonIgnore]
        private readonly WorkspaceViewModel WorkspaceViewModel;
        private int zIndex = Configurations.NodeStartZIndex; // initialize the start Z-Index of a note to the same as that of a node
        internal static int StaticZIndex = Configurations.NodeStartZIndex;

        private ConnectorPinModel model;
        [JsonIgnore]
        public ConnectorPinModel Model
        {
            get { return model; }
            set
            {
                model = value;
                RaisePropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// Element's left position is two-way bound to this value
        /// </summary>
        public double Left
        {
            get { return model.X; }
            set
            {
                model.X = value;
                RaisePropertyChanged(nameof(Left));
            }
        }

        /// <summary>
        /// Element's top position is two-way bound to this value
        /// </summary>
        public double Top
        {
            get { return model.Y; }
            set
            {
                model.Y = value;
                RaisePropertyChanged(nameof(Top));
            }
        }

        /// <summary>
        /// ZIndex represents the order on the z-plane in which the notes and other objects appear. 
        /// </summary>
        [JsonIgnore]
        public int ZIndex
        {

            get { return zIndex; }
            set { zIndex = value; RaisePropertyChanged(nameof(ZIndex)); }
        }

        /// <summary>
        /// Provides the ViewModel (this) with the selected state of the ConnectorPinModel.
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return model.IsSelected; }
        }

        private bool isHalftone;
        /// <summary>
        /// Provides the ViewModel (this) with the visibility state of the Connector.
        /// When set to 'hidden', 'IsHalftone' is set to true, and viceversa.
        /// </summary>
        [JsonIgnore]
        public bool IsHalftone
        {
            get
            {
                return isHalftone;
            }
            set
            {
                isHalftone = value;
                RaisePropertyChanged(nameof(IsHalftone));
            }
        }
        /// <summary>
        /// This property is purely used for serializing/ deserializing.
        /// In reconstructing ConnectorPins, we need to know what Connector they belong to.
        /// </summary>
        public Guid ConnectorGuid
        {
            get
            {
                return model.ConnectorId;
            }
        }

        #endregion

        #region Commands
        /// <summary>
        /// Delegate command handling the removal of this ConnectorPin from its corresponding connector.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand UnpinConnectorCommand { get; set; }

        private void UnpinWireCommandExecute(object parameter)
        {
            OnRequestRemove(this, EventArgs.Empty);
        }

        private void InitializeCommands()
        {
            UnpinConnectorCommand = new DelegateCommand(UnpinWireCommandExecute);
        }

        #endregion

        public ConnectorPinViewModel(WorkspaceViewModel workspaceViewModel, ConnectorPinModel model)
        {
            this.WorkspaceViewModel = workspaceViewModel;
            this.model = model;
            InitializeCommands();
            model.PropertyChanged += pin_PropertyChanged;
            ZIndex = ++StaticZIndex; // places the pin on top of all nodes/notes
        }

        public override void Dispose()
        {
            model.PropertyChanged -= pin_PropertyChanged;
            base.Dispose();
        }

        //respond to changes on the model's properties
        void pin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConnectorPinModel.X):
                    OnRequestRedraw(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(Left));
                    break;
                case nameof(ConnectorPinModel.Y):
                    OnRequestRedraw(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(Top));
                    break;
                case nameof(ConnectorPinModel.IsSelected):
                    OnRequestSelect(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(IsSelected));
                    break;
            }
        }
    }
}
