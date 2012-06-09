using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using HealthTracker.Infrastructure;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;
using HealthTracker.DataRepository.ViewModels;

namespace HealthTracker.DailyLog.ViewModels
{
   [Export]
   public class DailyLogViewModel : WorkspaceViewModel
   {
      #region Constructors
      [ImportingConstructor]
      public DailyLogViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( regionManager, interactionService, logger )
      {
         _dataRepository = dataRepository;

         _dataRepository.ItemAdded += this.OnRepositoryChanged;
         _dataRepository.ItemDeleted += this.OnRepositoryChanged;
         _dataRepository.ItemModified += this.OnRepositoryChanged;

         _showMealCommand = new RelayCommand( ShowMeal );

         _dailyLogNodes = new ObservableCollection<TreeNodeViewModel>();
         this.DailyLogNodes = new ReadOnlyObservableCollection<TreeNodeViewModel>( _dailyLogNodes );
         _dailyLogNodes.Add( new MealNodeViewModel( dataRepository, this.ShowMealCommand ) );

         CurrentDate = DateTime.Now.Date;
         BuildFoodGroups();
      }
      #endregion

      #region Public Interface
      private DateTime _currentDate;
      public DateTime CurrentDate
      {
         get
         {
            return _currentDate;
         }

         set
         {
            _currentDate = value;
            MealNodeViewModel mealNode = _dailyLogNodes.First( x => x.GetType() == typeof( MealNodeViewModel ) ) as MealNodeViewModel;
            if (mealNode != null)
            {
               mealNode.CurrentDate = value;
               BuildFoodGroups();
            }

            this.OnPropertyChanged( "CurrentDate" );
            this.OnPropertyChanged( "Calories" );
            this.OnPropertyChanged( "FoodGroupServings" );
         }
      }

      private ObservableCollection<TreeNodeViewModel> _dailyLogNodes;
      public ReadOnlyObservableCollection<TreeNodeViewModel> DailyLogNodes { get; private set; }

      private ReadOnlyObservableCollection<ServingViewModel<FoodGroup>> _foodGroupServings;
      public ReadOnlyObservableCollection<ServingViewModel<FoodGroup>> FoodGroupServings
      { 
         get
         {
            return _foodGroupServings;
         }  
      }

      public Decimal Calories
      {
         get
         {
            var meals = new List<Meal>( _dataRepository.GetAllMealsForDate( _currentDate ) );

            return (from meal in meals select meal.Calories).Sum();
         }
      }
      #endregion

      #region Commands
      private RelayCommand _showMealCommand;
      public ICommand ShowMealCommand
      {
         get { return _showMealCommand; }
      }

      private void ShowMeal( Object id )
      {
         NavigateToView( ViewNames.MealView, id );
      }

      private void NavigateToView( String viewName, Object id )
      {
         UriQuery query = new UriQuery();
         if (id != null)
         {
            query.Add( "ID", ((Guid)id).ToString() );
         }

         Uri navigateTo = new Uri( viewName + query.ToString(), UriKind.Relative );
         this.RegionManager.RequestNavigate( RegionNames.DailyLogTabsRegion, navigateTo );
      }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         _dataRepository.ItemAdded -= this.OnRepositoryChanged;
         _dataRepository.ItemDeleted -= this.OnRepositoryChanged;
         _dataRepository.ItemModified -= this.OnRepositoryChanged;
      }
      #endregion

      #region Private
      private IDataRepository _dataRepository;

      private void OnRepositoryChanged( object sender, RepositoryObjectEventArgs e )
      {
         Meal meal = e.Item as Meal;

         if (meal != null && meal.DateAndTimeOfMeal.Date == CurrentDate)
         {
            BuildFoodGroups();
            this.OnPropertyChanged( "Calories" );
            this.OnPropertyChanged( "FoodGroupServings" );
         }
      }

      private void BuildFoodGroups()
      {
         List<Serving<FoodGroup>> allFoodGroupServings = new List<Serving<FoodGroup>>();

         var meals = new List<Meal>( _dataRepository.GetAllMealsForDate( CurrentDate ) );

         foreach (var meal in meals)
         {
            allFoodGroupServings.AddRange( meal.FoodGroupServings );
         }

         // Aggregate the list created above
         List<ServingViewModel<FoodGroup>> aggregatedFoodGroups =
            (from foodGroupServing in allFoodGroupServings
             group foodGroupServing by foodGroupServing.Entity into fgs
             select new ServingViewModel<FoodGroup>( fgs.Key, fgs.Sum( q => q.Quantity ) )).ToList();

         _foodGroupServings = new ReadOnlyObservableCollection<ServingViewModel<FoodGroup>>(
           new ObservableCollection<ServingViewModel<FoodGroup>>( aggregatedFoodGroups ) );
      }
      #endregion
   }
}
