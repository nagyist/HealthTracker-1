using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using HealthTracker.DataRepository.Interfaces;
using System.Collections.ObjectModel;

namespace UnitTests.DataRepository
{
   [TestClass]
   public class AllFoodItemsViewModelTest
   {
      #region Private Helpers
      private Mock<IDataRepository> CreateMockDataRepository( List<FoodItem> foods )
      {
         var dataRespositoryMock = new Mock<IDataRepository>();
         dataRespositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( (from food in foods
                                                         select new FoodItem( food )).ToList() ) );

         return dataRespositoryMock;
      }

      private void AssertViewModelContents( AllFoodItemsViewModel viewModel, List<FoodItem> foods )
      {
         Assert.AreEqual( foods.Count, viewModel.Items.Count );

         foreach (var food in viewModel.Items)
         {
            var foodItemFromRepository = foods.Find( f => f.ID == food.ID );

            Assert.IsNotNull( food );
            Assert.AreEqual( foodItemFromRepository.Name, food.Name );
         }
      }
      #endregion

      #region Tests
      [TestMethod]
      public void AllFoodItemsViewModelContainsAllFoods()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.Foods() );

         var viewModel = new AllFoodItemsViewModel( dataRespositoryMock.Object );

         AssertViewModelContents( viewModel, data.Foods() );
         dataRespositoryMock.VerifyAll();
      }

      [TestMethod]
      public void FoodItemRemovedFromViewModelWhenRemovedFromRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.Foods() );

         var viewModel = new AllFoodItemsViewModel( dataRespositoryMock.Object );
         var food = data.Foods().Find( mt => mt.ID == MockData.baconCheeseBurgerID );
         data.Foods().Remove( food );
         dataRespositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( food ) );

         AssertViewModelContents( viewModel, data.Foods() );
      }

      [TestMethod]
      public void FoodItemAddedToViewModelWhenAddedToRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.Foods() );

         var viewModel = new AllFoodItemsViewModel( dataRespositoryMock.Object );
         var food = new FoodItem( Guid.NewGuid(), "New Type", "For a unit test", 42.0M );
         data.Foods().Add( food );
         dataRespositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( food ) );

         AssertViewModelContents( viewModel, data.Foods() );
      }

      [TestMethod]
      public void FoodItemModifiedInViewModelWhenChangedInRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.Foods() );
         var viewModel = new AllFoodItemsViewModel( dataRespositoryMock.Object );

         var food = data.Foods().Find( f => f.ID == MockData.baconCheeseBurgerID );
         food.Name += " Unit Test";
         dataRespositoryMock.Raise( e => e.ItemModified += null, new RepositoryObjectEventArgs( food ) );

         AssertViewModelContents( viewModel, data.Foods() );
      }
      #endregion
   }
}
