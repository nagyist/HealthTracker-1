using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.DailyLog.ViewModels
{
   [Export]
   [PartCreationPolicy( CreationPolicy.NonShared )]
   public class MealViewModel : MealViewModelBase
   {
      #region Constructors
      [ImportingConstructor]
      public MealViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         Title = DisplayStrings.NewMealTitle;
      }
      #endregion

      #region Public Interface
      public override bool IsNew
      {
         get { return !this.DataRepository.Contains( Model as Meal ); }
      }

      public override Boolean IsUsed
      {
         // Meals are not used elsewhere.
         get { return false; }
      }

      public override MealType TypeOfMeal
      {
         get
         {
            return base.TypeOfMeal;
         }
         set
         {
            // If the name of the Meal is NULL or if the name of the meal was
            // set based on the name of the meal type, set it according to the
            // current meal type.
            if (String.IsNullOrEmpty( Name ) ||
                (TypeOfMeal != null && Name == TypeOfMeal.Name))
            {
               // NOTE: Change the model directly so we don't get an extra mealType
               //       in the undo stack.
               if (value != null)
               {
                  (Model as Meal).Name = value.Name;
                  this.OnPropertyChanged( NamePropertyName );
               }
               else
               {
                  (Model as Meal).Name = null;
               }
            }
            base.TypeOfMeal = value;
         }
      }

      private const String dateOfMealPropertyName = "DateOfMeal";
      public DateTime DateOfMeal
      {
         get
         {
            return (Model as Meal).DateAndTimeOfMeal.Date;
         }
         set
         {
            if (((MealBase)Model).DateAndTimeOfMeal != value)
            {
               AddUndoableValue(
                  new UndoablePropertyValue<MealViewModelBase, DateTime>( this, dateOfMealPropertyName, UndoableActions.Modify, TimeOfMeal, value ) );
               ((Meal)Model).DateAndTimeOfMeal = value.Date + ((Meal)Model).DateAndTimeOfMeal.TimeOfDay;
               OnPropertyChanged( dateOfMealPropertyName );
               OnPropertyChanged( IsDirtyPropertyName );
               OnPropertyChanged( IsValidPropertyName );
            }

            OnPropertyChanged( dateOfMealPropertyName );
            OnPropertyChanged( IsValidPropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
         }
      }
      #endregion

      #region Command Handlers
      protected override void OnRequestClose()
      {
         switch (DetermineCloseAction( Messages.Question_Meal_Save, Messages.Question_Meal_Close ))
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

      protected override void Delete()
      {
         if (CanDelete)
         {
            if (this.UserWantsToDelete( Messages.Question_Meal_Delete ))
            {
               this.DataRepository.Remove( Model as Meal );
               this.OnRequestClose();
            }

         }
      }

      protected override void Save()
      {
         if (CanSave)
         {
            this.DataRepository.SaveItem( Model as Meal );
            Reset();
         }
      }
      #endregion

      #region INavigationAware Members
      public override void OnNavigatedTo( NavigationContext navigationContext )
      {
         if (Model == null)
         {
            String idParameter = navigationContext.Parameters["ID"];
            Guid myID;
            Meal meal = null;

            if (idParameter != null && Guid.TryParse( idParameter, out myID ))
            {
               meal = this.DataRepository.GetMeal( myID );
            }
            else
            {
               meal = new Meal();
               meal.DateAndTimeOfMeal = DateTime.Now;
            }
            Model = meal;

            base.OnNavigatedTo( navigationContext );
         }
      }
      #endregion
   }
}
