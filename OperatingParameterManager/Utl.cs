/*
    ============================================================================

    Namespace:			OperatingParameters_Demo

    Class Name:			Utl

	File Name:			Utl.cs

    Synopsis:			Store and manage an operating parameter and properties,
						such as the type of data stored therein and its expected
						use, along with a validity flag.

	Remarks:			These routines should be moved into existing libraries.

    Author:				David A. Gray

	License:            Copyright (C) 2018-2019, David A. Gray.
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Synopsis
    ---------- ------- ------ --------------------------------------------------
	2018/09/02 1.0     DAG    Initial implementation created, tested, and 
                              deployed.

    2019/06/29 1.0.3   DAG    Add missing XML documentation in preparation for
                              publication in a documented GitHub repository and
                              as a NuGet package.

    2019/06/30 1.0.6   DAG    Correct typographical errors and add clarification
                              to the XML documentation islands and this flower
                              box.

    2019/06/30 1.0.14   DAG   Eliminate rudundant WizardWrx.ConsoleAppAids3
                              using directive and correct overlooked formatting
                              inconsistencies and deviations from conventions.
    ============================================================================
*/


using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

using WizardWrx.FormatStringEngine;


namespace WizardWrx.OperatingParameterManager
{
    /// <summary>
    /// This static class exposes utility methods that are intended for internal
    /// use. Since some of these may be useful in their own right, the class is
    /// marked as public.
    /// </summary>
	public static class Utl
	{
		/// <summary>
		/// Get the value of the specified resource string name from the string
		/// resources embedded in the calling assembly.
		/// </summary>
		/// <param name="pstrStringName">
		/// Specify the unqualified name of the string as it appears in the Name
		/// column on the resource designer form.
		/// </param>
		/// <returns>
		/// If the method succeeds, the return value is the string Value read
		/// from the managed string resources embedded in the calling assembly.
		/// Otherwise, the return value is a null reference, which the caller
		/// must expect and handle by evaluating the result.
		/// </returns>
		public static string GetStringResourceByNameFromCallingAssembly ( string pstrStringName )
		{
            return GetStringResourceByNameFromAnyAssembly (
                pstrStringName ,
                Assembly.GetCallingAssembly ( ) );
		}   // GetStringResourcesFromCallingAssembly


		/// <summary>
		/// Get the value of the specified resource string name from the string
		/// resources embedded in the entry assembly.
		/// </summary>
		/// <param name="pstrStringName">
		/// Specify the unqualified name of the string as it appears in the Name
		/// column on the resource designer form.
		/// </param>
		/// <returns>
		/// If the method succeeds, the return value is the string Value read
		/// from the managed string resources embedded in the entry assembly.
		/// Otherwise, the return value is a null reference, which the caller
		/// must expect and handle by evaluating the result.
		/// </returns>
		public static string GetStringResourceByNameFromEntryAssembly ( string pstrStringName )
		{
			return GetStringResourceByNameFromAnyAssembly (
                pstrStringName ,
                Assembly.GetEntryAssembly ( ) );
		}   // GetStringResourceByNameFromEntryAssembly


