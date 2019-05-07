//	ActionNode.cs
//	Node parsing operations for document rendering.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Flee.PublicTypes;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ActionNodeCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ActionNodeItem Items.
	/// </summary>
	public class ActionNodeCollection : List<ActionNodeItem>
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
		/// Create a new Instance of the ActionNodeCollection Item.
		/// </summary>
		public ActionNodeCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ActionNodeCollection Item.
		/// </summary>
		/// <param name="parent">
		/// Reference to the parent node to which this item is associated.
		/// </param>
		public ActionNodeCollection(ActionNodeItem parent)
		{
			mParent = parent;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item being added.
		/// </param>
		public new void Add(ActionNodeItem item)
		{
			if(item.Parent == null)
			{
				item.Parent = this;
			}
			base.Add(item);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetAncestorNodeWithParam																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first node up the chain that has a matching parameter value.
		/// </summary>
		/// <param name="list">
		/// Reference to the collection of existing nodes to analyze.
		/// </param>
		/// <param name="paramName">
		/// Name of the parameter to search for.
		/// </param>
		/// <param name="paramIndex">
		/// Enumeration index of the named parameter.
		/// </param>
		/// <param name="paramValue">
		/// Value of the parameter to match.
		/// </param>
		/// <returns>
		/// Reference to ancestor node, if found. Otherwise, null.
		/// </returns>
		public static ActionNodeItem GetAncestorNodeWithParam(
			ActionNodeCollection list, string paramName,
			int paramIndex, string paramValue)
		{
			string attributeValue = "";
			ActionNodeItem parent = null;
			ActionNodeItem result = null;
			string vLower = "";

			if(list != null && list.mParent != null && paramName?.Length > 0)
			{
				vLower = paramValue.ToLower();
				parent = list.mParent;
				if(parent.Attributes.Count(x => x.Name.ToLower() == paramName) >
					paramIndex)
				{
					//	There are enough params in this item.
					attributeValue =
						parent.Attributes.GetValue(paramName, paramIndex).ToLower();
					if(attributeValue == vLower)
					{
						result = parent;
					}
				}
				if(result == null && parent.Parent != null)
				{
					result = GetAncestorNodeWithParam(parent.Parent,
						paramName, paramIndex, paramValue);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the first node up the chain that has a matching parameter value.
		/// </summary>
		/// <param name="list">
		/// Reference to the collection of existing nodes to analyze.
		/// </param>
		/// <param name="paramName">
		/// Name of the parameter to search for.
		/// </param>
		/// <param name="paramValue">
		/// Value of the parameter to match.
		/// </param>
		/// <returns>
		/// Reference to ancestor node, if found. Otherwise, null.
		/// </returns>
		public static ActionNodeItem GetAncestorNodeWithParam(
			ActionNodeCollection list, string paramName,
			string paramValue)
		{
			string attributeValue = "";
			ActionNodeItem parent = null;
			ActionNodeItem result = null;
			string vLower = "";

			if(list != null && list.mParent != null && paramName?.Length > 0)
			{
				vLower = paramValue.ToLower();
				parent = list.mParent;
				if(parent.Attributes.Count(x => x.Name.ToLower() == paramName) > 0)
				{
					//	There are enough params in this item.
					attributeValue =
						parent.Attributes.GetValue(paramName).ToLower();
					if(attributeValue == vLower)
					{
						result = parent;
					}
				}
				if(result == null && parent.Parent != null)
				{
					result = GetAncestorNodeWithParam(parent.Parent,
						paramName, paramValue);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the first node up the chain that has a matching parameter value.
		/// </summary>
		/// <param name="list">
		/// Reference to the collection of existing nodes to analyze.
		/// </param>
		/// <param name="paramList">
		/// List of parameter Name:Value entries to match.
		/// </param>
		/// <returns>
		/// Reference to ancestor node, if found. Otherwise, null.
		/// </returns>
		public static ActionNodeItem GetAncestorNodeWithParam(
			ActionNodeCollection list, string paramList)
		{
			bool bContinue = true;
			ActionNodeItem parent = null;
			string[][] pArray = null;
			ActionNodeItem result = null;

			if(list != null && list.mParent != null && paramList?.Length > 0)
			{
				bContinue = true;
				parent = list.mParent;
				pArray = Tools.GetParameterNameValueList(paramList);
				foreach(string[] namevalue in pArray)
				{
					bContinue = false;
					if(namevalue[0].Length == 0)
					{
						bContinue = true;
					}
					else if(parent.Attributes.
						Exists(x => x.Name.ToLower() == namevalue[0].ToLower()))
					{
						bContinue = true;
					}
					if(!bContinue)
					{
						break;
					}
				}
				if(bContinue)
				{
					//	The list matches in this node.
					result = parent;
				}
				if(result == null && parent.Parent != null)
				{
					result = GetAncestorNodeWithParam(parent.Parent, pArray);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the first node up the chain that has a matching parameter value.
		/// </summary>
		/// <param name="list">
		/// Reference to the collection of existing nodes to analyze.
		/// </param>
		/// <param name="paramArray">
		/// Array of parameter Name:Value entries to match.
		/// </param>
		/// <returns>
		/// Reference to ancestor node, if found. Otherwise, null.
		/// </returns>
		public static ActionNodeItem GetAncestorNodeWithParam(
			ActionNodeCollection list, string[][] paramArray)
		{
			bool bContinue = true;
			ActionNodeItem parent = null;
			ActionNodeItem result = null;

			if(list != null && list.mParent != null && paramArray?.Length > 0)
			{
				bContinue = true;
				parent = list.mParent;
				foreach(string[] namevalue in paramArray)
				{
					bContinue = false;
					if(namevalue[0].Length == 0)
					{
						bContinue = true;
					}
					else if(parent.Attributes.
						Exists(x => x.Name.ToLower() == namevalue[0].ToLower()))
					{
						bContinue = true;
					}
					if(!bContinue)
					{
						break;
					}
				}
				if(bContinue)
				{
					//	The list matches in this node.
					result = parent;
				}
				if(result == null && parent.Parent != null)
				{
					result = GetAncestorNodeWithParam(parent.Parent, paramArray);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ActionNodeItem mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent node.
		/// </summary>
		public ActionNodeItem Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ActionNodeItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A single node representing an action and related conditions.
	/// </summary>
	/// <remarks>
	/// The scope of this item includes this item and all parent items.
	/// </remarks>
	public class ActionNodeItem
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
		/// Create a new Instance of the ActionNodeItem Item.
		/// </summary>
		public ActionNodeItem()
		{
			mNodes = new ActionNodeCollection(this);
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ActionNodeItem Item.
		/// </summary>
		/// <param name="parent">
		/// Reference to the collection of nodes of which this item is a member.
		/// </param>
		public ActionNodeItem(ActionNodeCollection parent) : this()
		{
			mParent = parent;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Attributes																														*
		//*-----------------------------------------------------------------------*
		private AttributeCollection mAttributes = new AttributeCollection();
		/// <summary>
		/// Get a reference to the collection of attribute values on this node.
		/// </summary>
		public AttributeCollection Attributes
		{
			get { return mAttributes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HasLineEnd																														*
		//*-----------------------------------------------------------------------*
		private bool mHasLineEnd = true;
		/// <summary>
		/// Get/Set a value indicating whether this item has an implicit line end
		/// when outputting to text.
		/// </summary>
		/// <remarks>
		/// This value is true by default.
		/// </remarks>
		public bool HasLineEnd
		{
			get { return mHasLineEnd; }
			set { mHasLineEnd = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Nodes																																	*
		//*-----------------------------------------------------------------------*
		private ActionNodeCollection mNodes = null;
		/// <summary>
		/// Get a reference to the collection of nodes.
		/// </summary>
		public ActionNodeCollection Nodes
		{
			get { return mNodes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ActionNodeCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection.
		/// </summary>
		public ActionNodeCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the line value of this item.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ActionNodeTree																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Action node collection with project resources.
	/// </summary>
	public class ActionNodeTree : ActionNodeCollection
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Reference to the currently selected component.
		/// </summary>
		private ComponentItem mComponent = null;
		/// <summary>
		/// Stack of component / entry follower settings.
		/// </summary>
		private Stack<ComponentEntryFollower> mComponentEntryStack =
			new Stack<ComponentEntryFollower>();
		/// <summary>
		/// Selected collection of entry values.
		/// </summary>
		private AttributeCollection mEntryCollection = null;
		/// <summary>
		/// Internal indent tracking.
		/// </summary>
		private int mIndent = 0;
		/// <summary>
		///	Current node under test.
		/// </summary>
		private ActionNodeItem mNode = null;

		//*-----------------------------------------------------------------------*
		//*	AddParameters																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set any parameters for this item.
		/// </summary>
		/// <param name="match">
		/// Regular expression match containing the values to add.
		/// </param>
		/// <param name="node">
		/// Currently selected template node.
		/// </param>
		/// <param name="paramGroupName">
		/// Name of the match group to process.
		/// </param>
		private static void AddParameters(Match match, ActionNodeItem node,
			string paramGroupName)
		{
			string[][] pItems = new string[0][];
			string value = "";

			if(match != null && node != null && paramGroupName?.Length > 0)
			{
				value = Tools.GetValue(match, paramGroupName);
				pItems = Tools.GetParameterNameValueList(value);
				foreach(string[] namevalue in pItems)
				{
					if(namevalue[0].Length > 0)
					{
						node.Attributes.Add(namevalue[0], namevalue[1]);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AnalyzeCommands																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Analyze all of the commands found in the expression.
		/// </summary>
		/// <param name="values">
		/// Collection of resolved variables. Also contains the current expression
		/// to be reduced.
		/// </param>
		/// <param name="expression">
		/// The expression to analyze.
		/// </param>
		/// <returns>
		/// Value indicating whether the expression was fully processed.
		/// </returns>
		private bool AnalyzeCommands(AttributeCollection values, string expression)
		{
			string command = "";
			string expressionText = "";
			MatchCollection matches = null;
			string param = "";
			bool result = false;
			string statement = "";

			if(values != null)
			{
				if(expression?.Length > 0)
				{
					expressionText = expression;
					matches = Regex.Matches(expression,
						ResourceMain.CommandEvalPattern);
					foreach(Match match in matches)
					{
						statement = Tools.GetValue(match, "command");
						command = Tools.GetValue(match, "name");
						param = Tools.GetValue(match, "params");
						mEntryCollection.Add(command + "()", "");
						AnalyzeInnerVariables(param);
						expressionText = Tools.ReplaceFirst(expressionText, statement,
							Tools.RemoveBraces(statement,
							BraceEnumType.Curly | BraceEnumType.Parenthesis));
						result = true;
					}
				}
				if(result)
				{
					values.SetValue("expression", expressionText);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AnalyzeInnerVariables																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Analyze all of the inner variables of the specified expression, using
		/// an attribute collection to contain the variable assignments.
		/// </summary>
		/// <param name="expression">
		/// Expression to reduce.
		/// </param>
		private void AnalyzeInnerVariables(string expression)
		{
			bool bChanges = true;
			BraceEnumType brace = BraceEnumType.None;
			string expressionText = expression;
			MatchCollection matches = null;
			AttributeCollection result = new AttributeCollection();
			string name = "";
			string nameb = "";
			string value = "";

			if(expression?.Length > 0)
			{
				//	Convert config command expression.
				//	Formalize all non-parameter function calls.
				if(expressionText.IndexOf("{IncIndent}") > -1)
				{
					expressionText =
						expressionText.Replace("{IncIndent}", "{IncIndent()}");
					result.SetValue("expression", expressionText);
				}
				if(expressionText.IndexOf("{DecIndent}") > -1)
				{
					expressionText =
						expressionText.Replace("{DecIndent}", "{DecIndent()}");
					result.SetValue("expression", expressionText);
				}
				while(bChanges)
				{
					bChanges = false;
					//	Resolve variable names.
					matches = Regex.Matches(expressionText,
						ResourceMain.InnerVariablesNoQuotePattern);
					foreach(Match match in matches)
					{
						name = Tools.GetValue(match, "content");
						if(name.Length > 0)
						{
							nameb = Tools.StripBraces(name);
							brace = Tools.GetBraceType(name);
							switch(brace)
							{
								case BraceEnumType.Curly:
									//	Curly braces require config-style lookup.
									//	Prefix allows use of the same name in config and metadata.
									nameb = "C" + nameb;
									break;
								case BraceEnumType.Square:
									//	Square braces require metadata field lookup.
									//	Prefix allows use of the same name in config and metadata.
									if(nameb.IndexOf(":") > -1)
									{
										//	Remove the control section from the square.
										nameb = nameb.Split(new char[] { ':' })[1];
									}
									nameb = "M" + nameb;
									break;
							}
							switch(brace)
							{
								case BraceEnumType.Curly:
								case BraceEnumType.Square:
									//	Curly braces require config-style lookup.
									//	Square braces require metadata field lookup.
									mEntryCollection.Add(name, "");
									break;
							}
							result.SetValue(nameb, value);
							if(name != nameb)
							{
								expressionText = Tools.ReplaceFirst(expressionText, name, nameb);
								bChanges = true;
							}
						}
					}
					if(bChanges && result.Count > 0)
					{
						result.SetValue("expression", expressionText);
					}
					if(!bChanges)
					{
						bChanges = AnalyzeCommands(result, expressionText);
						if(bChanges)
						{
							expressionText = result.GetValue("expression");
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AnalyzeNodes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Analyze the loops, conditions, and variables within the laid out
		/// template.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes at the current level.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was a success.
		/// </returns>
		/// <remarks>
		/// All found elements are added to the mEntryCollection field, where they
		/// can be aggregated later.
		/// </remarks>
		private bool AnalyzeNodes(ActionNodeCollection nodes)
		{
			string expressionText = ""; //	Current expression.
			int nCount = 0;         //	Node count.
			int nIndex = 0;         //	Node index.
			string nName = "";
			ActionNodeItem node = null;
			bool result = false;
			string vLower = "";

			if(nodes?.Count > 0)
			{
				result = true;
				nCount = nodes.Count;
				for(nIndex = 0; nIndex < nCount && result == true; nIndex++)
				{
					mNode = node = nodes[nIndex];
					nName = node.Attributes.GetValue("name");
					vLower = node.Value.ToLower();
					//if(vLower.IndexOf("spaceto") > -1 ||
					//	vLower.IndexOf("plural") > -1)
					//{
					//	Console.WriteLine("Break here...");
					//}
					switch(vLower)
					{
						case "condition":
							//	{ConditionStart(Name:Value; ...)}
							expressionText = node.Attributes.GetValue("expression");
							mEntryCollection.Add("ConditionStart()", nName);
							AnalyzeInnerVariables(expressionText);
							if(node.Nodes.Count > 0)
							{
								mEntryCollection.Add(">", "");
								result = AnalyzeNodes(node.Nodes);
								mEntryCollection.Add("<", "");
							}
							mEntryCollection.Add("ConditionEnd()", nName);
							break;
						case "config":
							//	{ConfigSet(Name:Value)}
							//	Set the specified configuration entry name to the supplied
							//	value.
							mEntryCollection.Add("ConfigSet()", nName);
							AnalyzeInnerVariables(node.Attributes.GetValue("expression"));
							break;
						case "loop":
							//	{LoopStart(Name:Value; ...)}
							expressionText = node.Attributes.GetValue("expression");
							mEntryCollection.Add("LoopStart()", nName);
							AnalyzeInnerVariables(expressionText);
							if(node.Nodes.Count > 0)
							{
								mEntryCollection.Add(">", "");
								result = AnalyzeNodes(node.Nodes);
								mEntryCollection.Add("<", "");
							}
							mEntryCollection.Add("LoopEnd()", nName);
							break;
						default:
							AnalyzeInnerVariables(node.Value);
							break;
					}
					if(!result)
					{
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BuildTemplate																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Build the initial template structure.
		/// </summary>
		/// <param name="template">
		/// Reference to the template to be published.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was successful.
		/// </returns>
		private bool BuildTemplate(TemplateItem template)
		{
			int cBegin = 0;     //	Current begin.
			int cEnd = 0;       //	Current end.
			string cParam = "";
			string cSide = "";
			string cType = "";
			string cValue = "";
			MatchCollection matches = null;
			ActionNodeItem node = null;
			ActionNodeCollection nodes = this;
			int pEnd = 0;       //	Previous end.
			bool result = false;
			int tEnd = 0;       //	Text end.

			this.Clear();
			mTemplate = template;

			if(mTemplate != null && mTemplate.Lines.Count > 0)
			{
				result = true;
				//	Get the loop condition.
				foreach(string line in mTemplate.Lines)
				{
					tEnd = line.Length - 1;
					matches = Regex.Matches(line,
						ResourceMain.ActionNodeLineLoopPattern);
					//	groups:
					//	loop   - Each entire loop statement.
					//	type   - Submatch for Loop|Condition.
					//	side   - Submatch for Start|End.
					//	params - Submatch for parameters.
					if(matches.Count > 0)
					{
						pEnd = 0;
						foreach(Match match in matches)
						{
							cValue = Tools.GetValue(match, "loop");
							if(cValue.Length > 0)
							{
								//	A branch node was found.
								cType = Tools.GetValue(match, "type").ToLower();
								//if(cType == "condition")
								//{
								//	Console.WriteLine("Break here...");
								//}
								cSide = Tools.GetValue(match, "side").ToLower();
								cBegin = match.Index;
								cEnd = cBegin + match.Length - 1;
								if(cBegin > pEnd)
								{
									//	Some content was found on this line between the
									//	current start and the previous end.
									//	Create a normal text line at the current level
									//	before setting the branch.
									node = new ActionNodeItem();
									node.HasLineEnd = false;
									node.Value = line.Substring(pEnd, cBegin - pEnd);
									nodes.Add(node);
								}
								if(cSide == "start")
								{
									//	Start a new branch.
									node = new ActionNodeItem();
									node.HasLineEnd = false;
									node.Value = cType;
									AddParameters(match, node, "params");
									nodes.Add(node);
									nodes = node.Nodes;
								}
								else if(cSide == "end")
								{
									//	Complete the current branch.
									node = null;
									cParam = Tools.GetValue(match, "params");
									if(cParam.Length > 0)
									{
										//	Return to the named branch.
										node = GetAncestorNodeWithParam(nodes, cParam);
										if(node != null && node.Parent != null)
										{
											nodes = node.Parent;
										}
									}
									if(node == null &&
										nodes.Parent != null && nodes.Parent.Parent != null)
									{
										//	Return to the closest branch.
										nodes = nodes.Parent.Parent;
									}
								}
								else if(cType == "config" && cSide == "set")
								{
									//	Set a config value.
									node = new ActionNodeItem();
									node.HasLineEnd = false;
									node.Value = "config";
									node.Attributes.Add("access", cSide);
									node.Attributes.Add("expression",
										Tools.GetValue(match, "params"));
									node.Attributes.Add("name", mTemplate.Name);
									nodes.Add(node);
								}
							}
							pEnd = cEnd + 1;
						}
						if(pEnd < tEnd)
						{
							//	Post the text found to the right side of all branches.
							node = new ActionNodeItem();
							node.HasLineEnd = true;
							node.Value = line.Substring(pEnd, tEnd - pEnd);
							nodes.Add(node);
						}
					}
					else
					{
						//	No branch was found.
						node = new ActionNodeItem();
						node.HasLineEnd = true;
						node.Value = line;
						nodes.Add(node);
					}
				}
			}
			mConfigurations.EliminateNull();
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EvaluateExpression																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Evaluate the expression for a node value and return a value indicating
		/// whether or not it applies.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to inspect.
		/// </param>
		/// <returns>
		/// True if the expression is true or isn't specified. Otherwise, false.
		/// </returns>
		private bool EvaluateExpression(ActionNodeItem node)
		{
			bool result = true;
			if(node != null)
			{
				result = EvaluateExpression(node,
					node.Attributes.GetValue("expression"));
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Evaluate the expression and return a value indicating whether it
		/// applies or not.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check for unresolved values.
		/// </param>
		/// <param name="expression">
		/// Expression to evaluate.
		/// </param>
		/// <returns>
		/// True if the expression is true or isn't specified. Otherwise, false.
		/// </returns>
		private bool EvaluateExpression(ActionNodeItem node, string expression)
		{
			bool bExists = false;
			ExpressionContext context = null;
			IGenericExpression<bool> evaluator = null;
			string expressionText = expression;
			bool result = false;
			AttributeCollection values = null;
			VariableCollection variables = null;

			if(expression?.Length > 0)
			{
				//	Expression is specified.
				bExists = true;
				//	Resolve higher level commands.
				expressionText = Tools.ReduceHighLevelCommands(expressionText);
				context = new ExpressionContext();
				variables = context.Variables;
				values = ResolveInnerVariables(node, expressionText);
				if(values.Count > 0)
				{
					foreach(AttributeItem value in values)
					{
						if(value.Name.ToLower() != "expression")
						{
							if(value.Value.ToLower() == "true" ||
								value.Value.ToLower() == "false")
							{
								//	Boolean.
								variables.Add(value.Name, bool.Parse(value.Value));
							}
							else if(IsNumeric(value.Value))
							{
								//	Numeric.
								variables.Add(value.Name, double.Parse(value.Value));
							}
							else
							{
								//	String.
								variables.Add(value.Name, value.Value);
							}
						}
					}
					expressionText = values.GetValue("expression");
					//	Value is ready to use.
					try
					{
						evaluator = context.CompileGeneric<bool>(expressionText);
						result = evaluator.Evaluate();
					}
					catch(Exception ex)
					{
						Console.WriteLine(
							string.Format(
							"Error: Could not evaluate expression: {0}\r\n {1}",
							expression,
							ex.Message));
					}
				}
			}
			if(!bExists)
			{
				result = true;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Evaluate the expression and return a value indicating whether it
		/// applies or not.
		/// </summary>
		/// <param name="attributes">
		/// Collection of attributes that have been assigned with indirect values.
		/// </param>
		/// <returns>
		/// True if the expression evaluates to true or isn't specified at all.
		/// Otherwise, false.
		/// </returns>
		private bool EvaluateExpression(AttributeCollection attributes)
		{
			bool bExists = false;
			ExpressionContext context = null;
			IGenericExpression<bool> evaluator = null;
			string expressionText = "";
			bool result = false;
			VariableCollection variables = null;

			//	Expression is specified.
			bExists = true;
			context = new ExpressionContext();
			variables = context.Variables;
			if(attributes.Count > 0)
			{
				foreach(AttributeItem value in attributes)
				{
					if(value.Name.ToLower() != "expression")
					{
						if(value.Value.ToLower() == "true" ||
							value.Value.ToLower() == "false")
						{
							//	Boolean.
							variables.Add(value.Name, bool.Parse(value.Value));
						}
						else if(IsNumeric(value.Value))
						{
							//	Numeric.
							variables.Add(value.Name, double.Parse(value.Value));
						}
						else
						{
							//	String.
							variables.Add(value.Name, value.Value);
						}
					}
				}
				expressionText = attributes.GetValue("expression");
				//	Value is ready to use.
				try
				{
					evaluator = context.CompileGeneric<bool>(expressionText);
					result = evaluator.Evaluate();
				}
				catch(Exception ex)
				{
					Console.WriteLine(
						string.Format(
						"Error: Could not evaluate expression: {0}\r\n {1}",
						expressionText,
						ex.Message));
				}
			}
			if(!bExists)
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetConfigValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the first matching configuration entry.
		/// </summary>
		/// <param name="configs">
		/// Reference to a collection of pre-populated configurations.
		/// </param>
		/// <param name="name">
		/// Name of the setting to find.
		/// </param>
		/// <returns>
		/// The first matching value found within the configuration tables, if
		/// found. Otherwise, an empty string.
		/// </returns>
		private string GetConfigValue(ConfigurationCollection configs,
			string name)
		{
			AttributeCollection keys = null;
			MatchCollection matches = null;
			string result = "";

			if(configs?.Count > 0 && name?.Length > 0)
			{
				foreach(ConfigurationItem config in configs)
				{
					result = GetConfigValue(config, name);
					if(result.Length > 0)
					{
						break;
					}
				}
				if(result == "(blank)")
				{
					result = "";
				}
				else
				{
					while(result.IndexOf("{") > -1)
					{
						//	Config values to be resolved.
						keys = new AttributeCollection();
						matches = Regex.Matches(result,
							ResourceMain.ActionNodeResolveVariablePattern);
						if(matches.Count > 0)
						{
							foreach(Match match in matches)
							{
								keys.AddUnique(Tools.GetValue(match, "content"));
							}
							foreach(AttributeItem key in keys)
							{
								key.Value = GetConfigValue(configs, key.Name);
								result = result.Replace(key.Name, key.Value);
							}
						}
						else
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the first matching configuration entry.
		/// </summary>
		/// <param name="config">
		/// Reference to a pre-populated configuration.
		/// </param>
		/// <param name="name">
		/// Name of the setting to find.
		/// </param>
		/// <returns>
		/// The first matching value found within the configuration tables, if
		/// found. Otherwise, an empty string.
		/// </returns>
		private string GetConfigValue(ConfigurationItem config,
			string name)
		{
			string result = "";
			string nValue = Tools.StripBraces(name).ToLower();

			if(config.Entries?.Count > 0 && nValue.Length > 0)
			{
				if(config.Entries.Exists(x => x.Name.ToLower() == nValue))
				{
					//	Item found.
					result = config.Entries.First(x => x.Name.ToLower() == nValue).Value;
					if(result.Length == 0)
					{
						result = "(blank)";
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetIndefiniteArticle																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the indefinite article for the caller's word.
		/// </summary>
		/// <param name="value">
		/// The value to inspect.
		/// </param>
		/// <returns>
		/// Matching indefinite article for the caller's value, if found.
		/// Otherwise, an empty string.
		/// </returns>
		/// <remarks>
		/// The choice of 'a' or 'an' is determined not only by the
		/// consonant : vowel status of the first letter, but whether that letter
		/// is pronounced at the beginning of the word.
		/// For example, 'h' is a consonant, but since it is often silent, it is
		/// preceded with 'an'. In contrast, the 'y' is sometimes a vowel, but is
		/// pronounced, so is preceded by 'a'.
		/// This function doesn't pass in all cases, but should survive in the
		/// majority.
		/// </remarks>
		private static string GetIndefiniteArticle(string value)
		{
			string letter = "";
			string result = "a";

			if(value?.Length > 0)
			{
				letter = value.Substring(0, 1).ToLower();
				if(letter.IndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u', 'h' }) > -1)
				{
					result = "an";
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetMetaFieldValue																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve the value of the metadata field currently specified in
		/// component, entryName, and fieldName.
		/// </summary>
		/// <param name="fieldName">
		/// Name of the field to select for the current metafield attributes
		/// collection.
		/// </param>
		/// <returns>
		/// Value found in the currently selected attributes collection.
		/// </returns>
		private string GetMetaFieldValue(string fieldName)
		{
			AttributeCollection attributes = null;
			int fCount = 0;
			string fName = "";
			string[] fParts = new string[0];
			//ComponentItem component = null;
			string result = "";

			if(mComponent != null)
			{
				fName = Tools.StripBraces(fieldName);
				if(fName.IndexOf(":") > -1)
				{
					//	Override is present.
					fParts = Tools.StripBraces(fieldName).Split(new char[] { ':' });
					fCount = fParts.Length;
					if(fCount > 0)
					{
						//	Left side is override. Right side is field name.
						switch(fParts[0].ToLower())
						{
							case "entry":
							case "field":
								//	Current entry.
								if(mEntryCollection?.Count > 0)
								{
									attributes = mEntryCollection;
								}
								break;
							case "component":
							case "object":
								//	Base object.
								if(mComponent != null)
								{
									attributes = mComponent.Attributes.
										GetCollection("[DataType]=baseobject");
								}
								break;
						}
						fName = fParts[1];
					}
				}
				else if(mEntryCollection?.Count > 0)
				{
					//	Current entry position.
					attributes = mEntryCollection;
				}
				else
				{
					//	Base object.
					//attributes = mComponent.Attributes[0];
					attributes =
						mComponent.Attributes.GetCollection("[DataType]=baseobject");
				}
				if(attributes?.Count > 0)
				{
					//	Attributes collection is selected.
					if(fName?.Length > 0)
					{
						//	Get the specified value.
						result = attributes.GetValue(fName);
					}
					else
					{
						//	No value was specified. Use the first attribute.
						//	Typically, DataType=object.
						result = attributes[0].Value;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetPlural																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the plural version of the caller's word.
		/// </summary>
		/// <param name="value">
		/// The value to convert.
		/// </param>
		/// <remarks>
		/// For speed, there is no support for irregular nouns in this version.
		/// </remarks>
		private static string GetPlural(string value)
		{
			char right = '\0';
			string result = "s";
			string vLower = "";

			if(value?.Length > 0)
			{
				vLower = value.ToLower();
				right = vLower[vLower.Length - 1];
				switch(right)
				{
					case 'h':
					case 's':
					case 'x':
					case 'z':
						//	Value ends in ch, s, sh, x, or z.
						if(right == 'h')
						{
							if(vLower.Length > 1)
							{
								right = vLower[vLower.Length - 2];
								if(right == 'c' || right == 's')
								{
									//	Value ends with ch or sh.
									result = value + "es";
								}
								else
								{
									result = value + "s";
								}
							}
							else
							{
								result = value + "s";
							}
						}
						break;
					case 'y':
						//	Value ends in y.
						result = value.Substring(0, value.Length - 1) + "ies";
						break;
					default:
						//	All other possibilities.
						result = value + "s";
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsConfigValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's reference is a
		/// configuration variable.
		/// </summary>
		/// <param name="variable">
		/// Variable name to test.
		/// </param>
		/// <returns>
		/// True if the value should be treated as a confgiuration variable,
		/// resolvable through node attributes and the configuration table.
		/// False if the value is a component metadata field name.
		/// </returns>
		private static bool IsConfigValue(string variable)
		{
			bool result = true;

			if(variable?.Length > 0)
			{
				result = !(variable.StartsWith("[") && variable.EndsWith("]"));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsNumeric																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the value can be parsed to a floating
		/// point number.
		/// </summary>
		/// <param name="value">
		/// Value to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's value can be converted directly to a floating
		/// point number. Otherwise, false.
		/// </returns>
		private static bool IsNumeric(string value)
		{
			double dValue = 0d;
			bool result = false;

			if(value?.Length > 0)
			{
				double.TryParse(value, out dValue);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ListNodes																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// List the currently loaded nodes to the rendered content collection.
		/// </summary>
		/// <param name="nodes">
		/// Collection of nodes to inspect.
		/// </param>
		private void ListNodes(ActionNodeCollection nodes)
		{
			StringBuilder builder = new StringBuilder();
			string expressionText = ""; //	Current expression.
			int nCount = 0;         //	Node count.
			int nIndex = 0;         //	Node index.
			string nName = "";
			ActionNodeItem node = null;
			string vLower = "";

			if(nodes?.Count > 0)
			{
				nCount = nodes.Count;
				for(nIndex = 0; nIndex < nCount; nIndex++)
				{
					mNode = node = nodes[nIndex];
					nName = node.Attributes.GetValue("name");
					vLower = node.Value.ToLower();
					if(builder.Length > 0)
					{
						builder.Remove(0, builder.Length);
					}
					switch(vLower)
					{
						case "condition":
							//	{ConditionStart(Name:Value; ...)}
							expressionText = node.Attributes.GetValue("expression");
							if(mIndent > 0)
							{
								builder.Append(new string(' ', mIndent));
							}
							builder.Append("_condition_;");
							builder.Append("name:");
							builder.Append(node.Attributes.GetValue("name"));
							builder.Append(";level:");
							builder.Append(node.Attributes.GetValue("level"));
							builder.Append(";expression:");
							builder.Append(Tools.ReduceDensity(expressionText));
							mRenderedContent.Add(builder.ToString());
							if(node.Nodes.Count > 0)
							{
								mIndent++;
								ListNodes(node.Nodes);
								mIndent--;
							}
							break;
						case "config":
							//	{ConfigSet(Name:Value)}
							//	Set the specified configuration entry name to the supplied
							//	value.
							expressionText = node.Attributes.GetValue("expression");
							if(mIndent > 0)
							{
								builder.Append(new string(' ', mIndent));
							}
							builder.Append("_config_:");
							builder.Append(Tools.ReduceDensity(expressionText));
							mRenderedContent.Add(builder.ToString());
							break;
						case "loop":
							//	{LoopStart(Name:Value; ...)}
							expressionText = node.Attributes.GetValue("expression");
							if(mIndent > 0)
							{
								builder.Append(new string(' ', mIndent));
							}
							builder.Append("_loop_;");
							builder.Append("name:");
							builder.Append(node.Attributes.GetValue("name"));
							builder.Append(";level:");
							builder.Append(node.Attributes.GetValue("level"));
							builder.Append(";expression:");
							builder.Append(Tools.ReduceDensity(expressionText));
							mRenderedContent.Add(builder.ToString());
							if(node.Nodes.Count > 0)
							{
								mIndent++;
								ListNodes(node.Nodes);
								mIndent--;
							}
							break;
						default:
							expressionText = node.Value;
							if(mIndent > 0)
							{
								builder.Append(new string(' ', mIndent));
							}
							builder.Append(Tools.ReduceDensity(expressionText));
							mRenderedContent.Add(builder.ToString());
							break;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NodeHas																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether this node or any node up the chain,
		/// including the configuration table, have a value of the specified name.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to be inspected.
		/// </param>
		/// <param name="name">
		/// Name of the value to search for.
		/// </param>
		/// <returns>
		/// Value indicating whether the specified node, or any of its ancestors,
		/// has the specified value defined.
		/// </returns>
		private bool NodeHas(ActionNodeItem node, string name)
		{
			string nLower = "";
			bool result = false;

			if(node != null && name?.Length > 0)
			{
				nLower = name.ToLower();
				if(node.Attributes.Exists(x => x.Name.ToLower() == nLower))
				{
					result = true;
				}
				if(!result)
				{
					if(node.Parent != null)
					{
						result = NodeHas(node.Parent.Parent, name);
					}
					else
					{
						result = mConfigurations.Exists(name);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PostProcessing																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run post-processing operations on the data.
		/// </summary>
		/// <returns>
		/// Value indicating whether the operation was a success.
		/// </returns>
		private bool PostProcessing()
		{
			bool bMatch = false;  //	Line match.
			string command = "";  //	Working command.
			string content = "";
			string cValue = "";   //	Configuration value.
			int indent = 0;
			int iValue = 0;
			MatchCollection matches = null;
			bool result = true;
			int sCount = mRenderedContent.Count;
			string sFixed = "";
			int sIndex = 0;
			StringBuilder sLine = new StringBuilder();
			string sValue = "";
			char tChar = '\t';
			int tCount = 1;

			cValue = GetConfigValue(mConfigurations, "TabCharacter");
			if(cValue.Length > 0)
			{
				tChar = cValue[0];
			}
			cValue = GetConfigValue(mConfigurations, "TabCount");
			if(cValue.Length > 0)
			{
				int.TryParse(cValue, out iValue);
				if(iValue > 0)
				{
					tCount = iValue;
				}
			}
			for(sIndex = 0; sIndex < sCount; sIndex++)
			{
				bMatch = false;
				sFixed = sValue = mRenderedContent[sIndex];
				if(sLine.Length > 0)
				{
					sLine.Remove(0, sLine.Length);
				}
				//	{IncIndent}, {DecIndent}.
				matches = Regex.Matches(sValue,
					ResourceMain.PostProcessingIncDecPattern);
				foreach(Match match in matches)
				{
					content = Tools.GetValue(match, "content");
					if(content.Length > 0)
					{
						command = Tools.GetValue(match, "command").ToLower();
						switch(command)
						{
							case "decindent":
								if(indent > 0)
								{
									indent--;
								}
								bMatch = true;
								break;
							case "incindent":
								indent++;
								bMatch = true;
								break;
						}
						if(bMatch)
						{
							sFixed = sFixed.Replace(content, "").Trim();
						}
					}
				}
				if(!bMatch)
				{
					//	{Tab(x)}
					matches = Regex.Matches(sValue,
						ResourceMain.PostProcessingTabPattern);
					foreach(Match match in matches)
					{
						cValue = Tools.GetValue(match, "params");
						int.TryParse(cValue, out iValue);
						if(iValue > 0)
						{
							content = Tools.GetValue(match, "content");
							sFixed = sFixed.Replace(content,
								new string(tChar, iValue * tCount));
							bMatch = true;
						}
					}
				}
				//	Unescape all double-braced values.
				if(sFixed.IndexOf("[[") > -1 || sFixed.IndexOf("]]") > -1 ||
					sFixed.IndexOf("{{") > -1 || sFixed.IndexOf("}}") > -1)
				{
					sFixed = Regex.Replace(sFixed,
						ResourceMain.PostProcessingUnescapeBracePattern,
						ResourceMain.PostProcessingUnescapeBraceReplacement);
					bMatch = true;
				}
				if(bMatch && sFixed.Length > 0)
				{
					if(indent > 0)
					{
						sLine.Append(tChar, indent * tCount);
					}
					sLine.Append(sFixed);
					mRenderedContent[sIndex] = sLine.ToString();
				}
				else if(bMatch && sFixed.Length == 0)
				{
					mRenderedContent.RemoveAt(sIndex);
					sIndex--;
					sCount--;
				}
				else if(sValue.Length > 0)
				{
					//	If the item wasn't a command match, just use the current indent.
					if(indent > 0)
					{
						sLine.Append(tChar, indent * tCount);
					}
					sLine.Append(sValue);
					mRenderedContent[sIndex] = sLine.ToString();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessEntries																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process all entries on the currently selected component.
		/// </summary>
		/// <param name="node">
		/// Currently selected template node.
		/// </param>
		/// <param name="expression">
		/// Expression used to filter output.
		/// </param>
		/// <returns>
		/// Value indicating whether the result was successful.
		/// </returns>
		private bool ProcessEntries(ActionNodeItem node, string expression)
		{
			AttributeCatalog attributes = null;
			bool bFirst = true;		//	Flag - first item.
			//string componentName = "";
			int eCount = 0;   //	Element count.
			int eIndex = 0;   //	Element index.
			string entryName = "";
			string expressionText = expression;   //	Current expression.
			int iLast = 0;    //	Index of the last match.
			AttributeCollection properties = null;
			bool result = false;
			string source = "";
			string[] sourceList = new string[0];

			if(mComponent != null)
			{
				result = true;
				//	At the entry level, source refers to a list of metadata field names
				//	within the selected component.
				if(node.Attributes.Exists(x =>
					x.Name.ToLower() == "source"))
				{
					source = node.Attributes.GetValue("source");
					if(Tools.HasBraces(source,
						BraceEnumType.Curly |
						BraceEnumType.Square))
					{
						source = ResolveValue(node, source);
					}
				}
				if(source.Length > 0)
				{
					sourceList = Tools.DelimitedToArray(source);
					attributes = new AttributeCatalog();
					eCount = sourceList.Length;
					for(eIndex = 0; eIndex < eCount; eIndex ++)
					{
						properties = mComponent.Attributes.GetCollection(
							"[Name]=" + sourceList[eIndex]);
						if(properties != null)
						{
							attributes.Add(properties);
						}
					}
				}
				else
				{
					attributes = mComponent.Attributes;
				}
				eCount = attributes.Count;
				iLast = 0;
				for(eIndex = 0; eIndex < eCount; eIndex++)
				{
					//	Process for last valid item with expression.
					mEntryCollection = attributes[eIndex];
					if(!mEntryCollection.Exists(x =>
						x.Name.ToLower() == "datatype" &&
						x.Value.ToLower() == "baseobject"))
					{
						entryName = mEntryCollection.GetValue("Name");
						node.Attributes.SetValue("EntryName", entryName);
						if(EvaluateExpression(node, expressionText))
						{
							iLast = eIndex;
						}
					}
				}
				for(eIndex = 0; eIndex < eCount; eIndex++)
				{
					//	Each non-base entry.
					mEntryCollection = attributes[eIndex];
					if(!mEntryCollection.Exists(x =>
						x.Name.ToLower() == "datatype" &&
						x.Value.ToLower() == "baseobject"))
					{
						//	This is not a base object.
						entryName = mEntryCollection.GetValue("Name");
						node.Attributes.SetValue("EntryName", entryName);
						//node.Attributes.SetValue("ComponentName", componentName);
						if(EvaluateExpression(node, expressionText))
						{
							node.Attributes.SetValue("isFirst",
								(bFirst ? "true" : "false"));
							node.Attributes.SetValue("isLast",
								(eIndex == iLast ? "true" : "false"));
							result = ResolveNodes(node.Nodes);
							bFirst = false;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReplaceVariables																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Replace the variables found in the caller's expression string with the
		/// resolved values provided.
		/// </summary>
		/// <param name="node">
		/// Reference to the node providing base reference in this context.
		/// </param>
		/// <param name="expression">
		/// The expression containing possible resolved variables.
		/// </param>
		/// <param name="keys">
		/// Collection of variable names and resolved values.
		/// </param>
		/// <returns>
		/// If the expression was provided and variables were present, then
		/// the caller's expression where all applicable variable names have been
		/// replaced.<br />
		/// If the expression was provided and no variables were found, then
		/// the caller's expression, unchanged.<br />
		/// If the expression was not provided, but the node was present, then
		/// the node's base value.
		/// </returns>
		private string ReplaceVariables(ActionNodeItem node, string expression,
			AttributeCollection keys)
		{
			int kCount = 0;
			AttributeItem key = null;
			int kIndex = 0;
			string result = "";

			if(expression?.Length > 0)
			{
				if(keys.Count > 0)
				{
					result = expression;
					kCount = keys.Count;
					//	Finish resolving all variable names. There is no logical
					//	expression at this turn.
					for(kIndex = 0; kIndex < kCount; kIndex++)
					{
						key = keys[kIndex];
						if(key.Name.ToLower() != "expression" && key.Name != key.Value)
						{
							result = result.Replace(key.Name, key.Value);
						}
					}
				}
				else
				{
					result = expression;
				}
			}
			else if(node != null)
			{
				result = node.Value;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResolveConfig																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve a single configuration style variable with or without the
		/// curly braces.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to be inspected.
		/// </param>
		/// <param name="variable">
		/// Name of the variable to resolve, with or without curly braces.
		/// </param>
		/// <returns>
		/// The value found for the configuration name.
		/// </returns>
		private string ResolveConfig(ActionNodeItem node, string variable)
		{
			BraceEnumType brace = BraceEnumType.None;
			string result = "";
			string vVariable = "";    //	Variable value.

			if(variable?.Length > 0 &&
				node != null &&
				(node.Attributes.Count > 0 || node.Parent != null))
			{
				brace = Tools.GetBraceType(variable);
				vVariable = Tools.StripBraces(variable).ToLower();
				if(brace == BraceEnumType.Square)
				{
					//	Metadata field value.
					result = GetMetaFieldValue(vVariable);
				}
				else
				{
					//	A config variable can be resolved within the node parameters or
					//	the configuration table.
					if(node.Attributes.Exists(x => x.Name.ToLower() == vVariable))
					{
						result = node.Attributes.GetValue(vVariable);
					}
					else if(node.Parent != null)
					{
						//	Nothing found at the current level. Move upward.
						if(node.Parent is ActionNodeTree || node.Parent.Parent == null)
						{
							//	We have reached the base. Check config table.
							result = GetConfigValue(mConfigurations, variable);
						}
						else
						{
							//	Check the parent.
							result = ResolveConfig(node.Parent.Parent, variable);
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResolveInnerVariables																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve all of the inner variables of the specified expression, using
		/// an attribute collection to contain the variable assignments.
		/// </summary>
		/// <param name="node">
		/// Reference to the current node in focus.
		/// </param>
		/// <param name="expression">
		/// Expression to reduce.
		/// </param>
		/// <returns>
		/// Attribute collection containing the reduced expression and all
		/// variable assignments found, if any. If no variables were found,
		/// the collection will be zero length.
		/// </returns>
		/// <remarks>
		/// In this version, continue to reduce as long as changes are being made.
		/// </remarks>
		private AttributeCollection ResolveInnerVariables(ActionNodeItem node,
			string expression)
		{
			bool bChanges = true;
			BraceEnumType brace = BraceEnumType.None;
			string expressionText = expression;
			MatchCollection matches = null;
			List<string> removeNames = new List<string>();
			AttributeCollection result = new AttributeCollection();
			string name = "";
			string nameb = "";
			string nLower = "";
			int sIndex = 1;
			string value = "";

			if(node != null && expression?.Length > 0)
			{
				//	Convert config command expression.
				if(expressionText == "config" || expressionText == "loop")
				{
					expressionText = node.Attributes.GetValue("expression");
				}
				//	Formalize all non-parameter function calls.
				if(expressionText.IndexOf("{IncIndent}") > -1)
				{
					expressionText =
						expressionText.Replace("{IncIndent}", "{IncIndent()}");
					result.SetValue("expression", expressionText);
				}
				if(expressionText.IndexOf("{DecIndent}") > -1)
				{
					expressionText =
						expressionText.Replace("{DecIndent}", "{DecIndent()}");
					result.SetValue("expression", expressionText);
				}
				while(bChanges)
				{
					bChanges = false;
					//	Resolve variable names.
					matches = Regex.Matches(expressionText,
						ResourceMain.InnerVariablesNoQuotePattern);
					foreach(Match match in matches)
					{
						name = Tools.GetValue(match, "content");
						if(name.Length > 0)
						{
							nameb = Tools.StripBraces(name);
							brace = Tools.GetBraceType(name);
							switch(brace)
							{
								case BraceEnumType.Curly:
									//	Curly braces require config-style lookup.
									//	Prefix allows use of the same name in config and metadata.
									nameb = "C" + nameb;
									break;
								case BraceEnumType.Quote:
									//	Quoted items are replaced with names.
									nameb = string.Format("string{0}", sIndex++);
									break;
								case BraceEnumType.Square:
									//	Square braces require metadata field lookup.
									//	Prefix allows use of the same name in config and metadata.
									if(nameb.IndexOf(":") > -1)
									{
										//	Remove the control section from the square.
										nameb = nameb.Split(new char[] { ':' })[1];
									}
									nameb = "M" + nameb;
									break;
							}
							if(!result.Exists(x => x.Name.ToLower() == nameb.ToLower()))
							{
								//	Variables are single-instanced.
								switch(brace)
								{
									case BraceEnumType.Curly:
										//	Curly braces require config-style lookup.
										value = ResolveConfig(node, name);
										break;
									case BraceEnumType.Square:
										//	Square braces require metadata field lookup.
										value = GetMetaFieldValue(name);
										break;
									default:
										value = name;
										break;
								}
								result.SetValue(nameb, value);
								if(name != nameb)
								{
									expressionText = expressionText.Replace(name, nameb);
								}
								bChanges = true;
							}
						}
					}
					if(bChanges && result.Count > 0)
					{
						result.SetValue("expression", expressionText);
					}
					if(!bChanges)
					{
						bChanges = RunCommands(result, expressionText);
						if(bChanges)
						{
							expressionText = result.GetValue("expression");
							foreach(AttributeItem item in result)
							{
								nLower = item.Name.ToLower();
								if(nLower.StartsWith(":set:"))
								{
									if(node.Parent != null && node.Parent.Parent != null)
									{
										//	This value fits in the parent node.
										node.Parent.Parent.Attributes.
											SetValue(item.Name.Substring(5), item.Value);
									}
									else if(mConfigurations != null)
									{
										//	This value goes into the configuration table.
										SetConfigValue(mConfigurations,
											item.Name.Substring(5), item.Value);
									}
								}
								else
								{
									switch(nLower)
									{
										case ":canskip:":
											if(expressionText.Length == 0)
											{
												//	There was at least one command on the line, and now
												//	nothing remains.
												node.Attributes.SetValue(":skip:", "1");
											}
											break;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResolveValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the variable values from the caller's node specification and
		/// current settings scope.
		/// </summary>
		/// <param name="node">
		/// Reference to the node whose value is to be resolved.
		/// </param>
		/// <returns>
		/// Fully resolved value for the node, if values were found. Otherwise,
		/// an empty string.
		/// </returns>
		private string ResolveValue(ActionNodeItem node)
		{
			AttributeCollection keys = null;
			string result = "";

			if(node != null && node.Value?.Length > 0)
			{
				keys = ResolveInnerVariables(node, node.Value);
				result = keys.GetValue("expression");
				result = ReplaceVariables(node, result, keys);
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Resolve the variable values from the caller's closest available node
		/// specification and current settings scope.
		/// </summary>
		/// <param name="node">
		/// Reference to the node whose value is to be resolved.
		/// </param>
		/// <param name="variable">
		/// Name of the variable to resolve for the specified node.
		/// </param>
		/// <returns>
		/// The value of the specified variable, if found within the current scope
		/// of settings. Otherwise, an empty string.
		/// </returns>
		private string ResolveValue(ActionNodeItem node, string variable)
		{
			int kCount = 0;
			AttributeItem key = null;
			AttributeCollection keys = null;
			int kIndex = 0;
			string result = "";

			keys = ResolveInnerVariables(node, variable);
			if(keys.Count > 0)
			{
				result = keys.GetValue("expression");
				kCount = keys.Count;
				//	Finish resolving all variable names. There is no logical
				//	expression at this turn.
				for(kIndex = 0; kIndex < kCount; kIndex++)
				{
					key = keys[kIndex];
					if(key.Name.ToLower() != "expression" && key.Name != key.Value)
					{
						result = result.Replace(key.Name, key.Value);
					}
				}
			}
			else
			{
				result = node.Value;
			}

			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Resolve a value from available resources at the base level.
		/// </summary>
		/// <param name="variable">
		/// Name of the variable to resolve. If the value is enclosed in curly
		/// braces {x}, then its match is retrieved from the configuration table.
		/// If the value is enclosed in square brackets [x], then its match is
		/// retrieved from the directed component table, if specified, or from
		/// the first component table in which it is found, if the component
		/// table is not specified.
		/// </param>
		/// <returns>
		/// Value of the base resource, if found. Otherwise, an empty string.
		/// </returns>
		private string ResolveValue(string variable)
		{
			bool bConfig = false;
			string nValue = "";
			string result = "";

			if(variable?.Length > 0)
			{
				bConfig = IsConfigValue(variable);
				nValue = Tools.StripBraces(variable);
				if(bConfig)
				{
					//	Configuration table.
					result = GetConfigValue(mConfigurations, nValue);
				}
				else
				{
					//	Component metadata field name.
					result = GetMetaFieldValue(nValue);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResolveNodes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the loops, conditions, and variables within the laid out
		/// template.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes at the current level.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was a success.
		/// </returns>
		/// <remarks>
		/// Loops are sections that will be repeated for each member of the
		/// item at the level. Currently there are two levels defined:
		/// <list type="bullet">
		/// <item>Component - Each component both loaded from ComponentPage data,
		/// and specified in the Configuration data.</item>
		/// <item>Entry - Each entry within the selected Component. If not nested
		/// within a component, all metafields in all components will be processed
		/// to check for matching conditions.</item>
		/// </list><br />
		/// A Condition is similar to a Loop in structure, but its branch will only
		/// be used in the case that its expression matches within the currently
		/// selected component or entry.<br />
		/// When either a Loop or a Condition are specified and no expression is
		/// present, all component sheets or meta fields in the scope will be
		/// translated. When an expression is present, only the template elements
		/// that can evaluate to true on the expression are translated.
		/// </remarks>
		private bool ResolveNodes(ActionNodeCollection nodes)
		{
			bool bFirst = false;    //	Flag - First item.
			int cCount = 0;         //	Component count.
			int cIndex = 0;         //	Component index.
			ComponentEntryFollower componentEntry = null;
			string[] componentNames = new string[0];
			string expressionText = ""; //	Current expression.
			string[] fParts = new string[0];
			int iLast = 0;					//	Last indent.
			int nCount = 0;					//	Node count.
			int nIndex = 0;					//	Node index.
			ActionNodeItem node = null;
			bool result = false;
			int sCount = 0;         //	Reference stack count.
			string source = "";			//	Loop source.
			string vLower = "";

			if(nodes?.Count > 0)
			{
				result = true;
				nCount = nodes.Count;
				for(nIndex = 0; nIndex < nCount && result == true; nIndex ++)
				{
					mNode = node = nodes[nIndex];
					vLower = node.Value.ToLower();
					expressionText = "";
					//if(vLower.IndexOf("{{") > -1)
					//{
					//	Console.WriteLine("Break here...");
					//}
					switch(vLower)
					{
						case "condition":
							//	{ConditionStart(Name:Value; ...)}
							if(node.Nodes.Count > 0)
							{
								if(EvaluateExpression(node))
								{
									result = ResolveNodes(node.Nodes);
								}
							}
							break;
						case "config":
							//	{ConfigSet(Name:Value)}
							//	Set the specified configuration entry name to the supplied
							//	value.
							if(node.Attributes.Count > 0 || node.Parent != null)
							{
								expressionText = ResolveValue(node);
								if(expressionText.IndexOf(":") > -1)
								{
									//	Name:Value.
									fParts = expressionText.Split(new char[] { ':' });
									SetConfigValue(mConfigurations,
										fParts[0].Trim(), fParts[1].Trim());
								}
							}
							break;
						case "loop":
							//	{LoopStart(Name:Value; ...)}
							if(node.Nodes.Count > 0)
							{
								//if(node.Attributes.GetValue("name") ==
								//	"modelList")
								//{
								//	Console.WriteLine("Break here...");
								//}
								expressionText = node.Attributes.GetValue("expression");
								source = "";
								if(node.Attributes.Exists(x =>
									x.Name.ToLower() == "level" &&
									x.Value.ToLower() == "component"))
								{
									//	Process for components.
									if(node.Attributes.Exists(x =>
										x.Name.ToLower() == "source"))
									{
										source = node.Attributes.GetValue("source");
										if(Tools.HasBraces(source,
											BraceEnumType.Curly |
											BraceEnumType.Square))
										{
											source = ResolveValue(node, source);
										}
									}
									if(source.Length > 0)
									{
										componentNames = Tools.DelimitedToArray(source);
									}
									else
									{
										componentNames =
											Tools.DelimitedToArray(
												ResolveValue("{ComponentPages}"));
									}
									cCount = componentNames.Length;
									bFirst = true;
									iLast = 0;
									//	Resolve the isLast variable with expressions.
									for(cIndex = 0; cIndex < cCount; cIndex ++)
									{
										mComponent = mComponents[componentNames[cIndex]];
										mEntryCollection = null;
										node.Attributes.SetValue("ComponentName", mComponent.Name);
										node.Attributes.SetValue("ObjectName",
											GetMetaFieldValue("Name"));
										if(EvaluateExpression(node, expressionText))
										{
											iLast = cIndex;
										}
									}
									for(cIndex = 0; cIndex < cCount; cIndex ++)
									{
										sCount = mComponentEntryStack.Count;
										componentEntry =
											new ComponentEntryFollower(mComponent, mEntryCollection);
										mComponentEntryStack.Push(componentEntry);
										mComponent = mComponents[componentNames[cIndex]];
										mEntryCollection = null;
										node.Attributes.SetValue("ComponentName", mComponent.Name);
										node.Attributes.SetValue("ObjectName",
											GetMetaFieldValue("Name"));
										if(EvaluateExpression(node, expressionText))
										{
											node.Attributes.SetValue("isFirst",
												bFirst ? "true" : "false");
											node.Attributes.SetValue("isLast",
												cIndex == iLast ? "true" : "false");
											result = ResolveNodes(node.Nodes);
											bFirst = false;
										}
										while(mComponentEntryStack.Count > sCount)
										{
											//	Off-balance return is allowed from inner levels.
											componentEntry = mComponentEntryStack.Pop();
											mComponent = componentEntry.Component;
											mEntryCollection = componentEntry.Entry;
										}
									}
								}
								else if(node.Attributes.Count(
									x => x.Name.ToLower() == "level" &&
									x.Value.ToLower() == "entry") > 0)
								{
									//	Process for entries.
									sCount = mComponentEntryStack.Count;
									componentEntry =
										new ComponentEntryFollower(mComponent, mEntryCollection);
									mComponentEntryStack.Push(componentEntry);
									if(mComponent == null)
									{
										//	This loop is outside of a component loop.
										//	All components will be traced.
										componentNames =
											Tools.DelimitedToArray(
												ResolveValue("{ComponentPages}"));
										cCount = componentNames.Length;
										for(cIndex = 0; cIndex < cCount; cIndex ++)
										{
											mComponent = mComponents[componentNames[cIndex]];
											result = ProcessEntries(node, expressionText);
										}
										mComponent = null;
									}
									else
									{
										//	This loop is running inside of an active component.
										result = ProcessEntries(node, expressionText);
									}
									while(mComponentEntryStack.Count > sCount)
									{
										//	Off-balance return is allowed from inner levels.
										componentEntry = mComponentEntryStack.Pop();
										mComponent = componentEntry.Component;
										mEntryCollection = componentEntry.Entry;
									}
									if(!result)
									{
										break;
									}
								}
							}
							break;
						default:
							expressionText = ResolveValue(node);
							if(node.Attributes.GetValue(":skip:").Length == 0)
							{
								mRenderedContent.Add(expressionText);
							}
							else
							{
								//	Reset the node flags for the next use of this item.
								node.Attributes.RemoveName(":canskip:");
								node.Attributes.RemoveName(":skip:");
							}
							break;
					}
					if(!result)
					{
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RunCommands																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run all of the commands found in the expression.
		/// </summary>
		/// <param name="values">
		/// Collection of resolved variables. Also contains the current expression
		/// to be reduced.
		/// </param>
		/// <param name="expression">
		/// The expression containing possible commands to resolve and / or run.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was successful.
		/// </returns>
		private bool RunCommands(AttributeCollection values, string expression)
		{
			StringBuilder builder = new StringBuilder();
			string command = "";
			//string expression = "";
			string expressionText = "";
			int iCount = 0;
			int iIndex = 0;
			int iValue = 0;
			MatchCollection matches = null;
			string param = "";
			string param1 = "";
			string param2 = "";
			char[] pNameValue = new char[] { ':' };
			char[] pParameter = new char[] { ',' };
			string[] pParts = new string[0];
			bool result = false;
			string statement = "";
			string value = "";

			if(values != null)
			{
				if(expression?.Length > 0)
				{
					expressionText = expression;
					matches = Regex.Matches(expression,
						ResourceMain.CommandSpecialPattern);
					foreach(Match match in matches)
					{
						statement = Tools.GetValue(match, "command");
						command = Tools.GetValue(match, "name").ToLower();
						param = Tools.GetValue(match, "params");
						switch(command)
						{
							case "iif":
								pParts = param.Split(pParameter);
								//	The expression portion of the statement is expected to be
								//	resolved at the time this function is called.
								expression = pParts[0].Trim();
								values.SetValue("expression", expression);
								if(EvaluateExpression(values))
								{
									value = pParts[1];
									if(values.Exists(x =>
										x.Name.ToLower() == value.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									//	If the value is direct, border spacing is allowed.
									expressionText =
										Tools.ReplaceFirst(expressionText, statement,
										value);
								}
								else
								{
									value = pParts[2];
									if(values.Exists(x =>
										x.Name.ToLower() == value.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									//	If the value is direct, border spacing is allowed.
									expressionText =
										Tools.ReplaceFirst(expressionText, statement,
										value);
								}
								expression = expressionText;
								result = true;
								break;
						}
					}
					if(!result)
					{
						//	Only continue if special command has not been activated.
						matches = Regex.Matches(expression,
							ResourceMain.CommandEvalPattern);
						foreach(Match match in matches)
						{
							statement = Tools.GetValue(match, "command");
							command = Tools.GetValue(match, "name").ToLower();
							param = Tools.GetValue(match, "params");
							switch(command)
							{
								case "aan":
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									value = GetIndefiniteArticle(value);
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									result = true;
									break;
								case "clearrenderedcontent":
									//	Clear the rendered content collection, and reset the file
									//	dirty flag.
									mRenderedContent.Clear();
									Console.WriteLine("Rendered content cleared...");
									expressionText = Tools.ReplaceFirst(expressionText,
										statement, "");
									values.SetValue(":canskip:", "1");
									result = true;
									break;
								case "confighas":
									//	Return a value indicating whether the configuration table
									//	contains the specified name.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									expressionText =
										Tools.ReplaceFirst(expressionText, statement,
										mConfigurations.Exists(value).ToString());
									result = true;
									break;
								case "entryhas":
									//	Return a value indicating whether the currently selected
									//	metadata entry has a value of the specified name.
									if(mComponent != null)
									{
										if(values.Exists(x =>
											x.Name.ToLower() == param.Trim().ToLower()))
										{
											//	Indirect.
											value = values.GetValue(param.Trim());
										}
										else
										{
											//	Direct.
											value = param.Trim();
										}
										expressionText =
											Tools.ReplaceFirst(expressionText, statement,
											mComponent.EntryExists(value).ToString());
										result = true;
									}
									break;
								case "getcomponentcount":
									//	Return the number of components.
									expressionText =
										Tools.ReplaceFirst(expressionText, statement,
										mComponents.Count.ToString());
									result = true;
									break;
								case "getentrycount":
									//	Return the count of entries on the currently selected
									//	component.
									if(mComponent != null)
									{
										expressionText =
											Tools.ReplaceFirst(expressionText, statement,
											mComponent.GetEntryCount().ToString());
										result = true;
									}
									break;
								case "getentrynames":
									//	Return a delimited list of entry names on the currently
									//	selected component.
									if(mComponent != null)
									{
										expressionText =
											Tools.ReplaceFirst(expressionText, statement,
											mComponent.GetEntryNames());
									}
									break;
								case "instr":
									//	Return a number cooresponding to the location at which
									//	the specified pattern is found.
									value = "0";
									pParts = param.Split(pParameter);
									if(pParts.Length > 1)
									{
										//	At least two parameters.
										if(values.Exists(x =>
											x.Name.ToLower() == pParts[0].Trim().ToLower()))
										{
											//	Indirect.
											param1 = values.GetValue(pParts[0].Trim());
										}
										else
										{
											//	Direct.
											//	In this case, border spaces are allowed.
											param1 = pParts[0];
										}
										if(values.Exists(x =>
											x.Name.ToLower() == pParts[1].Trim().ToLower()))
										{
											//	Indirect.
											param2 = values.GetValue(pParts[1].Trim());
										}
										else
										{
											//	Direct.
											//	In this case, border spaces are allowed.
											param2 = pParts[1];
										}
										value = (param1.IndexOf(param2) + 1).ToString();
										expressionText =
											Tools.ReplaceFirst(expressionText, statement, value);
									}
									result = true;
									break;
								case "length":
									//	Return the length of the parameter.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									value = value.Length.ToString();
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									result = true;
									break;
								case "lower":
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									value = value.ToLower();
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									result = true;
									break;
								case "nodehas":
									//	Return a value indicating whether the current node
									//	contains the specified value.
									if(mNode != null)
									{
										if(values.Exists(x =>
											x.Name.ToLower() == param.Trim().ToLower()))
										{
											//	Indirect.
											value = values.GetValue(param.Trim());
										}
										else
										{
											//	Direct.
											value = param.Trim();
										}
										expressionText =
											Tools.ReplaceFirst(expressionText, statement,
											NodeHas(mNode, value).ToString());
										result = true;
									}
									break;
								case "objecthas":
									//	Return a value indicating whether the current object
									//	contains the specified value.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									expressionText =
										Tools.ReplaceFirst(expressionText, statement,
										mComponents.ComponentExists(value).ToString());
									result = true;
									break;
								case "plural":
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									value = GetPlural(value);
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									result = true;
									break;
								case "print":
									//	Print the contents of the parameter.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									Console.WriteLine(value);
									expressionText = Tools.ReplaceFirst(expressionText,
										statement, "");
									values.SetValue(":canskip:", "1");
									result = true;
									break;
								case "savefile":
									//	Save the current rendered output to the value specified
									//	in the parameter.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									//	All parts of the expression have to be resolved before
									//	saving.
									if(value.Length > 0)
									{
										value = ReplaceVariables(null, value, values);
										if(SaveFile(value))
										{
											Console.WriteLine(
												string.Format("File {0} saved...", value));
										}
									}
									expressionText = Tools.ReplaceFirst(expressionText,
										statement, "");
									values.SetValue(":canskip:", "1");
									result = true;
									break;
								case "sort":
									//	Sort the contents of the parameter, in ascending order.
									//	Contents are presented as a delimited string.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									expressionText = Tools.ReplaceFirst(expressionText,
										statement, Tools.SortDelimited(value, true));
									result = true;
									break;
								case "sortdesc":
									//	Sort the contents of the parameter, in descending order.
									//	Contents are presented as a delimited string.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									expressionText = Tools.ReplaceFirst(expressionText,
										statement, Tools.SortDelimited(value, false));
									result = true;
									break;
								case "spaceto":
									//	Create space to the specified place on the string.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									iCount = match.Length;
									iIndex = expressionText.IndexOf(statement);
									int.TryParse(value, out iValue);
									if(iValue > 1)
									{
										if(iIndex > 0)
										{
											//	Save the left text prior to the cursor.
											param1 = expressionText.Substring(0, iIndex);
										}
										else
										{
											param1 = "";
										}
										if(expressionText.Length > iIndex + iCount)
										{
											//	Save the right portion of the string.
											param2 = expressionText.Substring(iIndex + iCount,
												expressionText.Length - (iIndex + iCount));
										}
										else
										{
											param2 = "";
										}
										if(builder.Length > 0)
										{
											builder.Remove(0, builder.Length);
										}
										builder.Append(param1);
										if(builder.Length < iValue)
										{
											//	Spaces can be inserted.
											builder.Append(new string(' ', iValue - builder.Length));
										}
										builder.Append(param2);
										value = builder.ToString();
										expressionText = value;
									}
									else
									{
										expressionText =
											Tools.ReplaceFirst(expressionText, statement, "");
									}
									result = true;
									break;
								case "sqltype":
									//	Return the SQL data type corresponding to the specified
									//	general data type.
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim()).ToLower();
									}
									else
									{
										//	Direct.
										value = param.Trim().ToLower();
									}
									switch(value)
									{
										case "binary":
										case "bit":
										case "bool":
										case "yes/no":
											value = "bit";
											break;
										case "date":
										case "datetime":
										case "date/time":
										case "smalldatetime":
											value = "smalldatetime";
											break;
										case "decimal":
										case "float":
										case "single":
											value = "float";
											break;
										case "guid":
										case "uniqueidentifier":
											value = "uniqueidentifier";
											break;
										case "int":
										case "int32":
										case "pk":
											value = "int";
											break;
										case "string":
										case "varchar":
											value = "varchar";
											break;
										default:
											if(value.Length > 5 && value.StartsWith("float"))
											{
												value = "float";
											}
											break;
									}
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									break;
								case "tab":
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									int.TryParse(value, out iValue);
									if(iValue > 0)
									{
										expressionText =
											Tools.ReplaceFirst(expressionText, statement,
											new String('\t', iValue));
									}
									result = true;
									break;
								case "upper":
									if(values.Exists(x =>
										x.Name.ToLower() == param.Trim().ToLower()))
									{
										//	Indirect.
										value = values.GetValue(param.Trim());
									}
									else
									{
										//	Direct.
										value = param.Trim();
									}
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, value);
									result = true;
									break;
								case "valueset":
									//	Set a value on the current node.
									pParts = param.Split(pNameValue);
									if(pParts.Length > 0)
									{
										//	Name was found.
										if(pParts.Length > 1)
										{
											//	Value was found.
											values.SetValue(":set:" + pParts[0].Trim(),
												pParts[1].Trim());
										}
										else
										{
											//	Use the name only.
											values.SetValue(":set:" + pParts[0].Trim(), "");
										}
									}
									expressionText =
										Tools.ReplaceFirst(expressionText, statement, "");
									values.SetValue(":canskip:", "1");
									result = true;
									break;
							}
						}
						expression = expressionText;
					}
				}
				if(result)
				{
					values.SetValue("expression", expression);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetConfigValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the specified configuration setting.
		/// </summary>
		/// <param name="configs">
		/// Collection of configuration collections.
		/// </param>
		/// <param name="entryName">
		/// Name of the entry to find.
		/// </param>
		/// <param name="entryValue">
		/// Value to set.
		/// </param>
		private void SetConfigValue(ConfigurationCollection configs,
			string entryName, string entryValue)
		{
			configs.SetValue(entryName, entryValue);
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Components																														*
		//*-----------------------------------------------------------------------*
		private ComponentCollection mComponents = new ComponentCollection();
		/// <summary>
		/// Get/Set a reference to the collection of components assigned to this
		/// tree.
		/// </summary>
		public ComponentCollection Components
		{
			get { return mComponents; }
			set { mComponents = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Configurations																												*
		//*-----------------------------------------------------------------------*
		private ConfigurationCollection mConfigurations =
			new ConfigurationCollection();
		/// <summary>
		/// Get/Set a reference to the collection of configurations assigned to
		/// this tree.
		/// </summary>
		public ConfigurationCollection Configurations
		{
			get { return mConfigurations; }
			set { mConfigurations = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InventoryTemplate																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create an inventory report of the variables and commands found on the
		/// specified template, and  save that to the rendered content collection.
		/// </summary>
		/// <param name="template">
		/// Reference to the template to be reported upon.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was successful.
		/// </returns>
		public bool InventoryTemplate(TemplateItem template)
		{
			int aCount = 0;
			AttributeItem aggregate = null;
			AttributeCollection aggregator = null;
			bool result = false;

			if(template != null)
			{
				mRenderedContent.Add(
					string.Format(
					"Template Inventory: {0}", template.Name));
				result = BuildTemplate(template);
				if(result)
				{
					mEntryCollection = new AttributeCollection();
					AnalyzeNodes(this);
					//	Upon reaching this point, config values, metadata values, and
					//	command names are all listed in a single-file attribute
					//	collection.
					aggregator = new AttributeCollection();
					foreach(AttributeItem item in mEntryCollection)
					{
						aggregate = aggregator.AddUnique(item.Name);
						int.TryParse(aggregate.Value, out aCount);
						aCount++;
						aggregate.Value = aCount.ToString();
					}
					aggregator = AttributeCollection.SortByName(aggregator);
					foreach(AttributeItem item in aggregator)
					{
						mRenderedContent.Add(" " + item.Name + ": " + item.Value);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ListTemplate																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a report of the node found on the
		/// specified template, and  save that to the rendered content collection.
		/// </summary>
		/// <param name="template">
		/// Reference to the template to report upon.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was successful.
		/// </returns>
		public bool ListTemplate(TemplateItem template)
		{
			bool result = false;

			if(template != null)
			{
				mRenderedContent.Add(
					string.Format(
					"Node List: {0}", template.Name));
				result = BuildTemplate(template);
				if(result)
				{
					ListNodes(this);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectPath																														*
		//*-----------------------------------------------------------------------*
		private string mProjectPath = "";
		/// <summary>
		/// Get/Set the name of the current base project path.
		/// </summary>
		public string ProjectPath
		{
			get { return mProjectPath; }
			set { mProjectPath = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderedContent																												*
		//*-----------------------------------------------------------------------*
		private TrackingStringCollection mRenderedContent =
			new TrackingStringCollection();
		/// <summary>
		/// Get a reference to the rendered content of this tree.
		/// </summary>
		public TrackingStringCollection RenderedContent
		{
			get { return mRenderedContent; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderTemplate																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the tree using loaded template and the other available
		/// resources.
		/// </summary>
		/// <param name="template">
		/// The active template for this pass.
		/// </param>
		/// <returns>
		/// True if the operation was successful. Otherwise, false.
		/// </returns>
		/// <remarks>
		/// Loops and conditions will control the depth of the tree.
		/// </remarks>
		public bool RenderTemplate(TemplateItem template)
		{
			bool result = false;

			mRenderedContent.Clear();
			result = BuildTemplate(template);
			if(result)
			{
				//	Basic template is built. Set the global TemplateName config value.
				SetConfigValue(mConfigurations, "TemplateName", template.Name);
			}
			if(result)
			{
				//	The template is built. Resolve all nodes.
				result = ResolveNodes(this);
			}
			if(result)
			{
				//	Smoothing and finalization.
				result = PostProcessing();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SaveFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the resolved contents of the tree to the file specified in
		/// Configurations[.][TemplateName + ".OutputFilename"].
		/// </summary>
		/// <returns>
		/// Value indicating whether the file was saved successfully.
		/// </returns>
		public bool SaveFile()
		{
			FileInfo file = null;
			string filename = "";
			bool result = false;
			TextWriter writer = null;

			if(mTemplate != null && mTemplate.Name?.Length > 0)
			{
				filename =
					GetConfigValue(mConfigurations, mTemplate.Name + ".OutputFilename");
				if(filename.Length > 0)
				{
					if(Tools.IsRelative(filename))
					{
						file = new FileInfo(Path.Combine(mProjectPath, filename));
						Directory.CreateDirectory(file.DirectoryName);
						writer = File.CreateText(file.FullName);
					}
					else
					{
						writer = File.CreateText(filename);
					}
					foreach(string line in mRenderedContent)
					{
						writer.WriteLine(line);
					}
					writer.Flush();
					writer.Close();
					writer.Dispose();
					mRenderedContent.IsDirty = false;
					result = true;
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Save the resolved contents of the tree to the file specified in the
		/// file information object.
		/// </summary>
		/// <param name="file">
		/// File information object representing the full path of the file to save.
		/// </param>
		/// <returns>
		/// Value indicating whether the file was saved successfully.
		/// </returns>
		public bool SaveFile(FileInfo file)
		{
			bool result = false;
			TextWriter writer = null;

			try
			{
				Directory.CreateDirectory(file.DirectoryName);
				writer = File.CreateText(file.FullName);
				foreach(string line in mRenderedContent)
				{
					writer.WriteLine(line);
				}
				writer.Flush();
				writer.Close();
				writer.Dispose();
				mRenderedContent.IsDirty = false;
				result = true;
			}
			catch { }
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Save the current rendered file content to the specified file.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to which the content will be saved. If relative, the
		/// ProjectPath property will be used.
		/// </param>
		/// <returns>
		/// Value indicating whether the file save was a success.
		/// </returns>
		public bool SaveFile(string filename)
		{
			FileInfo iFile = null;
			bool result = false;

			if(filename?.Length > 0)
			{
				if(Tools.IsRelative(filename))
				{
					iFile = new FileInfo(Path.Combine(mProjectPath, filename));
				}
				else
				{
					iFile = new FileInfo(filename);
				}
				result = SaveFile(iFile);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Template																															*
		//*-----------------------------------------------------------------------*
		private TemplateItem mTemplate = new TemplateItem();
		/// <summary>
		/// Get/Set a reference to the template assigned to this tree.
		/// </summary>
		public TemplateItem Template
		{
			get { return mTemplate; }
			set { mTemplate = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
