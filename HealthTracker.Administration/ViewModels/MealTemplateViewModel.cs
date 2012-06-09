using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
   public class MealTemplateViewModel : MealViewModelBase, INavigationAware
   {
      #region Constructors
      [ImportingConstructor]
      public MealTemplateViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         Title = DisplayStrings.NewMealTemplateTitle;
      }
      #endregion

      #region Public Interface
      public override Boolean IsNew
      {
         get { return !this.DataRepository.Contains( Model as MealTemplate ); }
      }

      public override Boolean IsUsed
      {
         get { return this.DataRepository.ItemIsUsed( Model as MealTemplate ); }
      }
      #endregion

      #region Command Handlers
      protected override void OnRequestClose()
      {
         switch (DetermineCloseAction( Messages.Question_MealTemplate_Save, Messages.Question_MealTemplate_Close ))
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
            if (this.UserWantsToDelete( Messages.Question_MealTemplate_Delete ))
            {
               this.DataRepository.Remove( Model as MealTemplate );
               this.OnRequestClose();
            }

         }
      }

      protected override void Save()
      {
         if (CanSave)
         {
            this.DataRepository.SaveItem( Model as MealTemplate );
            Reset();
         }
      }
      #endregion

      #region IDataErrorInfo Methods
      public override String this[string propertyName]
      {
         get
         {
            String error = null;

            error = base[propertyName];

            if (error == null)
            {
               if (propertyName == "Name")
               {
                  error = ValidateName();
               }
            }

            return error;
         }
      }
      #endregion

      #region Validations
      private static readonly String[] ValidatedProperties = 
      {
         "Name"
      };

      /// <summary>
      /// Determine if the object is valid or not.
      /// </summary>
      /// <returns>true if the object is valid, false otherwise</returns>
      public override Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {
                  case "Name":
                     error = ValidateName();
                     break;


                  default:
                     Debug.Fail( "Unexpected property: " + propertyName );
                     break;
               }

               if (error != null)
               {
                  return false;
               }
            }

            return base.IsValid;
         }
      }

      protected override String ValidateName()
      {
         if (this.DataRepository.NameIsDuplicate( Model as MealTemplate ))
         {
            return Messages.Error_MealTemplate_Exists;
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
            MealTemplate mealTemplate = null;

            if (idParameter != null && Guid.TryParse( idParameter, out myID ))
            {
               mealTemplate = this.DataRepository.GetMealTemplate( myID );
            }
            else
            {
               mealTemplate = new MealTemplate();
               mealTemplate.DateAndTimeOfMeal = DateTime.Now.Date;
            }          
            Model = mealTemplate;

            base.OnNavigatedTo( navigationContext );
         }
      }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         this.ValidFoodItems.Dispose();
         this.ValidMealTypes.Dispose();
      }
      #endregion
   }
}
