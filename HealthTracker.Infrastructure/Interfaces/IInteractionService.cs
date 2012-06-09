using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HealthTracker.Infrastructure.Interfaces
{
   /// <summary>
   /// Interface to an Interaction Service.  An interaction service allows for several different types of 
   /// interactions with users, such as:
   ///    o Dialog Boxes
   ///    o Message Boxes
   ///    o File Open Dialog Boxes
   ///    o Other standard windows dialog boxes as required
   /// </summary>
   public interface IInteractionService
   {
      /// <summary>
      /// Use the service to display a message box
      /// </summary>
      MessageBoxResult ShowMessageBox( String messageText, String caption, MessageBoxButton buttons, MessageBoxImage icon );
   }
}