		/// <summary>
		/// Get the value of the specified resource string name from the string
		/// resources embedded in any assembly.
		/// </summary>
		/// <param name="pstrStringName">
		/// Specify the unqualified name of the string as it appears in the Name
		/// column on the resource designer form.
		/// </param>
		/// <param name="pasmAny"></param>
		/// Pass in a reference to the System.Reflection.Assembly to search for
		/// the string named in argument pstrStringName, which can be ANY
		/// assembly.
		/// <returns>
		/// If the method succeeds, the return value is the string Value read
		/// from the managed string resources embedded in the specified
		/// assembly. Otherwise, the return value is a null reference, which the
		/// caller must expect and handle by evaluating the result.
		/// </returns>
		private static string GetStringResourceByNameFromAnyAssembly (
			string pstrStringName ,
			Assembly pasmAny )
		{
			const string DEFAULT_STRING_RESOURCE_NAME = @"resources";

            if ( string.IsNullOrEmpty ( pstrStringName ) )
                throw new ArgumentNullException ( nameof ( pstrStringName ) );

            if ( pasmAny == null )
                throw new ArgumentNullException ( nameof ( pasmAny ) );

			if ( pasmAny.GetManifestResourceNames ( ).Length > ListInfo.LIST_IS_EMPTY )
			{   // The assembly contains SOME embedded resourcess, though not necessarily strings.
				#if UTL_DEBUG
					int intNIterations = MagicNumbers.ZERO;
				#endif // #if UTL_DEBUG

				using ( Stream strOfResources = pasmAny.GetManifestResourceStream (
					AssemblyUtils.SortableManagedResourceItem.GetInternalResourceName (
						DEFAULT_STRING_RESOURCE_NAME ,
						pasmAny ) ) )
				{   // Unless the assembly is devoid of string resources, perform a brute force scan of the list of names.
					if ( strOfResources != null )
					{	// The assembly contains SOME embedded string resources.
						using ( ResourceReader resReader4Embedded = new ResourceReader ( strOfResources ) )
						{   // Get an enumerator, so that the list can be searched for the desired name without throwing if the specified name is missing.
							IDictionaryEnumerator resourceEnumerator = resReader4Embedded.GetEnumerator ( );

							//	------------------------------------------------
							//	Since there is no mention in GetEnumerator docs
							//	about exceptions, assume that none can arise. If
							//	one does, it will bubble up.
							//	------------------------------------------------

							while ( resourceEnumerator.MoveNext ( ) )
							{   // Roll through the list, an O(1) operation.
								#if UTL_DEBUG
									intNIterations++;
								#endif // #if UTL_DEBUG

								if ( resourceEnumerator.Key.ToString ( ) == pstrStringName )
								{   // Match found.
									#if UTL_DEBUG
										Console.WriteLine ( "{0} FOUND after {1} iterations." ,
											pstrStringName ,
											intNIterations );
									#endif // #if UTL_DEBUG

									return resourceEnumerator.Value.ToString ( );
								}   // if ( resourceEnumerator.Key.ToString ( ) == pstrStringName )
							}   // while ( resourceEnumerator.MoveNext ( ) )
						}   // using ( ResourceReader resReader4Embedded = new ResourceReader ( strOfResources ) )
					}   // if ( strOfResources != null )
				}   // using ( Stream strOfResources = pasmAny.GetManifestResourceStream ( WizardWrx.AssemblyUtils.SortableManagedResourceItem.GetInternalResourceName ( DEFAULT_STRING_RESOURCE_NAME , pasmAny ) ) )

				#if UTL_DEBUG
					Console.WriteLine ( "{0} NOT FOUND after {1} iterations." ,
						pstrStringName ,
						intNIterations );
				#endif // #if UTL_DEBUG

				return null;	// The requested resource doesn't exist in this collection.
			}   // TRUE (anticipated outcome - The assembly contains resources of some kind.) block, if ( pasmAny.GetManifestResourceNames ( ).Length > ListInfo.LIST_IS_EMPTY )
			else
			{
				return null;
			}   // FALSE (unanticipated outcome - The assembly is devoid of resources.) block, if ( pasmAny.GetManifestResourceNames ( ).Length > ListInfo.LIST_IS_EMPTY )
		}   // GetStringResourceByNameFromAnyAssembly


		/// <summary>
		/// List the absolute (fully qualified) internal names of the resources
		/// stored in an assembly.
		/// </summary>
		/// <param name="pasmSource">
		/// Pass in a reference to the assembly from which you want a list of
		/// embedded resources.
		/// </param>
		public static void ListInternalResourceNames ( Assembly pasmSource )
		{
			Console.WriteLine (
				Properties.Resources.MESSAGE_RESOURCE_NAMES_HEADER ,            // Format Control String: {1}Resources Stored in Assembly {0}:{1}
				pasmSource.FullName ,                                           // Format Item 0: in Assembly {0}:
				Environment.NewLine );                                          // Format Item 1: platform-dependent newline
			int intItemNumber = ListInfo.LIST_IS_EMPTY;

			foreach ( string strManifestResourceName in pasmSource.GetManifestResourceNames ( ) )
			{
				Console.WriteLine (
					Properties.Resources.MESSAGE_RESOURCE_NAMES_DETAIL ,        // Format Control String:     Resource # {0}: {1}
					++intItemNumber ,                                           // Format Item 0: Resource # {0}:
					strManifestResourceName );                                  // Format Item 1: : {1}
			}   // foreach ( string strManifestResourceName in pasmSource.GetManifestResourceNames ( ) )

			Console.WriteLine (
				Properties.Resources.MESSAGE_RESOURCE_NAMES_FOOTER ,            // Format Control String: {1}Count of resources stored in assembly = {0}{1}
				intItemNumber ,                                                 // Format Item 0: in assembly = {0}
				Environment.NewLine );                                          // Format Item 1: platform-dependent newline
		}   // public ListInternalResourceNames Method


