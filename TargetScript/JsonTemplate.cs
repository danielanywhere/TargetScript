//	JsonTemplate.cs
//	Handling for JSON-based templates.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	JsonTemplateCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of JsonTemplateItem Items.
	/// </summary>
	public class JsonTemplateCollection : List<JsonTemplateItem>
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
		//*	ParseProject																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the project text content into an abstracted definition
		/// collection.
		/// </summary>
		/// <param name="content">
		/// JSON string content to be deserialized.
		/// </param>
		/// <param name="projectPath">
		/// Path to the local working directory.
		/// </param>
		/// <param name="configs">
		/// Configuration tables loaded in this session.
		/// </param>
		/// <param name="components">
		/// Metadata component pages loaded in this session.
		///	</param>
		///	<param name="templates">
		///	Template definitions loaded in this session.
		///	</param>
		/// <returns>
		/// Newly created and resolved JSON Template collection.
		/// </returns>
		/// <remarks>
		/// The project and template files both use generic templates.
		/// </remarks>
		public static void Parse(string content,
			string projectPath, ConfigurationCollection configs,
			ComponentCollection components, TemplateCollection templates)
		{
			ComponentItem iComponent = null;
			ConfigurationItem iConfig = null;
			TemplateItem iTemplate = null;
			JsonTemplateCollection project = JsonConvert.
					DeserializeObject<JsonTemplateCollection>(content);
			string typeName = "";

			foreach(JsonTemplateItem template in project)
			{
				typeName = template.TypeName.ToLower();
				switch(typeName)
				{
					case "componentpage":
						iComponent = ComponentItem.Parse(template.Name,
							template.Definition, projectPath);
						components.Add(iComponent);
						break;
					case "configuration":
						iConfig =
							ConfigurationItem.Parse(template.Name,
							template.Definition, projectPath);
						configs.Add(iConfig);
						break;
					case "template":
						iTemplate = TemplateItem.Parse(template.Name,
							template.Definition, projectPath);
						templates.Add(iTemplate);
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SplitTemplates																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of string collections, arranged by template type
		/// name.
		/// </summary>
		/// <param name="content">
		/// File content to inspect.
		/// </param>
		/// <returns>
		/// Catalog of strings, ordered by object section TypeName. If no
		/// JSON template objects were found, the catalog is empty.
		/// </returns>
		public static StringCatalog SplitTemplates(string content)
		{
			string itemContent = "";
			MatchCollection matches = null;
			StringCatalog result = new StringCatalog();
			string typeName = "";

			if(content?.Length > 0)
			{
				matches = Regex.Matches(content,
					ResourceMain.JsonTemplateSplitPattern);
				foreach(Match match in matches)
				{
					typeName = Tools.GetValue(match, "type");
					itemContent = Tools.GetValue(match, "object");
					result.AddUniqueCollection(typeName, itemContent);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	JsonTemplateItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A generic project template, expressed in JSON.
	/// </summary>
	public class JsonTemplateItem
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
		//*	Definition																														*
		//*-----------------------------------------------------------------------*
		private StringCollection mDefinition = new StringCollection();
		/// <summary>
		/// Get a reference to the definition elements of this template.
		/// </summary>
		[JsonProperty(Order = 3)]
		public StringCollection Definition
		{
			get { return mDefinition; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MakeTemplate																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the caller's file, converted to a JSON string-based template.
		/// </summary>
		/// <param name="content">
		/// Source file content.
		/// </param>
		/// <param name="tabCharacter">
		/// Tab character to seek.
		/// </param>
		/// <param name="tabCount">
		/// Number of characters to count as a single indent. This value is only
		/// useful when the tab character is a space or other pre-measured width
		/// character.
		/// </param>
		/// <returns>
		/// JSON text template compatible for use with TargetScript.
		/// </returns>
		public static string MakeTemplate(string content,
			char tabCharacter, int tabCount)
		{
			int fCount = 0;			//	File line count.
			int fIndex = 0;			//	File line index.
			int iCount = 0;     //	Indent count.
			int iIndex = 0;     //	Indent index.
			int indent = 0;			//	Current indent.
			string line = "";		//	Current line content.
			string[] lines = new string[0];   //	Collection of source lines.
			StringBuilder result = new StringBuilder();

			if(content?.Length > 0)
			{
				if(tabCount == 0)
				{
					tabCount = 1;
				}
				line = content.Replace("\r", "");
				lines = line.Split(new char[] { '\n' });
				fCount = lines.Length;
				for(fIndex = 0; fIndex < fCount; fIndex ++)
				{
					//	Each line.
					line = lines[fIndex];
					iCount = 0;
					if(line.Length > 0 && line[0] == tabCharacter)
					{
						for(iIndex = 0; iIndex < line.Length; iIndex ++)
						{
							if(line[iIndex] == tabCharacter)
							{
								iCount = iIndex + 1;
							}
							else
							{
								break;
							}
						}
						if(iCount >= 0)
						{
							//	Remove all indent characters from the start of line.
							line = line.Substring(iCount, line.Length - iCount);
							//	Reduce to 1 tab per x characters. iCount is now
							iCount /= tabCount;
							while(iCount > indent)
							{
								if(result.Length > 0)
								{
									result.Append(",\r\n");
								}
								if(indent > 0)
								{
									result.Append(new string('\t', indent));
								}
								indent++;
								result.Append("\"{IncIndent}\"");
							}
						}
					}
					if(line.Length > 0)
					{
						while(iCount < indent && indent > 0)
						{
							if(result.Length > 0)
							{
								result.Append(",\r\n");
							}
							result.Append(new string('\t', indent--));
							result.Append("\"{DecIndent}\"");
						}
					}
					if(result.Length > 0)
					{
						result.Append(",\r\n");
					}
					if(indent > 0)
					{
						result.Append(new string('\t', indent));
					}
					//	Padding on curly braces.
					line = Regex.Replace(line,
						ResourceMain.JsonTemplateCKeywordPattern,
						ResourceMain.JsonTemplateCKeywordReplacement);
					//	Padding on square braces.
					line = Regex.Replace(line,
						ResourceMain.JsonTemplateSKeywordPattern,
						ResourceMain.JsonTemplateSKeywordReplacement);
					line = line.Replace("\t", @"\t");
					line = line.Replace(@"\", @"\\");
					line = line.Replace("\"", "\\\"");
					result.Append("\"");
					result.Append(line);
					result.Append("\"");
				}
				result.Insert(0, "[\r\n");
				result.Append("\r\n]\r\n");
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this template.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TypeName																															*
		//*-----------------------------------------------------------------------*
		private string mTypeName = "";
		/// <summary>
		/// Get/Set the name of the template type.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string TypeName
		{
			get { return mTypeName; }
			set { mTypeName = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
