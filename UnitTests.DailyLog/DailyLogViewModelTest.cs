using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.DailyLog.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HealthTracker.DataRepository.Models;
using System.Collections.ObjectModel;
using HealthTracker.Infrastructure;
using Microsoft.Practices.Prism;
using UnitTests.Support;
using Microsoft.Practices.Prism.Logging;

namespace UnitTests.DailyLog
{
   [TestClass]
   public class DailyLogViewModelTest
   {
      #region Constructor Tests
      [TestMethod]
      public void DateDefaultsToToday()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                                     where meal.DateAndTimeOfMeal.Date == DateTime.Today
                                                     select meal).ToList() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.AreEqual( DateTime.Now.Date, viewModel.CurrentDate );
      }
      #endregion

      #region Date Handling Tests
      [TestMethod]
      public void DailyLogNodesReflectCurrentDate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                                     where meal.DateAndTimeOfMeal.Date == DateTime.Today
                                                     select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals ) ) )
            .Returns( new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                                     where meal.DateAndTimeOfMeal.Date == DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals )
                                                     select meal).ToList() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.AreEqual( 3, viewModel.DailyLogNodes[0].Children.Count );

         viewModel.CurrentDate = viewModel.CurrentDate.AddDays( MockData.DaysToAddForFutureMeals );
         Assert.AreEqual( 2, viewModel.DailyLogNodes[0].Children.Count );
      }

      [TestMethod]
      public void CaloriesAreCalculatedBasedOnSelectedDay()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Today
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals );

         Assert.AreEqual( 1425.5M, viewModel.Calories );

         viewModel.CurrentDate = DateTime.Today;
         Assert.AreEqual( 2357.5M, viewModel.Calories );

         dataRepositoryMock.VerifyAll();
      }

      [TestMethod]
      public void FoodGroupsAreCalculatedBasedOnSelectedDay()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Today
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in fullMealList
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;

         var foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 5, foodGroupServings.Count );
         var serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         viewModel.CurrentDate = DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals );
         foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 5, foodGroupServings.Count );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 1.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.0M, serving.Quantity );

         dataRepositoryMock.VerifyAll();
      }
      #endregion

      #region Data Repository Event Handling Tests
      [TestMethod]
      public void AddingMealForDateToRepositoryUpdatesCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) ).Returns(
            new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Int32 originalFoodGroupCount = viewModel.FoodGroupServings.Count;
         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 5, foodGroupServings.Count );
         var serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         var foodItem = new FoodItem( Guid.NewGuid(), "test", "", 42 );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test1", "" ), 2 ) );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test2", "" ), 1 ) );
         var newMeal = new Meal(
            Guid.NewGuid(), new MealType( Guid.NewGuid(), "Doesn't Matter", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "Just a test" );
         newMeal.FoodItemServings.Add( new Serving<FoodItem>( foodItem, 1 ) );
         todaysMeals.Add( newMeal );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( newMeal ) );
         Assert.AreEqual( originalCalories + 42, viewModel.Calories );
         Assert.AreEqual( originalFoodGroupCount + 2, viewModel.FoodGroupServings.Count );
         Assert.AreEqual( originalFoodGroupServings + 3, (from foodGroupServing in viewModel.FoodGroupServings
                                                          select foodGroupServing.Quantity).Sum() );

         foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 7, foodGroupServings.Count );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Test1" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Test2" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 1, serving.Quantity );

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( viewModel, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Calories" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupServings" ) );
      }

      [TestMethod]
      public void AddingMealForOtherDateToRepositoryDoesNotUpdateCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) ).Returns(
            new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Int32 originalFoodGroupCount = viewModel.FoodGroupServings.Count;
         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var foodItem = new FoodItem( Guid.NewGuid(), "test", "", 42 );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test1", "" ), 2 ) );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test2", "" ), 1 ) );
         var newMeal = new Meal(
            Guid.NewGuid(), new MealType( Guid.NewGuid(), "Doesn't Matter", "", DateTime.Now, false ), DateTime.Now.AddDays( 1 ), "Test Meal", "Just a test" );
         newMeal.FoodItemServings.Add( new Serving<FoodItem>( foodItem, 1 ) );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( newMeal ) );
         Assert.AreEqual( originalCalories, viewModel.Calories );
         Assert.AreEqual( originalFoodGroupCount, viewModel.FoodGroupServings.Count );
         Assert.AreEqual( originalFoodGroupServings, (from foodGroupServing in viewModel.FoodGroupServings
                                                      select foodGroupServing.Quantity).Sum() );

         Assert.AreEqual( 0, propertyChangedHandler.PropertiesChanged.Count );
      }

      [TestMethod]
      public void DeletingMealForDateUpdatesCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 5, foodGroupServings.Count );
         var serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         var mealToRemove = todaysMeals.Find( x => x.TypeOfMeal.Name == "Breakfast" );
         todaysMeals.Remove( mealToRemove );
         Assert.IsNotNull( mealToRemove );
         dataRepositoryMock.Raise( x => x.ItemDeleted += null, new RepositoryObjectEventArgs( mealToRemove ) );

         Assert.AreEqual( originalFoodGroupServings - (from foodGroupServing in mealToRemove.FoodGroupServings
                                                       select foodGroupServing.Quantity).Sum(),
                          (from foodGroupServing in viewModel.FoodGroupServings
                           select foodGroupServing.Quantity).Sum() );
         Assert.AreEqual( originalCalories - mealToRemove.Calories, viewModel.Calories );

         foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 4, foodGroupServings.Count );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNull( serving );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( viewModel, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Calories" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupServings" ) );
      }

      [TestMethod]
      public void DeletingMealNotForDateDoesNotUpdateCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var mealToRemove = fullMealList.Find( x => x.TypeOfMeal.Name == "Breakfast" && x.DateAndTimeOfMeal.Date != DateTime.Today );
         todaysMeals.Remove( mealToRemove );
         Assert.IsNotNull( mealToRemove );
         dataRepositoryMock.Raise( x => x.ItemDeleted += null, new RepositoryObjectEventArgs( mealToRemove ) );

         Assert.AreEqual( originalFoodGroupServings, (from foodGroupServing in viewModel.FoodGroupServings
                                                      select foodGroupServing.Quantity).Sum() );
         Assert.AreEqual( originalCalories, viewModel.Calories );

         Assert.AreEqual( 0, propertyChangedHandler.PropertiesChanged.Count );
      }

      [TestMethod]
      public void ModifyingMealOnDateUpdatesCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 5, foodGroupServings.Count );
         var serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         var mealToModify = todaysMeals.Find( x => x.TypeOfMeal.Name == "Breakfast" );
         Assert.IsNotNull( mealToModify );
         mealToModify.FoodItemServings.Remove( mealToModify.FoodItemServings.Find( x => x.Entity.Name == "Orange Juice" ) );
         dataRepositoryMock.Raise( x => x.ItemModified += null, new RepositoryObjectEventArgs( mealToModify ) );

         Assert.AreEqual( originalFoodGroupServings - 2.5M,
                          (from foodGroupServing in viewModel.FoodGroupServings
                           select foodGroupServing.Quantity).Sum() );
         Assert.AreEqual( originalCalories - 250, viewModel.Calories );

         foodGroupServings = viewModel.FoodGroupServings;
         Assert.AreEqual( 4, foodGroupServings.Count );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Fruit" );
         Assert.IsNull( serving );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Starch" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 5.0M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Dairy" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 4.75M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 2.5M, serving.Quantity );
         serving = foodGroupServings.ToList().Find( x => x.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( 3.5M, serving.Quantity );

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( viewModel, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Calories" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupServings" ) );
      }

      [TestMethod]
      public void ModifyingMealNotOnDateDoesNotUpdateCaloriesAndFoodGroups()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var navigationServiceMock = new Mock<IRegionNavigationService>();

         var data = new MockData();
         var fullMealList = data.Meals();
         var todaysMeals = (from meal in fullMealList
                            where meal.DateAndTimeOfMeal.Date == DateTime.Today
                            select meal).ToList();
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today ) )
            .Returns( new ReadOnlyCollection<Meal>( todaysMeals ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         viewModel.CurrentDate = DateTime.Today;
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Decimal originalFoodGroupServings = (from foodGroupServing in viewModel.FoodGroupServings
                                              select foodGroupServing.Quantity).Sum();
         Decimal originalCalories = viewModel.Calories;

         var mealToModify = new Meal( fullMealList.Find( x => x.TypeOfMeal.Name == "Breakfast" && x.DateAndTimeOfMeal.Date != DateTime.Today ) );
         Assert.IsNotNull( mealToModify );
         mealToModify.FoodItemServings.Remove( mealToModify.FoodItemServings.Find( x => x.Entity.Name == "Orange Juice" ) );
         dataRepositoryMock.Raise( x => x.ItemModified += null, new RepositoryObjectEventArgs( mealToModify ) );

         Assert.AreEqual( originalFoodGroupServings,
                          (from foodGroupServing in viewModel.FoodGroupServings
                           select foodGroupServing.Quantity).Sum() );
         Assert.AreEqual( originalCalories, viewModel.Calories );

         Assert.AreEqual( 0, propertyChangedHandler.PropertiesChanged.Count );
      }
      #endregion

      #region Navigation Tests
      [TestMethod]
      public void ShowMealWithNonNullIDLoadsMealIntoView()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         Guid mealId = Guid.NewGuid();

         var regionMock = new Mock<IRegion>();
         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealView + "?ID=" + mealId.ToString(), UriKind.Relative ),
               It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.DailyLogTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.DailyLogTabsRegion] ).Returns( regionMock.Object );

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( It.IsAny<DateTime>() ) )
            .Returns( new ReadOnlyCollection<Meal>( new List<Meal>() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsTrue( viewModel.ShowMealCommand.CanExecute( mealId ) );
         viewModel.ShowMealCommand.Execute( mealId );

         regionMock.VerifyAll();
      }

      [TestMethod]
      public void ShowMealWithNullIDLoadsEmptyMealView()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var regionMock = new Mock<IRegion>();
         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealView, UriKind.Relative ),
               It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.DailyLogTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.DailyLogTabsRegion] ).Returns( regionMock.Object );

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( It.IsAny<DateTime>() ) )
            .Returns( new ReadOnlyCollection<Meal>( new List<Meal>() ) );

         var viewModel = new DailyLogViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsTrue( viewModel.ShowMealCommand.CanExecute( null ) );
         viewModel.ShowMealCommand.Execute( null );

         regionMock.VerifyAll();
      }
      #endregion
   }
}
