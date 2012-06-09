using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DailyLog.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;
using HealthTracker.DataRepository.ViewModels;

namespace UnitTests.DailyLog
{
   [TestClass]
   public class MealViewModelTest
   {
      #region Private Helpers
      private MealViewModel CreateEmptyViewModel( Mock<IDataRepository> dataRepositoryMock )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         return viewModel;
      }

      private MealViewModel CreateEmptyViewModel()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
           .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         return CreateEmptyViewModel( dataRepositoryMock );
      }


      private MealViewModel CreateViewModelForMeal(
         Meal meal, Mock<IDataRepository> dataRepositoryMock, Mock<IRegionManager> regionManagerMock, Mock<IInteractionService> interactionServiceMock )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealView?ID=" + meal.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         return viewModel;
      }

      private MealViewModel CreateViewModelForMeal( Meal meal, Mock<IDataRepository> dataRepositoryMock )
      {
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         return CreateViewModelForMeal( meal, dataRepositoryMock, regionManagerMock, interactionServiceMock );
      }

      private MealViewModel CreateViewModelForMeal( Meal meal )
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         return CreateViewModelForMeal( meal, dataRepositoryMock );
      }


      private UserControl CreateViewInRegion( MealViewModel viewModel, Mock<IRegion> regionMock, Mock<IRegionManager> regionManagerMock )
      {
         // Set up region manager so we can determine if the view get removed or not.
         var view = new UserControl();
         view.DataContext = viewModel;

         var views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         var regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         return view;
      }
      #endregion

      #region Property Tests
      [TestMethod]
      public void NewMealIsNotUsed()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsFalse( viewModel.IsUsed );
      }

      [TestMethod]
      public void LoadedMealIsNotUsed()
      {
         var mockData = new MockData();
         var viewModel = CreateViewModelForMeal( mockData.Meals().Find( x => x.ID == MockData.DayOneLunchID ) );

         Assert.IsFalse( viewModel.IsUsed );
      }

      [TestMethod]
      public void NewMealIsNotDirty()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void NewMealIsDirtyAfterChange()
      {
         var viewModel = CreateEmptyViewModel();

         viewModel.Name = "Test";

         Assert.IsTrue( viewModel.IsDirty );
      }

      [TestMethod]
      public void ExistingMealIsNotDirty()
      {
         var data = new MockData();
         var viewModel = CreateViewModelForMeal( data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID ) );

         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void ExistingMealIsDirtyAfterChange()
      {
         var data = new MockData();
         var viewModel = CreateViewModelForMeal( data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID ) );

         viewModel.Description += "Mod";

         Assert.IsTrue( viewModel.IsDirty );
      }

      [TestMethod]
      public void TitleIsDefaultForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.AreEqual( DisplayStrings.NewMealTitle, viewModel.Title );
      }

      [TestMethod]
      public void TitleNewMealChangedToNameWhenSaved()
      {
         var viewModel = CreateEmptyViewModel();

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         viewModel.TimeOfMeal = DateTime.Now;
         viewModel.Name = "A New Meal";
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "testitem", "", 50 ), 2 ) );

         Assert.AreEqual( DisplayStrings.NewMealTitle, viewModel.Title );
         Assert.IsTrue( viewModel.SaveCommand.CanExecute( null ) );
         viewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "A New Meal", viewModel.Title );
      }

      [TestMethod]
      public void TitleIsNameForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayOneLunchID );
         var viewModel = CreateViewModelForMeal( meal );

         Assert.AreEqual( meal.Name, viewModel.Title );
      }

      [TestMethod]
      public void TitleForExistingMealDoesNotChangeUntilMealSaved()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayOneLunchID );
         var origMealName = meal.Name;
         var viewModel = CreateViewModelForMeal( meal );

         viewModel.Name += "Test";
         Assert.AreEqual( origMealName + "Test", viewModel.Name );
         Assert.AreEqual( origMealName, viewModel.Title );
         Assert.IsTrue( viewModel.SaveCommand.CanExecute( null ) );
         viewModel.SaveCommand.Execute( null );
         Assert.AreEqual( origMealName + "Test", viewModel.Title );
      }

      [TestMethod]
      public void NameNullForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Name ) );
      }

      [TestMethod]
      public void NameDefaultsToMealTypeIfNameNull()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Name ) );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Fourth Meal", "", DateTime.Now, false );
         Assert.AreEqual( "Fourth Meal", viewModel.Name );
      }

      [TestMethod]
      public void NameNotChangedToMealTypeIfNameNotNull()
      {
         var viewModel = CreateEmptyViewModel();

         viewModel.Name = "Some Name";
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Fourth Meal", "", DateTime.Now, false );

         Assert.AreEqual( "Some Name", viewModel.Name );
      }

      [TestMethod]
      public void NameLoadsCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );
         var viewModel = CreateViewModelForMeal( meal );

         Assert.AreEqual( meal.Name, viewModel.Name );
      }

      [TestMethod]
      public void DescriptionNullForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Description ) );
      }

      [TestMethod]
      public void DescriptionLoadsCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( meal );

         Assert.AreEqual( meal.Description, viewModel.Description );
      }

      [TestMethod]
      public void TypeOfMealNullForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsNull( viewModel.TypeOfMeal );
      }

      [TestMethod]
      public void TypeOfMealLoadsCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var allMeals = data.Meals();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( meal );

         Assert.AreEqual( meal.TypeOfMeal.ID, viewModel.TypeOfMeal.ID );
         Assert.AreEqual( meal.TypeOfMeal.Name, viewModel.TypeOfMeal.Name );
      }

      [TestMethod]
      public void SettingTimeOfMealDoesNotChangeDate()
      {
         var viewModel = CreateEmptyViewModel();

         DateTime dateOfMealBeforeChange = viewModel.DateOfMeal;
         viewModel.TimeOfMeal = DateTime.Now.AddDays( 13 ).AddMinutes( 14 );
         Assert.AreEqual( dateOfMealBeforeChange, viewModel.DateOfMeal );
      }

      [TestMethod]
      public void SettingDateOfMealDoesNotChangeTime()
      {
         var viewModel = CreateEmptyViewModel();

         DateTime timeOfMealBeforeChange = viewModel.TimeOfMeal;
         viewModel.DateOfMeal = DateTime.Now.AddDays( 13 ).AddMinutes( 14 );
         Assert.AreEqual( timeOfMealBeforeChange.TimeOfDay, viewModel.TimeOfMeal.TimeOfDay );
      }

      [TestMethod]
      public void TimeOfMealNowDateTodayForNewMeal()
      {
         DateTime startOfTestTime = DateTime.Now;
         var viewModel = CreateEmptyViewModel();
         DateTime endOfTestTime = DateTime.Now;

         Assert.IsTrue( viewModel.TimeOfMeal >= startOfTestTime );
         Assert.IsTrue( viewModel.TimeOfMeal <= endOfTestTime );
         Assert.AreEqual( DateTime.Today, viewModel.DateOfMeal );
      }

      [TestMethod]
      public void TimeOfMealSetBasedOnMealTypeIfFlagSetAndTimeNotManuallySet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), true );

         var targetTime = currentTime.Date + currentTime.AddHours( 5.5 ).TimeOfDay;
         Assert.AreEqual( targetTime, viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeOfMealNotSetBasedOnMealTypeIfFlagSetAndTimeManuallySet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TimeOfMeal = currentTime.AddHours( 1.75 );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), true );

         var targetTime = currentTime.Date + currentTime.AddHours( 1.75 ).TimeOfDay;
         Assert.AreEqual( targetTime, viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeOfMealNotSetBasedOnMealTypeIfFlagNotSetAndTimeNotManuallySet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), false );

         Assert.AreEqual( currentTime, viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeOfMealSetBasedOnNewMealTypeIfFlagSetAndTimeMatchesOldMealTypeWithFlagSet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), true );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Lunch", "Mid-day", currentTime.AddDays( -3 ).AddHours( 12.25 ), true );

         var targetTime = currentTime.Date + currentTime.AddHours( 12.25 ).TimeOfDay;
         Assert.AreEqual( targetTime, viewModel.TimeOfMeal );
      }


      [TestMethod]
      public void TimeOfMealNotSetBasedOnNewMealTypeIfFlagNotSetAndTimeMatchesOldMealTypeWithFlagSet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), true );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Lunch", "Mid-day", currentTime.AddDays( -2 ).AddHours( 12.25 ), false );

         var targetTime = currentTime.Date + currentTime.AddHours( 5.5 ).TimeOfDay;
         Assert.AreEqual( targetTime, viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeOfMealNotSetBasedOnNewMealTypeIfFlagSetAndTimeMatchesOldMealTypeWithFlagNotSet()
      {
         var viewModel = CreateEmptyViewModel();
         var currentTime = viewModel.TimeOfMeal;

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Meal Type", "Test Meal Type", currentTime.AddDays( -1 ).AddHours( 5.5 ), false );
         viewModel.TimeOfMeal = currentTime.AddHours( 5.5 );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Lunch", "Mid-day", currentTime.AddDays( -4 ).AddHours( 12.25 ), true );

         var targetTime = currentTime.Date + currentTime.AddHours( 5.5 ).TimeOfDay;
         Assert.AreEqual( targetTime, viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeAndDateOfMealLoadsCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.DateAndTimeOfMeal, viewModel.TimeOfMeal );
         Assert.AreEqual( meal.DateAndTimeOfMeal.Date, viewModel.DateOfMeal );
      }

      [TestMethod]
      public void TimeOfMealForExistingMealResetBasedOnMealTypeIfTimeNeverManuallySet()
      {
         double mealHour = 12.5;
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Today.AddDays( -42 ).AddHours( mealHour ), true );
         var meal = new Meal( Guid.NewGuid(), mealType, DateTime.Today.AddHours( mealHour ), "test", "test" );
         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Another Test", "d00d", DateTime.Today.AddDays( -345 ).AddHours( mealHour + 2.0 ), true );

         Assert.AreEqual( DateTime.Today.AddHours( mealHour + 2.0 ), viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void TimeOfMealForExistingMealNotResetIfTimeWasManuallySet()
      {
         double mealHour = 12.5;
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Today.AddDays( -42 ).AddHours( mealHour + 1.0 ), true );
         var meal = new Meal( Guid.NewGuid(), mealType, DateTime.Today.AddHours( mealHour ), "test", "test" );
         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         viewModel.TimeOfMeal = DateTime.Today.AddHours( mealHour + 2.0 );
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Another Test", "d00d", DateTime.Today.AddDays( -345 ).AddHours( mealHour + 2.0 ), true );

         Assert.AreEqual( DateTime.Today.AddHours( mealHour + 2.0 ), viewModel.TimeOfMeal );
      }

      [TestMethod]
      public void FoodItemServingsEmptyForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.AreEqual( 0, viewModel.FoodItemServings.Count );
      }

      [TestMethod]
      public void FoodItemServingsLoadsCorrectForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.FoodItemServings.Count, viewModel.FoodItemServings.Count );
         foreach (var foodItem in viewModel.FoodItemServings)
         {
            var mealFoodItem = meal.FoodItemServings.Find( x => x.Entity.ID == foodItem.Entity.ID );
            Assert.IsNotNull( mealFoodItem );
            Assert.AreEqual( mealFoodItem.Quantity, foodItem.Quantity );
         }
      }

      [TestMethod]
      public void FoodGroupServingsEmptyForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.AreEqual( 0, viewModel.FoodGroupServings.Count );
      }

      [TestMethod]
      public void FoodGroupServingsLoadCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.FoodGroupServings.Count, viewModel.FoodGroupServings.Count );
         foreach (var foodGroup in viewModel.FoodGroupServings)
         {
            var mealFoodGroup = meal.FoodGroupServings.Find( x => x.Entity.ID == foodGroup.Entity.ID );
            Assert.IsNotNull( mealFoodGroup );
            Assert.AreEqual( mealFoodGroup.Quantity, foodGroup.Quantity );
         }
      }

      [TestMethod]
      public void FoodGroupServingsAdjustedAsFoodItemsAdded()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         var foodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "", 1 );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test Food Group", "" ), 1 ) );
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( foodItem, 2 ) );

         Assert.AreEqual( meal.FoodGroupServings.Count + 1, viewModel.FoodGroupServings.Count );
      }

      [TestMethod]
      public void CaloriesZeroForNewMeal()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.AreEqual( 0, viewModel.Calories );
      }

      [TestMethod]
      public void CaloriesLoadCorrectlyForExistingMeal()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.Calories, viewModel.Calories );
      }

      [TestMethod]
      public void CaloriesAdjustedAsFoodItemsAdded()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.Calories, viewModel.Calories );
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "Test", "", 42.5M ), 3 ) );
         Assert.AreEqual( meal.Calories + 127.5M, viewModel.Calories );
      }

      [TestMethod]
      public void CaloriesAdjustedAsFoodItemsModified()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.Calories, viewModel.Calories );
         var foodItem = viewModel.FoodItemServings[0];
         var foodItemCalories = foodItem.Entity.CaloriesPerServing * foodItem.Quantity;
         foodItem.Quantity *= 2;
         Assert.AreEqual( meal.Calories + foodItemCalories, viewModel.Calories );
      }

      [TestMethod]
      public void CaloriesAdjustedAsFoodItemsRemoved()
      {
         var data = new MockData();
         var meal = data.Meals().Find( x => x.ID == MockData.DayTwoBreakfastID );

         var viewModel = CreateViewModelForMeal( new Meal( meal ) );

         Assert.AreEqual( meal.Calories, viewModel.Calories );
         var foodItem = viewModel.FoodItemServings[0];
         var foodItemCalories = foodItem.Entity.CaloriesPerServing * foodItem.Quantity;
         viewModel.FoodItemServings.RemoveAt( 0 );
         Assert.AreEqual( meal.Calories - foodItemCalories, viewModel.Calories );
      }

      [TestMethod]
      public void ValidFoodItemsContainsAllFoodItems()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         var allFoodItems = new List<FoodItem>();
         allFoodItems.Add( new FoodItem( Guid.NewGuid(), "Item1", "", 42 ) );
         allFoodItems.Add( new FoodItem( Guid.NewGuid(), "Item2", "", 52 ) );
         allFoodItems.Add( new FoodItem( Guid.NewGuid(), "Item3", "", 63 ) );
         allFoodItems.Add( new FoodItem( Guid.NewGuid(), "Item4", "", 78 ) );
         allFoodItems.Add( new FoodItem( Guid.NewGuid(), "Item5", "", 39 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( allFoodItems ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         Assert.AreEqual( allFoodItems.Count, viewModel.ValidFoodItems.Items.Count );
         foreach (var foodItem in viewModel.ValidFoodItems.Items)
         {
            Assert.IsNotNull( allFoodItems.Find( x => x.Name == foodItem.Name ) );
         }
         dataRepositoryMock.VerifyAll();
      }

      [TestMethod]
      public void ValidMealTypesContainsAllMealTypes()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         var allMealTypes = new List<MealType>();
         allMealTypes.Add( new MealType( Guid.NewGuid(), "Type1", "", DateTime.Now, false ) );
         allMealTypes.Add( new MealType( Guid.NewGuid(), "Type2", "", DateTime.Now, false ) );
         allMealTypes.Add( new MealType( Guid.NewGuid(), "Type3", "", DateTime.Now, false ) );
         allMealTypes.Add( new MealType( Guid.NewGuid(), "Type4", "", DateTime.Now, false ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( allMealTypes ) );

         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         Assert.AreEqual( allMealTypes.Count, viewModel.ValidMealTypes.Items.Count );
         foreach (var mealType in viewModel.ValidMealTypes.Items)
         {
            Assert.IsNotNull( allMealTypes.Find( x => x.Name == mealType.Name ) );
         }
         dataRepositoryMock.VerifyAll();
      }

      [TestMethod]
      public void ChangingMealTypeRaisesPropertyChangedNotIncludingTimeIfTimeWillNotChange()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "test", "", DateTime.Now, false );

         Assert.AreEqual( 4, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "TypeOfMeal" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingMealTypeRaisesPropertyChangedIncludingTimeIfTimeWillChange()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "test", "", DateTime.Now.AddHours( 1.25 ), true );

         Assert.AreEqual( 5, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "TypeOfMeal" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "TimeOfMeal" ) );
      }

      [TestMethod]
      public void ChangingNameRaisesPropertyChanged()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.Name = "This is a test";

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingDateRaisesPropertyChanged()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.TimeOfMeal = DateTime.Now.AddDays( 0.001 );

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "TimeOfMeal" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingTimeRaisesPropertyChanged()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.TimeOfMeal = DateTime.Now.AddDays( 2.5 );

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "TimeOfMeal" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingFoodItemsRaisesPropertyChanged()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "", 42 ), 2 ) );

         Assert.AreEqual( 5, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodItemServings" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupServings" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Calories" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingDescriptionRaisesPropertyChanged()
      {
         var propertyChangedHandler = new PropertyChangedHandler();

         var viewModel = CreateEmptyViewModel();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.Description = "This is a test";

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Description" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
      }
      #endregion

      #region Command Tests
      [TestMethod]
      public void SaveCalledForValidNewMeal()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( false );

         var mealViewModel = CreateEmptyViewModel( dataRepositoryMock );

         mealViewModel.Name = "New Meal";
         mealViewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Test Type", "", DateTime.Now, false );
         mealViewModel.TimeOfMeal = DateTime.Now;
         mealViewModel.FoodItemServings.Add(
            new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );
         Assert.IsTrue( mealViewModel.IsValid );
         Assert.IsTrue( mealViewModel.IsNew );

         mealViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidNewMeal()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( false );

         var mealViewModel = CreateEmptyViewModel( dataRepositoryMock );

         mealViewModel.Name = "New Meal";
         mealViewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Test Type", "", DateTime.Now, false );
         mealViewModel.TimeOfMeal = DateTime.Now;
         Assert.IsFalse( mealViewModel.IsValid );
         Assert.IsTrue( mealViewModel.IsNew );

         mealViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveCalledForValidChangedMeal()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Meal", "Test Meal" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( true );

         var mealViewModel = CreateViewModelForMeal( new Meal( meal ), dataRepositoryMock );
         mealViewModel.Description = "Something New";

         Assert.IsTrue( mealViewModel.IsValid );
         Assert.IsTrue( mealViewModel.IsDirty );
         Assert.IsFalse( mealViewModel.IsNew );

         mealViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidChangedMeal()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Meal", "Test Meal" );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( true );

         var mealViewModel = CreateViewModelForMeal( new Meal( meal ), dataRepositoryMock );
         mealViewModel.Description = "Something New";

         Assert.IsFalse( mealViewModel.IsValid );
         Assert.IsTrue( mealViewModel.IsDirty );
         Assert.IsFalse( mealViewModel.IsNew );

         mealViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveNotCalledForNonChangedMeal()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Meal", "Test Meal" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( true );

         var mealViewModel = CreateViewModelForMeal( new Meal( meal ), dataRepositoryMock );

         Assert.IsTrue( mealViewModel.IsValid );
         Assert.IsFalse( mealViewModel.IsDirty );
         Assert.IsFalse( mealViewModel.IsNew );

         mealViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Never() );
      }


      [TestMethod]
      public void CannotSaveAfterFoodItemAdditionSaved()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         var foodGroupOne = new FoodGroup( Guid.NewGuid(), "Food Group One", "" );
         var foodGroupTwo = new FoodGroup( Guid.NewGuid(), "Food Group Two", "" );

         var foodItemOne = new FoodItem( Guid.NewGuid(), "Food Item #1", "", 100 );
         foodItemOne.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupOne, 1.5M ) );

         var foodItemTwo = new FoodItem( Guid.NewGuid(), "Food Item #2", "", 150 );
         foodItemTwo.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupTwo, 2.25M ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( foodItemOne, 1 ) );

         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );

         var viewModel = CreateViewModelForMeal( meal, dataRepositoryMock );

         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( foodItemTwo, 2 ) );
         Assert.IsTrue( viewModel.IsDirty );
         Assert.IsTrue( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.SaveCommand.Execute( null );
         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );
      }

      [TestMethod]
      public void CannotSaveAfterFoodItemDeletionSaved()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         var foodGroupOne = new FoodGroup( Guid.NewGuid(), "Food Group One", "" );
         var foodGroupTwo = new FoodGroup( Guid.NewGuid(), "Food Group Two", "" );

         var foodItemOne = new FoodItem( Guid.NewGuid(), "Food Item #1", "", 100 );
         foodItemOne.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupOne, 1.5M ) );

         var foodItemTwo = new FoodItem( Guid.NewGuid(), "Food Item #2", "", 150 );
         foodItemTwo.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupTwo, 2.25M ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( foodItemOne, 1 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( foodItemTwo, 2 ) );

         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );

         var viewModel = CreateViewModelForMeal( meal, dataRepositoryMock );

         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.FoodItemServings.RemoveAt( 0 );
         Assert.IsTrue( viewModel.IsDirty );
         Assert.IsTrue( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.SaveCommand.Execute( null );
         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );
      }

      [TestMethod]
      public void CannotSaveAfterFoodItemModificationSaved()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );

         var foodGroupOne = new FoodGroup( Guid.NewGuid(), "Food Group One", "" );
         var foodGroupTwo = new FoodGroup( Guid.NewGuid(), "Food Group Two", "" );

         var foodItemOne = new FoodItem( Guid.NewGuid(), "Food Item #1", "", 100 );
         foodItemOne.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupOne, 1.5M ) );

         var foodItemTwo = new FoodItem( Guid.NewGuid(), "Food Item #2", "", 150 );
         foodItemTwo.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupTwo, 2.25M ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( foodItemOne, 1 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( foodItemTwo, 2 ) );

         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );

         var viewModel = CreateViewModelForMeal( meal, dataRepositoryMock );

         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.FoodItemServings[0].Quantity += 1.5M;
         Assert.IsTrue( viewModel.IsDirty );
         Assert.IsTrue( viewModel.SaveCommand.CanExecute( null ) );

         viewModel.SaveCommand.Execute( null );
         Assert.IsFalse( viewModel.IsDirty );
         Assert.IsFalse( viewModel.SaveCommand.CanExecute( null ) );
      }


      [TestMethod]
      public void CannotDeleteMealIfNew()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<Meal>() ) ).Returns( false );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = CreateEmptyViewModel( dataRepositoryMock );
         viewModel.Name = "test";
         viewModel.Description = "This is a test";
         viewModel.TimeOfMeal = DateTime.Now;
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false );
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 90.0M ), 1.25M ) );

         // Setup the regions so we can determine if the view has been removed or not
         var regionMock = new Mock<IRegion>();
         var regionManagerMock = new Mock<IRegionManager>();
         var view = CreateViewInRegion( viewModel, regionMock, regionManagerMock );

         Assert.IsTrue( viewModel.IsNew );
         Assert.IsFalse( viewModel.IsUsed );
         Assert.IsTrue( viewModel.IsValid );
         Assert.IsFalse( viewModel.DeleteCommand.CanExecute( null ) );

         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }

      [TestMethod]
      public void DeleteMealIfAnswerIsYes()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false ), DateTime.Now, "test", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 100.0M ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = CreateViewModelForMeal( meal, dataRepositoryMock, regionManagerMock, interactionServiceMock );

         // Setup the regions so we can determine if the view has been removed or not
         var regionMock = new Mock<IRegion>();
         var view = CreateViewInRegion( viewModel, regionMock, regionManagerMock );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox(
               Messages.Question_Meal_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.Yes );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( meal ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Exactly( 1 ) );
      }

      [TestMethod]
      public void DeleteMealNotPerformedIfAnswerIsNo()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false ), DateTime.Now, "test", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 100.0M ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = CreateViewModelForMeal( meal, dataRepositoryMock, regionManagerMock, interactionServiceMock );

         // Setup the regions so we can determine if the view has been removed or not
         var regionMock = new Mock<IRegion>();
         var view = CreateViewInRegion( viewModel, regionMock, regionManagerMock );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox(
               Messages.Question_Meal_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.No );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( meal ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }


      [TestMethod]
      public void UndoRedoMealViewModel()
      {
         // Create a meal and then navigate to it to bring it into the view model
         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Bob", "Battery Operated Buddy" );
         var glassOfWater = new FoodItem( Guid.NewGuid(), "Glass of Water", "", 0 );
         glassOfWater.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Water", "" ), 1 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( glassOfWater, 1.0M ) );

         var mealViewModel = CreateViewModelForMeal( meal );

         Assert.IsFalse( mealViewModel.IsDirty );
         Assert.IsFalse( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );

         // Make changes as such:
         //   o name changed from Bob to Pete
         //   o name changed from Pete to Peter
         //   o Description changed from "Battery Operated Buddy" to "The Rock"
         //   o name changed from Peter to Simon
         //   o name changed from Simon to Saul
         //   o description changed from "The Rock" to "The Persecutor"
         //   o description changed from "The Persecutor" to "The Apostle"
         //   o name changed from Saul to Paul
         // Verify can undo, cannot redo at each step
         mealViewModel.Name = "Pete";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Name += "r";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Description = "The Rock";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Name = "Simon";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Name = "Saul";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Description = "The Persecutor";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Description = "The Apostle";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.Name = "Paul";
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );

         Assert.AreEqual( "Paul", mealViewModel.Name );
         Assert.AreEqual( "The Apostle", mealViewModel.Description );

         // Undo once.  Verify last thing done is undone, and we can redo.
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Apostle", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );

         // Redo.  Verify last thing undone is redone, can no longer redo, can still undo.
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Paul", mealViewModel.Name );
         Assert.AreEqual( "The Apostle", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealViewModel.RedoCommand.CanExecute( null ) );

         // Undo 4 times, verify undo worked
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Apostle", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Persecutor", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );

         // Redo 2 times, verify
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Persecutor", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );

         // Undo 6 times.  Back to original, cannot undo, can redo
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", mealViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Pete", mealViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Bob", mealViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", mealViewModel.Description );
         Assert.IsFalse( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );

         // Redo 3 times, verify
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Pete", mealViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", mealViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
         mealViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", mealViewModel.Name );
         Assert.AreEqual( "The Rock", mealViewModel.Description );
         Assert.IsTrue( mealViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( mealViewModel.RedoCommand.CanExecute( null ) );
      }

      /// <summary>
      /// Close test meal.  All of the close tests follow this based pattern, so they all call this rather than repeating everything
      /// </summary>
      private void RunCloseTest( Boolean makeDirty, Boolean makeInvalid, MessageBoxResult messageResponse, Boolean expectRemove, Boolean expectSave )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
         Mock<IRegionNavigationService> regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         Mock<IRegionManager> regionManagerMock = new Mock<IRegionManager>();
         Mock<IRegion> regionWithoutViewMock = new Mock<IRegion>();
         Mock<IRegion> regionMock = new Mock<IRegion>();
         Mock<IInteractionService> interactionServiceMock = new Mock<IInteractionService>();

         // Set up the food group list in the data repository mock.
         FoodGroup testFoodGroup = new FoodGroup( Guid.NewGuid(), "test", "The only food group in the mock data repository" );
         List<FoodGroup> foodGroups = new List<FoodGroup>();
         foodGroups.Add( testFoodGroup );
         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() ).Returns( new ReadOnlyCollection<FoodGroup>( foodGroups ) );
         FoodItem testFoodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "Test Food Item Description", 42.0M );
         testFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( testFoodGroup, 5.0M ) );
         Assert.IsTrue( testFoodItem.IsValid );
         List<FoodItem> foodItems = new List<FoodItem>();
         foodItems.Add( testFoodItem );
         dataRepositoryMock.Setup( x => x.GetAllFoodItems() ).Returns( new ReadOnlyCollection<FoodItem>( foodItems ) );
         MealType testMealType = new MealType( Guid.NewGuid(), "Lunch", "A lunch for testing", DateTime.Now, false );
         Assert.IsTrue( testMealType.IsValid );
         List<MealType> mealTypes = new List<MealType>();
         mealTypes.Add( testMealType );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() ).Returns( new ReadOnlyCollection<MealType>( mealTypes ) );

         var meal = new Meal( Guid.NewGuid(), testMealType, DateTime.Now, "test meal", "This is a test" );
         meal.FoodItemServings.Add( new Serving<FoodItem>( testFoodItem, 1.5M ) );

         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );
         dataRepositoryMock.Setup( x => x.Contains( meal ) ).Returns( true );

         // Create the view model under test and associate it with a view
         var viewModel = new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         UserControl view = new UserControl();
         view.DataContext = viewModel;

         // Set up two regions each with their own set of views.
         List<UserControl> views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         List<UserControl> viewsWithoutView = new List<UserControl>();
         viewsWithoutView.Add( new UserControl() );
         viewsWithoutView.Add( new UserControl() );

         List<IRegion> regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionWithoutViewMock.Setup( x => x.Views.GetEnumerator() ).Returns( viewsWithoutView.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         // Navigate to the view that "displays" our food foodGroup.  This loads the view model
         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView?ID=" + meal.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         if (makeDirty)
         {
            if (makeInvalid)
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_Meal_Close, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsFalse( viewModel.IsValid );
            }
            else
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_Meal_Save, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "Something Else";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsTrue( viewModel.IsValid );
            }
         }
         else
         {
            // This will fail if we have passed in the non-sensical makeDirty == false, makeInvalid == true
            Assert.AreEqual( makeDirty, viewModel.IsDirty );
         }

         // Attempt a close.
         viewModel.CloseCommand.Execute( null );

         // If we were dirty, then we need to verify that the correct interaction was done, otherwise, that no interaction was done
         if (makeDirty)
         {
            interactionServiceMock.VerifyAll();
         }
         else
         {
            interactionServiceMock.Verify(
               x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ), Times.Never() );
         }

         if (expectRemove)
         {
            regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
            regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Exactly( 1 ) );
         }
         else
         {
            regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Never() );
         }

         if (expectSave)
         {
            dataRepositoryMock.Verify( x => x.SaveItem( meal ), Times.Exactly( 1 ) );
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Exactly( 1 ) );
         }
         else
         {
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<Meal>() ), Times.Never() );
         }
      }

      [TestMethod]
      public void CloseDirtyMealAnswerCancelNotClosed()
      {
         RunCloseTest( true, false, MessageBoxResult.Cancel, false, false );
      }

      [TestMethod]
      public void CloseDirtyMealAnswerNoNotClosed()
      {
         RunCloseTest( true, false, MessageBoxResult.No, true, false );
      }

      [TestMethod]
      public void CloseDirtyMealAnswerYesIsClosed()
      {
         RunCloseTest( true, false, MessageBoxResult.Yes, true, true );
      }

      [TestMethod]
      public void CloseInvalidMealAnswerNoNotClosed()
      {
         RunCloseTest( true, true, MessageBoxResult.No, false, false );
      }

      [TestMethod]
      public void CloseInvalidMealAnswerYesIsClosed()
      {
         RunCloseTest( true, true, MessageBoxResult.Yes, true, false );
      }

      [TestMethod]
      public void CloseMealRemovesCleanView()
      {
         RunCloseTest( false, false, MessageBoxResult.None, true, false );
      }
      #endregion

      #region Navigation Tests
      [TestMethod]
      public void NavigateToExistingMealLoadsMeal()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test", "This is a test meal" );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object,
               new Uri( "DoesItMatter?ID=" + meal.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( meal.ID, viewModel.ID );
         Assert.AreEqual( "Test", viewModel.Name );
         Assert.AreEqual( "This is a test meal", viewModel.Description );
         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void NavigateToNewMealCreatesEmptyMeal()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test", "This is a test meal" );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreNotEqual( meal.ID, viewModel.ID );
         Assert.IsNull( viewModel.Name );
         Assert.IsNull( viewModel.Description );
         Assert.AreEqual( 0, viewModel.FoodItemServings.Count );
         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void IsNavigationTargetTrueWhenIDMatches()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test", "This is a test meal" );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var query = new UriQuery();
         query.Add( "ID", meal.ID.ToString() );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView" + query.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         Assert.IsTrue( viewModel.IsNavigationTarget( navigationContext ) );
      }

      [TestMethod]
      public void IsNavigationTargetFalseWhenIDIsNull()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test", "This is a test meal" );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var query = new UriQuery();
         query.Add( "ID", meal.ID.ToString() );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView" + query.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView", UriKind.Relative ) );
         Assert.IsFalse( viewModel.IsNavigationTarget( navigationContext ) );
      }

      [TestMethod]
      public void IsNavigationTargetFalseWhenIDsDoNotMatch()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var dataRepositoryMock = new Mock<IDataRepository>();

         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var meal = new Meal( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false ), DateTime.Now, "Test", "This is a test meal" );
         dataRepositoryMock.Setup( x => x.GetMeal( meal.ID ) ).Returns( meal );

         var viewModel =
            new MealViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var query = new UriQuery();
         query.Add( "ID", meal.ID.ToString() );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView" + query.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         query = new UriQuery();
         query.Add( "ID", Guid.NewGuid().ToString() );
         navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealView" + query.ToString(), UriKind.Relative ) );
         Assert.IsFalse( viewModel.IsNavigationTarget( navigationContext ) );
      }
      #endregion
   }
}
