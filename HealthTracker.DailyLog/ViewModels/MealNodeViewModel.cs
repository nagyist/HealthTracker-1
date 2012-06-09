using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using System.Windows.Input;
using HealthTracker.Infrastructure.Properties;
using System.Collections.ObjectModel;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.DailyLog.ViewModels
{
   public class MealNodeViewModel : DataObjectCollectionNodeViewModel
   {
      #region Constructors
      public MealNodeViewModel( IDataRepository dataRepository, ICommand childClickCommand )
         : base( DisplayStrings.DailyLogMealTitle, dataRepository, childClickCommand )
      {
         _currentDate = DateTime.Now.Date;
         _dataRepository = dataRepository;

         var meals = dataRepository.GetAllMealsForDate( _currentDate );

         _children =
            new ObservableCollection<TreeNodeViewModel>(
            (from meal in dataRepository.GetAllMealsForDate( _currentDate )
             select new ClickableTreeNodeViewModel( meal.Name, childClickCommand, meal.ID )).ToList() );
         Children = new ReadOnlyObservableCollection<TreeNodeViewModel>( _children );
      }
      #endregion

      #region Public Interface
      private DateTime _currentDate;
      public DateTime CurrentDate
      {
         get { return _currentDate; }
         set
         {
            _currentDate = value.Date;
            _children.Clear();
            foreach (var meal in _dataRepository.GetAllMealsForDate(_currentDate))
            {
               _children.Add( new ClickableTreeNodeViewModel( meal.Name, this._childClickCommand, meal.ID ) );
            }
         }
      }
      #endregion

      #region Event Handlers
      protected override void OnItemAdded( object sender, RepositoryObjectEventArgs e )
      {
         Meal dataObject = e.Item as Meal;

         if (dataObject != null && dataObject.DateAndTimeOfMeal.Date == _currentDate)
         {
            _children.Add( new ClickableTreeNodeViewModel( dataObject.Name, _childClickCommand, dataObject.ID ) );
         }
      }
      #endregion

      #region Private Data
      private IDataRepository _dataRepository;
      #endregion
   }
}
