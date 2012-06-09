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
   public class UndoablePropertyValueTest
   {
      #region Constructor Tests
      [TestMethod]
      public void UndoablePropertyValueConstruct()
      {
         BookCollection myBooks = new BookCollection();
         myBooks.Name = "A few of my books";
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = ClockWorkOrange;

         // Construct an object representing a change to a basic property
         UndoablePropertyValue<BookCollection, String> nameChange =
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my books", "Some good books" );

         // Construct an object representing an addition to a collection
         UndoablePropertyValue<BookCollection, Book> addBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, MereChristianity );

         // Construct an object representing a removal from a collection
         UndoablePropertyValue<BookCollection, Book> removeBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Remove, MereChristianity, null );
      }
      #endregion

      #region Public Interface Test
      [TestMethod]
      public void UndoablePropertyValueUndo()
      {
         BookCollection myBooks = new BookCollection( "A few of my books" );
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = MereChristianity;

         // Undo of a basic property
         UndoablePropertyValue<BookCollection, String> nameChange =
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my favorite books", "A few of my books" );
         nameChange.Undo();
         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( MereChristianity, myBooks.FavoriteBook );

         UndoablePropertyValue<BookCollection,Book> favoriteChange =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, ClockWorkOrange, MereChristianity );
         favoriteChange.Undo();
         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );

         // Undo of a removal from a collection
         UndoablePropertyValue<BookCollection, Book> removeBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Remove, ClockWorkOrange, null );
         removeBook.Undo();
         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 3, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.IsTrue( myBooks.Library.Contains( ClockWorkOrange ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );

         // Undo of an addition to a collection
         UndoablePropertyValue<BookCollection, Book> addBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, DarwinsGod );
         addBook.Undo();
         Assert.AreEqual( "A few of my favorite books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.IsTrue( myBooks.Library.Contains( ClockWorkOrange ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );
      }


      [TestMethod]
      public void UndoablePropertyValueRedo()
      {
         BookCollection myBooks = new BookCollection();
         myBooks.Name = "A few of my favorite books";
         myBooks.Library.Add( ClockWorkOrange );
         myBooks.Library.Add( DarwinsGod );
         myBooks.Library.Add( SoftwareEstimates );
         myBooks.FavoriteBook = ClockWorkOrange;

         // Redo of a basic property
         UndoablePropertyValue<BookCollection, String> nameChange =
            new UndoablePropertyValue<BookCollection, String>( myBooks, "Name", UndoableActions.Modify, "A few of my favorite books", "A few of my books" );
         nameChange.Redo();
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 3, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.IsTrue( myBooks.Library.Contains( ClockWorkOrange ) );
         Assert.AreEqual( ClockWorkOrange, myBooks.FavoriteBook );

         UndoablePropertyValue<BookCollection, Book> favoriteChange =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "FavoriteBook", UndoableActions.Modify, ClockWorkOrange, MereChristianity );
         favoriteChange.Redo();
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 3, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.IsTrue( myBooks.Library.Contains( ClockWorkOrange ) );
         Assert.AreEqual( MereChristianity, myBooks.FavoriteBook );

         // Redo of a removal from a collection
         UndoablePropertyValue<BookCollection, Book> removeBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Remove, ClockWorkOrange, null );
         removeBook.Redo();
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 2, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.AreEqual( MereChristianity, myBooks.FavoriteBook );

         // Redo of an addition to a collection
         UndoablePropertyValue<BookCollection, Book> addBook =
            new UndoablePropertyValue<BookCollection, Book>( myBooks, "Library", UndoableActions.Add, null, MereChristianity );
         addBook.Redo();
         Assert.AreEqual( "A few of my books", myBooks.Name );
         Assert.AreEqual( 3, myBooks.Library.Count );
         Assert.IsTrue( myBooks.Library.Contains( SoftwareEstimates ) );
         Assert.IsTrue( myBooks.Library.Contains( DarwinsGod ) );
         Assert.IsTrue( myBooks.Library.Contains( MereChristianity ) );
         Assert.AreEqual( MereChristianity, myBooks.FavoriteBook );
      }
      #endregion

      #region Private Data
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