		/// <summary>
		/// Format names and values of the properties reported by the ToString
		/// method on any object for rendering on a report, such as the console
		/// display of a character-mode application.
		/// </summary>
		/// <param name="pobjToList">
		/// Pass in a reference to the object of interest. Its ToString method,
		/// presumably a custom (overrridden) method is called, and the string
		/// that it returns is parsed and formatted.
		/// 
		/// Its ToString method is expected to return a string that begins with
		/// the object's internal name (type), followed by a colon and a list of
		/// properties. Each property is expected to be listed as a name-value
		/// pair, with the name and its value sapearated by an equals sign, and
		/// properties delimited by either a single character, such as a comma,
		/// or the platform-dependent newline sequence, e. g., CR/LF for Windows
		/// or LF for Linux.
		/// </param>
		/// <returns>
		/// The return value is a string that can be fed through a standard
		/// format item to string.Format, Console.WriteLine, or any of their
		/// cousins.
		/// </returns>
		public static string ListPropertiesPerDefaultToString (	object pobjToList )
		{
			return ListPropertiesPerDefaultToString (
				pobjToList ,
				SpecialCharacters.NULL_CHAR ,
				ListInfo.EMPTY_STRING_LENGTH );
		}   // ListPropertiesPerDefaultToString method (1 of 4)


		/// <summary>
		/// Format names and values of the properties reported by the ToString
		/// method on any object for rendering on a report, such as the console
		/// display of a character-mode application.
		/// </summary>
		/// <param name="pobjToList">
		/// Pass in a reference to the object of interest. Its ToString method,
		/// presumably a custom (overrridden) method is called, and the string
		/// that it returns is parsed and formatted.
		/// 
		/// Its ToString method is expected to return a string that begins with
		/// the object's internal name (type), followed by a colon and a list of
		/// properties. Each property is expected to be listed as a name-value
		/// pair, with the name and its value sapearated by an equals sign, and
		/// properties delimited by either a single character, such as a comma,
		/// or the platform-dependent newline sequence, e. g., CR/LF for Windows
		/// or LF for Linux.
		/// </param>
		/// <param name="pintAdditionalPaddingChars">
		/// Specify the number of characters of extra white space, if any, to be
		/// prepended to subsequent output lines to cause the report to aligne
		/// vertically. This is useful when the format string through which it
		/// will be written contains text that precedes the output string, which
		/// would otherwise cause subsequent detail lines to be misaligned.
		/// </param>
		/// <returns>
		/// The return value is a string that can be fed through a standard
		/// format item to string.Format, Console.WriteLine, or any of their
		/// cousins.
		/// </returns>
		public static string ListPropertiesPerDefaultToString (
			object pobjToList ,
			int pintAdditionalPaddingChars )
		{
			return ListPropertiesPerDefaultToString (
				pobjToList ,
				SpecialCharacters.NULL_CHAR ,
				pintAdditionalPaddingChars );
		}   // ListPropertiesPerDefaultToString method (2 of 4)

