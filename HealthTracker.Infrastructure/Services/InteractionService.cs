using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Interfaces;
using System.ComponentModel.Composition;
using System.Windows;

namespace HealthTracker.Infrastructure.Services
{
   [Export( typeof( IInteractionService ) )]
   public class InteractionService : IInteractionService
   {
      #region Contstructors
      [ImportingConstructor]
      public InteractionService() { }
      #endregion

      #region IInteractionService Methods
      public MessageBoxResult ShowMessageBox( String messageText, String caption, MessageBoxButton buttons, MessageBoxImage icon )
      {
         return MessageBox.Show( messageText, caption, buttons, icon );
      }
      #endregion

      #region Private Helpers
      #endregion
   }
}
