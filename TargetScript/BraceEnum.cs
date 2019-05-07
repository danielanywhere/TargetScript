//	BraceEnum.cs
//	Enumeration of brace types.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using System;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	BraceEnumType																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of possible phrase brace types.
	/// </summary>
	[Flags]
	public enum BraceEnumType
	{
		/// <summary>
		/// No brace specified or unknown.
		/// </summary>
		None =				0x00,
		/// <summary>
		/// Parenthesis enclosure.
		/// </summary>
		Parenthesis =	0x01,
		/// <summary>
		/// Curly brace enclosure.
		/// </summary>
		Curly =				0x02,
		/// <summary>
		/// Square brace enclosure.
		/// </summary>
		Square =			0x04,
		/// <summary>
		/// Double-quote enclosure.
		/// </summary>
		Quote =				0x08,
		/// <summary>
		/// Single-quote enclosure.
		/// </summary>
		Apostrophe =	0x10
	}
	//*-------------------------------------------------------------------------*
}
