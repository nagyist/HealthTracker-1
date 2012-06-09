using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class UndoRedoManagerTest
   {
      #region Constructor Tests
      [TestMethod]
      public void UndoRedoManagerConstruct()
      {
      }
      #endregion

      #region Public Interface
      [TestMethod]
      public void UndoRedoManagerCanUndo()
      {
         UndoRedoManager undoRedoManager = new UndoRedoManager();

         BookCollection myBooks = new BookCollection();
         myBooks.Name = "A few of my favorite books";
         myBooks.Library.Add( ClockWorkOrange );
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = ClockWorkOrange;

         // Originally, cannot undo, there is nothing to undo.
         Assert.IsFalse( undoRedoManager.CanUndo );

         // Add an item, can now undo
         myBooks.Name = "Some Books";
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my favorite books", "Some Books" ) );
         Assert.IsTrue( undoRedoManager.CanUndo );

         // After undoing the one change, there is nothing to undo
         undoRedoManager.Undo();
         Assert.IsFalse( undoRedoManager.CanUndo );

         // After redoing, we should be able to undo it again
         undoRedoManager.Redo();
         Assert.IsTrue( undoRedoManager.CanUndo );

         // Make another change, then undo it, should still be able to undo as the first one is still in there.
         myBooks.FavoriteBook = MereChristianity;
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, ClockWorkOrange, MereChristianity ) );
         Assert.IsTrue( undoRedoManager.CanUndo );

         undoRedoManager.Undo();
         Assert.IsTrue( undoRedoManager.CanUndo );

         undoRedoManager.Undo();
         Assert.IsFalse( undoRedoManager.CanUndo );
      }

      [TestMethod]
      public void UndoRedoManagerCanRedo()
      {
         UndoRedoManager undoRedoManager = new UndoRedoManager();

         BookCollection myBooks = new BookCollection();
         myBooks.Name = "A few of my favorite books";
         myBooks.Library.Add( ClockWorkOrange );
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = ClockWorkOrange;

         // Originally, cannot redo, there is nothing to redo.
         Assert.IsFalse( undoRedoManager.CanRedo );

         // Add an item, still nothing to redo
         myBooks.Name = "Some Books";
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my favorite books", "Some Books" ) );
         Assert.IsFalse( undoRedoManager.CanRedo );

         // Undo this item, we now have something to redo
         undoRedoManager.Undo();
         Assert.IsTrue( undoRedoManager.CanRedo );

         // Redo it, and we no longer have anything to redo
         undoRedoManager.Redo();
         Assert.IsFalse( undoRedoManager.CanRedo );

         // Undo it, make a change, undo that one, have two items to redo
         undoRedoManager.Undo();
         myBooks.FavoriteBook = MereChristianity;
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, ClockWorkOrange, MereChristianity ) );
         undoRedoManager.Undo();

         Assert.IsTrue( undoRedoManager.CanRedo );
         undoRedoManager.Redo();
         Assert.IsTrue( undoRedoManager.CanRedo );
         undoRedoManager.Redo();
         Assert.IsFalse( undoRedoManager.CanRedo );
      }

      [TestMethod]
      public void UndoRedoManagerUndoAndRedo()
      {
         UndoRedoManager undoRedoManager = new UndoRedoManager();

         BookCollection myBooks = new BookCollection();
         myBooks.Name = "A few of my favorite books";
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = ClockWorkOrange;

         DarwinsGod.Pages = 700;
         SoftwareEstimates.Pages = 800;

         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         // Simulate the following editting that got to the state above:
         //    1 - Name -> "Some Books"
         //    2 - Library -> Add MereChristianity
         //    3 - Name -> "A few of my books"
         //    4 - Favorite -> DarwinsGod
         //    5 - Library -> Add DarwinsGod
         //    6 - DarwinsGod -> change pages to 700
         //    7 - SoftwareEstimates -> change pages to 800
         //    8 - Library -> Remove MereChristianity
         //    9 - Favorite -> ClockWorkOrange
         //   10 - Library -> Add SoftwareEstimates
         //   11 - Name -> "A few of my favorite books"
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, null, "Some Books" ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, MereChristianity ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "Some Books", "A few of my books" ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, null, DarwinsGod ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, DarwinsGod ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<Book, Int16>( DarwinsGod, "Pages", UndoableActions.Modify, 338, 700 ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<Book, Int16>( SoftwareEstimates, "Pages", UndoableActions.Modify, 308, 800 ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Remove, MereChristianity, null ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, DarwinsGod, ClockWorkOrange ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, SoftwareEstimates ) );
         undoRedoManager.Add(
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my books", "A few of my favorite books" ) );

         // Can undo, cannot redo.  Undo five items, verify undo() performed at each step.
         Assert.IsTrue( undoRedoManager.CanUndo );
         Assert.IsFalse( undoRedoManager.CanRedo );
         undoRedoManager.Undo(); // 11
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         undoRedoManager.Undo(); // 10
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 1, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         undoRedoManager.Undo(); // 9
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 1, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.AreEqual( DarwinsGod, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         undoRedoManager.Undo(); // 8
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( MereChristianity ) );
         Assert.AreEqual( DarwinsGod, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         undoRedoManager.Undo(); // 7
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( MereChristianity ) );
         Assert.AreEqual( DarwinsGod, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 308, SoftwareEstimates.Pages );

         // Redo two items
         undoRedoManager.Redo(); // 7
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( MereChristianity ) );
         Assert.AreEqual( DarwinsGod, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         undoRedoManager.Redo(); // 8
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 1, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.AreEqual( DarwinsGod, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         // Undo all items
         undoRedoManager.Undo(); // 8
         undoRedoManager.Undo(); // 7
         undoRedoManager.Undo(); // 6
         undoRedoManager.Undo(); // 5
         undoRedoManager.Undo(); // 4
         undoRedoManager.Undo(); // 3
         undoRedoManager.Undo(); // 2
         undoRedoManager.Undo(); // 1
         Assert.IsFalse( undoRedoManager.CanUndo );
         Assert.IsTrue( String.IsNullOrEmpty( myBooks.Name ) );
         Assert.AreEqual( 0, myBooks.Library.Count );
         Assert.IsNull( myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 338, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 308, SoftwareEstimates.Pages );

         // Redo all items
         undoRedoManager.Redo(); //  1
         undoRedoManager.Redo(); //  2
         undoRedoManager.Redo(); //  3
         undoRedoManager.Redo(); //  4
         undoRedoManager.Redo(); //  5
         undoRedoManager.Redo(); //  6
         undoRedoManager.Redo(); //  7
         undoRedoManager.Redo(); //  8
         undoRedoManager.Redo(); //  9
         undoRedoManager.Redo(); // 10
         undoRedoManager.Redo(); // 11
         Assert.IsFalse( undoRedoManager.CanRedo );
         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );
         Assert.AreEqual( 192, ClockWorkOrange.Pages );
         Assert.AreEqual( 700, DarwinsGod.Pages );
         Assert.AreEqual( 227, MereChristianity.Pages );
         Assert.AreEqual( 800, SoftwareEstimates.Pages );

         // Reset pages on potentially changed books
         DarwinsGod.Pages = 338;
         SoftwareEstimates.Pages = 308;
      }
      #endregion

      #region Private Data
      // NOTE: The tests here directly use the UndoRedoManager rather than having the test objects call it.
      //       In the real world, the ViewModel objects will interact with the UndoRedo manager.
      private class Book
      {
         public Book( String title, String author, Int16 pages )
         {
            Title = title;
            Author = author;
            Pages = pages;
         }

         public String Title { get; set; }
         public String Author { get; set; }
         public Int16 Pages { get; set; }
      }

      private class BookCollection
      {
         public BookCollection()
         {
            Library = new ObservableCollection<Book>();
         }

         public BookCollection( String name )
            : this()
         {
            Name = name;
         }
         public String Name { get; set; }

         public ObservableCollection<Book> Library { get; set; }

         public Book FavoriteBook { get; set; }
      }

      private Book ClockWorkOrange = new Book( "A Clockwork Orange", "Anthony Burgess", 192 );
      private Book DarwinsGod = new Book( "Finding Darwin's God", "Kenneth R. Miller", 338 );
      private Book MereChristianity = new Book( "Mere Christianity", "C. S. Lewis", 227 );
      private Book SoftwareEstimates = new Book( "Software Estimation - Demystifying the Black Art", "Steve McConnell", 308 );
      #endregion
   }
}
