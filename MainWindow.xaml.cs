using GymPos.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GymPos
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer == null)
            {
                return;
            }

            string tag = args.SelectedItemContainer.Tag.ToString();

            switch (tag)
            {
                case "SamplePage1":
                    contentFrame.Navigate(typeof(DashboardPage));
                    break;

                case "SamplePage2":
                    contentFrame.Navigate(typeof(ClientePage));
                    break;

                case "SamplePage3":
                    contentFrame.Navigate(typeof(AsistenciaPage));
                    break;

                case "SamplePage4":
                    contentFrame.Navigate(typeof(InicioPage));
                    break;
            }
        }
    }
}
