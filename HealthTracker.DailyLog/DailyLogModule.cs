using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.DailyLog.Views;
using HealthTracker.Infrastructure;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace HealthTracker.DailyLog
{
   [ModuleExport( typeof( DailyLogModule ) )]
   public class DailyLogModule : IModule
   {
      [ImportingConstructor]
      public DailyLogModule( IRegionManager regionManager )
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion( RegionNames.DailyLogRegion, typeof( DailyLogView ) );
      }

      private IRegionManager _regionManager;
   }
}