		/// <summary>
		/// Format names and values of the properties reported by the ToString
		/// method on any object for rendering on a report, such as the console
		/// display of a character-mode application.
		/// </summary>
		/// <param name="pobjToList">
		/// Pass in a reference to the object of interest. Its ToString method,
		/// presumably a custom (overrridden) method is called, and the string
		/// that it returns is parsed and formatted.
		/// 
		/// Its ToString method is expected to return a string that begins with
		/// the object's internal name (type), followed by a colon and a list of
		/// properties. Each property is expected to be listed as a name-value
		/// pair, with the name and its value sapearated by an equals sign, and
		/// properties delimited by either a single character, such as a comma,
		/// or the platform-dependent newline sequence, e. g., CR/LF for Windows
		/// or LF for Linux.
		/// </param>
		/// <param name="pchrDelimiter">
		/// Pass in the single-character delimiter that separates property names
		/// and their values from each other. To specify that the pairs are
		/// delimited by newlines, specify SpecialCharacters.NULL_CHAR, the NULL
		/// (numeric value zero) character.
		/// </param>
		/// <returns>
		/// The return value is a string that can be fed through a standard
		/// format item to string.Format, Console.WriteLine, or any of their
		/// cousins.
		/// </returns>
		public static string ListPropertiesPerDefaultToString (
			object pobjToList ,
			char pchrDelimiter )
		{
			return ListPropertiesPerDefaultToString (
				pobjToList ,
				pchrDelimiter ,
				ListInfo.EMPTY_STRING_LENGTH );
		}   // ListPropertiesPerDefaultToString method (3 of 4)


