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

        public Guid ConnectorGuid
        {
            get
            {
                return model.ConnectorId;
            }
        }

        #endregion

        #region Commands
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
            ZIndex = ++StaticZIndex; // places the note on top of all nodes/notes
        }

        public override void Dispose()
        {
            model.PropertyChanged -= pin_PropertyChanged;
            base.Dispose();
        }

        public void UpdateSizeFromView(double w, double h)
        {
            this.model.SetSize(w, h);
        }

        private bool CanSelect(object parameter)
        {
            if (!DynamoSelection.Instance.Selection.Contains(model))
            {
                return true;
            }
            return false;
        }

        //respond to changes on the connectorModel's properties
        void pin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X":
                    OnRequestRedraw(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(Left));
                    break;
                case "Y":
                    OnRequestRedraw(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(Top));
                    break;
                case "IsSelected":
                    OnRequestSelect(this, EventArgs.Empty);
                    RaisePropertyChanged(nameof(IsSelected));
                    break;
            }
        }

        private void CreateGroup(object parameters)
        {
            WorkspaceViewModel.DynamoViewModel.AddAnnotationCommand.Execute(null);
        }

        private bool CanCreateGroup(object parameters)
        {
            var groups = WorkspaceViewModel.Model.Annotations;
            //Create Group should be disabled when a group is selected
            if (groups != null && groups.Any(x => x.IsSelected))
            {
                return false;
            }

            //Create Group should be disabled when a node selected is already in a group
            if (!groups.Any(x => x.IsSelected))
            {
                var modelSelected = DynamoSelection.Instance.Selection.OfType<ModelBase>().Where(x => x.IsSelected);
                foreach (var model in modelSelected)
                {
                    if (groups.ContainsModel(model.GUID))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void UngroupConnectorPin(object parameters)
        {
            WorkspaceViewModel.DynamoViewModel.UngroupModelCommand.Execute(null);
        }

        private bool CanUngroupConnectorPin(object parameters)
        {
            var groups = WorkspaceViewModel.Model.Annotations;
            if (!groups.Any(x => x.IsSelected))
            {
                return (groups.ContainsModel(Model.GUID));
            }
            return false;
        }

        private void AddToGroup(object parameters)
        {
            WorkspaceViewModel.DynamoViewModel.AddModelsToGroupModelCommand.Execute(null);
        }

        private bool CanAddToGroup(object parameters)
        {
            var groups = WorkspaceViewModel.Model.Annotations;
            if (groups.Any(x => x.IsSelected))
            {
                return !(groups.ContainsModel(Model.GUID));
            }
            return false;
        }
    }
}
