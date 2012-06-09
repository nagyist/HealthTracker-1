using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Support
{
   /// <summary>
   /// A class that makes testing the handling of property changed events easier.
   /// Hook up the handler, and the handler collects the properties that have been
   /// changed since the last reset (or instantiation).
   /// 
   /// The handler also makes sure that all events are sent by the same object
   /// since the last reset.
   /// </summary>
   public class PropertyChangedHandler
   {
      #region Constructors
      public PropertyChangedHandler()
      {
         PropertiesChanged = new List<String>();
         Reset();
      }
      #endregion

      #region Public Interface
      public object Sender { get; private set; }
      public List<String> PropertiesChanged { get; private set; }

      public void Reset()
      {
         PropertiesChanged.Clear();
         Sender = null;
      }
      #endregion

      #region Event Handlers
      public void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
      {
         Assert.IsTrue( Sender == null || Sender == sender );

         Sender = sender;
         if (!PropertiesChanged.Contains( e.PropertyName ))
         {
            PropertiesChanged.Add( e.PropertyName );
         }
      }
      #endregion
   }
}
