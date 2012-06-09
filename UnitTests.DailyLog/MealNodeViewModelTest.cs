using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using HealthTracker.DailyLog.ViewModels;

namespace UnitTests.DailyLog
{
   [TestClass]
   public class MealNodeViewModelTest
   {
      #region Constructor Tests
      [TestMethod]
      public void MealNodeViewModelDefault()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );

         Assert.AreEqual( DisplayStrings.DailyLogMealTitle, mealNodeViewModel.Name );

         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );
         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            Meal meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }
      #endregion

      #region Date Handling Tests
      [TestMethod]
      public void MealListMatchesMealsForDay()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Today.AddDays( 1 ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( 1 )
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( DateTime.Today, mealNodeViewModel.CurrentDate );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         mealNodeViewModel.CurrentDate = mealNodeViewModel.CurrentDate.AddDays( 1 );
         Assert.AreEqual( 0, mealNodeViewModel.Children.Count );

         mealNodeViewModel.CurrentDate = mealNodeViewModel.CurrentDate.AddDays( 2 );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void MealForTodayAddedToChildrenWhenAddedToRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );


         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var newMeal = new Meal(
            Guid.NewGuid(), new MealType( Guid.NewGuid(), "Doesn't Matter", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "Just a test" );
         newMeal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "", 42 ), 1 ) );
         allMeals.Add( newMeal );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( newMeal ) );
         Assert.AreEqual( 4, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            Meal meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealForTomorrowAddedToChildrenWhenAddedToRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );


         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var newMeal = new Meal(
            Guid.NewGuid(), new MealType( Guid.NewGuid(), "Doesn't Matter", "", DateTime.Now, false ), DateTime.Now.AddDays( 1 ), "Test Meal", "Just a test" );
         newMeal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "", 42 ), 1 ) );
         allMeals.Add( newMeal );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( newMeal ) );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            Meal meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealForTodayAddedToChildrenWhenAddedToRepositoryIfDateNotToday()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );


         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         mealNodeViewModel.CurrentDate = DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );

         var newMeal = new Meal(
            Guid.NewGuid(), new MealType( Guid.NewGuid(), "Doesn't Matter", "", DateTime.Now, false ), DateTime.Now, "Test Meal", "Just a test" );
         newMeal.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "", 42 ), 1 ) );
         allMeals.Add( newMeal );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( newMeal ) );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            Meal meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void NonMealNotAddedToChildrenWhenAddedToRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var mealType = new MealType( Guid.NewGuid(), "A New Meal Type", "This should not already exist", DateTime.Now, false );
         Assert.IsTrue( mealType.IsValid );
         dataRepositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( mealType ) );

         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            Meal meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealForTodayRemovedFromChildrenWhenRemovedFromRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var removedMeal = allMeals.Find( m => m.DateAndTimeOfMeal.Date == DateTime.Today && m.TypeOfMeal.Name == "Lunch" );
         Assert.IsNotNull( removedMeal );
         allMeals.Remove( removedMeal );
         dataRepositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( removedMeal ) );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            var meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealForOtherDayNotRemovedFromChildrenWhenRemovedFromRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var removedMeal = allMeals.Find( m => m.DateAndTimeOfMeal.Date == DateTime.Today.AddDays( MockData.DaysToAddForFutureMeals ) && m.TypeOfMeal.Name == "Dinner" );
         Assert.IsNotNull( removedMeal );
         allMeals.Remove( removedMeal );
         dataRepositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( removedMeal ) );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            var meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealForTodayNotRemovedFromChildrenWhenRemovedFromRepositoryIfDateNotToday()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         mealNodeViewModel.CurrentDate = DateTime.Now.AddDays( MockData.DaysToAddForFutureMeals );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );

         var removedMeal = allMeals.Find( m => m.DateAndTimeOfMeal.Date == DateTime.Today && m.TypeOfMeal.Name == "Lunch" );
         Assert.IsNotNull( removedMeal );
         allMeals.Remove( removedMeal );
         dataRepositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( removedMeal ) );
         Assert.AreEqual( 2, mealNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealNodeViewModel.Children)
         {
            var meal = allMeals.Find( m => m.ID == (Guid)node.Parameter );
            Assert.IsNotNull( meal );
            Assert.AreEqual( meal.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealNodeModifiedIfMealModified()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();
         var allMeals = data.Meals();

         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date
                                           select meal).ToList() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealsForDate( DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals ) ) ).Returns(
            new ReadOnlyCollection<Meal>( (from meal in allMeals
                                           where meal.DateAndTimeOfMeal.Date == DateTime.Now.Date.AddDays( MockData.DaysToAddForFutureMeals )
                                           select meal).ToList() ) );

         var mealNodeViewModel = new MealNodeViewModel( dataRepositoryMock.Object, null );
         Assert.AreEqual( 3, mealNodeViewModel.Children.Count );

         var modifiedMeal = allMeals.Find( m => m.DateAndTimeOfMeal.Date == DateTime.Today && m.TypeOfMeal.Name == "Dinner" );
         Assert.IsNotNull( modifiedMeal );
         String originalName = modifiedMeal.Name;

         ClickableTreeNodeViewModel mealInTree = null;
         foreach (ClickableTreeNodeViewModel child in mealNodeViewModel.Children)
         {
            child.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
            if ((Guid)child.Parameter == modifiedMeal.ID)
            {
               mealInTree = child;
            }
         }

         Assert.IsNotNull( mealInTree );
         Assert.AreEqual( originalName, mealInTree.Name );

         modifiedMeal.Name += "Modified";
         dataRepositoryMock.Raise( e => e.ItemModified += null, new RepositoryObjectEventArgs( modifiedMeal ) );

         Assert.AreEqual( originalName + "Modified", mealInTree.Name );
         Assert.AreEqual( 1, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( mealInTree, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
      }
      #endregion
   }
}
