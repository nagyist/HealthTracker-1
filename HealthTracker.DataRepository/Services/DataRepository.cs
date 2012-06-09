using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism.Modularity;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.DataRepository.Services
{
   /// <summary>
   /// The general data repository.  Collections of various data objects used throughout the system are 
   /// maintained here.
   /// </summary>
   [Export( typeof( IDataRepository ) )]
   public class DataRepository : IDataRepository
   {
      #region Constructors
      // TODO: load from file or database.  For now, assuming file is fine
      [ImportingConstructor]
      public DataRepository( IConfiguration configuration )
         : this( configuration.FileName )
      {
         currentConfiguration = configuration;
         LoadRepository( configuration.FileName );
      }

      // TODO: Make this obsolete.  We can Mock the configuration where needed and the whole
      //       data repository when not testing the repository itself.
      public DataRepository( String dataFileName )
      {
         LoadRepository( dataFileName );
      }
      #endregion

      #region Events
      public event EventHandler<RepositoryObjectEventArgs> ItemAdded;
      public event EventHandler<RepositoryObjectEventArgs> ItemDeleted;
      public event EventHandler<RepositoryObjectEventArgs> ItemModified;
      #endregion

      #region Get and Find Routines
      /// <summary>
      /// Returns a shallow copied list of the food groups that are in the repository
      /// </summary>
      public ReadOnlyCollection<FoodGroup> GetAllFoodGroups()
      {
         return new ReadOnlyCollection<FoodGroup>( _foodGroups );
      }

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      public ReadOnlyCollection<FoodItem> GetAllFoodItems()
      {
         return new ReadOnlyCollection<FoodItem>( _foodItems );
      }

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      public ReadOnlyCollection<MealTemplate> GetAllMealTemplates()
      {
         return new ReadOnlyCollection<MealTemplate>( _mealTemplates );
      }

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      public ReadOnlyCollection<MealType> GetAllMealTypes()
      {
         return new ReadOnlyCollection<MealType>( _mealTypes );
      }

      public ReadOnlyCollection<Meal> GetAllMeals()
      {
         return new ReadOnlyCollection<Meal>( _meals );
      }

      public ReadOnlyCollection<Meal> GetAllMealsForDate( DateTime date )
      {
         return new ReadOnlyCollection<Meal>( (from meal in _meals
                                               where meal.DateAndTimeOfMeal.Date == date.Date
                                               select meal).ToList() );
      }

      public FoodGroup GetFoodGroup( Guid id )
      {
         return FindFoodGroup( fg => fg.ID == id );
      }

      public FoodGroup FindFoodGroup( Predicate<FoodGroup> match )
      {
         FoodGroup foodGroup = _foodGroups.Find( match );

         if (foodGroup != null)
         {
            return new FoodGroup( foodGroup );
         }

         return null;
      }

      public FoodItem GetFoodItem( Guid id )
      {
         return FindFoodItem( f => f.ID == id );
      }

      public FoodItem FindFoodItem( Predicate<FoodItem> match )
      {
         FoodItem foodItem = _foodItems.Find( match );

         if (foodItem != null)
         {
            return new FoodItem( foodItem );
         }

         return null;
      }

      public MealTemplate GetMealTemplate( Guid id )
      {
         return FindMealTemplate( m => m.ID == id );
      }

      public MealTemplate FindMealTemplate( Predicate<MealTemplate> match )
      {
         MealTemplate template = _mealTemplates.Find( match );

         if (template != null)
         {
            return new MealTemplate( template );
         }

         return null;
      }

      public MealType GetMealType( Guid id )
      {
         return FindMealType( m => m.ID == id );
      }

      public MealType FindMealType( Predicate<MealType> match )
      {
         MealType mealType = _mealTypes.Find( match );

         if (mealType != null)
         {
            return new MealType( mealType );
         }

         return null;
      }

      public Meal GetMeal( Guid id )
      {
         return FindMeal( m => m.ID == id );
      }

      public Meal FindMeal( Predicate<Meal> match )
      {
         var meal = _meals.Find( match );

         if (meal != null)
         {
            return new Meal( meal );
         }

         return null;
      }
      #endregion

      #region Delete and Save Items
      /// <summary>
      /// Add a food group to the repository.
      /// </summary>
      /// <param name="foodGroup">The food group to add</param>
      public void SaveItem( FoodGroup foodGroup )
      {
         if (foodGroup == null)
         {
            throw new ArgumentNullException( "foodGroup" );
         }

         if (!Contains( foodGroup ))
         {
            _foodGroups.Add( new FoodGroup( foodGroup ) );
            if (ItemAdded != null)
            {
               ItemAdded( this, new RepositoryObjectEventArgs( foodGroup ) );
            }
         }
         else
         {
            FoodGroup repositoryFoodGroup = _foodGroups.Find( item => item.ID == foodGroup.ID );
            Debug.Assert( repositoryFoodGroup != null );
            repositoryFoodGroup.InitializeData( foodGroup );
            if (ItemModified != null)
            {
               ItemModified( this, new RepositoryObjectEventArgs( foodGroup ) );
            }
         }

         this.SaveRepository();
      }

      /// <summary>
      /// Add a food item to the repository
      /// </summary>
      /// <param name="foodItem">The food item</param>
      public void SaveItem( FoodItem foodItem )
      {
         Debug.Assert( foodItem != null );

         if (!Contains( foodItem ))
         {
            _foodItems.Add( new FoodItem( foodItem ) );
            if (ItemAdded != null)
            {
               ItemAdded( this, new RepositoryObjectEventArgs( foodItem ) );
            }
         }
         else
         {
            FoodItem repositoryFoodItem = _foodItems.Find( f => f.ID == foodItem.ID );
            Debug.Assert( repositoryFoodItem != null );
            repositoryFoodItem.InitializeData( foodItem );
            if (ItemModified != null)
            {
               ItemModified( this, new RepositoryObjectEventArgs( foodItem ) );
            }
         }

         this.SaveRepository();
      }

      public void SaveItem( MealTemplate mealTemplate )
      {
         if (mealTemplate == null)
         {
            throw new ArgumentNullException( "mealTemplate" );
         }

         if (!Contains( mealTemplate ))
         {
            _mealTemplates.Add( new MealTemplate( mealTemplate ) );
            if (ItemAdded != null)
            {
               ItemAdded( this, new RepositoryObjectEventArgs( mealTemplate ) );
            }
         }
         else
         {
            MealTemplate repositoryMealTemplate = _mealTemplates.Find( m => m.ID == mealTemplate.ID );
            Debug.Assert( repositoryMealTemplate != null );
            repositoryMealTemplate.InitializeData( mealTemplate );
            if (ItemModified != null)
            {
               ItemModified( this, new RepositoryObjectEventArgs( mealTemplate ) );
            }
         }

         this.SaveRepository();
      }

      /// <summary>
      /// Add a mealTemplate type to the repository.
      /// </summary>
      /// <param name="mealType">The mealTemplate type to add</param>
      public void SaveItem( MealType mealType )
      {
         if (mealType == null)
         {
            throw new ArgumentNullException( "mealType" );
         }

         if (!Contains( mealType ))
         {
            _mealTypes.Add( new MealType( mealType ) );
            if (ItemAdded != null)
            {
               ItemAdded( this, new RepositoryObjectEventArgs( mealType ) );
            }
         }
         else
         {
            MealType repositoryMealType = _mealTypes.Find( item => item.ID == mealType.ID );
            Debug.Assert( repositoryMealType != null );
            repositoryMealType.InitializeData( mealType );
            if (ItemModified != null)
            {
               ItemModified( this, new RepositoryObjectEventArgs( mealType ) );
            }
         }

         this.SaveRepository();
      }

      public void SaveItem( Meal meal )
      {
         if (meal == null)
         {
            throw new ArgumentNullException( "meal" );
         }

         if (!Contains( meal ))
         {
            _meals.Add( new Meal( meal ) );
            if (ItemAdded != null)
            {
               ItemAdded( this, new RepositoryObjectEventArgs( meal ) );
            }
         }
         else
         {
            var repositoryMeal = _meals.Find( m => m.ID == meal.ID );
            Debug.Assert( repositoryMeal != null );
            repositoryMeal.InitializeData( meal );
            if (ItemModified != null)
            {
               ItemModified( this, new RepositoryObjectEventArgs( meal ) );
            }
         }

         this.SaveRepository();
      }

      /// <summary>
      /// Remove a food group from the repository
      /// </summary>
      /// <param name="foodGroup">The food group to delete</param>
      public void Remove( FoodGroup foodGroup )
      {
         if (foodGroup == null)
         {
            throw new ArgumentNullException( "foodGroup" );
         }

         if (Contains( foodGroup ))
         {
            // foodGroup is likely a deep copy, find by ID and remove.
            _foodGroups.Remove( _foodGroups.Find( fg => fg.ID == foodGroup.ID ) );
            if (ItemDeleted != null)
            {
               ItemDeleted( this, new RepositoryObjectEventArgs( foodGroup ) );
            }

            SaveRepository();
         }
      }

      /// <summary>
      /// Remove a food item from the repository
      /// </summary>
      /// <param name="foodItem">The food item</param>
      public void Remove( FoodItem foodItem )
      {
         Debug.Assert( foodItem != null );

         if (Contains( foodItem ))
         {
            _foodItems.Remove( _foodItems.Find( f => f.ID == foodItem.ID ) );
            if (ItemDeleted != null)
            {
               ItemDeleted( this, new RepositoryObjectEventArgs( foodItem ) );
            }
         }

         SaveRepository();
      }

      /// <summary>
      /// Remove a mealTemplate template from the repository.
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template to remove.</param>
      public void Remove( MealTemplate mealTemplate )
      {
         Debug.Assert( mealTemplate != null );

         if (Contains( mealTemplate ))
         {
            _mealTemplates.Remove( _mealTemplates.Find( mt => mt.ID == mealTemplate.ID ) );
            if (ItemDeleted != null)
            {
               ItemDeleted( this, new RepositoryObjectEventArgs( mealTemplate ) );
            }

            SaveRepository();
         }
      }

      /// <summary>
      /// Remove a mealTemplate type from the repository
      /// </summary>
      /// <param name="mealType">The mealTemplate type to delete</param>
      public void Remove( MealType mealType )
      {
         if (mealType == null)
         {
            throw new ArgumentNullException( "mealType" );
         }

         if (Contains( mealType ))
         {
            _mealTypes.Remove( _mealTypes.Find( mt => mt.ID == mealType.ID ) );
            if (ItemDeleted != null)
            {
               ItemDeleted( this, new RepositoryObjectEventArgs( mealType ) );
            }

            SaveRepository();
         }
      }

      public void Remove( Meal meal )
      {
         Debug.Assert( meal != null );

         if (Contains( meal ))
         {
            _meals.Remove( _meals.Find( mt => mt.ID == meal.ID ) );
            if (ItemDeleted != null)
            {
               ItemDeleted( this, new RepositoryObjectEventArgs( meal ) );
            }

            SaveRepository();
         }
      }
      #endregion

      #region Contains
      /// <summary>
      /// Determine if the repository contains a food group with the same ID.
      /// </summary>
      /// <param name="foodGroup">The food group</param>
      /// <returns>True of the food group is in the repository, false otherwise</returns>
      public Boolean Contains( FoodGroup foodGroup )
      {
         return (_foodGroups.Find( item => item.ID == foodGroup.ID ) != null);
      }

      /// <summary>
      /// Determine if the repository contains a mealTemplate type.
      /// </summary>
      /// <param name="mealType">The mealTemplate type</param>
      /// <returns>True of the mealTemplate type is in the repository, false otherwise</returns>
      public Boolean Contains( MealType mealType )
      {
         return (_mealTypes.Find( item => item.ID == mealType.ID ) != null);
      }

      /// <summary>
      /// Determine if the repository already contains the food item
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>Returns true if the food item is alread in the repository, false oterwise.</returns>
      public Boolean Contains( FoodItem foodItem )
      {
         return (_foodItems.Find( item => item.ID == foodItem.ID ) != null);
      }

      /// <summary>
      /// Determine if the repository contains the given mealTemplate template
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template to look for</param>
      /// <returns>True of the mealTemplate template is in the repository, false otherwise</returns>
      public Boolean Contains( MealTemplate mealTemplate )
      {
         return (_mealTemplates.Find( item => item.ID == mealTemplate.ID ) != null);
      }

      /// <summary>
      /// Determine if the repository contains the given mealTemplate
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate to look for</param>
      /// <returns>True of the mealTemplate is in the repository, false otherwise</returns>
      public Boolean Contains( Meal meal )
      {
         return (_meals.Find( item => item.ID == meal.ID ) != null);
      }
      #endregion

      #region Editing Routines
      /// <summary>
      /// Determine if a food group is referenenced somewhere.
      /// </summary>
      /// <param name="foodGroup">The food group to look for</param>
      /// <returns>True if the food group is in use, false otherwise</returns>
      public Boolean ItemIsUsed( FoodGroup foodGroup )
      {
         foreach (FoodItem foodItem in _foodItems)
         {
            if (foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.ID == foodGroup.ID ) != null)
            {
               return true;
            }
         }

         return false;
      }

      /// <summary>
      /// Determine if a mealTemplate type is referenenced somewhere.
      /// </summary>
      /// <param name="mealType">The mealTemplate type to look for</param>
      /// <returns>True if the mealTemplate type is in use, false otherwise</returns>
      public Boolean ItemIsUsed( MealType mealType )
      {
         return (_mealTemplates.Find( m => m.TypeOfMeal.ID == mealType.ID ) != null);
      }

      /// <summary>
      /// Determine if a food item is currently referenced by some other entitiy in the repository.
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>True if used, false otherwise.</returns>
      public Boolean ItemIsUsed( FoodItem foodItem )
      {
         // I foresee this getting slow once we get a lot of meals in there.
         // May need to implement a usage count on the food item list.
         foreach (MealTemplate mealTemplate in _mealTemplates)
         {
            foreach (Serving<FoodItem> serving in mealTemplate.FoodItemServings)
            {
               if (serving.Entity.ID == foodItem.ID)
               {
                  return true;
               }
            }
         }
         return false;
      }

      /// <summary>
      /// Determine if a mealTemplate template is referenced elsewhere in the repository
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template</param>
      /// <returns>Always returns false</returns>
      public Boolean ItemIsUsed( MealTemplate mealTemplate )
      {
         return false;
      }

      /// <summary>
      /// Determine if a food group's name is being used by some other food group 
      /// in the repository.
      /// </summary>
      /// <param name="foodGroup">The food group</param>
      /// <returns>True if some other food group in the repository has the same name, false otherwise.</returns>
      public Boolean NameIsDuplicate( FoodGroup foodGroup )
      {
         return (_foodGroups.Find( fg => fg.Name == foodGroup.Name && fg.ID != foodGroup.ID ) != null);
      }

      /// <summary>
      /// Determine if a food item's name is already being used by another food item.
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>True if some other food item is using the name, false otherwise.</returns>
      public Boolean NameIsDuplicate( FoodItem foodItem )
      {
         return (_foodItems.Find( food => food.Name == foodItem.Name && food.ID != foodItem.ID ) != null);
      }

      /// <summary>
      /// Determine if a mealTemplate type's name is being used by some other mealTemplate type 
      /// in the repository.
      /// </summary>
      /// <param name="mealType">The mealTemplate type</param>
      /// <returns>True if some other mealTemplate type in the repository has the same name, false otherwise.</returns>
      public Boolean NameIsDuplicate( MealType mealType )
      {
         return (_mealTypes.Find( mt => mt.Name == mealType.Name && mt.ID != mealType.ID ) != null);
      }

      /// <summary>
      /// Determine if a mealTemplate template's name is already being used by another mealTemplate template.
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template</param>
      /// <returns>True if some other mealTemplate template is using this same name, false otherwise</returns>
      public Boolean NameIsDuplicate( MealTemplate mealTemplate )
      {
         return (_mealTemplates.Find(
            template => template.Name == mealTemplate.Name && template.ID != mealTemplate.ID ) != null);
      }
      #endregion

      #region Private Data
      private List<FoodGroup> _foodGroups;
      private List<FoodItem> _foodItems;
      private List<MealType> _mealTypes;

      /// <summary>
      /// A mealTemplate template is a mealTemplate.  It is just that it is set up to be used as template
      /// for other meals.  That is, if a family often has taco dinners, the mealTemplate can be
      /// set up as a template, and then used to create another mealTemplate that can then be
      /// tweaked as needed.
      /// 
      /// TODO: Meal (base)
      ///       Meal Template - Meal + Name & Description
      ///          
      /// NOTE: May want to look at relaxing the TimeOfMeal requirement for templates.
      ///       This may involve adding an IsTemplate property to the mealTemplate class.
      /// </summary>
      private List<MealTemplate> _mealTemplates;
      private List<Meal> _meals;

      private IConfiguration currentConfiguration;
      #endregion

      #region Constants
      // Node Names
      private const String rootNodeTag = "Data";
      private const String foodGroupsNodeTag = "FoodGroups";
      private const String foodGroupNodeTag = "FoodGroup";
      private const String foodItemsNodeTag = "FoodItems";
      private const String foodItemNodeTag = "FoodItem";
      private const String foodGroupServingsNodeTag = "FoodGroupServings";
      private const String mealTypesNodeTag = "MealTypes";
      private const String mealTypeNodeTag = "MealType";
      private const String mealTemplatesNodeTag = "MealTemplates";
      private const String mealNodeTag = "Meal";
      private const String mealsNodeTag = "Meals";
      private const String foodItemServingsNodeTag = "FoodItemServings";
      private const String descriptionNodeTag = "description";
      private const String servingNodeTag = "Serving";

      // Attributes
      private const String idAttribute = "id";
      private const String nameAttribute = "name";
      private const String caloriesAttribute = "calories";
      private const String quantityAttribute = "quantity";
      private const String mealTypeIdAttribute = "mealTypeId";
      private const String foodGroupIdAttribute = "foodGroupId";
      private const String foodItemIdAttribute = "foodItemId";
      private const String dateTimeAttribute = "datetime";
      private const String useDefaultDateTimeAttribute = "useDefaultDatetime";
      #endregion

      #region Load and Save Repository
      // This is really, really ugly.  It works, but it is not pretty.  Look into changing this.
      private void LoadRepository( String dataFileName )
      {
         XDocument xmlDoc;

         using (FileStream stream = new FileStream( dataFileName, FileMode.Open ))
         {
            using (XmlReader xmlRdr = new XmlTextReader( stream ))
            {
               xmlDoc = XDocument.Load( xmlRdr );

               // If we don't have the root element
               if (xmlDoc.Element( rootNodeTag ) == null)
               {
                  _foodGroups = new List<FoodGroup>();
                  _foodItems = new List<FoodItem>();
                  _mealTypes = new List<MealType>();
                  _mealTemplates = new List<MealTemplate>();
                  _meals = new List<Meal>();
               }
               else
               {
                  if (xmlDoc.Element( rootNodeTag ).Element( foodGroupsNodeTag ) != null)
                  {
                     _foodGroups =
                        (from foodGroupElement in xmlDoc.Element( rootNodeTag ).Element( foodGroupsNodeTag ).Elements( foodGroupNodeTag )
                         select new FoodGroup(
                            (Guid)foodGroupElement.Attribute( idAttribute ),
                            (String)foodGroupElement.Attribute( nameAttribute ),
                            foodGroupElement.Element( descriptionNodeTag ) == null ? null :
                            (String)foodGroupElement.Element( descriptionNodeTag ).Value )).ToList();
                  }
                  else
                  {
                     _foodGroups = new List<FoodGroup>();
                  }

                  // FoodItems
                  _foodItems = new List<FoodItem>();
                  if (xmlDoc.Element( rootNodeTag ).Element( foodItemsNodeTag ) != null)
                  {
                     foreach (XElement foodItemElement in xmlDoc.Element( rootNodeTag ).Element( foodItemsNodeTag ).Elements( foodItemNodeTag ))
                     {
                        // Construct a food item and add it to the collection
                        FoodItem foodItem = new FoodItem( (Guid)foodItemElement.Attribute( idAttribute ),
                           (String)foodItemElement.Attribute( nameAttribute ),
                           foodItemElement.Element( descriptionNodeTag ) == null ? null :
                           (String)foodItemElement.Element( descriptionNodeTag ).Value,
                           (Decimal)foodItemElement.Attribute( caloriesAttribute ) ); // TODO: Should probably do a tryparse on that...
                        _foodItems.Add( foodItem );

                        // Loop through the FoodGroupServings to build that list
                        if (foodItemElement.Element( foodGroupServingsNodeTag ) != null)
                        {
                           foreach (XElement servingElement in foodItemElement.Element( foodGroupServingsNodeTag ).Elements( servingNodeTag ))
                           {
                              Serving<FoodGroup> foodGroupServing = new Serving<FoodGroup>(
                                 _foodGroups.Find( fg => fg.ID == (Guid)servingElement.Attribute( foodGroupIdAttribute ) ),
                                 (Decimal)servingElement.Attribute( quantityAttribute ) );
                              foodItem.FoodGroupsPerServing.Add( foodGroupServing );
                           }
                        }
                     }
                  }

                  if (xmlDoc.Element( rootNodeTag ).Element( mealTypesNodeTag ) != null)
                  {
                     _mealTypes =
                        (from mealTypeElement in xmlDoc.Element( rootNodeTag ).Element( mealTypesNodeTag ).Elements( mealTypeNodeTag )
                         select new MealType(
                            (Guid)mealTypeElement.Attribute( idAttribute ),
                            (String)mealTypeElement.Attribute( nameAttribute ),
                            mealTypeElement.Element( descriptionNodeTag ) == null ? null :
                            (String)mealTypeElement.Element( descriptionNodeTag ).Value,
                            (DateTime)mealTypeElement.Attribute( dateTimeAttribute ),
                            (Boolean)mealTypeElement.Attribute( useDefaultDateTimeAttribute ) )).ToList();
                  }
                  else
                  {
                     _mealTypes = new List<MealType>();
                  }

                  // Meal Templates
                  _mealTemplates = new List<MealTemplate>();
                  if (xmlDoc.Element( rootNodeTag ).Element( mealTemplatesNodeTag ) != null)
                  {
                     foreach (XElement mealTemplateElement in xmlDoc.Element( rootNodeTag ).Element( mealTemplatesNodeTag ).Elements( mealNodeTag ))
                     {
                        // Construct a food item and add it to the collection
                        MealTemplate mealTemplate = new MealTemplate(
                           (Guid)mealTemplateElement.Attribute( idAttribute ),
                           _mealTypes.Find( mt => mt.ID == (Guid)mealTemplateElement.Attribute( mealTypeIdAttribute ) ),
                           (DateTime)mealTemplateElement.Attribute( dateTimeAttribute ),
                           (String)mealTemplateElement.Attribute( nameAttribute ),
                            mealTemplateElement.Element( descriptionNodeTag ) == null ? null :
                            (String)mealTemplateElement.Element( descriptionNodeTag ).Value );
                        _mealTemplates.Add( mealTemplate );

                        // Loop through the FoodItemServings to build that list
                        if (mealTemplateElement.Element( foodItemServingsNodeTag ) != null)
                        {
                           foreach (XElement servingElement in mealTemplateElement.Element( foodItemServingsNodeTag ).Elements( servingNodeTag ))
                           {
                              Serving<FoodItem> foodItem = new Serving<FoodItem>(
                                 _foodItems.Find( fi => fi.ID == (Guid)servingElement.Attribute( foodItemIdAttribute ) ),
                                 (Decimal)servingElement.Attribute( quantityAttribute ) );
                              mealTemplate.FoodItemServings.Add( foodItem );
                           }
                        }
                     }
                  }

                  // Meals
                  _meals = new List<Meal>();
                  if (xmlDoc.Element( rootNodeTag ).Element( mealsNodeTag ) != null)
                  {
                     foreach (XElement mealElement in xmlDoc.Element( rootNodeTag ).Element( mealsNodeTag ).Elements( mealNodeTag ))
                     {
                        // Construct a food item and add it to the collection
                        var meal = new Meal(
                           (Guid)mealElement.Attribute( idAttribute ),
                           _mealTypes.Find( mt => mt.ID == (Guid)mealElement.Attribute( mealTypeIdAttribute ) ),
                           (DateTime)mealElement.Attribute( dateTimeAttribute ),
                           (String)mealElement.Attribute( nameAttribute ),
                            mealElement.Element( descriptionNodeTag ) == null ? null :
                            (String)mealElement.Element( descriptionNodeTag ).Value );
                        _meals.Add( meal );

                        // Loop through the FoodItemServings to build that list
                        if (mealElement.Element( foodItemServingsNodeTag ) != null)
                        {
                           foreach (XElement servingElement in mealElement.Element( foodItemServingsNodeTag ).Elements( servingNodeTag ))
                           {
                              Serving<FoodItem> foodItem = new Serving<FoodItem>(
                                 _foodItems.Find( fi => fi.ID == (Guid)servingElement.Attribute( foodItemIdAttribute ) ),
                                 (Decimal)servingElement.Attribute( quantityAttribute ) );
                              meal.FoodItemServings.Add( foodItem );
                           }
                        }
                     }
                  }
               }
            }
         }
      }

      private void SaveRepository()
      {
         using (FileStream stream = new FileStream( currentConfiguration.FileName, FileMode.Truncate ))
         {
            XmlWriter xmlWriter = XmlWriter.Create( stream );
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement( rootNodeTag );

            // Food Groups
            xmlWriter.WriteStartElement( foodGroupsNodeTag );
            foreach (FoodGroup foodGroup in _foodGroups)
            {
               xmlWriter.WriteStartElement( foodGroupNodeTag );
               xmlWriter.WriteAttributeString( idAttribute, foodGroup.ID.ToString() );
               xmlWriter.WriteAttributeString( nameAttribute, foodGroup.Name );

               xmlWriter.WriteStartElement( descriptionNodeTag );
               xmlWriter.WriteString( foodGroup.Description );
               xmlWriter.WriteEndElement();

               xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            // Meal Types
            xmlWriter.WriteStartElement( mealTypesNodeTag );
            foreach (MealType mealType in _mealTypes)
            {
               xmlWriter.WriteStartElement( mealTypeNodeTag );
               xmlWriter.WriteAttributeString( idAttribute, mealType.ID.ToString() );
               xmlWriter.WriteAttributeString( nameAttribute, mealType.Name );
               xmlWriter.WriteAttributeString( dateTimeAttribute, mealType.DefaultTimeOfMeal.ToString() );
               xmlWriter.WriteAttributeString( useDefaultDateTimeAttribute, mealType.UseDefaultMealTime.ToString() );

               xmlWriter.WriteStartElement( descriptionNodeTag );
               xmlWriter.WriteString( mealType.Description );
               xmlWriter.WriteEndElement();

               xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            // Food Items
            xmlWriter.WriteStartElement( foodItemsNodeTag );
            foreach (FoodItem foodItem in _foodItems)
            {
               xmlWriter.WriteStartElement( foodItemNodeTag );
               xmlWriter.WriteAttributeString( idAttribute, foodItem.ID.ToString() );
               xmlWriter.WriteAttributeString( nameAttribute, foodItem.Name );
               xmlWriter.WriteAttributeString( caloriesAttribute, foodItem.CaloriesPerServing.ToString() );

               xmlWriter.WriteStartElement( descriptionNodeTag );
               xmlWriter.WriteString( foodItem.Description );
               xmlWriter.WriteEndElement();

               xmlWriter.WriteStartElement( foodGroupServingsNodeTag );
               foreach (Serving<FoodGroup> foodGroupServing in foodItem.FoodGroupsPerServing)
               {
                  xmlWriter.WriteStartElement( servingNodeTag );
                  xmlWriter.WriteAttributeString( foodGroupIdAttribute, foodGroupServing.Entity.ID.ToString() );
                  xmlWriter.WriteAttributeString( quantityAttribute, foodGroupServing.Quantity.ToString() );
                  xmlWriter.WriteEndElement();
               }
               xmlWriter.WriteEndElement();

               xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            // Meal Templates
            xmlWriter.WriteStartElement( mealTemplatesNodeTag );
            foreach (MealTemplate mealTemplate in _mealTemplates)
            {
               xmlWriter.WriteStartElement( mealNodeTag );
               xmlWriter.WriteAttributeString( idAttribute, mealTemplate.ID.ToString() );
               xmlWriter.WriteAttributeString( nameAttribute, mealTemplate.Name );
               xmlWriter.WriteAttributeString( mealTypeIdAttribute, mealTemplate.TypeOfMeal.ID.ToString() );
               xmlWriter.WriteAttributeString( dateTimeAttribute, mealTemplate.DateAndTimeOfMeal.ToString() );

               xmlWriter.WriteStartElement( descriptionNodeTag );
               xmlWriter.WriteString( mealTemplate.Description );
               xmlWriter.WriteEndElement();

               xmlWriter.WriteStartElement( foodItemServingsNodeTag );
               foreach (Serving<FoodItem> foodItemServing in mealTemplate.FoodItemServings)
               {
                  xmlWriter.WriteStartElement( servingNodeTag );
                  xmlWriter.WriteAttributeString( foodItemIdAttribute, foodItemServing.Entity.ID.ToString() );
                  xmlWriter.WriteAttributeString( quantityAttribute, foodItemServing.Quantity.ToString() );
                  xmlWriter.WriteEndElement();
               }
               xmlWriter.WriteEndElement();

               xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            // Meals
            xmlWriter.WriteStartElement( mealsNodeTag );
            foreach (Meal meal in _meals)
            {
               xmlWriter.WriteStartElement( mealNodeTag );
               xmlWriter.WriteAttributeString( idAttribute, meal.ID.ToString() );
               xmlWriter.WriteAttributeString( nameAttribute, meal.Name );
               xmlWriter.WriteAttributeString( mealTypeIdAttribute, meal.TypeOfMeal.ID.ToString() );
               xmlWriter.WriteAttributeString( dateTimeAttribute, meal.DateAndTimeOfMeal.ToString() );

               xmlWriter.WriteStartElement( descriptionNodeTag );
               xmlWriter.WriteString( meal.Description );
               xmlWriter.WriteEndElement();

               xmlWriter.WriteStartElement( foodItemServingsNodeTag );
               foreach (Serving<FoodItem> foodItemServing in meal.FoodItemServings)
               {
                  xmlWriter.WriteStartElement( servingNodeTag );
                  xmlWriter.WriteAttributeString( foodItemIdAttribute, foodItemServing.Entity.ID.ToString() );
                  xmlWriter.WriteAttributeString( quantityAttribute, foodItemServing.Quantity.ToString() );
                  xmlWriter.WriteEndElement();
               }
               xmlWriter.WriteEndElement();

               xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
         }
      }
      #endregion
   }
}
