using System;
using System.Linq;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using HealthTracker.DataRepository.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitTests.DataRepository
{
   [TestClass]
   public class AllFoodGroupsViewModelTest
   {
      #region Private Helpers
      private Mock<IDataRepository> CreateMockDataRepository( List<FoodGroup> foodGroups )
      {
         var dataRespositoryMock = new Mock<IDataRepository>();
         dataRespositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( (from foodGroup in foodGroups
                                                         select new FoodGroup( foodGroup )).ToList() ) );

         return dataRespositoryMock;
      }

      private void AssertViewModelContents( AllFoodGroupsViewModel viewModel, List<FoodGroup> foodGroups )
      {
         Assert.AreEqual( foodGroups.Count, viewModel.Items.Count );

         foreach (var foodGroup in viewModel.Items)
         {
            var foodGroupFromRepository = foodGroups.Find( fg => fg.ID == foodGroup.ID );

            Assert.IsNotNull( foodGroup );
            Assert.AreEqual( foodGroupFromRepository.Name, foodGroup.Name );
         }
      }
      #endregion

      #region Tests
      [TestMethod]
      public void AllFoodGroupsViewModelContainsAllTypes()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.FoodGroups() );

         var viewModel = new AllFoodGroupsViewModel( dataRespositoryMock.Object );

         AssertViewModelContents( viewModel, data.FoodGroups() );
         dataRespositoryMock.VerifyAll();
      }

      [TestMethod]
      public void FoodGroupRemovedFromViewModelWhenRemovedFromRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.FoodGroups() );

         var viewModel = new AllFoodGroupsViewModel( dataRespositoryMock.Object );
         var foodGroup = data.FoodGroups().Find( mt => mt.ID == MockData.dairyID );
         data.FoodGroups().Remove( foodGroup );
         dataRespositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( foodGroup ) );

         AssertViewModelContents( viewModel, data.FoodGroups() );
      }

      [TestMethod]
      public void FoodGroupAddedToViewModelWhenAddedToRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.FoodGroups() );

         var viewModel = new AllFoodGroupsViewModel( dataRespositoryMock.Object );
         var foodGroup = new FoodGroup( Guid.NewGuid(), "New Group", "Some newly dreamed up food group" );
         data.FoodGroups().Add( foodGroup );
         dataRespositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( foodGroup ) );

         AssertViewModelContents( viewModel, data.FoodGroups() );
      }

      [TestMethod]
      public void FoodGroupModifiedInViewModelWhenChangedInRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.FoodGroups() );
         var viewModel = new AllFoodGroupsViewModel( dataRespositoryMock.Object );

         var foodGroup = data.FoodGroups().Find( mt => mt.ID == MockData.meatID );
         foodGroup.Name += " Unit Test";
         dataRespositoryMock.Raise( e => e.ItemModified += null, new RepositoryObjectEventArgs( foodGroup ) );

         AssertViewModelContents( viewModel, data.FoodGroups() );
      }
      #endregion
   }
}
