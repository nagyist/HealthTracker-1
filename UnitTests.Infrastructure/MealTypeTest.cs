using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthTracker.DataRepository.Models;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class MealTypeTest
   {
      [TestMethod]
      public void TimeDefaultsToNowWithDefaultConstructor()
      {
         var currentTime = DateTime.Now;
         var mealType = new MealType();

         Assert.IsTrue( currentTime <= mealType.DefaultTimeOfMeal );
         Assert.IsTrue( currentTime.AddSeconds( 1.0 ) >= mealType.DefaultTimeOfMeal );
      }

      [TestMethod]
      public void TimeMatchesParameterWithFullConstructor()
      {
         var targetTime = DateTime.Now.AddHours( 1.0 );
         var mealType = new MealType( Guid.NewGuid(), "New Meal Type", "This is a description for the meal type", targetTime, true );

         Assert.AreEqual( targetTime, mealType.DefaultTimeOfMeal );
      }

      [TestMethod]
      public void TimeCopiedFromSourceWithCopyConstructor()
      {
         var targetTime = DateTime.Now.AddHours( 1.0 );
         var sourceMealType = new MealType( Guid.NewGuid(), "New Meal Type", "This is a description for the meal type", targetTime, true );
         var targetMealType = new MealType( sourceMealType );

         Assert.AreEqual( targetTime, targetMealType.DefaultTimeOfMeal );
      }

      [TestMethod]
      public void UseDefaultTimeDefaultsToTrueWithDefaultConstructor()
      {
         var mealType = new MealType();

         Assert.IsTrue( mealType.UseDefaultMealTime );
      }

      [TestMethod]
      public void UseDefaultTimeMatchesParameterWithFullConstructor()
      {
         var mealType = new MealType( Guid.NewGuid(), "Test", "Test Description", DateTime.Now, false );
         Assert.IsFalse( mealType.UseDefaultMealTime );

         mealType = new MealType( Guid.NewGuid(), "Test", "Test Description", DateTime.Now, true );
         Assert.IsTrue( mealType.UseDefaultMealTime );
      }

      [TestMethod]
      public void UseDefaultTimeCopiedFromSourceWithCopyConstructor()
      {
         var mealType = new MealType( Guid.NewGuid(), "Test", "Test Description", DateTime.Now, false );
         var targetMealType = new MealType( mealType );
         Assert.IsFalse( targetMealType.UseDefaultMealTime );

         mealType = new MealType( Guid.NewGuid(), "Test", "Test Description", DateTime.Now, true );
         targetMealType = new MealType( mealType );
         Assert.IsTrue( targetMealType.UseDefaultMealTime );
      }

      [TestMethod]
      public void IntializeDataSetsAllData()
      {
         var sourceMealType = new MealType( Guid.NewGuid(), "Test", "Test Description", DateTime.Today.AddHours( 14.25 ), false );
         var targetMealType = new MealType();

         targetMealType.InitializeData( sourceMealType );

         Assert.AreEqual( sourceMealType.ID, targetMealType.ID );
         Assert.AreEqual( sourceMealType.Name, targetMealType.Name );
         Assert.AreEqual( sourceMealType.Description, targetMealType.Description );
         Assert.AreEqual( sourceMealType.DefaultTimeOfMeal, targetMealType.DefaultTimeOfMeal );
         Assert.AreEqual( sourceMealType.UseDefaultMealTime, targetMealType.UseDefaultMealTime );
      }
   }
}
