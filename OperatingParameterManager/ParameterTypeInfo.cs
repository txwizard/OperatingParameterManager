/*
    ============================================================================

    Namespace:			WizardWrx.OperatingParameterManager

    Class Name:			ParameterTypeInfo

	File Name:			ParameterTypeInfo.cs

    Synopsis:			Store and manage operating parameter type information
						loaded from an embedded text resource file.

    Remarks:			Several critical features of the constructor design may
						not be immediately obvious.

						1)	Instances of this class are constructed from lines
							of text that are read from an embedded text file
							resource. The file is organized as a standard TAB
							delimited table of data, the first row of which is
							the column labels.

						2)	Static string array s_astrParameterNames stores the
							parsed label row of the enbedded text file resource,
							which is parsed from pastrParameterInfoColumnNames,
							its second argument, and stored for subsequent use
							when the first instance in a process is constructed.
							Subsequent constructions avoid the parsing cost by
							using the stored labels.

							Hence, subsequent constructions ignore the second
							argument. Since the Microsoft .NET Framework uses
							the __fastcall calling convention, the only waste is
							the effort required to push the redundant second
							argument into a CPU register.

						3)	All constructors use the column names in the label
							row to make decisions about what to do with the data
							in each field (column) of the detail record to which
							pstrParameterInfoDetail, the first constructor
							argument, points.

						4)	InternalName, a string, is taken at face value and
							stored.

						5)	ParamType is an enumerated type; an EnumConverter is
							constructed around the type of the _enmParameterType
							member, and is used to validate and convert the text
							into the correct enumeration value.

							A future version of this class may replace the hard
							coded enumeration with a generic, if the generic can
							be appropriately constrained.

						6)	Making the call to the converter a one-statement try
							block permits the exception that is finally caught
							and logged to include contextual detail that would
							otherwise be lost.

    Author:				David A. Gray

	License:            Copyright (C) 2018, David A. Gray.
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
	2018/09/01 1.0     DAG    Initial implementation created, tested, and 
                              deployed.
    ============================================================================
*/

using System;
using System.ComponentModel;
using WizardWrx;
using WizardWrx.AnyCSV;

namespace WizardWrx.OperatingParameterManager
{
	/// <summary>
	/// This class maps the columns of a table of parameter type properties and
	/// their values to the properties of an instance of itself. The properties
	/// of a set of parameters are expected to be read into instances of this
	/// class, one instance per row, from an input file, with the objective of
	/// making addition of parameters easier by storing their basic attributes
	/// in a file, which may exist either as a text file in the file system or
	/// an embedded resource in the application assembly.
	/// </summary>
	/// <typeparam name="T">
	/// Type T is expected to be an enumeration that corresponds to the rules
	/// for evaluating parameters (e. g., the parameter must be the name of an
	/// existing file, the paraameter must be a valid file name, but the file
	/// must not exist, the parameter must be the name of an existing directory.
	/// 
	/// All constructors MUST specify the same type of enumeration, as must
	/// the OperatingParametersCollection class and the OperatingParameters
	/// that go into it. Otherwise, the behavior of the OperatingParameters
	/// collection and its members is undefined.
	/// </typeparam>
	public class ParameterTypeInfo<T> where T : Enum
	{
		#region Instance Constructors
		/// <summary>
		/// The default constructor is marked as private to force consumers to
		/// create fully initialized instances.
		/// </summary>
		private ParameterTypeInfo ( ) { }


