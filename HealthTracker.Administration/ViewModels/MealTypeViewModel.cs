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
   public class MealTypeViewModel : DataRepositoryObjectViewModel, INavigationAware
   {
      #region Constructors
      [ImportingConstructor]
      public MealTypeViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         Title = DisplayStrings.NewMealTypeTitle;
      }
      #endregion

      #region Public Interface
      public override Boolean IsUsed
      {
         get
         {
            return this.DataRepository.ItemIsUsed( Model as MealType );
         }
      }

      public override Boolean IsNew
      {
         get { return !this.DataRepository.Contains( Model as MealType ); }
      }

      public override Boolean IsDirty
      {
         get
         {
            return base.IsDirty;
         }
      }

      public DateTime DefaultTimeOfMeal
      {
         get
         {
            return ((MealType)Model).DefaultTimeOfMeal;
         }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<MealTypeViewModel, DateTime>( this, "DefaultTimeOfMeal", UndoableActions.Modify, DefaultTimeOfMeal, value ) );
            ((MealType)Model).DefaultTimeOfMeal = value;
         }
      }
      public Boolean UseDefaultTimeOfMeal
      {
         get
         {
            return ((MealType)Model).UseDefaultMealTime;
         }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<MealTypeViewModel, Boolean>( this, "UseDefaultTimeOfMeal", UndoableActions.Modify, UseDefaultTimeOfMeal, value ) );
            ((MealType)Model).UseDefaultMealTime = value;
         }
      }
      #endregion

      #region Command Handlers
      protected override void OnRequestClose()
      {
         switch (DetermineCloseAction( Messages.Question_MealType_Save, Messages.Question_MealType_Close ))
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
            this.DataRepository.SaveItem( Model as MealType );
            Reset();
         }
      }

      protected override void Delete()
      {
         if (CanDelete)
         {
            if (this.UserWantsToDelete( Messages.Question_MealType_Delete ))
            {
               this.DataRepository.Remove( Model as MealType );
               this.OnRequestClose();
            }
         }
      }
      #endregion

      #region Validations
      protected override String ValidateName()
      {
         if (this.DataRepository.NameIsDuplicate( Model as MealType ))
         {
            return Messages.Error_MealType_Exists;
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
            MealType mealType = null;

            if (idParameter != null && Guid.TryParse( idParameter, out myID ))
            {
               mealType = this.DataRepository.GetMealType( myID );
            }
            else
            {
               mealType = new MealType();
            }
            Model = mealType;

            base.OnNavigatedTo( navigationContext );
         }
      }
      #endregion
   }
}
