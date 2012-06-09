using System;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class ServingTest
   {
      [TestMethod]
      public void ServingDefault()
      {
         Serving<String> serving = new Serving<String>();

         Assert.AreEqual(default(String), serving.Entity);
         Assert.AreEqual(0, serving.Quantity);
         Assert.IsFalse( serving.IsValid );
         Assert.AreEqual( Messages.Error_No_ServingItem, serving["Entity"] );
         Assert.AreEqual( Messages.Error_No_Quantity, serving["Quantity"] );

         // Set the Item
         serving.Entity = "This is a test";
         Assert.IsFalse( serving.IsValid );
         Assert.IsNull( serving["Entity"] );
         Assert.AreEqual( Messages.Error_No_Quantity, serving["Quantity"] );

         // Set the Quantity to another invalid value
         serving.Quantity = -1;
         Assert.IsFalse( serving.IsValid );
         Assert.IsNull( serving["Entity"] );
         Assert.AreEqual( Messages.Error_No_Quantity, serving["Quantity"] );

         // Set the Quantity to a valid value
         serving.Quantity = 1;
         Assert.IsTrue( serving.IsValid );
         Assert.IsNull( serving["Entity"] );
         Assert.IsNull( serving["Quantity"] );
      }

      [TestMethod]
      public void ServingEntityQuantity()
      {
         Serving<String> serving = new Serving<String>( "This is a test string", 42 );

         Assert.AreEqual( "This is a test string", serving.Entity );
         Assert.AreEqual( 42, serving.Quantity );
         Assert.IsTrue( serving.IsValid );
         Assert.IsNull( serving["Entity"] );
         Assert.IsNull( serving["Quantity"] );
      }
   }
}
