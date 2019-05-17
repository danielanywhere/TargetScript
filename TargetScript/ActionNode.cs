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
		/// <summary>
		/// Get a reference to the collection of attribute values on this node.
		/// </summary>
		public AttributeCollection Attributes
		{
			get
			{
				AttributeCollection result = null;
				if(mExpression != null)
				{
					result = mExpression.Attributes;
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Expression																														*
		//*-----------------------------------------------------------------------*
		private ExpressionTree mExpression = null;
		/// <summary>
		/// Get/Set a reference to the pre-partitioned expression for this node.
		/// </summary>
		public ExpressionTree Expression
		{
			get { return mExpression; }
			set { mExpression = value; }
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
		/// Stack of component / element follower settings.
		/// </summary>
		private Stack<ComponentElementFollower> mComponentElementStack =
			new Stack<ComponentElementFollower>();
		/// <summary>
		/// Selected collection of metadata entry values.
		/// </summary>
		private AttributeCollection mElement = null;
		/// <summary>
		/// Expression element handler for all expressions simultaneously.
		/// </summary>
		private ExpressionEventBoard mEventBoard = null;
		/// <summary>
		/// Internal indent tracking.
		/// </summary>
		private int mIndent = 0;
		/// <summary>
		///	Current node under test.
		/// </summary>
		private ActionNodeItem mNode = null;

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
						mElement.Add(command + "()", "");
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

			//if(expression.IndexOf("bnk") > -1 &&
			//	expression.IndexOf("{ObjectName}") > -1)
			//{
			//	Console.WriteLine("Break here...");
			//}
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
									mElement.Add(name, "");
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
		/// All found elements are added to the mElement field, where they
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
							mElement.Add("ConditionStart()", nName);
							AnalyzeInnerVariables(expressionText);
							if(node.Nodes.Count > 0)
							{
								mElement.Add(">", "");
								result = AnalyzeNodes(node.Nodes);
								mElement.Add("<", "");
							}
							mElement.Add("ConditionEnd()", nName);
							break;
						case "config":
							//	{SetConfig(Name:Value)}
							//	Set the specified configuration entry name to the supplied
							//	value.
							mElement.Add("SetConfig()", nName);
							AnalyzeInnerVariables(node.Attributes.GetValue("expression"));
							break;
						case "loop":
							//	{LoopStart(Name:Value; ...)}
							expressionText = node.Attributes.GetValue("expression");
							mElement.Add("LoopStart()", nName);
							AnalyzeInnerVariables(expressionText);
							if(node.Nodes.Count > 0)
							{
								mElement.Add(">", "");
								result = AnalyzeNodes(node.Nodes);
								mElement.Add("<", "");
							}
							mElement.Add("LoopEnd()", nName);
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
			List<string> commands = null;
			ExpressionElement eNode = null;
			ExpressionTree tree = null;
			int iCount = 0;
			int iIndex = 0;
			string line = "";
			string name = "";
			ActionNodeItem node = null;
			ActionNodeCollection nodes = this;
			bool result = false;
			string value = "";

			this.Clear();
			mTemplate = template;

			if(mEventBoard == null)
			{
				mEventBoard = new ExpressionEventBoard();
				mEventBoard.RequestConfigValue +=
					new ExpressionElementEventHandler(expression_RequestConfigValue);
				mEventBoard.RequestMetadataValue +=
					new ExpressionElementEventHandler(expression_RequestMetadataValue);
				mEventBoard.RequestRunCommand +=
					new ExpressionElementEventHandler(expression_RequestRunCommand);
			}
			if(mTemplate != null && mTemplate.Lines.Count > 0)
			{
				result = true;
				//	Get the loop condition.
				iCount = mTemplate.Lines.Count;
				for(iIndex = 0; iIndex < iCount; iIndex ++)
				{
					//	Each line in the source file.
					line = mTemplate.Lines[iIndex];
					tree = ExpressionTree.Parse(line, mEventBoard);
					commands = ExpressionTree.GetCommandNames(tree);
					//	The followimg commands will be processed at this stage.
					//	- Continue. Merge the contents of the next line onto this one.
					//	- LoopStart / LoopEnd. Prepare the local attributes with the
					//		Name:Value list contents.
					//	-	ConditionStart / ConditionEnd. Prepare the local attributes
					//		with the Name:Value list contents.
					if(commands.Exists(x => x.ToLower() == "continue"))
					{
						//	If the continue command is present, take care of that first
						//	and reprocess the line.
						eNode = ExpressionTree.GetCommand(tree, "continue");
						if(eNode != null)
						{
							if(eNode.Index > 0)
							{
								line = line.Substring(0, eNode.Index);
							}
							else
							{
								line = "";
							}
							if(iIndex + 1 < iCount)
							{
								//	Something can be taken from the next line.
								line += mTemplate.Lines[iIndex + 1];
								mTemplate.Lines.RemoveAt(iIndex + 1);
							}
							mTemplate.Lines[iIndex] = line;
							if(iIndex + 1 < iCount)
							{
								iIndex--;
								iCount--;
							}
							continue;
						}
					}
					else if(commands.Exists(x => x.ToLower() == "loopstart"))
					{
						node = new ActionNodeItem();
						node.Value = "loop";
						tree.ActionNode = node;
						node.Expression = tree;
						nodes.Add(node);
						nodes = node.Nodes;
					}
					else if(commands.Exists(x => x.ToLower() == "loopend"))
					{
						name = tree.Attributes.GetValue("name").ToLower();
						if(name.Length > 0)
						{
							while(node.Parent != null)
							{
								nodes = node.Parent;
								node = nodes.Parent;
								if(node != null)
								{
									nodes = node.Parent;
									if(node.Value == "loop" &&
										node.Attributes.GetValue("name").ToLower() == name)
									{
										//	Last named loop.
										break;
									}
								}
							}
						}
						else
						{
							while(node.Parent != null)
							{
								nodes = node.Parent;
								node = nodes.Parent;
								if(node != null)
								{
									nodes = node.Parent;
									if(node.Value == "loop")
									{
										//	Last known loop.
										break;
									}
								}
								else
								{
									break;
								}
							}
						}
					}
					else if(commands.Exists(x => x.ToLower() == "conditionstart"))
					{
						node = new ActionNodeItem();
						node.Value = "condition";
						tree.ActionNode = node;
						node.Expression = tree;
						nodes.Add(node);
						nodes = node.Nodes;
					}
					else if(commands.Exists(x => x.ToLower() == "conditionend"))
					{
						name = tree.Attributes.GetValue("name").ToLower();
						if(name.Length > 0)
						{
							while(node.Parent != null)
							{
								nodes = node.Parent;
								node = nodes.Parent;
								if(node != null)
								{
									nodes = node.Parent;
									if(node.Value == "condition" &&
										node.Attributes.GetValue("name").ToLower() == name)
									{
										//	Last named loop.
										break;
									}
								}
							}
						}
						else
						{
							while(node.Parent != null)
							{
								nodes = node.Parent;
								node = nodes.Parent;
								if(node != null)
								{
									nodes = node.Parent;
									if(node.Value == "condition")
									{
										//	Last known condition.
										break;
									}
								}
								else
								{
									break;
								}
							}
						}
					}
					else
					{
						//	Normal line.
						if(commands.Exists(x => x.ToLower() == "savefile"))
						{
							//	The save file command is present on this line. Make sure
							//	it is the first thing on the line.
							eNode = ExpressionTree.GetCommand(tree, "savefile");
							if(eNode.Index > 0)
							{
								//	Place the preceeding text in the current line, and
								//	the command in the following line. Afterward, reprocess
								//	the current line.
								value = line.Substring(0, eNode.Index);
								line = line.Substring(eNode.Index);
								mTemplate.Lines[iIndex] = value;
								mTemplate.Lines.Insert(iIndex + 1, line);
								iIndex--;
								iCount++;
							}
							else
							{
								//	Command is at the beginning of the line.
								//	Add as normal item.
								node = new ActionNodeItem();
								node.Value = line;
								tree.ActionNode = node;
								node.Expression = tree;
								nodes.Add(node);
							}
						}
						else
						{
							node = new ActionNodeItem();
							node.Value = line;
							tree.ActionNode = node;
							node.Expression = tree;
							nodes.Add(node);
						}
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
			AttributeItem attribute = null;
			bool bExists = false;
			ExpressionContext context = null;
			IGenericExpression<bool> evaluator = null;
			string expressionText = expression;
			MatchCollection matches = null;
			ExpressionTree resolver = null;
			bool result = false;
			List<AttributeItem> values = null;
			VariableCollection variables = null;
			string word = "";

			if(expression?.Length > 0)
			{
				//	Expression is specified.
				bExists = true;
				//	Convert from tagged reference, if necessary.
				expressionText =
					ExpressionTree.Render(node.Expression, expressionText);
				//	Resolve higher level commands.
				expressionText = Tools.ReduceHighLevelCommands(expressionText);
				//	Resolve all variables and run-time functions.
				resolver =
					ExpressionTree.Parse(expressionText, node.Expression.EventBoard);
				resolver.ActionNode = node;
				expressionText = resolver.Reduce();

				context = new ExpressionContext();
				variables = context.Variables;
				values = AttributeCollection.CloneItems(node.Attributes);
				//	Add all normal words as variables.
				matches = Regex.Matches(expressionText,
					ResourceMain.ExpressionWordPattern);
				foreach(Match match in matches)
				{
					//	Normal words were found.
					word = Tools.GetValue(match, "word");
					if(!Regex.IsMatch(word,
						ResourceMain.ExpressionPlaceholderPattern) &&
						!Regex.IsMatch(word,
						ResourceMain.ExpressionOperatorPattern))
					{
						if(!values.Exists(x => x.Name.ToLower() == word.ToLower()))
						{
							attribute = new AttributeItem();
							attribute.Name = word;
							attribute.Value = word;
							values.Add(attribute);
						}
					}
				}
				if(expressionText.Length > 0)
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
						else
						{
							variables.Add(value.Name.ToLower(), value.Name.ToLower());
						}
					}
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
			}
			if(!bExists)
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	expression_RequestConfigValue																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Called when a config value is being requested for the caller's
		/// expression.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Expression element event arguments.
		/// </param>
		private void expression_RequestConfigValue(object sender,
			ExpressionElementEventArgs e)
		{
			ActionNodeItem node = ExpressionTree.GetActionNode(e.Element);

			if(node != null)
			{
				e.ResultValue = ResolveConfig(node, e.RequestName);
				e.Handled = true;
			}
			else
			{
				Console.WriteLine(
					string.Format(
					"Warning: No node assigned for {0} resolution.",
					e.RequestName));
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*expression_RequestMetadataValue																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Called when a metadata value is being requested for the caller's
		/// expression.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Expression element event arguments.
		/// </param>
		private void expression_RequestMetadataValue(object sender,
			ExpressionElementEventArgs e)
		{
			e.ResultValue = GetMetaFieldValue(e.RequestName);
			e.Handled = true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	expression_RequestRunCommand																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Called when the expression processor needs to run a command to resolve
		/// a value in the host context.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Expression element event arguments.
		/// </param>
		private void expression_RequestRunCommand(object sender,
			ExpressionElementEventArgs e)
		{
			ActionNodeItem node = ExpressionTree.GetActionNode(e.Element);

			if(node != null)
			{
				e.ResultValue =
					RunCommand(node, e.Element, e.RequestName, e.Parameters);
				e.Handled = true;
			}
			else
			{
				Console.WriteLine(
					string.Format(
					"Warning: No node assigned for command {0}.",
					e.RequestName));
			}
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
			string result = "";

			if(fieldName?.Length > 0)
			{
				fName = Tools.StripBraces(fieldName).ToLower();
				if(fName.IndexOf(":") > -1)
				{
					//	Level override is present.
					fParts = fName.Split(new char[] { ':' });
					fCount = fParts.Length;
					if(fCount > 1)
					{
						//	Left side is override. Right side is field name.
						switch(fParts[0])
						{
							case "element":
							case "entry":
							case "field":
								//	Current element.
								if(mElement?.Count > 0)
								{
									attributes = mElement;
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
				else
				{
					//	Only one name specified. Try to get the value first from the
					//	element, then from the component.
					if(mElement != null)
					{
						attributes = mElement;
					}
					else if(mComponent != null)
					{
						attributes =
							mComponent.Attributes.GetCollection("[DataType]=baseobject");
						if(attributes == null ||
							!attributes.Exists(x => x.Name.ToLower() == fName))
						{
							//	The base object of the component doesn't contain the
							//	specified element.
							attributes =
								mComponent.Attributes.GetCollectionWithMember(fName);
						}
					}
				}
				if(attributes != null)
				{
					result = attributes.GetValue(fName);
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
							//	{SetConfig(Name:Value)}
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
		//*	PostProcessingNeeded																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether post-processing is needed for the
		/// rendered content.
		/// </summary>
		/// <returns>
		/// A value indicating whether post processing is needed on the current
		/// state of the content.
		/// </returns>
		private bool PostProcessingNeeded()
		{
			string line = "";
			bool result = false;

			foreach(string source in mRenderedContent)
			{
				line = source.ToLower();
				if(line == "{decindent()}" || line == "{incindent()}" ||
					line.IndexOf("{{") > -1 || line.IndexOf("[[") > -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PostProcessLine																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform basic post-processing on the caller's printable line.
		/// </summary>
		private static string PostProcessLine(string expression)
		{
			string result = expression;
			
			if(expression?.Length > 0)
			{
				if(result.IndexOf("[[") > -1 || result.IndexOf("]]") > -1 ||
					result.IndexOf("{{") > -1 || result.IndexOf("}}") > -1)
				{
					result = Regex.Replace(result,
						ResourceMain.PostProcessingUnescapeBracePattern,
						ResourceMain.PostProcessingUnescapeBraceReplacement);
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
			ExpressionTree resolver = null;
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
					if(source.Length > 0)
					{
						source = ExpressionTree.Render(node.Expression, source);
						resolver = ExpressionTree.Parse(source, mEventBoard);
						resolver.ActionNode = node;
						source = resolver.Reduce();
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
					mElement = attributes[eIndex];
					if(!mElement.Exists(x =>
						x.Name.ToLower() == "datatype" &&
						x.Value.ToLower() == "baseobject"))
					{
						entryName = mElement.GetValue("Name");
						node.Attributes.SetValue("ElementName", entryName);
						if(EvaluateExpression(node, expressionText))
						{
							iLast = eIndex;
						}
					}
				}
				for(eIndex = 0; eIndex < eCount; eIndex++)
				{
					//	Each non-base entry.
					mElement = attributes[eIndex];
					if(!mElement.Exists(x =>
						x.Name.ToLower() == "datatype" &&
						x.Value.ToLower() == "baseobject"))
					{
						//	This is not a base object.
						entryName = mElement.GetValue("Name");
						node.Attributes.SetValue("ElementName", entryName);
						//node.Attributes.SetValue("ComponentName", componentName);
						if(EvaluateExpression(node, expressionText))
						{
							if(mVerbose)
							{
								Console.WriteLine("   Element:" + entryName);
							}
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
					//	The parent node contains any named setting for the current level.
					if(node.Parent != null && node.Parent.Parent != null)
					{
						if(node.Parent.Parent.Attributes.Exists(x =>
							x.Name.ToLower() == vVariable))
						{
							result = node.Parent.Parent.Attributes.GetValue(vVariable);
						}
						else
						{
							result = ResolveConfig(node.Parent.Parent, variable);
						}
					}
					else
					{
						//	We have searched to the base. Get the value from configuration
						//	table.
						result = GetConfigValue(mConfigurations, variable);
					}
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
		/// <item>Element - Each entry within the selected Component. If not nested
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
			ComponentElementFollower componentEntry = null;
			string[] componentNames = new string[0];
			int dCount = 0;
			string expressionText = ""; //	Current expression.
			string[] fParts = new string[0];
			int iLast = 0;          //	Last indent.
			int nCount = 0;					//	Node count.
			int nIndex = 0;					//	Node index.
			ActionNodeItem node = null;
			ExpressionTree resolver = null;
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
					if(mVerbose)
					{
						switch(vLower)
						{
							case "condition":
								Console.WriteLine("Action>Condition - n:" +
									node.Attributes.GetValue("name") + " e:" +
									node.Attributes.GetValue("expression"));
								break;
							case "loop":
								Console.WriteLine("Action>Loop - n:" +
									node.Attributes.GetValue("name") + " l:" +
									node.Attributes.GetValue("level") + " e:" +
									node.Attributes.GetValue("expression"));
								break;
							default:
								Console.WriteLine("Action>" + node.Value);
								break;
						}
					}
					expressionText = "";
					switch(vLower)
					{
						case "condition":
							//	{ConditionStart(Name:Value; ...)}
							if(node.Attributes.GetValue("name").ToLower() ==
								"setprimarykey")
							{
								//	Break here...
								dCount++;
								dCount--;
							}
							if(node.Nodes.Count > 0)
							{
								if(EvaluateExpression(node))
								{
									result = ResolveNodes(node.Nodes);
								}
							}
							break;
						case "loop":
							//	{LoopStart(Name:Value; ...)}
							if(node.Nodes.Count > 0)
							{
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
										if(source.Length > 0)
										{
											source = ExpressionTree.Render(node.Expression, source);
											resolver = ExpressionTree.Parse(source, mEventBoard);
											resolver.ActionNode = node;
											source = resolver.Reduce();
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
												GetConfigValue(mConfigurations, "{ComponentPages}"));
									}
									cCount = componentNames.Length;
									bFirst = true;
									iLast = 0;
									//	Resolve the isLast variable with expressions.
									for(cIndex = 0; cIndex < cCount; cIndex ++)
									{
										mComponent = mComponents[componentNames[cIndex]];
										if(mComponent != null)
										{
											mElement = null;
											node.Attributes.SetValue("ComponentName", mComponent.Name);
											node.Attributes.SetValue("ObjectName",
												GetMetaFieldValue("Name"));
											if(EvaluateExpression(node, expressionText))
											{
												iLast = cIndex;
											}
										}
									}
									for(cIndex = 0; cIndex < cCount; cIndex ++)
									{
										sCount = mComponentElementStack.Count;
										componentEntry =
											new ComponentElementFollower(mComponent, mElement);
										mComponentElementStack.Push(componentEntry);
										mComponent = mComponents[componentNames[cIndex]];
										if(mComponent != null)
										{
											mElement = null;
											node.Attributes.SetValue("ComponentName", mComponent.Name);
											node.Attributes.SetValue("ObjectName",
												GetMetaFieldValue("Name"));
											if(EvaluateExpression(node, expressionText))
											{
												if(mVerbose)
												{
													Console.WriteLine(" Component:" + mComponent.Name);
												}
												node.Attributes.SetValue("isFirst",
													bFirst ? "true" : "false");
												node.Attributes.SetValue("isLast",
													cIndex == iLast ? "true" : "false");
												result = ResolveNodes(node.Nodes);
												bFirst = false;
											}
										}
										while(mComponentElementStack.Count > sCount)
										{
											//	Off-balance return is allowed from inner levels.
											componentEntry = mComponentElementStack.Pop();
											mComponent = componentEntry.Component;
											mElement = componentEntry.Element;
										}
									}
								}
								else if(node.Attributes.Count(
									x => x.Name.ToLower() == "level" &&
									x.Value.ToLower() == "element") > 0)
								{
									//	Process for entries.
									sCount = mComponentElementStack.Count;
									componentEntry =
										new ComponentElementFollower(mComponent, mElement);
									mComponentElementStack.Push(componentEntry);
									if(mComponent == null)
									{
										//	This loop is outside of a component loop.
										//	All components will be traced.
										componentNames =
											Tools.DelimitedToArray(
												GetConfigValue(mConfigurations, "{ComponentPages}"));
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
									while(mComponentElementStack.Count > sCount)
									{
										//	Off-balance return is allowed from inner levels.
										componentEntry = mComponentElementStack.Pop();
										mComponent = componentEntry.Component;
										mElement = componentEntry.Element;
									}
									if(!result)
									{
										break;
									}
								}
							}
							break;
						default:
							expressionText = node.Expression.Reduce();
							if(node.Attributes.GetValue(":skip:") != "1")
							{
								mRenderedContent.Add(expressionText);
								if(mVerbose)
								{
									Console.WriteLine(expressionText + "\r\n");
								}
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
		//*	RunCommand																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run one specific command, and return the resulting information to
		/// the caller.
		/// </summary>
		/// <param name="node">
		/// Node whose context is being used for any side references.
		/// </param>
		/// <param name="command">
		/// The name of the command to resolve and / or run.
		/// </param>
		/// <param name="parameters">
		/// String of zero or more parameters to be processed with the command.
		/// </param>
		/// <returns>
		/// Resolved value of the command.
		/// </returns>
		/// <remarks>
		/// Please take note of the following considerations.
		/// <list type="bullet">
		/// <item>An attribute with the name of :canskip: might be included
		/// with the node attributes, depending upon the type of command
		/// processed.</item>
		/// <item>Unlike the RunCommands method, which allows both direct
		/// and indirect parameter values, this command's parameters
		/// must all be pre-resolved prior to calling the method.</item>
		/// <item>Some commands allow leading and trailing spaces, while
		/// others don't. This has to do with the kind of processing
		/// needed to validate the parameter.</item>
		/// </list>
		/// </remarks>
		private string RunCommand(ActionNodeItem node, ExpressionElement element,
			string command, string parameters)
		{
			StringBuilder builder = new StringBuilder();
			string cLower = "";
			string expressionText = "";
			int iCount = 0;
			int iIndex = 0;
			int iValue = 0;
			string param1 = "";
			string param2 = "";
			char[] pList = new char[] { ';' };
			char[] pNameValue = new char[] { ':' };
			char[] pParameter = new char[] { ',' };
			string[] pParts = new string[0];
			string result = "";
			string value = "";

			if(command?.Length > 0)
			{
				result = "{" + command + "(" + parameters + ")}";
				cLower = command.ToLower();
				switch(cLower)
				{
					case "aan":
						//	Direct.
						value = parameters.Trim();
						result = GetIndefiniteArticle(value);
						//processed = true;
						break;
					case "clearrenderedcontent":
						//	Clear the rendered content collection, and reset the file
						//	dirty flag.
						mRenderedContent.Clear();
						Console.WriteLine("Rendered content cleared...");
						result = "";
						node.Attributes.SetValue(":canskip:", "1");
						break;
					case "confighas":
						//	Return a value indicating whether the configuration table
						//	contains the specified name.
						result = mConfigurations.Exists(parameters).ToString().ToLower();
						break;
					case "decindent":
						//	Delayed processing.
						break;
					case "elementhas":
						//	Return a value indicating whether the currently selected
						//	metadata entry has a value of the specified name.
						result = mComponent.ElementExists(parameters).ToString().ToLower();
						break;
					case "getcomponentcount":
						//	Return the number of components.
						result = mComponents.Count.ToString();
						break;
					case "getelementcount":
						//	Return the count of entries on the currently selected
						//	component.
						if(mComponent != null)
						{
							result = mComponent.GetElementCount().ToString();
						}
						else
						{
							result = "0";
						}
						break;
					case "getelementnames":
						//	Return a delimited list of entry names on the currently
						//	selected component.
						if(mComponent != null)
						{
							result = mComponent.GetElementNames();
						}
						break;
					case "ifnull":
						//	Evaluate the expression in parameter 1.
						//	If blank, use the value in parameter 2.
						//	Otherwise, use the value in parameter 3.
						result = "";
						pParts = parameters.Split(pParameter);
						if(pParts.Length == 3)
						{
							result = (pParts[0].Length == 0 ? pParts[1] : pParts[2]);
						}
						else
						{
							pParts = parameters.Split(pList);
							if(pParts.Length == 3)
							{
								result = (pParts[0].Length == 0 ? pParts[1] : pParts[2]);
							}
						}
						break;
					case "iif":
						//	Evaluate the expression in parameter 1.
						//	If true, use the value in the second parameter.
						//	If false, use the value in the third parameter.
						result = "";
						pParts = parameters.Split(pParameter);
						if(pParts.Length == 3)
						{
							//	Three parameters with comma.
							if(EvaluateExpression(node, pParts[0].Trim()))
							{
								result = pParts[1];
							}
							else
							{
								result = pParts[2];
							}
						}
						else
						{
							pParts = parameters.Split(pList);
							if(pParts.Length == 3)
							{
								//	Three parameters with semicolon.
								if(EvaluateExpression(node, pParts[0].Trim()))
								{
									result = pParts[1];
								}
								else
								{
									result = pParts[2];
								}
							}
						}
						break;
					case "incindent":
						//	Delayed command.
						break;
					case "instr":
						//	Return a number cooresponding to the location at which
						//	the specified pattern is found.
						value = "0";
						pParts = parameters.Split(pParameter);
						if(pParts.Length == 2)
						{
							//	Two parameters using comma.
							param1 = pParts[0];
							param2 = pParts[1];
							result = (param1.IndexOf(param2) + 1).ToString();
						}
						else
						{
							pParts = parameters.Split(pList);
							if(pParts.Length == 2)
							{
								//	Two parameters using semicolon.
								param1 = pParts[0];
								param2 = pParts[1];
								result = (param1.IndexOf(param2) + 1).ToString();
							}
						}
						break;
					case "length":
						//	Return the length of the parameter.
						result = parameters.Length.ToString();
						break;
					case "lower":
						result = parameters.ToLower();
						break;
					case "nodehas":
						//	Return a value indicating whether the current node
						//	contains the specified value.
						if(mNode != null)
						{
							value = parameters.Trim();
							result = NodeHas(mNode, value).ToString().ToLower();
						}
						break;
					case "objecthas":
						//	Return a value indicating whether the current object
						//	contains the specified value.
						value = parameters.Trim();
						result = mComponents.ComponentExists(value).ToString().ToLower();
						break;
					case "plural":
						value = parameters.Trim();
						result = GetPlural(value);
						break;
					case "print":
						//	Print the contents of the parameter.
						Console.WriteLine(PostProcessLine(parameters));
						result = "";
						node.Attributes.SetValue(":canskip:", "1");
						break;
					case "savefile":
						//	Save the current rendered output to the value specified
						//	in the parameter.
						value = parameters.Trim();
						//	All parts of the expression have to be resolved before
						//	saving.
						if(value.Length > 0)
						{
							if(SaveFile(value))
							{
								Console.WriteLine(
									string.Format("File {0} saved...", value));
							}
						}
						node.Attributes.SetValue(":canskip:", "1");
						result = "";
						break;
					case "selectcomponent":
						//	Select the current component from metadata.
						value = parameters.Trim();
						mComponent = mComponents[value];
						result = "";
						node.Attributes.SetValue(":canskip:", "1");
						break;
					case "selectelement":
						//	Select the current element from metadata by name.
						//	If the component is non-null, the element is selected
						//	from that component. Otherwise, the first matching
						//	element in metadata is found and selected.
						value = parameters.Trim();
						mElement = mComponents.GetElement(mComponent, value);
						result = "";
						node.Attributes.SetValue(":canskip:", "1");
						break;
					case "setconfig":
						//	Set a value in the configuration table.
						pParts = parameters.Split(pNameValue);
						if(pParts.Length > 0)
						{
							//	Name was found.
							if(pParts.Length > 1)
							{
								//	Value was found.
								SetConfigValue(mConfigurations, pParts[0].Trim(), pParts[1]);
							}
							else
							{
								//	Use the name only.
								SetConfigValue(mConfigurations, pParts[0].Trim(), "");
							}
						}
						node.Attributes.SetValue(":canskip:", "1");
						result = "";
						break;
					case "setvalue":
						//	Set a value on the current node.
						pParts = parameters.Split(pNameValue);
						if(pParts.Length > 0)
						{
							//	Name was found.
							if(pParts.Length > 1)
							{
								//	Value was found.
								SetNodeValue(node, pParts[0].Trim(), pParts[1]);
							}
							else
							{
								//	Use the name only.
								SetNodeValue(node, pParts[0].Trim(), "");
							}
						}
						node.Attributes.SetValue(":canskip:", "1");
						result = "";
						break;
					case "sort":
						//	Sort the contents of the parameter, in ascending order.
						//	Contents are presented as a delimited string.
						value = parameters.Trim();
						result = Tools.SortDelimited(value, true);
						break;
					case "sortdesc":
						//	Sort the contents of the parameter, in descending order.
						//	Contents are presented as a delimited string.
						value = parameters.Trim();
						result = Tools.SortDelimited(value, false);
						break;
					case "spaceto":
						//	Create space to the specified place on the string.
						value = parameters;
						int.TryParse(value, out iValue);
						param1 =
							ExpressionElement.GetParentTextLeftOf(element);
						iIndex = param1.Length;
						iCount = expressionText.Length;
						if(iValue > iIndex)
						{
							if(builder.Length > 0)
							{
								builder.Remove(0, builder.Length);
							}
							//	Spaces can be inserted.
							builder.Append(new string(' ', iValue - iIndex));
							result = builder.ToString();
						}
						else
						{
							result = "";
						}
						break;
					case "sqltype":
						//	Return the SQL data type corresponding to the specified
						//	general data type.
						value = parameters.Trim().ToLower();
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
						result = value;
						break;
					case "tab":
						value = parameters.Trim();
						int.TryParse(value, out iValue);
						if(iValue > 0)
						{
							result = new string('\t', iValue);
						}
						break;
					case "upper":
						result = parameters.ToUpper();
						break;
					default:
						result = "";
						break;
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

		//*-----------------------------------------------------------------------*
		//*	SetNodeValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set a named value for the node and all siblings and children.
		/// </summary>
		/// <param name="node">
		/// Reference to the node for which the value is being set.
		/// </param>
		/// <param name="name">
		/// Name of the value to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the node.
		/// </param>
		private void SetNodeValue(ActionNodeItem node,
			string name, string value)
		{
			if(name?.Length > 0)
			{
				if(node == null || node.Parent == null || node.Parent.Parent == null)
				{
					//	The value goes to the configuration table.
					SetConfigValue(mConfigurations, name, value);
				}
				else
				{
					//	Target node is available.
					node.Parent.Parent.Attributes.SetValue(name, value);
				}
			}
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
		//*	InventoryDetail																												*
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
		public bool InventoryDetail(TemplateItem template)
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
					mElement = new AttributeCollection();
					AnalyzeNodes(this);
					//	Upon reaching this point, config values, metadata values, and
					//	command names are all listed in a single-file attribute
					//	collection.
					aggregator = new AttributeCollection();
					foreach(AttributeItem item in mElement)
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
		//*	InventorySummary																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create an inventory report of the variables and commands found on the
		/// specified template, and  save that to the rendered content collection.
		/// </summary>
		/// <param name="templates">
		/// Reference to the collection of templates to be reported upon.
		/// </param>
		/// <returns>
		/// Value indicating whether the operation was successful.
		/// </returns>
		public bool InventorySummary(TemplateCollection templates)
		{
			int aCount = 0;
			AttributeItem aggregate = null;
			AttributeCollection aggregator = null;
			bool result = false;

			if(templates?.Count > 0)
			{
				mElement = new AttributeCollection();
				foreach(TemplateItem template in templates)
				{
					result = BuildTemplate(template);
					if(result)
					{
						AnalyzeNodes(this);
					}
				}
				//	Upon reaching this point, config values, metadata values, and
				//	command names are all listed in a single-file attribute
				//	collection.
				aggregator = new AttributeCollection();
				foreach(AttributeItem item in mElement)
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
				if(PostProcessingNeeded())
				{
					PostProcessing();
				}
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

			if(PostProcessingNeeded())
			{
				PostProcessing();
			}
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

		//*-----------------------------------------------------------------------*
		//*	Verbose																																*
		//*-----------------------------------------------------------------------*
		private bool mVerbose = false;
		/// <summary>
		/// Get/Set a value indicating whether actions will be logged to the
		/// console on a verbose level.
		/// </summary>
		public bool Verbose
		{
			get { return mVerbose; }
			set { mVerbose = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
