using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets.Editor.Factories
{
  public interface ITypesProvider
  {
    List<Type> GetClassesWithAttribute<T>() where T : Attribute;
    List<ISheetDataContainer> CreateSheetDataContainers(List<Type> types);
    List<Type> GetClassesWithAttribute<T>(Func<T, bool> condition) where T : Attribute;
  }

  public class TypesProvider : ITypesProvider
  {
    public List<Type> GetClassesWithAttribute<T>() where T : Attribute
    {
      var list = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assembly
          .GetTypes()
          .Where(type => (type.GetCustomAttribute(typeof(T)) is T))
          .ToList()
          .ForEach(x => list.Add(x));
      }

      return list;
    }

    public List<Type> GetClassesWithAttribute<T>(Func<T, bool> condition) where T : Attribute
    {
      var list = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assembly
          .GetTypes()
          .Where(type => (type.GetCustomAttribute(typeof(T)) is T attribute) && condition.Invoke(attribute))
          .ToList()
          .ForEach(x => list.Add(x));
      }

      return list;
    }

    public List<ISheetDataContainer> CreateSheetDataContainers(List<Type> types)
    {
      var list = new List<ISheetDataContainer>();
      types.ForEach(x =>
      {
        var constructedType = typeof(SpreadSheetDataContainer<>).MakeGenericType(x);
        var dataContainer = Activator.CreateInstance(constructedType) as ISheetDataContainer;
        list.Add(dataContainer);
      });

      return list;
    }
  }
}