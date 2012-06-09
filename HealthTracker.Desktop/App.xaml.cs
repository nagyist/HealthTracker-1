using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using HealthTracker.Administration.ViewModels;
using HealthTracker.DataRepository;

namespace HealthTracker.Desktop
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      protected override void OnStartup( StartupEventArgs e )
      {
         base.OnStartup( e );

#if (DEBUG)
         RunInDebugMode();
#else
         RunInReleaseMode();
#endif

         this.ShutdownMode = ShutdownMode.OnMainWindowClose;
      }

      private static void RunInDebugMode()
      {
         HealthTrackerBootstrapper bootstrapper = new HealthTrackerBootstrapper();
         bootstrapper.Run();
      }

      private static void RunInReleaseMode()
      {
         AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
         try
         {
            HealthTrackerBootstrapper bootstrapper = new HealthTrackerBootstrapper();
            bootstrapper.Run();
         }
         catch (Exception ex)
         {
            HandleException( ex );
         }
      }

      private static void AppDomainUnhandledException( object sender, UnhandledExceptionEventArgs e )
      {
         HandleException( e.ExceptionObject as Exception );
      }

      private static void HandleException( Exception ex )
      {
         if (ex == null)
         {
            return;
         }

         // ExceptionPolicy is part of the enterprise library  (one step @ a time...)
         //ExceptionPolicy.HandleException(ex, "Default Policy");
         MessageBox.Show( HealthTracker.Desktop.Properties.Resources.UnhandledException );
         Environment.Exit( 1 );
      }
   }
}
