using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HealthTracker.Infrastructure
{
   /// <summary>
   /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
   /// The default return value for the CanExecute method is 'true'.
   /// </summary>
   //
   // This class has been copied almost verbatim from the sample code for this tutorial:
   // http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
   //
   public class RelayCommand : ICommand
   {
      private readonly Action<Object> _execute;
      private readonly Predicate<Object> _canExecute;

      /// <summary>
      /// Creates a new command that can always execute.
      /// </summary>
      /// <param name="execute">The execution logic.</param>
      public RelayCommand( Action<Object> execute )
         : this( execute, null )
      {
      }

      /// <summary>
      /// Creates a new command.
      /// </summary>
      /// <param name="execute">The execution logic.</param>
      /// <param name="canExecute">The execution status logic.</param>
      public RelayCommand( Action<Object> execute, Predicate<Object> canExecute )
      {
         if (execute == null)
            throw new ArgumentNullException( "execute" );

         _execute = execute;
         _canExecute = canExecute;
      }


      #region ICommand Members

      public Boolean CanExecute( Object parameter )
      {
         return _canExecute == null ? true : _canExecute( parameter );
      }

      public event EventHandler CanExecuteChanged
      {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }

      public void Execute( Object parameter )
      {
         _execute( parameter );
      }

      #endregion // ICommand Members
   }
}
