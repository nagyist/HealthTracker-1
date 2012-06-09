using System;
using HealthTracker.Administration.ViewModels;
using HealthTracker.DataRepository.Services;
using System.IO;

namespace UnitTests.Support
{
   public class FullTestData
   {
      public static void Reset()
      {
         File.Copy( CacheFileName, DataFileName, true );
      }

      #region Constants
      // File Names
      private static readonly String CacheFileName = "../../../UnitTests.Support/Test Data/FullTestData.xml";
      public static readonly String DataFileName = "../../../UnitTests.Support/Test Data/FullTestData.test.xml";

      // Food Groups
      public static readonly Guid VegetableID = new Guid( "269203a1-c066-4bca-a1ca-b80dc0cb72f3" );
      public static readonly Guid FruitID = new Guid( "45d58965-9c16-4a94-88cb-0b775cf69728" );
      public static readonly Guid MeatID = new Guid( "e024273b-a01e-4fb4-a535-e3dc632be5e4" );
      public static readonly Guid DairyID = new Guid( "05f8dfc3-436f-42a1-a006-64c301c79640" );
      public static readonly Guid GrainID = new Guid( "bb89556f-8c6c-4f14-8a47-9adf84a71bde" );
      public static readonly Guid WaterID = new Guid( "57ee9b15-46e8-4be4-a22a-2cbd8e8f359b" );
      public static readonly Guid JunkFoodID = new Guid( "5bbcff09-ae21-4753-b562-6fb435824001" );

      // Meal Types
      public static readonly Guid BreakfastID = new Guid( "47AB9DB8-83A3-41D8-BCE7-81F42D45FBC9" );
      public static readonly Guid LunchID = new Guid( "176b6819-8aae-4bf7-a3d9-83ad4d39ea2e" );
      public static readonly Guid DinnerID = new Guid( "a832aa99-722a-4619-87f0-7214db172221" );
      public static readonly Guid SnackID = new Guid( "c466b59b-ccba-4929-97f9-b3bc274dea04" );

      // Food Items
      public static readonly Guid CheeseBurgerID = new Guid( "4082072e-d6fd-4522-9535-d2fd6528a6be" );
      public static readonly Guid FruitSaladID = new Guid( "58af2a07-aae6-4f3d-8503-92c8f133fc37" );
      public static readonly Guid GlassOfMilkID = new Guid( "46d04859-f95d-4ed6-9697-3e8a15c0bc91" );
      public static readonly Guid CarrotID = new Guid( "d29701af-a487-466d-b752-34a6ae7269cd" );
      public static readonly Guid GlassOfWaterID = new Guid( "7fa82225-2870-4ab8-92f2-76df3fcb0632" );

      // Meal Templates
      public static readonly Guid CheeseBurgerLunchID = new Guid( "b59490be-da57-495d-a03c-18893fb1e065" );
      public static readonly Guid FruitSaladBreakfastID = new Guid( "762bc19c-63de-4723-b782-f1faafd83d2d" );
      #endregion
   }
}
