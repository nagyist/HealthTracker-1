using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
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

namespace HealthTracker.Desktop
{
   /// <summary>
   /// Interaction logic for Shell.xaml
   /// </summary>
   [Export]
   public partial class Shell : Window
   {
      public Shell()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Sets the ViewModel.
      /// </summary>
      /// <remarks>
      /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
      /// the appropriate view model.
      /// </remarks>
      [Import]
      [SuppressMessage( "Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF" )]
      ShellViewModel ViewModel
      {
         set
         {
            this.DataContext = value;
         }
      }
   }
}
