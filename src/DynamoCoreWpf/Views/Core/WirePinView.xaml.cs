using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Dynamo.Configuration;
using Dynamo.Selection;
using Dynamo.UI;
using Dynamo.UI.Prompts;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using DynCmd = Dynamo.Models.DynamoModel;

namespace Dynamo.Nodes
{
    /// <summary>
    /// Interaction logic for WirePinView.xaml
    /// </summary>
    public partial class WirePinView : IViewModelView<WirePinViewModel>
    {
        public WirePinViewModel ViewModel { get; private set; }

        public WirePinView(WirePinViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            DataContext = ViewModel;

            //PinBtn.PreviewMouseDown += OnPinPreviewMouseDown;

            Loaded += OnPinViewLoaded;
            Unloaded += OnPinViewUnloaded;
        }
        public WirePinView()
        {
            InitializeComponent();

           // PinBtn.PreviewMouseDown += OnPinPreviewMouseDown;

            Loaded += OnPinViewLoaded;
            Unloaded += OnPinViewUnloaded;
        }


        void OnPinViewLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = this.DataContext as WirePinViewModel;
            ViewModel.RequestsSelection += OnViewModelRequestsSelection;

        }

        void OnPinViewUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.RequestsSelection -= OnViewModelRequestsSelection;
        }

        void OnViewModelRequestsSelection(object sender, EventArgs e)
        {
            if (!ViewModel.Model.IsSelected)
            {
                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    DynamoSelection.Instance.ClearSelection();
                }

                DynamoSelection.Instance.Selection.AddUnique(ViewModel.Model);

            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    DynamoSelection.Instance.Selection.Remove(ViewModel.Model);
                }
            }
        }
  

        /// <summary>
        /// Sets ZIndex of the particular note to be the highest in the workspace
        /// This brings the note to the forefront of the workspace when clicked
        /// </summary>
        private void BringToFront()
        {
            if (NoteViewModel.StaticZIndex == int.MaxValue)
            {
                PrepareZIndex();
            }

            ViewModel.ZIndex = ++NoteViewModel.StaticZIndex;
        }


        /// <summary>
        /// If ZIndex is more then max value of int, it should be set back to 0 for all elements.
        /// </summary>
        private void PrepareZIndex()
        {
            NoteViewModel.StaticZIndex = Configurations.NodeStartZIndex;

            var parent = TemplatedParent as ContentPresenter;
            if (parent == null) return;

            // reset the ZIndex for all Notes
            foreach (var child in parent.ChildrenOfType<NoteView>())
            {
                child.ViewModel.ZIndex = Configurations.NodeStartZIndex;
            }

            // reset the ZIndex for all Nodes
            foreach (var child in parent.ChildrenOfType<Controls.NodeView>())
            {
                child.ViewModel.ZIndex = Configurations.NodeStartZIndex;
            }
        }

        private void OnPinMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Guid pinGuid = this.ViewModel.Model.GUID;
           
            //ViewModel.WorkspaceViewModel.DynamoViewModel.ExecuteCommand(
            //    new DynCmd.SelectModelCommand(pinGuid, Keyboard.Modifiers.AsDynamoType()));
            DynamoSelection.Instance.Selection.AddUnique(ViewModel.Model);
            BringToFront();
        }
    }
}
