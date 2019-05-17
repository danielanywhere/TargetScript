//	Attribute.cs
//	Attribute catalog, collection, and item.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Flee.PublicTypes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	AttributeCatalog																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of AttributeCollection Items.
	/// </summary>
	[JsonConverter(typeof(AttributeCatalogJsonConverter))]
	public class AttributeCatalog : List<AttributeCollection>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//*	GetCollection																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first collection matching the specified expression.
		/// </summary>
		/// <param name="expression">
		/// Basic expression to parse. Any attribute names are square bracketed.
		/// For example, to find a collection where the datatype attribute value
		/// is equal to 'object', use [datatype] = object
		/// </param>
		/// <returns>
		/// Reference to the first matching collection, if found. Otherwise, null.
		/// </returns>
		public AttributeCollection GetCollection(string expression)
		{
			string content = "";
			ExpressionContext context = null;
			IGenericExpression<bool> evaluator = null;
			string expressionText = "";
			StringCollection fieldNames = new StringCollection();
			string keyword = "";
			MatchCollection matches = null;
			//double nValue = 0d;
			AttributeCollection result = null;
			VariableCollection variables = null;

			if(expression?.Length > 0)
			{
				expressionText = expression;
				context = new ExpressionContext();
				variables = context.Variables;
				matches = Regex.Matches(expression,
					ResourceMain.AttributeKeywordPattern);
				foreach(Match match in matches)
				{
					keyword = Tools.GetValue(match, "keyword");
					content = Tools.GetValue(match, "content");
					if(content.Length > 0)
					{
						//	This value is either a field name or a match value.
						if(keyword.Length > 0)
						{
							//	Field name found.
							fieldNames.AddUnique(content);
							expressionText = expressionText.Replace(keyword, "F" + content);
							variables.Add("F" + content, content);
						}
						else
						{
							//	Normal value.
							variables.Add(content, content);
						}
					}
				}
				foreach(AttributeCollection collection in this)
				{
					//	Prepare the local variables.
					foreach(string field in fieldNames)
					{
						variables["F" + field] = collection.GetValue(field);
					}
					evaluator = context.CompileGeneric<bool>(expressionText);
					if(evaluator.Evaluate())
					{
						result = collection;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetCollectionWithMember																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Find and return the first collection containing the specified member.
		/// </summary>
		/// <param name="attributeName">
		/// Name of the attribute to find.
		/// </param>
		/// <returns>
		/// The first matching collection, if found. Otherwise, null.
		/// </returns>
		public AttributeCollection GetCollectionWithMember(string attributeName)
		{
			string nLower = "";
			AttributeCollection result = null;

			if(attributeName?.Length > 0)
			{
				nLower = attributeName.ToLower();
				foreach(AttributeCollection collection in this)
				{
					if(collection.Exists(x => x.Name.ToLower() == nLower))
					{
						result = collection;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValueForExpression																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of an attribute from a collection in the catalog for
		/// a matching value in the expression on the collection.
		/// </summary>
		/// <param name="returnAttributeName">
		/// Attribute name to find on the matching collection.
		/// </param>
		/// <param name="expression">
		/// Expression used to match the desired collection.
		/// </param>
		/// <returns>
		/// Attribute value found, if any.
		/// </returns>
		public string GetValueForExpression(string returnAttributeName,
			string expression)
		{
			string result = "";
			foreach(AttributeCollection attributes in this)
			{
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	AttributeCatalogJsonConverter																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// JSON conversion class.
	/// </summary>
	internal class AttributeCatalogJsonConverter : JsonConverter
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	AssignValues																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign JSON tokens to their list.
		/// </summary>
		/// <param name="list">
		/// Attribute collection to which the tokens will be assigned.
		/// </param>
		/// <param name="tokens">
		/// JSON tokens currently under inspection.
		/// </param>
		public void AssignValues(AttributeCollection list, JToken tokens)
		{
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	CanConvert																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the collection can be created from
		/// the source object.
		/// </summary>
		/// <param name="objectType">
		/// Data type to test.
		/// </param>
		/// <returns>
		/// Value indicating whether the collection can be created from an object
		/// of the specified type.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return typeof(AttributeCatalog).IsAssignableFrom(objectType);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReadJson																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read JSON content from the reader.
		/// </summary>
		/// <param name="reader">
		/// JSON reader.
		/// </param>
		/// <param name="objectType">
		/// Data type of the object being converted.
		/// </param>
		/// <param name="existingValue">
		/// The value being converted.
		/// </param>
		/// <param name="serializer">
		/// JSON serializer.
		/// </param>
		/// <returns>
		/// Converted object.
		/// </returns>
		public override object ReadJson(JsonReader reader, Type objectType,
			object existingValue, JsonSerializer serializer)
		{
			AttributeItem attribute = null;
			AttributeCollection attributes = null;
			JToken jToken = null;
			AttributeCatalog result = new AttributeCatalog();

			try
			{
				jToken = JToken.Load(reader);
			}
			catch { }

			if(jToken != null)
			{
				foreach(JToken obj in jToken)
				{
					attributes = new AttributeCollection();
					result.Add(attributes);
					if(obj.Count() > 0)
					{
						JProperty item = (JProperty)obj.First;
						while(item != null)
						{
							attribute = new AttributeItem();
							attribute.Name = item.Name;
							attribute.Value = item.Value.ToString();
							attributes.Add(attribute);
							item = (JProperty)item.Next;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WriteJson																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Write JSON content to the serializer.
		/// </summary>
		/// <param name="writer">
		/// JSON writer.
		/// </param>
		/// <param name="value">
		/// Object to convert to JSON.
		/// </param>
		/// <param name="serializer">
		/// JSON serializer.
		/// </param>
		public override void WriteJson(JsonWriter writer, object value,
			JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	AttributeCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of AttributeItem Items.
	/// </summary>
	public class AttributeCollection : List<AttributeItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Indexer																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve the specified Attribute from the Collection.
		/// </summary>
		/// <param name="name">
		/// Name of the item to retrieve.
		/// </param>
		public AttributeItem this[string name]
		{
			get
			{
				string nLower = "";
				AttributeItem ro = null;

				if(name != null)
				{
					nLower = name.ToLower();
					foreach(AttributeItem ai in this)
					{
						if(ai.Name.ToLower() == nLower)
						{
							ro = ai;
							break;
						}
					}
				}
				return ro;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an Attribute by member values.
		/// </summary>
		/// <param name="name">
		/// Name of the item to add.
		/// </param>
		/// <param name="value">
		/// Value of the new item.
		/// </param>
		/// <returns>
		/// Newly created and added attribute item.
		/// </returns>
		/// <remarks>
		/// This version allows multiple instances of the same name.
		/// To retain single-instance storage, use the SetValue method.
		/// </remarks>
		public AttributeItem Add(string name, string value)
		{
			AttributeItem result = null;

			if(result == null)
			{
				result = new AttributeItem();
			}
			result.Name = name;
			result.Value = value;
			this.Add(result);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AddUnique																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection if its name is unique.
		/// </summary>
		/// <param name="name">
		/// Name of the attribute.
		/// </param>
		/// <returns>
		/// A new attribute item, if the name was unique in the collection.
		/// Otherwise, a reference to the first matching attribute found.
		/// </returns>
		public AttributeItem AddUnique(string name)
		{
			string nLower = "";
			AttributeItem result = null;

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				if(this.Exists(x => x.Name.ToLower() == nLower))
				{
					result = this.First(x => x.Name.ToLower() == nLower);
				}
				else
				{
					result = new AttributeItem();
					result.Name = name;
					this.Add(result);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add an item to the collection if its name is unique.
		/// </summary>
		/// <param name="name">
		/// Name of the attribute.
		/// </param>
		/// <param name="value">
		/// New value to which a new or existing attribute will be set.
		/// </param>
		/// <returns>
		/// Reference to a newly created attribute in the collection, if its name
		/// was unique, or the first-found previously existing attribute with the
		/// same name.
		/// </returns>
		public AttributeItem AddUnique(string name, string value)
		{
			AttributeItem result = AddUnique(name);
			if(result != null)
			{
				result.Value = value;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CloneItems																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value-wise clone of all items in the specified collection.
		/// </summary>
		/// <param name="attributes">
		/// Collection of attributes to clone.
		/// </param>
		/// <returns>
		/// List of cloned attribute items.
		/// </returns>
		public static List<AttributeItem> CloneItems(
			AttributeCollection attributes)
		{
			AttributeItem item = null;
			List<AttributeItem> result = new List<AttributeItem>();

			if(attributes?.Count > 0)
			{
				foreach(AttributeItem attribute in attributes)
				{
					item = new AttributeItem();
					item.Name = attribute.Name;
					item.Value = attribute.Value;
					result.Add(item);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified attribute in the collection.
		/// </summary>
		/// <param name="name">
		/// Name of the attribute to find.
		/// </param>
		/// <returns>
		/// The value of the specified attribute, if found. Otherwise, empty
		/// string.
		/// </returns>
		public string GetValue(string name)
		{
			string nLower = "";
			string result = "";

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				foreach(AttributeItem item in this)
				{
					if(item.Name.ToLower() == nLower)
					{
						result = item.Value;
						break;
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the specified attribute in the collection.
		/// </summary>
		/// <param name="name">
		/// Name of the attribute to find.
		/// </param>
		/// <param name="index">
		/// 0-based repeat index of the named item. For example, if there are
		/// two param items, GetValue("param", 1) will return the second one.
		/// </param>
		/// <returns>
		/// The value of the specified attribute, if found. Otherwise, empty
		/// string.
		/// </returns>
		public string GetValue(string name, int index)
		{
			int fIndex = 0;
			string nLower = "";
			string result = "";

			if(name?.Length > 0)
			{
				foreach(AttributeItem item in this)
				{
					if(item.Name.ToLower() == nLower)
					{
						if(fIndex == index)
						{
							result = item.Value;
							break;
						}
						fIndex++;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValues																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of values of the specified attribute in the collection.
		/// </summary>
		/// <param name="name">
		/// Name of the attribute to find.
		/// </param>
		/// <returns>
		/// Collection of value of the specified attribute, if found. Otherwise,
		/// zero length collection.
		/// </returns>
		public List<string> GetValues(string name)
		{
			List<string> result = new List<string>();
			foreach(AttributeItem item in this)
			{
				if(item.Name == name)
				{
					result.Add(item.Value);
					break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RemoveName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the item with the specified name from the collection.
		/// </summary>
		/// <param name="name">
		/// Name of the item to remove.
		/// </param>
		public void RemoveName(string name)
		{
			AttributeItem item = this[name];

			if(item != null)
			{
				this.Remove(item);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RemoveNameStarting																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the item with the specified name from the collection.
		/// </summary>
		/// <param name="partialName">
		/// First part of the name of the item to remove.
		/// </param>
		public void RemoveNameStarting(string partialName)
		{
			int aCount = this.Count;
			int aIndex = 0;
			AttributeItem item = null;
			string nLower = "";

			if(partialName?.Length > 0)
			{
				nLower = partialName.ToLower();
				for(aIndex = 0; aIndex < aCount; aIndex ++)
				{
					item = this[aIndex];
					if(item.Name.ToLower().StartsWith(nLower))
					{
						this.Remove(item);
						aIndex--;
						aCount--;
					}
				}
				this.Remove(item);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the attribute having the specified name.
		/// </summary>
		/// <param name="name">
		/// Name of the item to set.
		/// </param>
		/// <param name="value">
		/// Value to place in the discovered item.
		/// </param>
		/// <returns>
		/// Reference to a new AttributeItem, if the item did not already exist,
		/// or a pre-existing AttributeItem, if a matching name was found.
		/// </returns>
		public AttributeItem SetValue(string name, string value)
		{
			return AddUnique(name, value);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SortByName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Sort the contents of the collection by name.
		/// </summary>
		/// <param name="attributes">
		/// Collection of attributes to sort.
		/// </param>
		/// <param name="ascending">
		/// Value indicating whether to sort in ascending order.
		/// </param>
		/// <returns>
		/// Newly sorted collection of attributes.
		/// </returns>
		public static AttributeCollection SortByName(
			AttributeCollection attributes,
			bool ascending = true)
		{
			//	The following varable is used because C# doesn't allow covariance,
			//	and the result has to be coerced back into the form of the
			//	original collection in which it was created. :-S
			List<AttributeItem> transient = null;
			AttributeCollection result = null;

			if(attributes != null)
			{
				if(ascending)
				{
					transient = attributes.
						OrderBy(x => x.Name).ToList();
				}
				else
				{
					transient = attributes.
						OrderByDescending(x => x.Name).ToList();
				}
				if(transient != null)
				{
					result = new AttributeCollection();
					foreach(AttributeItem item in transient)
					{
						result.Add(item);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	AttributeItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an Attribute.
	/// </summary>
	public class AttributeItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the Name of this Item.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the Value of this Item.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
