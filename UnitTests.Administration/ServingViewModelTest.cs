using System;
using HealthTracker.Administration.ViewModels;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Support;
using HealthTracker.DataRepository.ViewModels;

namespace UnitTests.Administration
{
   [TestClass]
   public class ServingViewModelTest
   {
      #region Constructor Tests
      [TestMethod]
      public void ServingViewModelDefault()
      {
         FoodGroup foodGroupOne = new FoodGroup( Guid.NewGuid(), "FoodGroupOne", "Test Food Group One" );

         ServingViewModel<FoodGroup> servingViewModel = new ServingViewModel<FoodGroup>();
         Assert.IsFalse( servingViewModel.IsValid );
         Assert.IsNull( servingViewModel.Entity );
         Assert.AreEqual( 0, servingViewModel.Quantity );
         Assert.AreEqual( Messages.Error_No_Quantity, servingViewModel["Quantity"] );
         Assert.AreEqual( Messages.Error_No_ServingItem, servingViewModel["Entity"] );

         servingViewModel.Quantity = 42;
         Assert.IsFalse( servingViewModel.IsValid );
         Assert.IsNull( servingViewModel["Quantity"] );
         Assert.AreEqual( Messages.Error_No_ServingItem, servingViewModel["Entity"] );

         servingViewModel.Entity = foodGroupOne;
         Assert.IsTrue( servingViewModel.IsValid );
         Assert.IsNull( servingViewModel["Quantity"] );
         Assert.IsNull( servingViewModel["Entity"] );
      }

      [TestMethod]
      public void ServingViewModelEntityQuantity()
      {
         FoodGroup foodGroupOne = new FoodGroup( Guid.NewGuid(), "Food Group One", "Test Food Group One" );
         ServingViewModel<FoodGroup> servingViewModel = new ServingViewModel<FoodGroup>( foodGroupOne, 43 );
         Assert.IsTrue( servingViewModel.IsValid );
         Assert.AreEqual( foodGroupOne, servingViewModel.Entity );
         Assert.AreEqual( 43, servingViewModel.Quantity );
         Assert.IsNull( servingViewModel["Quantity"] );
         Assert.IsNull( servingViewModel["Entity"] );
      }

      [TestMethod]
      public void ServingViewModelServing()
      {
         FoodGroup foodGroupOne = new FoodGroup( Guid.NewGuid(), "FoodGroupOne", "Test Food Group #1" );
         Serving<FoodGroup> serving = new Serving<FoodGroup>( foodGroupOne, 2.5M );
         ServingViewModel<FoodGroup> servingViewModel = new ServingViewModel<FoodGroup>( serving );

         Assert.IsTrue( servingViewModel.IsValid );
         Assert.AreEqual( foodGroupOne, servingViewModel.Entity );
         Assert.AreEqual( 2.5M, servingViewModel.Quantity );
         Assert.IsNull( servingViewModel["Quantity"] );
         Assert.IsNull( servingViewModel["Entity"] );
      }
      #endregion

