using System.Windows;
using System.Windows.Controls;
using Dynamo.LintingViewExtension.Properties;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;

namespace Dynamo.LintingViewExtension
{
    public class LintingViewExtension : IViewExtension
    {
        private const string EXTENSION_NAME = "Dynamo Linter";
        private const string EXTENSION_GUID = "3467481b-d20d-4918-a454-bf19fc5c25d7";

        private ViewLoadedParams viewLoadedParamsReference;
        private MenuItem linterMenuItem;
        private LinterViewModel linterViewModel;
        private LinterView linterView;


        public string UniqueId { get { return EXTENSION_GUID; } }

        public string Name { get { return EXTENSION_NAME; } }

        public void Startup(ViewStartupParams viewStartupParams)
        {
            // Do nothing for now
        }

        public void Loaded(ViewLoadedParams viewLoadedParams)
        {
            this.viewLoadedParamsReference = viewLoadedParams;
            this.linterViewModel = new LinterViewModel((viewLoadedParams.DynamoWindow.DataContext as DynamoViewModel).LinterManager, viewLoadedParamsReference);
            this.linterView = new LinterView() { DataContext = linterViewModel };

            // Add a button to Dynamo View menu to manually show the window
            this.linterMenuItem = new MenuItem { Header = Resources.MenuItemText, IsCheckable = true };
            this.linterMenuItem.Checked += MenuItemCheckHandler;
            this.linterMenuItem.Unchecked += MenuItemUnCheckedHandler;
            this.viewLoadedParamsReference.AddMenuItem(MenuBarType.View, this.linterMenuItem);
        }

        private void MenuItemUnCheckedHandler(object sender, RoutedEventArgs e)
        {
            viewLoadedParamsReference.CloseExtensioninInSideBar(this);
        }

        private void MenuItemCheckHandler(object sender, RoutedEventArgs e)
        {
            this.viewLoadedParamsReference?.AddToExtensionsSideBar(this, this.linterView);
        }

        public void Shutdown()
        {
            // Do nothing for now 
        }
        public void Dispose()
        {
            // Do nothing for now 
        }
    }
}
