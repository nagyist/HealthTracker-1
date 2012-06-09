using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using HealthTracker.BaseClasses.ViewModels;
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
   [PartCreationPolicy(CreationPolicy.NonShared)]
   public class FoodItemViewModel : DataRepositoryObjectViewModel, INavigationAware
   {
      #region Constructors
      [ImportingConstructor]
      public FoodItemViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         FoodGroupsPerServing = new ObservableCollection<ServingViewModel<FoodGroup>>();
         ValidFoodGroups = new AllFoodGroupsViewModel( dataRepository );
         Title = DisplayStrings.NewFoodItemTitle;
      }
      #endregion

      #region Public Interface
      public override bool IsNew
      {
         get { return !this.DataRepository.Contains( Model as FoodItem ); }
      }
      public override Boolean IsUsed
      {
         get { return this.DataRepository.ItemIsUsed( Model as FoodItem ); }
      }

      protected String FoodGroupsPerServingPropertyName = "FoodGroupsPerServing";
      public ObservableCollection<ServingViewModel<FoodGroup>> FoodGroupsPerServing { get; private set; }

      private String CaloriesPerServingPropertyName = "CaloriesPerServing";
      public Decimal CaloriesPerServing
      {
         get
         {
            return ((FoodItem)Model).CaloriesPerServing;
         }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<FoodItemViewModel, Decimal>(
                  this, CaloriesPerServingPropertyName, UndoableActions.Modify, CaloriesPerServing, value ) );
            ((FoodItem)Model).CaloriesPerServing = value;
            OnPropertyChanged( CaloriesPerServingPropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
            OnPropertyChanged( IsValidPropertyName );
         }
      }

      public override Boolean IsDirty
      {
         get
         {
            if (base.IsDirty == true ||
             this.CaloriesPerServing != this.PreviousCaloriesPerServing ||
             this.FoodGroupsPerServing.Count != this.PreviousNumberOfFoodGroupServings)
            {
               return true;
            }

            // If we are here, we need to check the individual food group items.
            // For now, just use a brute force loop and find.  These lists should
            // always be small, so this should be OK.
            foreach (ServingViewModel<FoodGroup> servingViewModel in FoodGroupsPerServing)
            {
               if (servingViewModel.IsDirty)
               {
                  return true;
               }
            }

            // Made it.  Must not be dirty.
            return false;
         }
      }

      public AllFoodGroupsViewModel ValidFoodGroups { get; private set; }
      #endregion

      #region Protected Virtual Members
      protected override void Reset()
      {
         base.Reset();

         SetCacheValues();
      }
      #endregion

      #region Command Handlers
      protected override void OnRequestClose()
      {
         switch (DetermineCloseAction( Messages.Question_FoodItem_Save, Messages.Question_FoodItem_Close ))
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
            if (this.UserWantsToDelete( Messages.Question_FoodItem_Delete ))
            {
               this.DataRepository.Remove( Model as FoodItem );
               this.OnRequestClose();
            }
         }
      }

      protected override void Save()
      {
         if (CanSave)
         {
            this.DataRepository.SaveItem( Model as FoodItem );
            Reset();
         }
      }
      #endregion

      #region Validations
      protected override String ValidateName()
      {
         if (this.DataRepository.NameIsDuplicate( Model as FoodItem ))
         {
            return Messages.Error_FoodItem_Exists;
         }

         return base.ValidateName();
      }
      #endregion

      #region Private Helpers
      //
      // Previous Value Cache - helps in deteremining if the current data is dirty.
      private Decimal PreviousCaloriesPerServing { get; set; }
      private Int32 PreviousNumberOfFoodGroupServings { get; set; }
      private void SetCacheValues()
      {
         PreviousCaloriesPerServing = CaloriesPerServing;
         PreviousNumberOfFoodGroupServings = FoodGroupsPerServing.Count;

         foreach (ServingViewModel<FoodGroup> servingViewModel in FoodGroupsPerServing)
         {
            servingViewModel.ResetPreviousValueCache();
         }
      }

      private void OnFoodGroupServingPropertyChanged( object seder, PropertyChangedEventArgs e )
      {
         OnPropertyChanged( FoodGroupsPerServingPropertyName );
         OnPropertyChanged( IsDirtyPropertyName );
         OnPropertyChanged( IsValidPropertyName );
      }

      private void OnFoodGroupServingsChanged( object sender, NotifyCollectionChangedEventArgs e )
      {
         FoodItem foodItem = Model as FoodItem;

         if (e.NewItems != null && e.NewItems.Count > 0)
         {
            foreach (ServingViewModel<FoodGroup> servingViewModel in e.NewItems)
            {
               servingViewModel.PropertyChanged += this.OnFoodGroupServingPropertyChanged;
               servingViewModel.UndoManager = this.UndoManager;
               foodItem.FoodGroupsPerServing.Add( servingViewModel.Model );
               AddUndoableValue(
                  new UndoablePropertyValue<FoodItemViewModel, ServingViewModel<FoodGroup>>( this, FoodGroupsPerServingPropertyName, UndoableActions.Add,
                     null, servingViewModel ) );
            }
         }

         if (e.OldItems != null && e.OldItems.Count > 0)
         {
            foreach (ServingViewModel<FoodGroup> servingViewModel in e.OldItems)
            {
               servingViewModel.PropertyChanged -= this.OnFoodGroupServingPropertyChanged;
               foodItem.FoodGroupsPerServing.Remove( servingViewModel.Model );
               AddUndoableValue(
                  new UndoablePropertyValue<FoodItemViewModel, ServingViewModel<FoodGroup>>( this, FoodGroupsPerServingPropertyName, UndoableActions.Remove,
                     servingViewModel, null ) );
            }
         }

         if (e.Action == NotifyCollectionChangedAction.Reset)
         {
            foodItem.FoodGroupsPerServing.Clear();
         }

         OnPropertyChanged( FoodGroupsPerServingPropertyName );
         OnPropertyChanged( IsDirtyPropertyName );
         OnPropertyChanged( IsValidPropertyName );
      }
      #endregion

      #region INavigationAware Members
      public override void OnNavigatedTo( NavigationContext navigationContext )
      {
         if (Model == null)
         {
            String idParameter = navigationContext.Parameters["ID"];
            Guid myID;
            FoodItem foodItem = null;

            if (idParameter != null && Guid.TryParse( idParameter, out myID ))
            {
               foodItem = this.DataRepository.GetFoodItem( myID );
            }
            else
            {
               foodItem = new FoodItem();
            }
            Model = foodItem;

            // TODO: Read up and see if there is a better way of diabling the event.
            //       Also, refactor this into a private method.
            FoodGroupsPerServing.CollectionChanged -= OnFoodGroupServingsChanged;
            FoodGroupsPerServing.Clear();
            foreach (Serving<FoodGroup> serving in foodItem.FoodGroupsPerServing)
            {
               ServingViewModel<FoodGroup> servingViewModel = new ServingViewModel<FoodGroup>( serving );
               servingViewModel.PropertyChanged += OnFoodGroupServingPropertyChanged;
               FoodGroupsPerServing.Add( servingViewModel );
            }
            FoodGroupsPerServing.CollectionChanged += OnFoodGroupServingsChanged;

            base.OnNavigatedTo( navigationContext );
         }
      }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         this.ValidFoodGroups.Dispose();
      }
      #endregion
   }
}