		/// <summary>
		/// Format names and values of the properties reported by the ToString
		/// method on any object for rendering on a report, such as the console
		/// display of a character-mode application.
		/// </summary>
		/// <param name="pobjToList">
		/// Pass in a reference to the object of interest. Its ToString method,
		/// presumably a custom (overrridden) method is called, and the string
		/// that it returns is parsed and formatted.
		/// 
		/// Its ToString method is expected to return a string that begins with
		/// the object's internal name (type), followed by a colon and a list of
		/// properties. Each property is expected to be listed as a name-value
		/// pair, with the name and its value sapearated by an equals sign, and
		/// properties delimited by either a single character, such as a comma,
		/// or the platform-dependent newline sequence, e. g., CR/LF for Windows
		/// or LF for Linux.
		/// </param>
		/// <param name="pchrDelimiter">
		/// Pass in the single-character delimiter that separates property names
		/// and their values from each other. To specify that the pairs are
		/// delimited by newlines, specify SpecialCharacters.NULL_CHAR, the NULL
		/// (numeric value zero) character.
		/// </param>
		/// <param name="pintAdditionalPaddingChars">
		/// Specify the number of characters of extra white space, if any, to be
		/// prepended to subsequent output lines to cause the report to aligne
		/// vertically. This is useful when the format string through which it
		/// will be written contains text that precedes the output string, which
		/// would otherwise cause subsequent detail lines to be misaligned.
		/// </param>
		/// <returns>
		/// The return value is a string that can be fed through a standard
		/// format item to string.Format, Console.WriteLine, or any of their
		/// cousins.
		/// </returns>
		public static string ListPropertiesPerDefaultToString (
			object pobjToList ,
			char pchrDelimiter ,
			int pintAdditionalPaddingChars )
		{
			const string THIRD_FORMAT_ITEM = @" {2}{3}";

			StringBuilder rsb = null;

			string [ ] astrProperties = null;

			astrProperties = pchrDelimiter == SpecialCharacters.NULL_CHAR
				? WizardWrx.EmbeddedTextFile.Readers.StringOfLinesToArray ( pobjToList.ToString ( ) )
				: pobjToList.ToString ( ).Split ( pchrDelimiter );
			LabelAndValue [ ] autpLabelAndValue = new LabelAndValue [ astrProperties.Length ];
			string strTypeName = null;  // The string.IsNullOrEmpty method requires an initialized argument.

			for ( int intIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ;
					  intIndex < astrProperties.Length ;
					  intIndex++ )
			{
				string [ ] astrLabelAndItsValue = null;

				if ( intIndex == ArrayInfo.ARRAY_FIRST_ELEMENT )
				{   // The first item contains also the property type name, followed by a colon and a space.
					int intPosColon = astrProperties [ intIndex ].IndexOf ( SpecialCharacters.COLON );
					int intPosNext = intPosColon + ArrayInfo.NEXT_INDEX;

					//	------------------------------------------------
					//	As always, use a belt and suspenders, covering 
					//	the chance that the first label immediately
					//	follows the colon that delimits it from the
					//	object type name.
					//
					//	Whether the break occurs at the colon or the
					//	immediately following space, the position is set
					//	such that the last charecter goes into the type
					//	name field, leaving the remainder of the string
					//	to be divided into the propertry value and its
					//	label.
					//	------------------------------------------------

					int intPosBreak = ArrayInfo.OrdinalFromIndex (
						astrProperties [ intIndex ] [ intPosNext ] == SpecialCharacters.SPACE_CHAR
							? intPosNext
							: intPosColon );
					strTypeName = astrProperties [ intIndex ].Substring (
						ListInfo.SUBSTR_BEGINNING ,
						intPosBreak );
					astrLabelAndItsValue = astrProperties [ intIndex ].Substring (
						intPosBreak ).Split ( SpecialCharacters.EQUALS_SIGN );
				}   // TRUE (This iteration processess the first property item.) block, if ( intIndex == ArrayInfo.ARRAY_FIRST_ELEMENT )
				else
				{
					astrLabelAndItsValue = astrProperties [ intIndex ].Split ( SpecialCharacters.EQUALS_SIGN );
				}   // FALSE (This iteration processes a subsequent property item.) block, if ( intIndex == ArrayInfo.ARRAY_FIRST_ELEMENT )

				autpLabelAndValue [ intIndex ].Label = astrLabelAndItsValue [ ArrayInfo.ARRAY_FIRST_ELEMENT ].Trim ( );
				autpLabelAndValue [ intIndex ].Value = astrLabelAndItsValue [ ArrayInfo.ARRAY_SECOND_ELEMENT ].Trim ( );
			}   // for ( int intIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intIndex < astrProperties.Length ; intIndex++ )

			//	--------------------------------------------------------
			//	Compute the length of the longest label string, so that
			//	a format string that renders the labels and their values
			//	in a neatly aligned list can be constructed.
			//	--------------------------------------------------------

			int intLabelFieldWidth = ComputeLabelFieldWidth ( autpLabelAndValue );
			string strPropertyPadding = SpecialStrings.EMPTY_STRING.PadRight ( strTypeName.Length );

            //	--------------------------------------------------------
            //	A string is constructed with two right-padded format
            //	items and a third and fourth default format items.
            //
            //	1)	The first format item gets the type name string,
            //		strTypeName, on the first iteration of a loop over 
            //		the autpLabelAndValue array, while subsequent
            //		iterations use the strPropertyPadding string.
            //
            //	2)	The second format item gets the property name from
            //		the Label member of a LabelAndValue structure stored
            //		in the current element of the autpLabelAndValue
            //		array.
            //
            //	3)	The third format item gets the Value member of the
            //		LabelAndValue structure stored in the current 
            //		element of the autpLabelAndValue array.
            //
            //	4)	The fourth format item gets the platform-specific
            //		newline character or sequence thereof.
            //	--------------------------------------------------------

            string strDynamicFormatString = string.Concat ( new string [ ]
            {
                FormatItem.AdjustToMaximumWidth (
                    ArrayInfo.ARRAY_FIRST_ELEMENT ,
                    strTypeName.Length ,
                    FormatItem.Alignment.Left ,
                    SpecialStrings.EMPTY_STRING ) ,
                SpecialCharacters.SPACE_CHAR.ToString ( ) ,
                FormatItem.AdjustToMaximumWidth (
                    ArrayInfo.ARRAY_SECOND_ELEMENT ,
                    intLabelFieldWidth ,
                    FormatItem.Alignment.Left ,
                    SpecialStrings.EMPTY_STRING ) ,
                THIRD_FORMAT_ITEM
            } );

			//	--------------------------------------------------------
			//	The initial capacity of the StringBuilder is twice the
			//	label width plus the length of the type name, times the
			//	number of label/value pairs in the autpLabelAndValue
			//	array.
			//	--------------------------------------------------------

			rsb = new StringBuilder ( ( intLabelFieldWidth * MagicNumbers.PLUS_TWO + strTypeName.Length ) * autpLabelAndValue.Length );

			for ( int intItemIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ;
					  intItemIndex < autpLabelAndValue.Length ;
					  intItemIndex++ )
			{
				rsb.AppendFormat (
					strDynamicFormatString ,
					new string [ ]
					{
						intItemIndex == ArrayInfo.ARRAY_FIRST_ELEMENT
							? strTypeName
							: strPropertyPadding ,
						autpLabelAndValue [ intItemIndex ].Label ,
						autpLabelAndValue [ intItemIndex ].Value ,
						Environment.NewLine } );

				//	------------------------------------------------------------
				//	Unless pintAdditionalPaddingChars is zero, the first pass
				//	generates a new format control string that allows more
				//	characters to its first format item.
				//	------------------------------------------------------------

				if ( pintAdditionalPaddingChars > ListInfo.EMPTY_STRING_LENGTH && intItemIndex == ArrayInfo.ARRAY_FIRST_ELEMENT )
				{
					strDynamicFormatString = string.Concat ( new string [ ]
					{
						FormatItem.AdjustToMaximumWidth (
							ArrayInfo.ARRAY_FIRST_ELEMENT ,
							strTypeName.Length + pintAdditionalPaddingChars ,
							FormatItem.Alignment.Left ,
							SpecialStrings.EMPTY_STRING ) ,
						SpecialCharacters.SPACE_CHAR.ToString ( ) ,
						FormatItem.AdjustToMaximumWidth (
							ArrayInfo.ARRAY_SECOND_ELEMENT ,
							intLabelFieldWidth ,
							FormatItem.Alignment.Left ,
							SpecialStrings.EMPTY_STRING ) ,
						THIRD_FORMAT_ITEM
					} );
				}   // if ( pintAdditionalPaddingChars > ListInfo.EMPTY_STRING_LENGTH && intItemIndex == ArrayInfo.ARRAY_FIRST_ELEMENT )
			}   // for ( int intItemIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intItemIndex < autpLabelAndValue.Length ; intItemIndex++ )

			return rsb.ToString ( );
		}   // ListPropertiesPerDefaultToString method (4 of 4)


