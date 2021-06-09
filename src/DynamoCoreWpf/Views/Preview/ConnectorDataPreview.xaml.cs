using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dynamo.Configuration;
using Dynamo.UI;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using Dynamo.Wpf.UI;
using Dynamo.Wpf.Utilities;
using InfoBubbleViewModel = Dynamo.ViewModels.InfoBubbleViewModel;

namespace Dynamo.Controls
{
    /// <summary>
    /// Interaction logic for ConnectorDataPreview.xaml
    /// </summary>
    public partial class ConnectorDataPreview : UserControl
    {
        public ConnectorDataPreview()
        {
            InitializeComponent();
        }


        private void ContentContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (this.IsDisconnected)
            //    return;

            //if (ViewModel.InfoBubbleStyle == InfoBubbleViewModel.Style.ErrorCondensed ||
            //    ViewModel.InfoBubbleStyle == InfoBubbleViewModel.Style.WarningCondensed)
            //    ShowErrorBubbleFullContent();

            //ShowInfoBubble();

            //this.Cursor = CursorLibrary.GetCursor(CursorSet.Pointer);
        }

        private void InfoBubble_MouseLeave(object sender, MouseEventArgs e)
        {
            // It is possible for MouseLeave message (that was scheduled earlier) to reach
            // InfoBubbleView when it becomes disconnected from InfoBubbleViewModel (i.e. 
            // when the NodeModel it belongs is deleted by user). In this case, InfoBubbleView
            //// should simply ignore the message, since the node is no longer valid.
            //if (this.IsDisconnected)
            //    return;

            //switch (ViewModel.InfoBubbleStyle)
            //{
            //    case InfoBubbleViewModel.Style.Warning:
            //    case InfoBubbleViewModel.Style.Error:
            //        ShowErrorBubbleCondensedContent();
            //        break;

            //    default:
            //        FadeOutInfoBubble();
            //        break;
            //}

            //this.Cursor = CursorLibrary.GetCursor(CursorSet.Pointer);
        }

        private void InfoBubble_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.Cursor = CursorLibrary.GetCursor(CursorSet.Condense);
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            //if (this.IsDisconnected)
            //    return;

            //if (!isResizing)
            //    return;

            //Point mouseLocation = Mouse.GetPosition(mainGrid);
            //if (!isResizeHeight)
            //    mouseLocation.Y = double.MaxValue;
            //if (!isResizeWidth)
            //    mouseLocation.X = double.MaxValue;

            ////ViewModel.ResizeCommand.Execute(mouseLocation);
            //Resize(mouseLocation);
        }

        private void InfoBubble_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }

        /// <summary>
        /// Dispose function adding resubscribe logic
        /// </summary>
        public void Dispose()
        {
            //viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            //viewModel.RequestAction -= InfoBubbleRequestAction;

            //// make sure we unsubscribe from handling the hyperlink click event
            //if (this.hyperlink != null)
            //    this.hyperlink.RequestNavigate -= RequestNavigateToDocumentationLinkHandler;
        }
    }
}
