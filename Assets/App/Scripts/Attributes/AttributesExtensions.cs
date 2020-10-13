using System;
using System.Reflection;

namespace Scripts.Attributes
{
	public static class AttributesExtensions
	{
		public static T GetCustomAttribute<T>(Type actualClassThatPropertySitOn, string nameOfFieldOrProperty)
			where  T : System.Attribute
		{
			var fieldInfo = actualClassThatPropertySitOn
				.GetField(nameOfFieldOrProperty, BindingFlags.NonPublic | BindingFlags.Instance);
			if (fieldInfo != null)
			{
				return fieldInfo.GetCustomAttribute<T>();
			}

			var propertyInfo = actualClassThatPropertySitOn
				.GetProperty(nameOfFieldOrProperty, BindingFlags.NonPublic | BindingFlags.Instance);
			if (propertyInfo != null)
			{
				return propertyInfo.GetCustomAttribute<T>();
			}

			return null;
		}
	}
}