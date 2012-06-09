using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.Administration.Views;
using HealthTracker.Infrastructure;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace HealthTracker.Administration
{
   [ModuleExport( typeof( AdministrationModule ) )]
   public class AdministrationModule : IModule
   {
      #region Constructors
      [ImportingConstructor]
      public AdministrationModule( IRegionManager regionManager )
      {
         _regionManager = regionManager;
      }
      #endregion

      #region IModule Methods
      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion( RegionNames.AdministrationRegion, typeof( AdministrationView ) );
      }
      #endregion

      #region Private Data
      IRegionManager _regionManager;
      #endregion
   }
}
