using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ExpressionElement																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual element of the expression tree collection.
	/// </summary>
	public class ExpressionElement
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ExpressionElement Item.
		/// </summary>
		public ExpressionElement()
		{
			mElements = new ExpressionElementCollection(this);
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ExpressionElement Item.
		/// </summary>
		public ExpressionElement(ExpressionElementCollection parent) : this()
		{
			mParent = parent;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Builder																																*
		//*-----------------------------------------------------------------------*
		private StringBuilder mBuilder = new StringBuilder();
		/// <summary>
		/// Get a reference to the active value builder for this element.
		/// </summary>
		public StringBuilder Builder
		{
			get { return mBuilder; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ClearText																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the text content of the specified element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element for which the text will be cleared.
		/// </param>
		public static void ClearText(ExpressionElement element)
		{
			if(element != null)
			{
				if(element.mBuilder.Length > 0)
				{
					element.mBuilder.Remove(0, element.mBuilder.Length);
				}
				element.mParameterType = ParameterTypeEnum.None;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value-wise clone of the specified item.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to clone.
		/// </param>
		/// <returns>
		/// Value-wise clone of the caller's element.
		/// </returns>
		public static ExpressionElement Clone(ExpressionElement element)
		{
			ExpressionElement result = new ExpressionElement();

			if(element != null)
			{
				result.mBuilder.Append(element.mBuilder);
				//	Avoiding AddRange to avoid parent reference issues.
				foreach(ExpressionElement child in element.mElements)
				{
					result.mElements.Add(ExpressionElement.Clone(child));
				}
				result.mId = element.mId;
				result.mIndex = element.mIndex;
				result.mParameterType = element.mParameterType;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetName																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the name portion of an indirect value, function, single
		/// name:value entry, or single word.
		/// </summary>
		/// <param name="element">
		/// Reference to the element containing the calue to retrieve.
		/// </param>
		/// <returns>
		/// Value corresponding to the concept of a within the element builder.
		/// </returns>
		public static string GetName(ExpressionElement element)
		{
			Match match = null;
			string result = "";
			string text = "";

			if(element != null && element.mBuilder.Length > 0)
			{
				text = element.mBuilder.ToString();
				if((element.ParameterType & (ParameterTypeEnum.Command |
					ParameterTypeEnum.IndirectConfig |
					ParameterTypeEnum.IndirectMeta)) != 0)
				{
					//	Function or indirect.
					match = Regex.Match(text,
						ResourceMain.ExpressionKeywordNamePattern);
					if(match.Success)
					{
						result = Tools.GetValue(match, "name");
					}
				}
				else if((element.ParameterType & ParameterTypeEnum.NameValue) > 0 &&
					(element.ParameterType & ParameterTypeEnum.List) == 0)
				{
					//	Name:Value without list.
				}
				else
				{
					//	Check to see if this is a single word.
					match = Regex.Match(text,
						ResourceMain.ExpressionSingleWord);
					if(match.Success)
					{
						result = Tools.GetValue(match, "word");
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetParameters																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the full parameter text for the specified element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element for which the parameters will be retrieved.
		/// </param>
		/// <returns>
		/// Full text of the parameters for this item.
		/// </returns>
		/// <remarks>
		/// This element is assumed to take a shape similar to or matching a
		/// command, where only one set of parenthesis is found on this line. If
		/// there are multiple parenthesis, the parameters are considered to be
		/// the information inside the first set. If there are no parenthesis,
		/// an empty string is returned.<br />
		/// Before calling this method, it is assumed that all dependent
		/// variables and levels have already been resolved.
		/// </remarks>
		public static string GetParameters(ExpressionElement element)
		{
			int index = 0;
			Match match = null;
			string result = "";

			if(element != null)
			{
				match = Regex.Match(GetText(element),
					ResourceMain.ExpressionParameterPattern);
				if(match.Success)
				{
					//	Parameter tag found.
					int.TryParse(Tools.GetValue(match, "index"), out index);
					result = Render(element.Elements[index]);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetParentTextLeftOf																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the parent text to the left of the the reference to the
		/// specified item.
		/// </summary>
		/// <param name="element">
		/// </param>
		/// <returns>
		/// </returns>
		public static string GetParentTextLeftOf(ExpressionElement element)
		{
			ExpressionElement parent = null;
			string result = "";
			string tag = "";
			string text = "";

			if(element != null)
			{
				parent = element.ParentElement;
				if(parent != null)
				{
					text = GetText(parent);
					tag = GetReference(element);
					if(text.IndexOf(tag) > -1)
					{
						result = text.Substring(0, text.IndexOf(tag));
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetParentTextRightOf																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the parent text to the right of the the reference to the
		/// specified item.
		/// </summary>
		/// <param name="element">
		/// </param>
		/// <returns>
		/// </returns>
		public static string GetParentTextRightOf(ExpressionElement element)
		{
			ExpressionElement parent = null;
			string result = "";
			string tag = "";
			string text = "";

			if(element != null)
			{
				parent = element.ParentElement;
				if(parent != null)
				{
					text = GetText(parent);
					tag = GetReference(element);
					if(text.IndexOf(tag) > -1)
					{
						result = text.Substring(text.IndexOf(tag) + tag.Length);
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetReference																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the tag value within the parent element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to inspect.
		/// </param>
		/// <returns>
		/// Exact text of the tag identifying this value.
		/// </returns>
		public static string GetReference(ExpressionElement element)
		{
			int index = 0;
			MatchCollection matches = null;
			ExpressionElement parent = null;
			string result = "";
			string text = "";
			ExpressionElement value = null;

			if(element != null && element.ParentElement != null)
			{
				parent = element.ParentElement;
				text = GetText(parent);
				matches = Regex.Matches(text,
					ResourceMain.ExpressionPlaceholderPattern);
				foreach(Match match in matches)
				{
					//	Variable placeholders were found.
					int.TryParse(Tools.GetValue(match, "index"), out index);
					value = parent.Elements[index];
					if(value == element)
					{
						//	The current tag is the placeholder for the element.
						result = Tools.GetValue(match, "placeholder");
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the text value of the element.
		/// </summary>
		public static string GetText(ExpressionElement element)
		{
			string result = "";

			if(element != null)
			{
				result = element.mBuilder.ToString();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Id																																		*
		//*-----------------------------------------------------------------------*
		private int mId = 0;
		/// <summary>
		/// Get/Set the ID of this element within the expression structure.
		/// </summary>
		public int Id
		{
			get { return mId; }
			set { mId = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Index																																	*
		//*-----------------------------------------------------------------------*
		private int mIndex = 0;
		/// <summary>
		/// Get/Set the starting character index of this pattern within the
		/// original string.
		/// </summary>
		public int Index
		{
			get { return mIndex; }
			set { mIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Elements																															*
		//*-----------------------------------------------------------------------*
		private ExpressionElementCollection mElements = null;
		/// <summary>
		/// Get/Set a reference to the collection of child elements on this
		/// element.
		/// </summary>
		public ExpressionElementCollection Elements
		{
			get { return mElements; }
			set { mElements = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ParameterType																													*
		//*-----------------------------------------------------------------------*
		private ParameterTypeEnum mParameterType = ParameterTypeEnum.None;
		/// <summary>
		/// Get/Set the type of parameter discovered at this level.
		/// </summary>
		public ParameterTypeEnum ParameterType
		{
			get { return mParameterType; }
			set { mParameterType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ExpressionElementCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the collection of which this item is a member.
		/// </summary>
		public ExpressionElementCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ParentElement																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a reference to the parent element of this item.
		/// </summary>
		/// <remarks>
		/// In the case of any indirect variable, the parent element contains the
		/// relative and absolute tag references to this item.
		/// </remarks>
		public ExpressionElement ParentElement
		{
			get
			{
				ExpressionElement result = null;
				if(mParent != null)
				{
					result = mParent.Parent;
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Render																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Rebuild the text from the caller's expression.
		/// </summary>
		/// <param name="element">
		/// Reference to the expression element at which to start parsing.
		/// </param>
		/// <returns>
		/// Current form of expression text available within the specified object.
		/// </returns>
		public static string Render(ExpressionElement element)
		{
			ExpressionElement child = null;
			int index = 0;
			MatchCollection matches = null;
			string result = "";

			if(element != null)
			{
				result = GetText(element);
				matches = Regex.Matches(result, ResourceMain.ExpressionTagPattern);
				foreach(Match match in matches)
				{
					index = int.Parse(Tools.GetValue(match, "index"));
					child = element.Elements[index];
					result = result.Replace(Tools.GetValue(match, "tag"), Render(child));
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Override the text value of the specified element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to set.
		/// </param>
		/// <param name="value">
		/// Value to set in the Builder property.
		/// </param>
		public static void SetText(ExpressionElement element, string value)
		{
			if(element != null)
			{
				//	Builder will not receive a null value.
				if(element.mBuilder.Length > 0)
				{
					element.mBuilder.Remove(0, element.mBuilder.Length);
				}
				if(value?.Length > 0)
				{
					element.mBuilder.Append(value);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ExpressionElementCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ExpressionElement Items.
	/// </summary>
	public class ExpressionElementCollection : List<ExpressionElement>
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ExpressionElementCollection Item.
		/// </summary>
		public ExpressionElementCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ExpressionElementCollection Item.
		/// </summary>
		public ExpressionElementCollection(ExpressionElement parent)
		{
			mParent = parent;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new element to the collection and return it to the caller.
		/// </summary>
		/// <returns>
		/// Reference to the newly created and added element.
		/// </returns>
		public ExpressionElement Add()
		{
			ExpressionElement result = new ExpressionElement(this);
			base.Add(result);
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add a new item to the collection.
		/// </summary>
		/// <param name="value">
		/// Reference to the expression element to be added.
		/// </param>
		public new void Add(ExpressionElement value)
		{
			if(value != null && value.Parent == null)
			{
				value.Parent = this;
			}
			base.Add(value);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ExpressionElement mParent = null;
		/// <summary>
		/// Get/Set a reference to the element of which this collection is a
		/// member.
		/// </summary>
		public ExpressionElement Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ExpressionElementEventArgs																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the element item receiving changes.
	/// </summary>
	public class ExpressionElementEventArgs
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ExpressionElementEventArgs Item.
		/// </summary>
		public ExpressionElementEventArgs()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ExpressionElementEventArgs Item.
		/// </summary>
		/// <param name="element">
		/// Reference to the expression element to be handled.
		/// </param>
		public ExpressionElementEventArgs(ExpressionElement element)
		{
			mElement = element;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Element																																*
		//*-----------------------------------------------------------------------*
		private ExpressionElement mElement = null;
		/// <summary>
		/// Get/Set a reference to the element being handled.
		/// </summary>
		public ExpressionElement Element
		{
			get { return mElement; }
			set { mElement = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Handled																																*
		//*-----------------------------------------------------------------------*
		private bool mHandled = false;
		/// <summary>
		/// Get/Set a value indicating whether this event has been handled.
		/// </summary>
		public bool Handled
		{
			get { return mHandled; }
			set { mHandled = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parameters																														*
		//*-----------------------------------------------------------------------*
		private string mParameters = "";
		/// <summary>
		/// Get/Set a parameter string to be sent with command-based events.
		/// </summary>
		public string Parameters
		{
			get { return mParameters; }
			set { mParameters = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RequestName																														*
		//*-----------------------------------------------------------------------*
		private string mRequestName = "";
		/// <summary>
		/// Get/Set the request name for this event.
		/// </summary>
		public string RequestName
		{
			get { return mRequestName; }
			set { mRequestName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResultValue																														*
		//*-----------------------------------------------------------------------*
		private string mResultValue = "";
		/// <summary>
		/// Get/Set the resulting value.
		/// </summary>
		/// <remarks>
		/// This value will be ignored if Handled is false.
		/// </remarks>
		public string ResultValue
		{
			get { return mResultValue; }
			set { mResultValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ExpressionElementEventHandler																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Event handler for expression tree element item events.
	/// </summary>
	/// <param name="sender">
	/// The object raising this event.
	/// </param>
	/// <param name="e">
	/// Expression treen element item event arguments.
	/// </param>
	public delegate void ExpressionElementEventHandler(object sender,
		ExpressionElementEventArgs e);
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ExpressionEventBoard																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Connection board that allows all expression element events to be
	/// registered from a single driving location.
	/// </summary>
	public class ExpressionEventBoard
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
		//*	GetConfigValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raise the RequestConfigValue event to retrieve the configuration
		/// value for the element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to analyze.
		/// </param>
		/// <param name="name">
		/// The name for which the request is being made.
		/// </param>
		/// <returns>
		/// The referenced configuration value, if it was found. Otherwise, an
		/// empty string.
		/// </returns>
		public string GetConfigValue(ExpressionElement element,
			string name)
		{
			ExpressionElementEventArgs ea = null;
			string result = "";

			if(RequestConfigValue != null)
			{
				ea = new ExpressionElementEventArgs();
				ea.Element = element;
				ea.RequestName = name;
				RequestConfigValue(this, ea);
				if(ea.Handled)
				{
					result = ea.ResultValue;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetMetadataValue																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raise the RequestMetadataValue event to retrieve the metadata value for
		/// the element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to analyze.
		/// </param>
		/// <param name="name">
		/// The name for which the request is being made.
		/// </param>
		/// <returns>
		/// The referenced configuration value, if it was found. Otherwise, an
		/// empty string.
		/// </returns>
		public string GetMetadataValue(ExpressionElement element, string name)
		{
			ExpressionElementEventArgs ea = null;
			string result = "";

			if(RequestMetadataValue != null)
			{
				ea = new ExpressionElementEventArgs();
				ea.Element = element;
				ea.RequestName = name;
				RequestMetadataValue(this, ea);
				if(ea.Handled)
				{
					result = ea.ResultValue;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RequestConfigValue																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when a configuration value is being requested for an expression
		/// element.
		/// </summary>
		public event ExpressionElementEventHandler RequestConfigValue;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RequestMetadataValue																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when a metadata value is being requested for an expression
		/// element.
		/// </summary>
		public event ExpressionElementEventHandler RequestMetadataValue;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RequestRunCommand																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when a command needs to be run.
		/// </summary>
		public event ExpressionElementEventHandler RequestRunCommand;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RunCommand																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raise the RequestRunCommand event to retrieve function result for the
		/// specified element.
		/// </summary>
		/// <param name="element">
		/// Reference to the element to analyze.
		/// </param>
		/// <param name="name">
		/// The name for which the request is being made.
		/// </param>
		/// <param name="parameters">
		/// Parameters to submit with the command.
		/// </param>
		/// <returns>
		/// The function return value, if it was found. Otherwise, an
		/// empty string.
		/// </returns>
		public string RunCommand(ExpressionElement element, string name,
			string parameters)
		{
			ExpressionElementEventArgs ea = null;
			string result = "";

			if(RequestRunCommand != null)
			{
				ea = new ExpressionElementEventArgs();
				ea.Element = element;
				ea.RequestName = name;
				ea.Parameters = parameters;
				RequestRunCommand(this, ea);
				if(ea.Handled)
				{
					result = ea.ResultValue;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ExpressionTree																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tree of expression elements.
	/// </summary>
	public class ExpressionTree : ExpressionElementCollection
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	EliminateFalseVariables																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Review the caller's expression and eliminate any items marked as
		/// variables but don't follow naming conventions.
		/// </summary>
		/// <param name="expression">
		/// Reference to the expression to review.
		/// </param>
		private static void EliminateFalseVariables(
			ExpressionElementCollection expression)
		{
			string text = "";

			if(expression?.Count > 0)
			{
				foreach(ExpressionElement element in expression)
				{
					if((element.ParameterType & ParameterTypeEnum.IndirectConfig) != 0)
					{
						//	Configuration value.
						text = ExpressionElement.GetText(element);
						if(!Regex.IsMatch(text,
							ResourceMain.ExpressionKeywordConfigPattern))
						{
							//	This value doesn't match naming conventions.
							element.ParameterType &= ~ParameterTypeEnum.IndirectConfig;
						}
					}
					if((element.ParameterType & ParameterTypeEnum.IndirectMeta) != 0)
					{
						//	Metadata value.
						text = ExpressionElement.GetText(element);
						if(!Regex.IsMatch(text,
							ResourceMain.ExpressionKeywordMetadataPattern))
						{
							//	This value doesn't match naming conventions.
							element.ParameterType &= ~ParameterTypeEnum.IndirectMeta;
						}
					}
					if((element.ParameterType & ParameterTypeEnum.Command) != 0)
					{
						//	Command value.
						text = ExpressionElement.GetText(element);
						if(!Regex.IsMatch(text,
							ResourceMain.ExpressionKeywordCommandPattern))
						{
							//	This value doesn't match naming conventions.
							element.ParameterType &= ~ParameterTypeEnum.Command;
						}
					}
					EliminateFalseVariables(element.Elements);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InitializeAttributes																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the attributes list for the entire base expression.
		/// </summary>
		/// <param name="elements">
		/// Reference to the collection of elements to inspect.
		/// </param>
		/// <param name="attributes">
		/// Reference to the collection of attributes to which any values will be
		/// appended.
		/// </param>
		private static void InitializeAttributes(
			ExpressionElementCollection elements, AttributeCollection attributes)
		{
			//int id = 0;
			//int index = 0;
			//MatchCollection matches = null;
			string[] pair = new string[0];
			char[] pDelim = new char[] { ':' };
			int sCount = 0;
			char[] sDelim = new char[] { ';' };
			string[] set = new string[0];
			int sIndex = 0;
			//Match test = null;
			string text = "";
			string value = "";

			if(elements?.Count > 0 && attributes != null)
			{
				foreach(ExpressionElement element in elements)
				{
					text = ExpressionElement.GetText(element);
					if(element.Builder.Length > 0)
					{
						//	Resolve name:value lists.
						if((element.ParameterType & ParameterTypeEnum.NameValue) != 0)
						{
							//	Names and values were found.
							value = ExpressionElement.GetText(element);
							if((element.ParameterType & ParameterTypeEnum.List) != 0)
							{
								//	List.
								set = value.Split(sDelim);
								sCount = set.Length;
								for(sIndex = 0; sIndex < sCount; sIndex ++)
								{
									pair = set[sIndex].Split(pDelim);
									if(pair.Length > 1)
									{
										attributes.SetValue(pair[0].Trim(), pair[1]);
									}
								}
							}
							else
							{
								//	Single pair.
								pair = value.Split(pDelim);
								attributes.SetValue(pair[0].Trim(), pair[1]);
							}
						}
					}
					InitializeAttributes(element.Elements, attributes);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RestoreEscapes																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Restore any escaped values that were hidden during parsing.
		/// </summary>
		/// <param name="expression">
		/// Reference to a collection of expression elements to review.
		/// </param>
		private static void RestoreEscapes(ExpressionElementCollection expression)
		{
			string expressionL = "";
			string text = "";

			if(expression?.Count > 0)
			{
				foreach(ExpressionElement element in expression)
				{
					text = ExpressionElement.GetText(element);
					if(text.IndexOf("^~") > -1 || text.IndexOf("^|") > -1)
					{
						//	Possible hidden escape.
						do
						{
							expressionL = text;
							text = Regex.Replace(text,
								ResourceMain.ExpressionEscapeDPattern,
								ResourceMain.ExpressionEscapeDReplacement);
						} while(text != expressionL);
						do
						{
							expressionL = text;
							text = Regex.Replace(text,
								ResourceMain.ExpressionEscapeTPattern,
								ResourceMain.ExpressionEscapeTReplacement);
						} while(text != expressionL);
						ExpressionElement.SetText(element, text);
					}
					if(element.Elements.Count > 0)
					{
						RestoreEscapes(element.Elements);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StoreEscapes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Hide any escaped brackets inside the text so they are not mistaked as
		/// variable placeholders.
		/// </summary>
		/// <param name="expression">
		/// The raw expression to inspect.
		/// </param>
		/// <returns>
		/// Caller's expression with any escaped brackets hidden from the parser.
		/// </returns>
		private static string StoreEscapes(string expression)
		{
			string expressionL = "";
			string result = "";

			if(expression?.Length > 0)
			{
				result = expression;
				do
				{
					expressionL = result;
					result = Regex.Replace(result,
						ResourceMain.ExpressionEscapeCPattern,
						ResourceMain.ExpressionEscapeCReplacement);
				} while(result != expressionL);
				do
				{
					expressionL = result;
					result = Regex.Replace(result,
						ResourceMain.ExpressionEscapeSPattern,
						ResourceMain.ExpressionEscapeSReplacement);
				} while(result != expressionL);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ExpressionTree Item.
		/// </summary>
		public ExpressionTree()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ExpressionTree Item.
		/// </summary>
		/// <param name="eventBoard">
		/// Reference to a common event board handling all expressions.
		/// </param>
		public ExpressionTree(ExpressionEventBoard eventBoard)
		{
			mEventBoard = eventBoard;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActionNode																														*
		//*-----------------------------------------------------------------------*
		private ActionNodeItem mActionNode = null;
		/// <summary>
		/// Get/Set a reference to the action node being serviced by this
		/// expression.
		/// </summary>
		public ActionNodeItem ActionNode
		{
			get { return mActionNode; }
			set { mActionNode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Attributes																														*
		//*-----------------------------------------------------------------------*
		private AttributeCollection mAttributes = new AttributeCollection();
		/// <summary>
		/// Get/Set a reference to the collection of resolved name/value
		/// assignments on this item.
		/// </summary>
		public AttributeCollection Attributes
		{
			get { return mAttributes; }
			set { mAttributes = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value-wise clone of the caller's expression.
		/// </summary>
		public static ExpressionTree Clone(ExpressionTree expression)
		{
			ExpressionTree result = new ExpressionTree();

			if(expression != null)
			{
				result.mActionNode = expression.mActionNode;
				result.mAttributes.AddRange(
					AttributeCollection.CloneItems(expression.mAttributes));
				result.mEventBoard = expression.mEventBoard;
				foreach(ExpressionElement element in expression)
				{
					result.Add(ExpressionElement.Clone(element));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EventBoard																														*
		//*-----------------------------------------------------------------------*
		private ExpressionEventBoard mEventBoard = null;
		/// <summary>
		/// Get/Set a reference to the event board handling events for this
		/// expression line.
		/// </summary>
		public ExpressionEventBoard EventBoard
		{
			get { return mEventBoard; }
			set { mEventBoard = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FindElement																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the element matching the specified text.
		/// </summary>
		/// <param name="expression">
		/// Reference to the expression containing the specified element.
		/// </param>
		/// <param name="elementText">
		/// Text to match on element.
		/// </param>
		/// <returns>
		/// A reference to the element having the matching text, if found.
		/// Otherwise, null.
		/// </returns>
		public static ExpressionElement FindElement(
			ExpressionElementCollection expression,
			string elementText)
		{
			ExpressionElement result = null;

			if(expression?.Count > 0 && elementText?.Length > 0)
			{
				foreach(ExpressionElement element in expression)
				{
					if(ExpressionElement.GetText(element) == elementText)
					{
						result = element;
						break;
					}
					if(result == null)
					{
						result = FindElement(element.Elements, elementText);
						if(result != null)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetActionNode																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the closest action node associated with the caller's expression
		/// element.
		/// </summary>
		/// <param name="element">
		/// Reference to an expression element to trace.
		/// </param>
		/// <returns>
		/// Reference to the action node associated with the base expression of
		/// the caller's element.
		/// </returns>
		public static ActionNodeItem GetActionNode(ExpressionElement element)
		{
			ActionNodeItem result = null;

			if(element != null)
			{
				result = GetActionNode(element.Parent);
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the closest action node associated with the caller's expression
		/// element collection.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to trace for action node at the base.
		/// </param>
		/// <returns>
		/// Reference to the action node associated with the base expression of
		/// the caller's element.
		/// </returns>
		public static ActionNodeItem GetActionNode(
			ExpressionElementCollection elements)
		{
			ActionNodeItem result = null;

			if(elements != null)
			{
				if(elements is ExpressionTree)
				{
					result = ((ExpressionTree)elements).mActionNode;
				}
				else
				{
					result = GetActionNode(elements.Parent);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetCommand																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first matching instance of the specified command.
		/// </summary>
		/// <param name="elements">
		/// Reference to a collection of elements to search for the specified
		/// command.
		/// </param>
		/// <param name="name">
		/// Name of the command to search for.
		/// </param>
		/// <returns>
		/// The first matching command of the specified name, searching by depth
		/// first, then occurrence. If the named command is not found, null is
		/// returned.
		/// </returns>
		public static ExpressionElement GetCommand(
			ExpressionElementCollection elements, string name)
		{
			int dCount = 0;     //	Total depth.
			int dIndex = 0;     //	Current depth.
			ExpressionElement result = null;

			if(elements?.Count > 0 && name?.Length > 0)
			{
				dCount = GetDepth(elements);
				for(dIndex = dCount - 1; dIndex > -1; dIndex--)
				{
					result = GetCommand(elements, name, dIndex);
					if(result != null)
					{
						break;
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the first matching instance of the specified command.
		/// </summary>
		/// <param name="elements">
		/// Reference to a collection of elements to search for the specified
		/// command.
		/// </param>
		/// <param name="name">
		/// Name of the command to search for.
		/// </param>
		/// <param name="depth">
		/// Depth at which to search for the command.
		/// </param>
		/// <returns>
		/// The first matching command of the specified name, searching by depth
		/// first, then occurrence. If the named command is not found, null is
		/// returned.
		/// </returns>
		private static ExpressionElement GetCommand(
			ExpressionElementCollection elements, string name, int depth)
		{
			int innerdepth = depth - 1;
			string nLower = "";
			ExpressionElement result = null;

			if(elements?.Count > 0 && depth > -1 && name?.Length > 0)
			{
				nLower = name.ToLower();
				foreach(ExpressionElement item in elements)
				{
					if(depth == 0)
					{
						//	Caller is asking for items at this level.
						if((item.ParameterType & ParameterTypeEnum.Command) != 0)
						{
							//	This is a command.
							if(ExpressionElement.GetName(item).ToLower() == nLower)
							{
								//	Match found.
								result = item;
								break;
							}
						}
					}
					else
					{
						//	Request is for elements at a deeper level.
						result = GetCommand(item.Elements, name, innerdepth);
						if(result != null)
						{
							//	Match found.
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetCommandNames																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of command names found in the expression, deepest level
		/// first, then by order of occurrence.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements at which to begin inspecting.
		/// </param>
		/// <returns>
		/// List of command names found.
		/// </returns>
		public static List<string> GetCommandNames(
			ExpressionElementCollection elements)
		{
			int dCount = 0;			//	Total depth.
			int dIndex = 0;			//	Current depth.
			List<string> result = new List<string>();

			if(elements?.Count > 0)
			{
				dCount = GetDepth(elements);
				for(dIndex = dCount - 1; dIndex > -1; dIndex --)
				{
					result.AddRange(GetCommandNames(elements, dIndex));
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the list of command names found at the caller-specified level
		/// within the collection.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to inspect.
		/// </param>
		/// <param name="depth">
		/// Depth from which to pull the values.
		/// </param>
		/// <returns>
		/// List of commands found at the specified level.
		/// </returns>
		private static List<string> GetCommandNames(
			ExpressionElementCollection elements, int depth)
		{
			int innerdepth = depth - 1;
			List<string> result = new List<string>();

			if(elements?.Count > 0 && depth > -1)
			{
				foreach(ExpressionElement item in elements)
				{
					if(depth == 0)
					{
						//	Caller is asking for items at this level.
						if((item.ParameterType & ParameterTypeEnum.Command) != 0)
						{
							//	This is a command.
							result.Add(ExpressionElement.GetName(item));
						}
					}
					else
					{
						//	Request is for elements at a deeper level.
						result.AddRange(GetCommandNames(item.Elements, innerdepth));
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetCommands																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the list of command elements found at the caller-specified level
		/// within the collection.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to inspect.
		/// </param>
		/// <param name="depth">
		/// Depth from which to pull the values.
		/// </param>
		/// <returns>
		/// List of commands found at the specified level.
		/// </returns>
		private static List<ExpressionElement> GetCommands(
			ExpressionElementCollection elements, int depth)
		{
			int innerdepth = depth - 1;
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(elements?.Count > 0 && depth > -1)
			{
				foreach(ExpressionElement item in elements)
				{
					if(depth == 0)
					{
						//	Caller is asking for items at this level.
						if((item.ParameterType &
							ParameterTypeEnum.Command) != 0)
						{
							//	This is a variable.
							result.Add(item);
						}
					}
					else
					{
						//	Request is for elements at a deeper level.
						result.AddRange(GetCommands(item.Elements, innerdepth));
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the resolvable commands for the specified expression.
		/// </summary>
		/// <param name="expression">
		/// The expression tree for which the variables will be found.
		/// </param>
		/// <returns>
		/// List of variables found in the expression, ordered by inner items
		/// first, then by order of occurrence.
		/// </returns>
		/// <remarks>
		/// This method returns the first order of variables. Variables embedded
		/// in values found here are resolved through additional expression
		/// instances.
		/// </remarks>
		public static List<ExpressionElement> GetCommands(
			ExpressionTree expression)
		{
			int dCount = 0;     //	Total depth.
			int dIndex = 0;     //	Current depth.
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(expression != null)
			{
				dCount = GetDepth(expression);
				for(dIndex = dCount - 1; dIndex > -1; dIndex--)
				{
					result.AddRange(GetCommands(expression, dIndex));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetDepth																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the maximum depth of the specified expression.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to test for depth.
		/// </param>
		/// <returns>
		/// Maximum depth in the expression tree.
		/// </returns>
		public static int GetDepth(ExpressionElementCollection elements)
		{
			int depth = 0;
			int result = 0;

			if(elements?.Count > 0)
			{
				foreach(ExpressionElement element in elements)
				{
					depth = GetDepth(element.Elements) + 1;
					result = Math.Max(result, depth);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetElement																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Find and return the referenced element from the provided expression.
		/// </summary>
		/// <param name="expression">
		/// Reference to the full expression to be searched.
		/// </param>
		/// <param name="tag">
		/// Name of the tag representing the element to retrieve.
		/// </param>
		/// <returns>
		/// The specified expression element, if found. Otherwise, null.
		/// </returns>
		public static ExpressionElement GetElement(
			ExpressionElementCollection expression,
			string tag)
		{
			int index = 0;
			MatchCollection matches = null;
			ExpressionElement result = null;

			if(expression?.Count > 0 && tag?.Length > 0)
			{
				foreach(ExpressionElement element in expression)
				{
					matches = Regex.Matches(ExpressionElement.GetText(element),
						ResourceMain.ExpressionTagPattern);
					foreach(Match match in matches)
					{
						if(Tools.GetValue(match, "tag") == tag)
						{
							index = int.Parse(Tools.GetValue(match, "index"));
							result = element.Elements[index];
							break;
						}
					}
					if(result == null)
					{
						//	Drill down.
						result = GetElement(element.Elements, tag);
						if(result != null)
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetInnerValues																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the list of discrete inner value elements found at the
		/// caller-specified level within the collection.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to inspect.
		/// </param>
		/// <param name="depth">
		/// Depth from which to pull the values.
		/// </param>
		/// <returns>
		/// List of inner value elements found at the specified level.
		/// </returns>
		private static List<ExpressionElement> GetInnerValues(
			ExpressionElementCollection elements, int depth)
		{
			int innerdepth = depth - 1;
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(elements?.Count > 0 && depth > -1)
			{
				foreach(ExpressionElement item in elements)
				{
					if(depth == 0)
					{
						//	Caller is asking for items at this level.
						result.Add(item);
					}
					else
					{
						//	Request is for elements at a deeper level.
						result.AddRange(GetInnerValues(item.Elements, innerdepth));
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the list of discrete inner value elements for the specified
		/// expression.
		/// </summary>
		/// <param name="expression">
		/// The expression tree for which the variables will be found.
		/// </param>
		/// <returns>
		/// List of elements found in the expression, ordered by inner items
		/// first, then by order of occurrence.
		/// </returns>
		public static List<ExpressionElement> GetInnerValues(
			ExpressionTree expression)
		{
			int dCount = 0;     //	Total depth.
			int dIndex = 0;     //	Current depth.
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(expression != null)
			{
				dCount = GetDepth(expression);
				for(dIndex = dCount - 1; dIndex > -1; dIndex--)
				{
					result.AddRange(GetInnerValues(expression, dIndex));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the complete text, as it appears with all placeholders intact.
		/// </summary>
		public static string GetText(ExpressionTree expression)
		{
			StringBuilder result = new StringBuilder();

			if(expression != null)
			{
				foreach(ExpressionElement element in expression)
				{
					result.Append(ExpressionElement.GetText(element));
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetVariables																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the list of variable elements found at the caller-specified level
		/// within the collection.
		/// </summary>
		/// <param name="elements">
		/// Collection of elements to inspect.
		/// </param>
		/// <param name="depth">
		/// Depth from which to pull the values.
		/// </param>
		/// <returns>
		/// List of variables found at the specified level.
		/// </returns>
		private static List<ExpressionElement> GetVariables(
			ExpressionElementCollection elements, int depth)
		{
			int innerdepth = depth - 1;
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(elements?.Count > 0 && depth > -1)
			{
				foreach(ExpressionElement item in elements)
				{
					if(depth == 0)
					{
						//	Caller is asking for items at this level.
						if((item.ParameterType &
							(ParameterTypeEnum.IndirectConfig |
							ParameterTypeEnum.IndirectMeta)) != 0)
						{
							//	This is a variable.
							result.Add(item);
						}
					}
					else
					{
						//	Request is for elements at a deeper level.
						result.AddRange(GetVariables(item.Elements, innerdepth));
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the resolvable variables for the specified expression.
		/// </summary>
		/// <param name="expression">
		/// The expression tree for which the variables will be found.
		/// </param>
		/// <returns>
		/// List of variables found in the expression, ordered by inner items
		/// first, then by order of occurrence.
		/// </returns>
		/// <remarks>
		/// This method returns the first order of variables. Variables embedded
		/// in values found here are resolved through additional expression
		/// instances.
		/// </remarks>
		public static List<ExpressionElement> GetVariables(
			ExpressionTree expression)
		{
			int dCount = 0;     //	Total depth.
			int dIndex = 0;     //	Current depth.
			List<ExpressionElement> result = new List<ExpressionElement>();

			if(expression != null)
			{
				dCount = GetDepth(expression);
				for(dIndex = dCount - 1; dIndex > -1; dIndex--)
				{
					result.AddRange(GetVariables(expression, dIndex));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HasVariables																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified expression has one or
		/// more of the variables of the specified types.
		/// </summary>
		/// <param name="expression">
		/// A reference to the expression to test.
		/// </param>
		/// <param name="type">
		/// The parameter types to test for. This is a maskable value.
		/// </param>
		/// <returns>
		/// True if any of the types were found in the expression. Otherwise,
		/// false.
		/// </returns>
		public static bool HasVariables(ExpressionElementCollection expression,
			ParameterTypeEnum type)
		{
			bool result = false;

			if(expression?.Count > 0 && type != ParameterTypeEnum.None)
			{
				foreach(ExpressionElement element in expression)
				{
					if((element.ParameterType & type) != 0)
					{
						result = true;
						break;
					}
					else
					{
						if(element.Elements.Count > 0)
						{
							result = HasVariables(element.Elements, type);
							if(result)
							{
								break;
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the caller's expression and return the corresponding expression
		/// tree.
		/// </summary>
		/// <param name="expression">
		/// Expression to parse.
		/// </param>
		/// <param name="board">
		/// Reference to the expression board used to wire up all expressions to
		/// a single handler.
		/// </param>
		/// <returns>
		/// Resulting expression tree.
		/// </returns>
		public static ExpressionTree Parse(string expression,
			ExpressionEventBoard board)
		{
			int aIndex = 0;
			char cchar = '\0';
			char[] echar = new char[0];
			int eCount = 0;
			int eIndex = 0;
			ExpressionElement element = null;
			string expressionText = "";
			ExpressionTree result = new ExpressionTree(board);
			string value = "";

			if(expression?.Length > 0)
			{
				//	Hide the escaped brackets.
				expressionText = StoreEscapes(expression);
				echar = expressionText.ToCharArray();
				eCount = echar.Length;
				if(eCount > 0)
				{
					element = new ExpressionElement(result);
					result.Add(element);
				}
				for(eIndex = 0; eIndex < eCount; eIndex ++)
				{
					cchar = echar[eIndex];
					switch(cchar)
					{
						case '(':
							element.Builder.Append(cchar);
							element.Builder.Append("@");
							element.Builder.Append(element.Elements.Count.ToString());
							element.Builder.Append("#");
							element.Builder.Append(aIndex.ToString());
							if((element.ParameterType & ParameterTypeEnum.IndirectConfig) != 0)
							{
								//	Change from config value to function type.
								element.ParameterType &= ~ParameterTypeEnum.IndirectConfig;
								element.ParameterType |= ParameterTypeEnum.Command;
								if(element.Builder.ToString().ToLower().StartsWith("{loopstart"))
								{
									//	Default level for loops is component.
									result.Attributes.SetValue("level", "component");
								}
							}
							element = element.Elements.Add();
							element.Index = eIndex + 1;
							element.Id = aIndex;
							aIndex++;
							break;
						case '[':
							element.Builder.Append("@");
							element.Builder.Append(element.Elements.Count.ToString());
							element.Builder.Append("#");
							element.Builder.Append(aIndex.ToString());
							element = element.Elements.Add();
							element.Id = aIndex;
							element.Index = eIndex;
							element.Builder.Append(cchar);
							element.ParameterType |= ParameterTypeEnum.IndirectMeta;
							aIndex++;
							break;
						case '{':
							element.Builder.Append("@");
							element.Builder.Append(element.Elements.Count.ToString());
							element.Builder.Append("#");
							element.Builder.Append(aIndex.ToString());
							element = element.Elements.Add();
							element.Id = aIndex;
							element.Index = eIndex;
							element.Builder.Append(cchar);
							element.ParameterType |= ParameterTypeEnum.IndirectConfig;
							aIndex++;
							break;
						case ')':
							if(element.Parent != null && element.Parent.Parent != null)
							{
								element = element.Parent.Parent;
							}
							element.Builder.Append(cchar);
							break;
						case ']':
							value = ExpressionElement.GetText(element).ToLower();
							element.Builder.Append(cchar);
							if(element.Parent != null && element.Parent.Parent != null)
							{
								element = element.Parent.Parent;
							}
							break;
						case '}':
							//	Before closing completely, make sure to handle well-known
							//	function names that have no parenthesis.
							value = ExpressionElement.GetText(element).ToLower();
							switch(value)
							{
								case "{conditionend":
								case "{continue":
								case "{loopend":
									//	Change from config value to command type.
									element.ParameterType &= ~ParameterTypeEnum.IndirectConfig;
									element.ParameterType |= ParameterTypeEnum.Command;
									break;
								case "{decindent":
								case "{incindent":
									//	Change from config value to delayed command.
									element.ParameterType &= ~ParameterTypeEnum.IndirectConfig;
									element.ParameterType |= ParameterTypeEnum.DelayedCommand;
									//	Use empty parenthesis for post processing.
									element.Builder.Append("()");
									break;
							}
							element.Builder.Append(cchar);
							if(element.Parent != null && element.Parent.Parent != null)
							{
								element = element.Parent.Parent;
							}
							break;
						case ',':
							element.Builder.Append(cchar);
							element.ParameterType |= ParameterTypeEnum.Delimited;
							break;
						case ':':
							element.Builder.Append(cchar);
							element.ParameterType |= ParameterTypeEnum.NameValue;
							break;
						case ';':
							element.Builder.Append(cchar);
							element.ParameterType |= ParameterTypeEnum.List;
							break;
						default:
							element.Builder.Append(cchar);
							break;
					}
				}
				//	Unhide any escaped brackets.
				RestoreEscapes(result);
				EliminateFalseVariables(result);
			}
			InitializeAttributes(result, result.Attributes);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Reduce																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reduce the entire expression using the available background resource
		/// support.
		/// </summary>
		public string Reduce()
		{
			bool bRepeat = false;
			ExpressionTree clone = null;
			string commandName = "";
			string expressionText = "";
			string name = "";
			ActionNodeItem node = null;
			string parameters = "";
			string result = "";
			string text = "";
			string value = "";
			List<ExpressionElement> variables = null;

			if(HasVariables(this,
				ParameterTypeEnum.Command |
				ParameterTypeEnum.IndirectConfig |
				ParameterTypeEnum.IndirectMeta))
			{
				clone = Clone(this);
				if(mEventBoard != null)
				{
					//	Values can be resolved by the host system.
					//	Resolve all of the variables first, in to out, left to right.
					bRepeat = true;
					while(bRepeat)
					{
						bRepeat = false;
						//	Process all variables for the current state.
						variables = GetVariables(clone);
						foreach(ExpressionElement variable in variables)
						{
							text = ExpressionElement.GetText(variable);
							name = ExpressionElement.GetReference(variable);
							if((variable.ParameterType &
								ParameterTypeEnum.IndirectConfig) != 0)
							{
								//	Configuration value.
								value = mEventBoard.GetConfigValue(variable,
									ExpressionElement.GetText(variable));
							}
							else if(
								(variable.ParameterType & ParameterTypeEnum.IndirectMeta) != 0)
							{
								//	Metadata value.
								value = mEventBoard.GetMetadataValue(variable,
									ExpressionElement.GetText(variable));
							}
							if(name?.Length > 0)
							{
								if(variable.ParentElement != null)
								{
									text = ExpressionElement.GetText(variable.ParentElement);
									text = text.Replace(name, value);
									ExpressionElement.SetText(variable.ParentElement, text);
									ExpressionElement.ClearText(variable);
								}
								else
								{
									text = ExpressionElement.GetText(variable);
									text = text.Replace(name, value);
									ExpressionElement.SetText(variable, text);
								}
							}
						}
						variables = GetCommands(clone);
						foreach(ExpressionElement variable in variables)
						{
							name = ExpressionElement.GetReference(variable);
							commandName = ExpressionElement.GetName(variable);
							if(name?.Length > 0 && commandName?.Length > 0)
							{
								if((variable.ParameterType & ParameterTypeEnum.Command) != 0)
								{
									//	Command.
									parameters = ExpressionElement.GetParameters(variable);
									value = mEventBoard.RunCommand(variable,
										commandName, parameters);
								}
								if(variable.ParentElement != null)
								{
									text = ExpressionElement.GetText(variable.ParentElement);
									text = text.Replace(name, value);
									ExpressionElement.SetText(variable.ParentElement, text);
									ExpressionElement.ClearText(variable);
								}
								else
								{
									text = ExpressionElement.GetText(variable);
									text = text.Replace(name, value);
									ExpressionElement.SetText(variable, text);
								}
							}
						}
						expressionText = ExpressionTree.Render(clone);
						if(expressionText.Length > 0)
						{
							node = clone.mActionNode;
							if(node != null)
							{
								InitializeAttributes(clone, node.Attributes);
								clone = Parse(expressionText, clone.mEventBoard);
								clone.mActionNode = node;
								if(HasVariables(clone,
									ParameterTypeEnum.Command |
									ParameterTypeEnum.IndirectConfig |
									ParameterTypeEnum.IndirectMeta))
								{
									bRepeat = true;
								}
							}
						}
						else if(mAttributes.Count > 0)
						{
							if(mAttributes.GetValue(":canskip:") == "1")
							{
								mAttributes.SetValue(":skip:", "1");
							}
						}
					}
					//	When repeats are done, the entire value should rest in the
					//	builder of the base item of the collection.
					if(clone.Count > 0)
					{
						result = Render(clone);
					}
				}
				else
				{
					//	If no event board exists, the transient values can only be solved
					//	by attribute.
				}
			}
			else
			{
				//	No variables found.
				result = Render(this);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Render																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Rebuild the text from the caller's expression.
		/// </summary>
		/// <param name="expression">
		/// Reference to the expression to parse.
		/// </param>
		/// <returns>
		/// Current form of expression text available within the specified object.
		/// </returns>
		public static string Render(ExpressionElementCollection expression)
		{
			ExpressionElement child = null;
			int index = 0;
			MatchCollection matches = null;
			StringBuilder result = new StringBuilder();
			string text = "";

			if(expression?.Count > 0)
			{
				foreach(ExpressionElement element in expression)
				{
					text = ExpressionElement.GetText(element);
					matches = Regex.Matches(text, ResourceMain.ExpressionTagPattern);
					foreach(Match match in matches)
					{
						index = int.Parse(Tools.GetValue(match, "index"));
						child = element.Elements[index];
						text = text.Replace(Tools.GetValue(match, "tag"),
							ExpressionElement.Render(child));
					}
					result.Append(text);
				}
			}
			return result.ToString();
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Custom render an output string based upon information provided in the
		/// caller's template.
		/// </summary>
		/// <param name="expression">
		/// </param>
		/// <param name="template">
		/// </param>
		/// <returns>
		/// Custom rendered expression, based on values in the existing expression
		/// and caller's template.
		/// </returns>
		public static string Render(ExpressionElementCollection expression,
			string template)
		{
			MatchCollection matches = null;
			string result = "";
			string tag = "";

			if(expression?.Count > 0 && template?.Length > 0)
			{
				result = template;
				matches = Regex.Matches(template, ResourceMain.ExpressionTagPattern);
				foreach(Match match in matches)
				{
					tag = Tools.GetValue(match, "tag");
					result = result.Replace(tag,
						ExpressionElement.Render(GetElement(expression, tag)));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