      #region IEditableObject Tests
      [TestMethod]
      public void ServingViewModelEditting()
      {
         MealType pi = new MealType( Guid.NewGuid(), "pi", "not the tasty kind", DateTime.Now, false );
         MealType unity = new MealType( Guid.NewGuid(), "unity", "it is itself", DateTime.Now, false );
         MealType golden = new MealType( Guid.NewGuid(), "golden", "it is very nice", DateTime.Now, false );

         ServingViewModel<MealType> servingViewModel = new ServingViewModel<MealType>( pi, 3.14159M );

         // Cancel Test.  Begin the edit, change the data, then cancel.
         servingViewModel.BeginEdit();
         servingViewModel.Entity = unity;
         servingViewModel.Quantity = 1;
         Assert.AreEqual( unity, servingViewModel.Entity );
         Assert.AreEqual( 1, servingViewModel.Quantity );
         servingViewModel.CancelEdit();
         Assert.AreEqual( pi, servingViewModel.Entity );
         Assert.AreEqual( 3.14159M, servingViewModel.Quantity );

         // In Transaction Test.  Begin the edit, change the data, begin again, cancel.  Data should still match original.
         servingViewModel.BeginEdit();
         servingViewModel.Entity = unity;
         servingViewModel.Quantity = 1;
         Assert.AreEqual( unity, servingViewModel.Entity );
         Assert.AreEqual( 1, servingViewModel.Quantity );
         servingViewModel.BeginEdit();
         servingViewModel.Entity = golden;
         servingViewModel.Quantity = 1.61803M;
         Assert.AreEqual( golden, servingViewModel.Entity );
         Assert.AreEqual( 1.61803M, servingViewModel.Quantity );
         servingViewModel.CancelEdit();
         Assert.AreEqual( pi, servingViewModel.Entity );
         Assert.AreEqual( 3.14159M, servingViewModel.Quantity );

         // End Edit Test.  Begin the edit, change the data, end the edit, change should stick
         servingViewModel.BeginEdit();
         servingViewModel.Entity = golden;
         servingViewModel.Quantity = 1.61803M;
         Assert.AreEqual( golden, servingViewModel.Entity );
         Assert.AreEqual( 1.61803M, servingViewModel.Quantity );
         servingViewModel.EndEdit();
         Assert.AreEqual( golden, servingViewModel.Entity );
         Assert.AreEqual( 1.61803M, servingViewModel.Quantity );
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void ServingViewModelPropertyChanged()
      {
         MealType pi = new MealType( Guid.NewGuid(), "pi", "not the tasty kind", DateTime.Now, false );
         MealType e = new MealType( Guid.NewGuid(), "e", "exponent", DateTime.Now, false );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         Serving<MealType> serving = new Serving<MealType>( pi, 3.14159M );
         ServingViewModel<MealType> servingViewModel = new ServingViewModel<MealType>( serving );
         servingViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;

         Assert.AreEqual( pi, servingViewModel.Entity );
         Assert.AreEqual( 3.14159M, servingViewModel.Quantity );
         Assert.IsNull( propertyChangedHandler.Sender );
         Assert.AreEqual( 0, propertyChangedHandler.PropertiesChanged.Count );

         servingViewModel.Entity = e;
         Assert.AreEqual( servingViewModel, propertyChangedHandler.Sender );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Entity" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( e, serving.Entity );
         Assert.AreEqual( 3.14159M, serving.Quantity );

         propertyChangedHandler.Reset();
         servingViewModel.Quantity = 2.71828183M;
         Assert.AreEqual( servingViewModel, propertyChangedHandler.Sender );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Quantity" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( e, serving.Entity );
         Assert.AreEqual( 2.71828183M, serving.Quantity );
      }
      #endregion

      #region Public Interface Tests
      [TestMethod]
      public void ServingViewModelIsDirty()
      {
         FoodGroup foodGroupOne = new FoodGroup( Guid.NewGuid(), "FoodGroupOne", "Test Food Group #1" );
         FoodGroup foodGroupTwo = new FoodGroup( Guid.NewGuid(), "FoodGroupTwo", "Test Food Group #2" );
         FoodGroup foodGroupThree = new FoodGroup( Guid.NewGuid(), "FoodGroupThree", "Test Food Group #3" );

         // Upon creation, the view model should be clean.
         ServingViewModel<FoodGroup> servingViewModel = new ServingViewModel<FoodGroup>( foodGroupOne, 2 );
         Assert.IsFalse( servingViewModel.IsDirty );

         // Change the Entity, the view model should be dirty
         servingViewModel.Entity = foodGroupTwo;
         Assert.IsTrue( servingViewModel.IsDirty );

         // Change the Entity back to the original value, the view model should no longer be dirty
         servingViewModel.Entity = foodGroupOne;
         Assert.IsFalse( servingViewModel.IsDirty );

         // Change the Quantity, the view model should be dirty
         servingViewModel.Quantity = 3;
         Assert.IsTrue( servingViewModel.IsDirty );

         // Change the Quantity back to the original value, the view model should no lonber be dirty
         servingViewModel.Quantity = 2;
         Assert.IsFalse( servingViewModel.IsDirty );

         // Make it dirty again, then reset the cache, the flag should fo false.
         servingViewModel.Entity = foodGroupThree;
         servingViewModel.Quantity = 4.5M;
         Assert.IsTrue( servingViewModel.IsDirty );

         servingViewModel.ResetPreviousValueCache();
         Assert.IsFalse( servingViewModel.IsDirty );
      }
      #endregion
   }
}
