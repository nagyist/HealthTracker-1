using System;
using System.ComponentModel.Composition;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.Administration.ViewModels
{
   [Export]
   [PartCreationPolicy( CreationPolicy.NonShared )]
   public class FoodGroupViewModel : DataRepositoryObjectViewModel, INavigationAware
   {
      #region Constructors
      [ImportingConstructor]
      public FoodGroupViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         Title = DisplayStrings.NewFoodGroupTitle;
      }
      #endregion

      #region Public Interface
      public override Boolean IsUsed
      {
         get
         {
            return this.DataRepository.ItemIsUsed( Model as FoodGroup );
         }
      }

      public override Boolean IsNew
      {
         get { return !this.DataRepository.Contains( Model as FoodGroup ); }
      }

      public override Boolean IsDirty
      {
         get
         {
            return base.IsDirty;
         }
      }
      #endregion

      #region Command Handlers
      protected override void OnRequestClose()
      {
         switch (DetermineCloseAction( Messages.Question_FoodGroup_Save, Messages.Question_FoodGroup_Close ))
         {
            case CloseAction.SaveAndClose:
               this.Save();
               base.OnRequestClose();
               break;

            case CloseAction.Close:
               base.OnRequestClose();
               break;

            default:
               break;
         }
      }

      protected override void Save()
      {
         if (CanSave)
         {
            this.DataRepository.SaveItem( Model as FoodGroup );
            Reset();
         }
      }

      protected override void Delete()
      {
         if (CanDelete)
         {
            if (UserWantsToDelete( Messages.Question_FoodGroup_Delete ))
            {
               this.DataRepository.Remove( Model as FoodGroup );
               base.OnRequestClose();
            }
         }
      }
      #endregion

      #region Validations
      protected override String ValidateName()
      {
         if (this.DataRepository.NameIsDuplicate( Model as FoodGroup ))
         {
            return Messages.Error_FoodGroup_Exists;
         }

         return base.ValidateName();
      }
      #endregion

      #region INavigationAware Members
      public override void OnNavigatedTo( NavigationContext navigationContext )
      {
         if (Model == null)
         {
            String idParameter = navigationContext.Parameters["ID"];
            Guid myID;
            FoodGroup foodGroup = null;

            if (idParameter != null && Guid.TryParse( idParameter, out myID ))
            {
               foodGroup = this.DataRepository.GetFoodGroup( myID );
            }
            else
            {
               foodGroup = new FoodGroup();
            }
            Model = foodGroup;

            base.OnNavigatedTo( navigationContext );
         }
      }
      #endregion
   }
}
