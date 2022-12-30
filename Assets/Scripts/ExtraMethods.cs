using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;


// I took 3 classes
// last 2 are abombinations
internal static class ExtraMethods
{
	internal static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
	{
		int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
		int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
		int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

		int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
		int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
		int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

		T[][] jaggedArray = new T[numberOfRows][];
		for (int i = 0; i < numberOfRows; i++)
		{
			jaggedArray[i] = new T[numberOfColumns];

			for (int j = 0; j < numberOfColumns; j++)
			{
				jaggedArray[i][j] = twoDimensionalArray[i + rowsFirstIndex, j + columnsFirstIndex];
			}
		}
		return jaggedArray;
	}
	
	internal static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue)
		where TEnum : struct/*, IConvertible*/
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}

		TEnum result;
		return Enum.TryParse<TEnum>(value, true, out result) ? result : defaultValue;
	}
}

[Serializable]
public class SerializationCallbackScript<TKey, TValue> : MonoBehaviour, ISerializationCallbackReceiver
{
	public List<TKey> _keys = new List<TKey> {  };
	public List<TValue> _values = new List<TValue> { };

	//Unity doesn't know how to serialize a Dictionary
	public Dictionary<TKey, TValue> _myDictionary = new Dictionary<TKey, TValue>();

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();

		foreach (var kvp in _myDictionary)
		{
			_keys.Add(kvp.Key);
			_values.Add(kvp.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		_myDictionary = new Dictionary<TKey, TValue>();

		for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
			_myDictionary.Add(_keys[i], _values[i]);
	}

	void OnGUI()
	{
		foreach (var kvp in _myDictionary)
			GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
	}
}