using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Dynamo.Configuration;
using Dynamo.Controls;
using Dynamo.Selection;
using Dynamo.UI;
using Dynamo.UI.Controls;
using Dynamo.UI.Prompts;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using DynCmd = Dynamo.Models.DynamoModel;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Dynamo.Nodes
{
    /// <summary>
    /// Interaction logic for ConnectorPinView.xaml
    /// </summary>
    public partial class ConnectorPinView : IViewModelView<ConnectorPinViewModel>
    {
        public ConnectorPinViewModel ViewModel { get; private set; }

        public ConnectorPinView()
        {
            InitializeComponent();

            Loaded += OnPinViewLoaded;
            Unloaded += OnPinViewUnloaded;
        }


        void OnPinViewLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = this.DataContext as ConnectorPinViewModel;
            ViewModel.RequestsSelection += OnViewModelRequestsSelection;

            MouseLeave += ConnectorPin_MouseLeave;

        }

        private void ConnectorPin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ViewModel != null && ViewModel.OnMouseLeave != null)
                ViewModel.OnMouseLeave();
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
            if (ConnectorPinViewModel.StaticZIndex == int.MaxValue)
            {
                PrepareZIndex();
            }

            ViewModel.ZIndex = ++ConnectorPinViewModel.StaticZIndex;
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
            DynamoSelection.Instance.Selection.AddUnique(ViewModel.Model);
            BringToFront();
        }

        private void OnNodeViewMouseEnter(object sender, MouseEventArgs e)
        {

        }

        //private void OnPinMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    DynamoSelection.Instance.Selection.Remove(ViewModel.Model);
        //}

        //private void WirePinView_OnMouseLeave(object sender, MouseEventArgs e)
        //{
        //    DynamoSelection.Instance.Selection.Remove(ViewModel.Model);
        //}
    }
}
