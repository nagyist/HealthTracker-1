using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.DataRepository.ViewModels
{
   public abstract class MealViewModelBase : DataRepositoryObjectViewModel
   {
      #region Constructors
      public MealViewModelBase( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( dataRepository, regionManager, interactionService, logger )
      {
         _foodGroupServings = new ObservableCollection<ServingViewModel<FoodGroup>>();
         FoodGroupServings = new ReadOnlyObservableCollection<ServingViewModel<FoodGroup>>( _foodGroupServings );

         FoodItemServings = new ObservableCollection<ServingViewModel<FoodItem>>();
         
         ValidFoodItems = new AllFoodItemsViewModel( dataRepository );
         ValidMealTypes = new AllMealTypesViewModel( dataRepository );

         _timeSetManually = false;
      }
      #endregion

      #region Public Interface
      public override bool IsDirty
      {
         get
         {
            if (base.IsDirty ||
                TypeOfMeal != _previousTypeOfMeal ||
                TimeOfMeal != _previousTimeOfMeal ||
                FoodItemServings.Count != _previousFoodItemCount)
            {
               return true;
            }

            foreach (ServingViewModel<FoodItem> foodItemServing in FoodItemServings)
            {
               if (foodItemServing.IsDirty)
               {
                  return true;
               }
            }

            return false;
         }
      }

      public virtual MealType TypeOfMeal
      {
         get
         {
            return ((MealBase)Model).TypeOfMeal;
         }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<MealViewModelBase, MealType>( this, typeOfMealPropertyName, UndoableActions.Modify, TypeOfMeal, value ) );

            if (!_timeSetManually && value.UseDefaultMealTime)
            {
               TimeOfMeal = value.DefaultTimeOfMeal;
               _timeSetManually = false;
            }

            ((MealBase)Model).TypeOfMeal = value;
            OnPropertyChanged( typeOfMealPropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
            OnPropertyChanged( IsValidPropertyName );
         }
      }

      private Boolean _timeSetManually;
      public DateTime TimeOfMeal
      {
         get
         {
            return ((MealBase)Model).DateAndTimeOfMeal;
         }
         set
         {
            if (((MealBase)Model).DateAndTimeOfMeal != value)
            {
               AddUndoableValue(
                  new UndoablePropertyValue<MealViewModelBase, DateTime>( this, timeOfMealPropertyName, UndoableActions.Modify, TimeOfMeal, value ) );
               ((MealBase)Model).DateAndTimeOfMeal = ((MealBase)Model).DateAndTimeOfMeal.Date + value.TimeOfDay;
               OnPropertyChanged( timeOfMealPropertyName );
               OnPropertyChanged( IsDirtyPropertyName );
               OnPropertyChanged( IsValidPropertyName );

               _timeSetManually = true;
            }
         }
      }

      public ObservableCollection<ServingViewModel<FoodItem>> FoodItemServings { get; private set; }

      private ObservableCollection<ServingViewModel<FoodGroup>> _foodGroupServings;
      public ReadOnlyObservableCollection<ServingViewModel<FoodGroup>> FoodGroupServings { get; private set; }


      public Decimal Calories
      {
         get
         {
            return ((MealBase)Model).Calories;
         }
      }

      public AllFoodItemsViewModel ValidFoodItems { get; private set; }
      public AllMealTypesViewModel ValidMealTypes { get; private set; }
      #endregion

      #region Protected Virtual Methods
      protected override void Reset()
      {
         base.Reset();
         
         if (Name != null)
         {
            Title = Name;
         }

         SetCacheValues();

         foreach (var foodItemServing in this.FoodItemServings)
         {
            foodItemServing.ResetPreviousValueCache();
         }

         // This will change the IsDirty flag if it was true
         OnPropertyChanged( IsDirtyPropertyName );
      }
      #endregion

      #region IDataErrorInfo Methods
      public override String this[string propertyName]
      {
         get
         {
            String error = null;

            error = base[propertyName];
            return error;
         }
      }
      #endregion

      #region INavigationAware Members
      public override void OnNavigatedTo( NavigationContext navigationContext )
      {
         FoodItemServings.CollectionChanged -= this.OnFoodItemServingsChanged;
         FoodItemServings.Clear();
         foreach (Serving<FoodItem> foodItemServing in ((MealBase)Model).FoodItemServings)
         {
            ServingViewModel<FoodItem> foodItemServingViewModel = new ServingViewModel<FoodItem>( foodItemServing );
            foodItemServingViewModel.PropertyChanged += this.OnFoodItemServingPropertyChanged;
            FoodItemServings.Add( foodItemServingViewModel );
         }
         FoodItemServings.CollectionChanged += this.OnFoodItemServingsChanged;

         RebuildFoodGroupServingList();

         // Assume that the time is automatically set if the time of the meal matches the time on the meal type.
         if (TypeOfMeal != null)
         {
            _timeSetManually = !(TypeOfMeal.UseDefaultMealTime && (TypeOfMeal.DefaultTimeOfMeal.TimeOfDay == TimeOfMeal.TimeOfDay));
         }
         else
         {
            _timeSetManually = false;
         }

         base.OnNavigatedTo( navigationContext );
      }
      #endregion

      #region Validations
      private String typeOfMealPropertyName = "TypeOfMeal";
      private String timeOfMealPropertyName = "TimeOfMeal";
      private String caloriesPropertyName = "Calories";
      private String foodGroupServingsPropertyName = "FoodGroupServings";
      private String foodItemServingsPropertyName = "FoodItemServings";
      #endregion

      #region Private Helpers
      private void RebuildFoodGroupServingList()
      {
         _foodGroupServings.Clear();
         foreach (Serving<FoodGroup> foodGroup in ((MealBase)Model).FoodGroupServings)
         {
            _foodGroupServings.Add( new ServingViewModel<FoodGroup>( foodGroup ) );
         }

         // If the food groups needed to be rebuilt, then all of the following have (potentially) changed.
         OnPropertyChanged( foodGroupServingsPropertyName );
         OnPropertyChanged( foodItemServingsPropertyName );
         OnPropertyChanged( caloriesPropertyName );
         OnPropertyChanged( IsDirtyPropertyName );
         OnPropertyChanged( IsValidPropertyName );
      }

      private void OnFoodItemServingPropertyChanged( object sender, PropertyChangedEventArgs e )
      {
         RebuildFoodGroupServingList();
      }

      private void OnFoodItemServingsChanged( object sender, NotifyCollectionChangedEventArgs e )
      {
         MealBase mealBase = Model as MealBase;

         if (e.NewItems != null && e.NewItems.Count > 0)
         {
            foreach (ServingViewModel<FoodItem> foodItemServing in e.NewItems)
            {
               foodItemServing.PropertyChanged += this.OnFoodItemServingPropertyChanged;
               foodItemServing.UndoManager = this.UndoManager;
               mealBase.FoodItemServings.Add( foodItemServing.Model );
               AddUndoableValue(
                  new UndoablePropertyValue<MealViewModelBase, ServingViewModel<FoodItem>>(
                     this, foodItemServingsPropertyName, UndoableActions.Add, null, foodItemServing ) );
            }
         }

         if (e.OldItems != null && e.OldItems.Count > 0)
         {
            foreach (ServingViewModel<FoodItem> foodItemServing in e.OldItems)
            {
               foodItemServing.PropertyChanged -= this.OnFoodItemServingPropertyChanged;
               mealBase.FoodItemServings.Remove( foodItemServing.Model );
               AddUndoableValue(
                  new UndoablePropertyValue<MealViewModelBase, ServingViewModel<FoodItem>>(
                     this, foodItemServingsPropertyName, UndoableActions.Remove, foodItemServing, null ) );
            }
         }

         if (e.Action == NotifyCollectionChangedAction.Reset)
         {
            mealBase.FoodItemServings.Clear();
         }

         RebuildFoodGroupServingList();
      }

      // Previous value cache (used to determine IsDirty)
      private DateTime _previousTimeOfMeal;
      private MealType _previousTypeOfMeal;
      private Int32 _previousFoodItemCount;
      private void SetCacheValues()
      {
         _previousFoodItemCount = FoodItemServings.Count;
         _previousTypeOfMeal = TypeOfMeal;
         _previousTimeOfMeal = TimeOfMeal;
      }
      #endregion
   }
}
