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
using HealthTracker.DailyLog.ViewModels;
using HealthTracker.Infrastructure;

namespace HealthTracker.DailyLog.Views
{
   /// <summary>
   /// Interaction logic for MealView.xaml
   /// </summary>
   [Export( ViewNames.MealView )]
   [PartCreationPolicy( CreationPolicy.NonShared )]
   public partial class MealView : UserControl
   {
      public MealView()
      {
         InitializeComponent();
      }

      [Import]
      [SuppressMessage( "Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF" )]
      private MealViewModel ViewModel
      {
         set { this.DataContext = value; }
      }
   }
}
