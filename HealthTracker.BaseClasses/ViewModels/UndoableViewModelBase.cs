using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;

namespace HealthTracker.BaseClasses.ViewModels
{
   public abstract class UndoableViewModelBase
   {
      #region Constructors
      public UndoableViewModelBase() : this( true ) { }

      public UndoableViewModelBase( Boolean instantiateManager)
      {
         if (instantiateManager)
         {
            UndoManager = new UndoRedoManager();
         }
      }
      #endregion

      #region Public Interface
      public UndoRedoManager UndoManager { get; set; }
      #endregion

      #region Commands
      private RelayCommand _undoCommand;
      public ICommand UndoCommand
      {
         get
         {
            if (_undoCommand == null)
            {
               _undoCommand = new RelayCommand( param => this.Undo(), param => this.CanUndo );
            }

            return _undoCommand;
         }
      }

      private RelayCommand _redoCommand;
      public ICommand RedoCommand
      {
         get
         {
            if (_redoCommand == null)
            {
               _redoCommand = new RelayCommand( param => this.Redo(), param => this.CanRedo );
            }

            return _redoCommand;
         }
      }

      protected virtual void Undo()
      {
         if (CanUndo)
         {
            UndoManager.Undo();
         }
      }
      protected virtual Boolean CanUndo { get { return UndoManager.CanUndo; } }

      protected virtual void Redo()
      {
         if (CanRedo)
         {
            UndoManager.Redo();
         }
       }
      protected virtual Boolean CanRedo { get { return UndoManager.CanRedo; } }
      #endregion

      #region Protected Interface
      protected void AddUndoableValue<T>( T undoableValue ) where T : IUndoRedo
      {
         // If the undo manager is currently performing an undo or redo, then we will get some adds
         // that we don't actually want to add...
         if (!UndoManager.IsPerformingUndoOrRedo)
         {
            UndoManager.Add( undoableValue );
         }
      }
      #endregion
   }
}
