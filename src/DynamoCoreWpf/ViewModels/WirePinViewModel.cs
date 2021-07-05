﻿using System;
using System.Linq;
using Dynamo.Configuration;
using Dynamo.Graph;
using Dynamo.Selection;
using Dynamo.UI.Commands;
using Dynamo.Wpf.ViewModels.Core;
using Newtonsoft.Json;

namespace Dynamo.ViewModels
{
    public partial class WirePinViewModel : ViewModelBase
    {
        #region Events

        public event EventHandler RequestsSelection;
        public virtual void OnRequestsSelection(Object sender, EventArgs e)
        {
            if (RequestsSelection != null)
            {
                RequestsSelection(this, e);
            }
        }

        public event EventHandler RequestRemove;
        public virtual void OnRequestRemove(Object sender, EventArgs e)
        {
            RequestRemove(this, e);
        }

        #endregion

        #region Properties

        private WirePinModel _model;

        [JsonIgnore]
        public readonly WorkspaceViewModel WorkspaceViewModel;
        private int zIndex = Configurations.NodeStartZIndex; // initialize the start Z-Index of a note to the same as that of a node
        internal static int StaticZIndex = Configurations.NodeStartZIndex;

        [JsonIgnore]
        public WirePinModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                RaisePropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// Element's left position is two-way bound to this value
        /// </summary>
        [JsonIgnore]
        public double Left
        {
            get { return _model.X; }
            set
            {
                _model.X = value;
                RaisePropertyChanged(nameof(Left));
            }
        }

        /// <summary>
        /// Element's top position is two-way bound to this value
        /// </summary>
        [JsonIgnore]
        public double Top
        {
            get { return _model.Y; }
            set
            {
                _model.Y = value;
                RaisePropertyChanged("Top");
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


        [JsonIgnore]
        public bool IsSelected
        {
            get { return _model.IsSelected; }
        }

        #endregion

        #region Commands

        public DelegateCommand UnpinWireCommand { get; set; }

        private void UnpinWireCommandExecute(object parameter)
        {
            OnRequestRemove(this, EventArgs.Empty);
        }
        private void InitializeCommands()
        {
            UnpinWireCommand = new DelegateCommand(UnpinWireCommandExecute);
        }



#endregion

        public WirePinViewModel(WorkspaceViewModel workspaceViewModel, WirePinModel model)
        {
            this.WorkspaceViewModel = workspaceViewModel;
            _model = model;
            InitializeCommands();
            model.PropertyChanged += pin_PropertyChanged;
           // DynamoSelection.Instance.Selection.CollectionChanged += SelectionOnCollectionChanged;
            ZIndex = ++StaticZIndex; // places the note on top of all nodes/notes
        }

        public override void Dispose()
        {
            _model.PropertyChanged -= pin_PropertyChanged;

            //if (IsDebugBuild)
            //{
            //    DynamoViewModel.EngineController.AstBuilt -= EngineController_AstBuilt;
            //}

            //foreach (var p in WirePin)
            //{
            //    p.Dispose();
            //}

            //foreach (var p in OutPorts)
            //{
            //    p.Dispose();
            //}
            //ErrorBubble.Dispose();
            //DynamoSelection.Instance.Selection.CollectionChanged -= SelectionOnCollectionChanged;
            base.Dispose();
        }

        //private void SelectionOnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //CreateGroupCommand.RaiseCanExecuteChanged();
        //    //AddToGroupCommand.RaiseCanExecuteChanged();
        //    //UngroupCommand.RaiseCanExecuteChanged();
        //}

        private void Select(object parameter)
        {
            OnRequestsSelection(this, EventArgs.Empty);
        }

        public void UpdateSizeFromView(double w, double h)
        {
            this._model.SetSize(w, h);
        }

        private bool CanSelect(object parameter)
        {
            if (!DynamoSelection.Instance.Selection.Contains(_model))
            {
                return true;
            }
            return false;
        }

        //respond to changes on the model's properties
        void pin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X":
                    RaisePropertyChanged(nameof(Left));
                    break;
                case "Y":
                    RaisePropertyChanged(nameof(Top));
                    break;
                case "IsSelected":
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

        private void UngroupWirePin(object parameters)
        {
            WorkspaceViewModel.DynamoViewModel.UngroupModelCommand.Execute(null);
        }

        private bool CanUngroupWirePin(object parameters)
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
