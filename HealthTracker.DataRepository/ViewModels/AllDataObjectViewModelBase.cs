using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthTracker.DataRepository.ViewModels
{
   public class AllDataObjectViewModelBase : IDisposable
   {
      #region IDisposable Methods
      public void Dispose()
      {
         this.OnDispose( true );
         GC.SuppressFinalize( this );
      }

      /// <summary>
      /// Deriving classes should override this method if they have stuff that needs disposing.
      /// </summary>
      /// <param name="disposing"></param>
      protected virtual void OnDispose( Boolean disposing ) { }

      ~AllDataObjectViewModelBase()
      {
         this.OnDispose( false );
      }
      #endregion
   }
}
