using System;
using System.Collections.Generic;
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
using System.ComponentModel.Composition;
using HealthTracker.DailyLog.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DailyLog.Views
{
   /// <summary>
   /// Interaction logic for DailyLogView.xaml
   /// </summary>
   [Export]
   public partial class DailyLogView : UserControl
   {
      public DailyLogView()
      {
         InitializeComponent();
      }

      [Import]
      [SuppressMessage( "Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF" )]
      private DailyLogViewModel ViewModel
      {
         set
         {
            DataContext = value;
         }
      }
   }
}
