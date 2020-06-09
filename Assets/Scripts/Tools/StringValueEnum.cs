using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class StringValueEnum
{
	public static string GetStringValue(this Enum value)
	{
		// Get the type
		Type type = value.GetType();

		// Get fieldInfo for this type
		FieldInfo fieldInfo = type.GetField(value.ToString());

		// Return the first if there was a match.
		return fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[] attribs && attribs.Length > 0 ? attribs[0].StringValue : null;
	}
}
