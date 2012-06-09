using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HealthTracker.DataRepository.Models;
using HealthTracker.DataRepository.Services;

namespace HealthTracker.DataRepository.Interfaces
{
   public interface IDataRepository
   {
      #region Events
      event EventHandler<RepositoryObjectEventArgs> ItemAdded;
      event EventHandler<RepositoryObjectEventArgs> ItemDeleted;
      event EventHandler<RepositoryObjectEventArgs> ItemModified;
      #endregion

      #region Get and Find Routines
      FoodGroup FindFoodGroup( Predicate<FoodGroup> match );

      FoodItem FindFoodItem( Predicate<FoodItem> match );

      Meal FindMeal( Predicate<Meal> match );

      MealTemplate FindMealTemplate( Predicate<MealTemplate> match );

      MealType FindMealType( Predicate<MealType> match );

      /// <summary>
      /// Returns a shallow copied list of the food groups that are in the repository
      /// </summary>
      ReadOnlyCollection<FoodGroup> GetAllFoodGroups();

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      ReadOnlyCollection<FoodItem> GetAllFoodItems();

      /// <summary>
      /// Returns a shallow copied list of the meals that are in the repository
      /// </summary>
      ReadOnlyCollection<Meal> GetAllMeals();

      /// <summary>
      /// Returns a shallow copied list of the meals eaten on a specific date that are in the repository
      /// </summary>
      ReadOnlyCollection<Meal> GetAllMealsForDate( DateTime date );

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      ReadOnlyCollection<MealTemplate> GetAllMealTemplates();

      /// <summary>
      /// Returns a shallow copied list of the mealTemplate types that are in the repository
      /// </summary>
      ReadOnlyCollection<MealType> GetAllMealTypes();

      FoodGroup GetFoodGroup( Guid id );

      FoodItem GetFoodItem( Guid id );

      Meal GetMeal( Guid id );

      MealTemplate GetMealTemplate( Guid id );

      MealType GetMealType( Guid id );
      #endregion

      #region Delete and Save Items
      /// <summary>
      /// Add a food group to the repository.
      /// </summary>
      /// <param name="foodGroup">The food group to add</param>
      void SaveItem( FoodGroup foodGroup );

      /// <summary>
      /// Add a food item to the repository
      /// </summary>
      /// <param name="foodItem">The food item</param>
      void SaveItem( FoodItem foodItem );

      void SaveItem( Meal meal );

      void SaveItem( MealTemplate mealTemplate );

      /// <summary>
      /// Add a mealTemplate type to the repository.
      /// </summary>
      /// <param name="mealType">The mealTemplate type to add</param>
      void SaveItem( MealType mealType );

      /// <summary>
      /// Remove a food group from the repository
      /// </summary>
      /// <param name="foodGroup">The food group to delete</param>
      void Remove( FoodGroup foodGroup );

      /// <summary>
      /// Remove a food item from the repository
      /// </summary>
      /// <param name="foodItem">The food item</param>
      void Remove( FoodItem foodItem );

      /// <summary>
      /// Remove a mealTemplate from the repository.
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate to remove.</param>
      void Remove( Meal meal );

      /// <summary>
      /// Remove a mealTemplate template from the repository.
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template to remove.</param>
      void Remove( MealTemplate mealTemplate );

      /// <summary>
      /// Remove a mealTemplate type from the repository
      /// </summary>
      /// <param name="mealType">The mealTemplate type to delete</param>
      void Remove( MealType mealType );
      #endregion

      #region Contains
      /// <summary>
      /// Determine if the repository contains a food group with the same ID.
      /// </summary>
      /// <param name="foodGroup">The food group</param>
      /// <returns>True of the food group is in the repository, false otherwise</returns>
      Boolean Contains( FoodGroup foodGroup );

      /// <summary>
      /// Determine if the repository contains a mealTemplate type.
      /// </summary>
      /// <param name="foodGroup">The mealTemplate type</param>
      /// <returns>True of the mealTemplate type is in the repository, false otherwise</returns>
      Boolean Contains( MealType mealType );

      /// <summary>
      /// Determine if the repository already contains the food item
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>Returns true if the food item is alread in the repository, false oterwise.</returns>
      Boolean Contains( FoodItem foodItem );

      /// <summary>
      /// Determine if the repository contains the given mealTemplate
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate to look for</param>
      /// <returns>True of the mealTemplate is in the repository, false otherwise</returns>
      Boolean Contains( Meal meal );

      /// <summary>
      /// Determine if the repository contains the given mealTemplate template
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template to look for</param>
      /// <returns>True of the mealTemplate template is in the repository, false otherwise</returns>
      Boolean Contains( MealTemplate mealTemplate );
      #endregion

      #region Editing Routines
      /// <summary>
      /// Determine if a food group is referenenced somewhere.
      /// </summary>
      /// <param name="foodGroup">The food group to look for</param>
      /// <returns>True if the food group is in use, false otherwise</returns>
      Boolean ItemIsUsed( FoodGroup foodGroup );

      /// <summary>
      /// Determine if a mealTemplate type is referenenced somewhere.
      /// </summary>
      /// <param name="foodGroup">The mealTemplate type to look for</param>
      /// <returns>True if the mealTemplate type is in use, false otherwise</returns>
      Boolean ItemIsUsed( MealType mealType );

      /// <summary>
      /// Determine if a food item is currently referenced by some other entitiy in the repository.
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>True if used, false otherwise.</returns>
      Boolean ItemIsUsed( FoodItem foodItem );

      /// <summary>
      /// Determine if a mealTemplate template is referenced elsewhere in the repository
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template</param>
      /// <returns>Always returns false</returns>
      Boolean ItemIsUsed( MealTemplate mealTemplate );

      /// <summary>
      /// Determine if a food group's name is being used by some other food group 
      /// in the repository.
      /// </summary>
      /// <param name="foodGroup">The food group</param>
      /// <returns>True if some other food group in the repository has the same name, false otherwise.</returns>
      Boolean NameIsDuplicate( FoodGroup foodGroup );

      /// <summary>
      /// Determine if a food item's name is already being used by another food item.
      /// </summary>
      /// <param name="foodItem">The food item</param>
      /// <returns>True if some other food item is using the name, false otherwise.</returns>
      Boolean NameIsDuplicate( FoodItem foodItem );

      /// <summary>
      /// Determine if a mealTemplate type's name is being used by some other mealTemplate type 
      /// in the repository.
      /// </summary>
      /// <param name="foodGroup">The mealTemplate type</param>
      /// <returns>True if some other mealTemplate type in the repository has the same name, false otherwise.</returns>
      Boolean NameIsDuplicate( MealType mealType );

      /// <summary>
      /// Determine if a mealTemplate template's name is already being used by another mealTemplate template.
      /// </summary>
      /// <param name="mealTemplate">The mealTemplate template</param>
      /// <returns>True if some other mealTemplate template is using this same name, false otherwise</returns>
      Boolean NameIsDuplicate( MealTemplate mealTemplate );
      #endregion
   }
}
