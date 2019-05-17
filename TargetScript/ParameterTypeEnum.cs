using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ParameterTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of possible parameter types.
	/// </summary>
	[Flags]
	public enum ParameterTypeEnum
	{
		/// <summary>
		/// Unknown or undefined.
		/// </summary>
		None =						0x00,
		/// <summary>
		/// Direct or literal value.
		/// </summary>
		Direct =					0x01,
		/// <summary>
		/// Indirect configuration-based value.
		/// </summary>
		IndirectConfig =	0x02,
		/// <summary>
		/// Indirect value from metadata.
		/// </summary>
		IndirectMeta =		0x04,
		/// <summary>
		/// Item resolves to a command or function call.
		/// </summary>
		Command =					0x08,
		/// <summary>
		/// Item is a command, but occupies text area until post-processing.
		/// </summary>
		DelayedCommand =	0x10,
		/// <summary>
		/// Single dimensional delimited list.
		/// </summary>
		Delimited =				0x20,
		/// <summary>
		/// Name:Value.
		/// </summary>
		NameValue =				0x40,
		/// <summary>
		/// ;...;Name:Value list
		/// </summary>
		List =						0x80
	}
	//*-------------------------------------------------------------------------*
}