		/// <summary>
		/// The public constructor requires the calling routine to supply the
		/// values of a label row and the corresponding details in the form of a
		/// pair of well-formed TAB delimited strings.
		/// </summary>
		/// <param name="pstrParameterInfoDetail">
		/// Supply a TAB delimited string that contains a field (column) for
		/// each property named in the pastrParameterInfoColumnNames string.
		/// </param>
		/// <param name="pastrParameterInfoColumnNames">
		/// Specify a TAB delimited string that names the columns (properties)
		/// of the parameter that are specified in the pstrParameterInfoDetail
		/// string that accompanies it.
		/// 
		/// This parameter is processed once only, the first time an application
		/// calls the constructor. The string is parsed into a static array of
		/// strings that is visible to all instances in the application. Though
		/// subsequent instances may supply the same or different values, even a
		/// null reference or the empty string, subsequent constructions ignore
		/// this parameter, since the object knows that it is already populated.
		/// 
		/// Currently, only two properties are supported.
		/// 
		/// 1) InternalName is the name by which the parameter is identified by
		/// the code; it is expected to correspond to the name of a command line
		/// argument, application setting, or both. Instances do not verify that
		/// InternalName values are unique; that is the responsbility of the
		/// calling routine.
		/// 
		/// 2) ParamType is expected to correspond to a valid value of generic
		/// type T. Values are parsed, and invalid values cause an exception to
		/// be thrown, since such an error is the result of a misconfigured or
		/// corrupted application installation.
		/// 
		/// InternalName and ParamType may be specified in any order, so long as
		/// the order is consistent across all instances.
		/// </param>
		public ParameterTypeInfo (
			string pstrParameterInfoDetail ,
			string pastrParameterInfoColumnNames )
		{
			if ( s_astrParameterNames == null )
			{   // Perform this expensive operation once only.
				s_astrParameterNames = s_parser.Parse ( pastrParameterInfoColumnNames );
			}   // if ( s_astrParameterNames == null )

			string [ ] astrParameterTypeInfo = s_parser.Parse ( pstrParameterInfoDetail );

			if ( astrParameterTypeInfo.Length == s_astrParameterNames.Length )
			{
				for ( int intPosition = ArrayInfo.ARRAY_FIRST_ELEMENT ;
					      intPosition < astrParameterTypeInfo.Length ;
						  intPosition++ )
				{
					switch ( s_astrParameterNames [ intPosition ] )
					{
						case @"InternalName":
							_strParameterName = astrParameterTypeInfo [ intPosition ];
							break;
						case @"ParamType":
							EnumConverter enumConverter = new EnumConverter ( _enmParameterType.GetType ( ) );

							//	------------------------------------------------
							//	Private member _enmParameterType corresponds to
							//	the public ParameterType property, which is an
							//	enumeration of generic type T, which must be a
							//	valid Microsoft .NET Enum type. The ConvertFrom
							//	method either parses a test string into a valid
							//	Enum value of the type that was specified to the
							//	constructor, or throws a FormatException 
							//	exception, which is caught and wrapped in a new
							//	InvalidOperationException exception, which is
							//	thrown to the calling routine.
							//	------------------------------------------------

							try
							{   // Catching the FormatException exception here permits augmenting the message with contextual details that would otherwise be lost.
								_enmParameterType = ( T ) enumConverter.ConvertFrom ( astrParameterTypeInfo [ intPosition ] );
							}
							catch ( FormatException formatException )
							{   // The catch block bounds the scope of this strMessage.
								string strMessage = string.Format (									// Prepare a detailed diagnostic message.
									Properties.Resources.MESSAGE_INVALID_PARAMETER_TYPE_VALUE ,     // Format Control String
									formatException.Message ,                                       // Format Item 0: {0}{4}
									s_astrParameterNames [ intPosition ] ,                          // Format Item 1: Column Name  = {1}{4}
									intPosition ,                                                   // Format Item 2: Column Index = {2}{4}
									pstrParameterInfoDetail ,                                       // Format Item 3: Whole Record = {3}{4}
									Environment.NewLine );                                          // Format Item 4: Platform-dependent newline.
								throw new InvalidOperationException (
									strMessage ,
									formatException );
							}   // catch ( FormatException formatException )
							break;  // case @"ParamType":
						default:
							{   // Bound the scope of this strMessage.
								string strMessage = string.Format (
									Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_004 ,
									s_astrParameterNames [ intPosition ] ,
									Environment.NewLine );
								throw new InvalidOperationException ( strMessage );
							}   // Make this strMessage disappear.
					}   // switch ( s_astrParameterNames [ intPosition ] )
				}   // for ( int intPosition = ArrayInfo.ARRAY_FIRST_ELEMENT ; intPosition < astrParameterTypeInfo.Length ; intPosition++ )
			}   // TRUE (anticipated outcome) block, if ( astrParameterTypeInfo.Length == s_astrParameterNames.Length )
			else
			{   // Toss cookies and fall over dead. The ELSE block bounds the scope of this strMessage.
				string strMessage = string.Format ( 
					Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_002 ,	// Format Control String
					s_astrParameterNames.Length ,                               // Format Item 0: Expected field count = {0}
					astrParameterTypeInfo.Length ,                              // Format Item 1: Actual field count   = {1}
					pstrParameterInfoDetail ,                                   // Format Item 2: Actual detail record = {2}
					Environment.NewLine );                                      // Format Item 3: Platform-dependent newline sequence
				throw new InvalidOperationException ( strMessage );
			}   // FALSE (unanticipated outcome) block, if ( astrParameterTypeInfo.Length == s_astrParameterNames.Length )
		}   // ParameterTypeInfo constructor for first or subsequent detail item
		#endregion // Instance Constructors


		#region Public Property Getter Methods
		/// <summary>
		/// The read-only ParameterName property returns the parameter name that
		/// was passed into the constructor.
		/// </summary>
		public string ParameterName { get { return _strParameterName; } }

		/// <summary>
		/// The read-only ParameterType property returns the enumerated
		/// parameter type value that was parsed from the string that was passed
		/// into the constructor.
		/// </summary>
		public T ParameterType { get { return _enmParameterType; } }
		#endregion  // Public Property Getter Methods


		#region Private Storage for Instance Properties
		private string _strParameterName;
		private T _enmParameterType;
		#endregion  // Private Storage for Instance Properties


		#region Private Static Storage Shared by All Instances
		/// <summary>
		/// The s_astrParameterNames array is initialized by the first call to a
		/// constructor by parsing its pastrParameterInfoColumnNames parameter
		/// into an array of strings by calling the Parse method on the
		/// WizardWrx.AnyCSV.Parser object stored in the read-only s_parser
		/// object, which defines a parser that splits delimited strings at each
		/// TAB character, unles the TAB is inside a pair of double quotation
		/// marks (the Guard Character). Double quotation marks, if present, are
		/// discarded.
		/// </summary>
		private static string [ ] s_astrParameterNames;

		/// <summary>
		/// The s_parser member defines a parser that splits delimited strings
		/// at each TAB character, unles the TAB is inside a pair of double
		/// quotation marks (the Guard Character). Double quotation marks, if
		/// present, are discarded.
		/// </summary>
		private static readonly Parser s_parser = new WizardWrx.AnyCSV.Parser (
			CSVParseEngine.DelimiterChar.Tab ,
			CSVParseEngine.GuardChar.DoubleQuote ,
			CSVParseEngine.GuardDisposition.Strip );
		#endregion  // Private Static Storage Shared by All Instances
	}   // class ParameterTypeInfo
}   // partial namespace WizardWrx.OperatingParameterManager