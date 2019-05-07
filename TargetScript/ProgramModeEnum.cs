//	ProgramModeEnum.cs
//	Enumeration of program mode types.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ProgramModeTypeEnum																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of program mode types.
	/// </summary>
	public enum ProgramModeTypeEnum
	{
		/// <summary>
		/// No mode specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Render the JSON project to output specified documents.
		/// </summary>
		RenderProject,
		/// <summary>
		/// Convert a file to template.
		/// </summary>
		ConvertToTemplate,
		/// <summary>
		/// Load the project and take inventory on all of the variables and
		/// commands.
		/// </summary>
		InventoryProject,
		/// <summary>
		/// List the node placement in the specified project.
		/// </summary>
		ListNodes
	}
	//*-------------------------------------------------------------------------*
}