		/// <summary>
		/// Render something visible for ANY string value, including the empty
		/// string and the null reference.
		/// </summary>
		/// <param name="pstrValue">
		/// Pass in a reference to the string to render, which MAY be the empty
		/// string or a null reference.
		/// </param>
		/// <returns>
		/// Return one of the following values.
		/// 
		/// 1) [Null] for a null reference
		/// 
		/// 2) [Empty] for the empty string
		/// 
		/// 3) pstrValue if it is neither empty nor null
		/// </returns>
		public static string RenderStringValue ( string pstrValue )
		{
			if ( string.IsNullOrEmpty ( pstrValue ) )
			{
				if ( pstrValue == null )
				{
					return Properties.Resources.RENDERED_STRING_IS_NULL;
				}   // TRUE (The string is a NULL reference.) block, if ( pstrValue == null )
				else
				{
					return Properties.Resources.RENDERED_STRING_IS_NULL;
				}   // FALSE (The string is the empty string.) block, if ( pstrValue == null )
			}   // TRUE (The input string is either a null reference or the empty string.) block, if ( string.IsNullOrEmpty ( pstrValue ) )
			else
			{
				return pstrValue;
			}   // FALSE (The string is neither null, nor the empty string.) block, if ( string.IsNullOrEmpty ( pstrValue ) )
		}   // public RenderStringValue Method


		private struct LabelAndValue
		{
			public string Label;
			public string Value;
		}   // LabelAndValue


		private static int ComputeLabelFieldWidth ( LabelAndValue [ ] pautpLabelAndValue )
		{
			List<string> lstAllLabels = new List<string> ( pautpLabelAndValue.Length );

			foreach ( LabelAndValue labelAndValue in pautpLabelAndValue )
			{
				lstAllLabels.Add ( labelAndValue.Label );
			}   // foreach ( LabelAndValue labelAndValue in pautpLabelAndValue )

			return ReportHelpers.MaxStringLength ( lstAllLabels );
		}   // private ComputeLabelFieldWidth Method
	}   // internal static class Utl
}   // partial namespace WizardWrx.OperatingParameterManager