using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Administration.ViewModels;

namespace HealthTracker.Administration.Views
{
   /// <summary>
   /// Interaction logic for AdministrationView.xaml
   /// </summary>
   [Export]
   public partial class AdministrationView : UserControl
   {
      public AdministrationView()
      {
         InitializeComponent();
      }

      [Import]
      [SuppressMessage( "Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF" )]
      private AdminModuleViewModel ViewModel
      {
         set
         {
            DataContext = value;
         }
      }

      private void AddButton_Click( object sender, RoutedEventArgs e )
      {
         ItemToAddMenu.PlacementTarget = this;
         ItemToAddMenu.IsOpen = true;
      }
   }
}
