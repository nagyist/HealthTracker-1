using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using HealthTracker.Administration;
using HealthTracker.Configuration;
using HealthTracker.DailyLog;
using HealthTracker.DataRepository;
using HealthTracker.Infrastructure;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.Desktop
{
   public class HealthTrackerBootstrapper : MefBootstrapper
   {
      protected override void ConfigureAggregateCatalog()
      {
         base.ConfigureAggregateCatalog();
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( HealthTrackerBootstrapper ).Assembly) );
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( DataRepositoryModule ).Assembly ) );
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( AdministrationModule ).Assembly ) );
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( ConfigurationModule ).Assembly ) );
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( InfrastructureModule ).Assembly ) );
         this.AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( DailyLogModule ).Assembly ) );
      }

      protected override System.Windows.DependencyObject CreateShell()
      {
         return this.Container.GetExportedValue<Shell>();
      }

      protected override void InitializeShell()
      {
         base.InitializeShell();

         App.Current.MainWindow = (Shell)this.Shell;
         App.Current.MainWindow.Show();
      }

      private readonly TraceLogger logger = new TraceLogger();
      protected override ILoggerFacade CreateLogger()
      {
         return this.logger;
      }
   }
}
