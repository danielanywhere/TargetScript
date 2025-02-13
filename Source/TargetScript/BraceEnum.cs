/*
 * Copyright (c). 2018 - 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

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
