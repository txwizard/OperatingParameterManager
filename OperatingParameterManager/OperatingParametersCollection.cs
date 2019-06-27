/*
    ============================================================================

    Namespace:			WizardWrx.OperatingParameterManager

    Class Name:			OperatingParametersCollection

	File Name:			OperatingParametersCollection.cs

    Synopsis:			This abstract base class defines most of the generic
						code and data required to store and manage operating
						parameters as a collection.

    Remarks:			This class is implemented as a Singleton, and uses a
						generic Dictionary as its backing store.

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
using System.Collections.Generic;
using WizardWrx;
using WizardWrx.Core;

namespace WizardWrx.OperatingParameterManager
{
	/// <summary>
	/// This class defines a collection of OperatingParameters objects that are
	/// stored in a generic Dictionary that is keyed by the name of each
	/// parameter.
	/// 
	/// The idea behind the collection is that parameters are loaded into it,
	/// validated, and subsequently called forth as needeed by the program,
	/// providing thereby an easily extensible in-memory store for the program's
	/// operating parameters.
	/// 
	/// Since an application needs at most one collection of operating
	/// parameters, this object is implemented as a Singleton, so that your
	/// program isn't littered with multiple copies of its parameters, nor does
	/// it waste time processing them more than once.
	/// </summary>
	/// <typeparam name="T">
	/// Type T is expected to be an enumeration that corresponds to the rules
	/// for evaluating parameters (e. g., the parameter must be the name of an
	/// existing file, the paraameter must be a valid file name, but the file
	/// must not exist, the parameter must be the name of an existing directory.
	/// 
	/// All ParameterTypeInfo constructors MUST specify the same type of
	/// enumeration, as must the OperatingParameters that go into it. Otherwise,
	/// the behavior of the OperatingParameters collection and its members is
	/// undefined.
	/// </typeparam>
	/// <typeparam name="U">
	/// Type U is expected to be an emumeration that corresponds to the source
	/// from which the parameter's value originated. This is used internally to
	/// identify the source, e. g., ApplicationSettings or CommandLine, for
	/// reporting purposes and for preserving a default value read from one
	/// source, and overridden by a second source.
	/// 
	/// The OperatingParameters objects that go into it MUST specify the
	/// same generic type. Otherwise, the behavior of this OperatingParameters
	/// collection and its members is undefined.
	/// </typeparam>
	public class OperatingParametersCollection<T, U>
		: GenericSingletonBase<OperatingParametersCollection<T, U>>
		where T : Enum
		where U : Enum
	{
		/// <summary>
		/// This private string is the name of an embedded text file resource,
		/// which is expected to be a text file that contains a TAB delimited
		/// label row and a collection of matching TAB delimited detail rows,
		/// each representing the basic properties of an OperatingParameter that
		/// goes into the collection managed by this instance.
		/// </summary>
		private const string PARAMETER_TYPE_INFO_RESOURCE_NAME = @"ParameterTypeInfo.txt";


		/// <summary>
		/// The static constructor calls this method to initialize the singleton
		/// returned by the base constructor.
		/// </summary>
		/// <param name="pstrDisplayNameTemplate">
		/// This paramter specifies a template from which to construct the name
		/// of a managed string resource in the calling assembly to use as the
		/// display name. If no such string exists, the InternalName property is
		/// the display name. Likewise, if this string is a null reference or
		/// the empty string, the InternalName property is the display name.
		/// </param>
		/// <param name="penmDefaultParameterSource">
		/// This parameter specifies the value source enumeration member to use
		/// as the parameter source for any OperatingParameter for which the
		/// ApplicationSettings has a value.
		/// </param>
		private void InitiaalizeInstance (
			System.Configuration.SettingsPropertyCollection psettingsPropertyValueCollection ,
			string pstrDisplayNameTemplate ,
			U penmDefaultParameterSource )
		{
			//	----------------------------------------------------------------
			//	Generic dictionary _dctOperatingParameters, which holds the
			//	collection of OperatingParameter objects, indexed by name, is
			//	initialized and sized per the count of argument names, while the
			//	local dctParameterTypeInfo dictionary is initialized from the
			//	string array constructed from PARAMETER_TYPE_INFO_RESOURCE_NAME,
			//	an embedded text file resource. Iterating the argument names in
			//	pastrArgNames, the foreach loop extracts its internally-defined
			//	properties from dctParameterTypeInfo, the local dictionary, so
			//	that an OperatingParameter object, with its Value property left
			//	uninitialized, can be stored in the _dctOperatingParameters
			//	dictionary, so that it can be quickly found and updated with the
			//	value read from a command line argument, which happens in the
			//	calling routine after this method returns the fully loaded
			//	_dctOperatingParameters dictionary.
			//
			//	Since everything that went into dctParameterTypeInfo is encoded
			//	into an OperatingParameter, local object dctParameterTypeInfo is
			//	redundant, and is allowed to vanish when the method returns.
			//	----------------------------------------------------------------

			Dictionary<string , ParameterTypeInfo<T>> dctParameterTypeInfo = GetParameterTypeInfo (
				EmbeddedTextFile.Readers.LoadTextFileFromEntryAssembly (
					PARAMETER_TYPE_INFO_RESOURCE_NAME ) );
			_dctOperatingParameters = new Dictionary<string , OperatingParameter<T , U>> ( dctParameterTypeInfo.Count );

			foreach ( string strName in dctParameterTypeInfo.Keys )
			{
				ParameterTypeInfo<T> ptiForThis = null;

				if ( dctParameterTypeInfo.TryGetValue ( strName , out ptiForThis ) )
				{   // Create the uninitialized parameter object, and add it to the collection.
					OperatingParameter<T , U> opThis = new OperatingParameter<T , U> (
						psettingsPropertyValueCollection ,
						strName ,                                                       // string pstrInternalName
						pstrDisplayNameTemplate ,                                       // string pstrDisplayName - The constructor will sort it out.
						ptiForThis.ParameterType ,                                      // T penmParameterType
						penmDefaultParameterSource );									// U penmDefaultParameterSource
					_dctOperatingParameters.Add (										// Add it to the collection.
						strName ,														// Key
						opThis );														// Value
				}   // TRUE (anticipated outcome) block, if ( dctParameterTypeInfo.TryGetValue ( strName , out ptiForThis ) )
				else
				{	// Prepare and log a detailed exception report.
					string strMessage = string.Format (									// Assemble a detailed diagnostic message.
						Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_005 ,		// Format control string
						strName ,														// Format Item 0: Type information for the {0} parameter
						PARAMETER_TYPE_INFO_RESOURCE_NAME ,								// Format Item 1: cannot be found in the embedded {1} resource
						Environment.NewLine );                                          // Format Item 2: Platform-dependent newline sequence
					throw new InvalidOperationException ( strMessage );					// Toss cookies and die.
				}   // FALSE (unanticipated outcome) block, if ( dctParameterTypeInfo.TryGetValue ( strName , out ptiForThis ) )
			}   // foreach ( string strName in dctParameterTypeInfo.Keys )
		}   // InitiaalizeInstance Method


		private OperatingParametersCollection ( )
		{   // Singletons have exactly one of everything, including its constructor, which must be private.
			InitializeOnFirstUse (
				s_srCriticalSection ,   // This static is initialized inline.
				this );                 // Since the call has been pushed forward, the static instance handle has yet to be initialized.
		}   // OperatingParameters constructor


		/// <summary>
		/// Access to the single instance is gated through this method, which is
		/// called as needed to get references, so that the parameters need not
		/// be passed around or stored in a global variable.
		/// </summary>
		/// <param name="pstrDisplayNameTemplate">
		/// This paramter specifies a template from which to construct the name
		/// of a managed string resource in the calling assembly to use as the
		/// display name. If no such string exists, the InternalName property is
		/// the display name. Likewise, if this string is a null reference or
		/// the empty string, the InternalName property is the display name.
		/// </param>
		/// <param name="penmDefaultParameterSource">
		/// Specify the member of the ParameterSource, generic type U,
		/// enumeration with which to mark the object if the ApplicationSettings
		/// collection includes a default value.
		/// 
		/// Please <see cref="OperatingParametersCollection{T, U}"/> for more
		/// details.
		/// </param>
		/// <returns>
		/// Unless something causes it to fail, the return value is reference to
		/// a fully initialized object, ready to be loaded with parameters.
		/// </returns>
		public static OperatingParametersCollection<T , U> GetTheSingleInstance (
			System.Configuration.SettingsPropertyCollection psettingsPropertyValueCollection ,
			string pstrDisplayNameTemplate ,
			U penmDefaultParameterSource )
		{
			OperatingParametersCollection<T , U> rtheOperatingParametersCollection = s_genTheOnlyInstance;
			rtheOperatingParametersCollection.InitiaalizeInstance (
				psettingsPropertyValueCollection ,
				pstrDisplayNameTemplate ,
				penmDefaultParameterSource );
			return rtheOperatingParametersCollection;
		}   // GetTheSingleInstance Method


		private void InitializeOnFirstUse (
			SyncRoot ps_srCriticalSection ,
			OperatingParametersCollection<T, U> operatingParametersCollection )
		{
			lock ( ps_srCriticalSection )
			{
			}   // lock ( ps_srCriticalSection )
		}   // private void InitializeOnFirstUse


		/// <summary>
		/// This method creates a generic dictionary of ParameterTypeInfo
		/// objects, keyed by parameter name from an array of strings, each of which
		/// is the internal name of a parameter.
		/// </summary>
		/// <param name="pastrParameterTypeInfoArray">
		/// This array of strings is a list of parameter names and property
		/// values, such as generic ParameterType enumeration values, that is
		/// read from a TAB delimited text file that is stored in an embeddede
		/// assembly resource.
		/// 
		/// The first string is expected to be a label row that maps the fields
		/// in the remaining strings to their values.
		/// </param>
		/// <returns>
		/// If it succeeds, the return value is a populated dictionary of 
		/// ParameterTypeInfo objects keyed by their internal parameter names.
		/// Invalid data or a corrupted parameter resource file causes the
		/// ParameterTypeInfo constructor to throw an InvalidOperationException.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// A detailed InvalidOperationException exception report arises if the
		/// array of parameters is invalid.
		/// 
		/// Correct code should never throw this exception.
		/// </exception>
		private Dictionary<string , ParameterTypeInfo<T>> GetParameterTypeInfo ( string [ ] pastrParameterTypeInfoArray )
		{
			Dictionary<string , ParameterTypeInfo<T>> rdctParameterTypeInfo = new Dictionary<string , ParameterTypeInfo<T>> ( ArrayInfo.IndexFromOrdinal ( pastrParameterTypeInfoArray.Length ) );

			for ( int intPosition = ArrayInfo.ARRAY_SECOND_ELEMENT ;
					  intPosition < pastrParameterTypeInfoArray.Length ;
					  intPosition++ )
			{
				ParameterTypeInfo<T> pti = new ParameterTypeInfo<T> (
					pastrParameterTypeInfoArray [ intPosition ] ,                       // string pstrParameterInfoDetail
					pastrParameterTypeInfoArray [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );    // string pastrParameterInfoColumnNames
				rdctParameterTypeInfo.Add (
					pti.ParameterName ,                                                 // Key
					pti );                                                              // Value
			}   // for ( int intPosition = ArrayInfo.ARRAY_SECOND_ELEMENT ; intPosition < pastrParameterTypeInfoArray.Length ; intPosition++ )

			return rdctParameterTypeInfo;
		}   // GetParameterTypeInfo


		/// <summary>
		/// Call this method to get a count of OperatingParameter objects in the
		/// collection.
		/// </summary>
		/// <returns>
		/// For a fully initialized instance, the return value is the count of
		/// OperatingParameter objects in the collection.
		/// </returns>
		public int GetCount ( )
		{
			return _dctOperatingParameters != null ? _dctOperatingParameters.Count : ArrayInfo.ARRAY_IS_EMPTY;
		}   // GetCount Method


		/// <summary>
		/// Call this method to return a reference to a named OperatingParameter
		/// object.
		/// </summary>
		/// <param name="pstrParameterName">
		/// Specify the internal name of the parameter for which the
		/// OperatingParameter object is required.
		/// </param>
		/// <returns>
		/// If it succeeds, the return value is the initialized OperatingParameter
		/// object that repressents its value and status (validated or not).
		/// Otherwise, an ArgumentException arises.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// An ArgumentException arises when pstrParameterName is an invalid
		/// parameter name.
		/// 
		/// Correct code should never throw this exception.
		/// </exception>
		public OperatingParameter<T, U> GetParameterByName ( string pstrParameterName )
		{
			OperatingParameter<T, U> roperatingParameter;

			if ( _dctOperatingParameters.TryGetValue ( pstrParameterName , out roperatingParameter ) )
			{
				return roperatingParameter;
			}   // TRUE (The specified parameter name is valid.) block, if ( _dctOperatingParameters.TryGetValue ( pstrParameterName , out roperatingParameter ) )
			else
			{
				string strMessage = string.Format (
					Properties.Resources.ERRMSG_UNDEFINED_ARGNAME ,
					pstrParameterName );
				throw new ArgumentException ( strMessage );
			}   // FALSE (The specified parameter name is INvalid.) block, if ( _dctOperatingParameters.TryGetValue ( pstrParameterName , out roperatingParameter ) )
		}   // GetParameterByName


		/// <summary>
		/// Call this method to get a list of the valid parameter names. This is
		/// useful for pupulating the list of valid parameters in a command line
		/// argument parser, such as, for example, a CmdLneArgsBasic instance.
		/// </summary>
		/// <returns>
		/// If it succeeds, the return value is an array of string, each of
		/// which is the internal name of a parameter. Otherwise, the return
		/// value is a null reference, and the calling routine should abort.
		/// </returns>
		public string [ ] GetParameterNames ( )
		{
			if ( _dctOperatingParameters != null )
			{
				string [ ] rastrKeys = new string [ _dctOperatingParameters.Keys.Count ];
				_dctOperatingParameters.Keys.CopyTo ( rastrKeys , ArrayInfo.ARRAY_FIRST_ELEMENT );
				return rastrKeys;
			}   // TRUE (anticipated outcome) block, if ( _dctOperatingParameters != null )
			else
			{	// This should almost certainly throw an exception, and I may eventually make it so.
				return null;
			}   // FALSE (unanticipated outcome) block, if ( _dctOperatingParameters != null )
		}   // GetParameterNames Method


		/// <summary>
		/// Set the parameter values from an initialized CmdLneArgsBasic object,
		/// overriding any that were initialized from the Application Settings.
		/// </summary>
		/// <param name="pcmdArgs">
		/// Pass in a reference to a CmdLneArgsBasic object that was initialized
		/// from the string array returned by the GetParameterNames method.
		/// </param>
		/// <param name="penmParameterSource">
		/// Specify the member of the ParameterSource, generic type U,
		/// enumeration with which to mark the object if the CmdLneArgsBasic
		/// object includes a value.
		/// 
		/// Please <see cref="OperatingParametersCollection{T, U}"/> for more
		/// details.
		/// </param>
		public void SetFromCommandLineArguments ( CmdLneArgsBasic pcmdArgs , U penmParameterSource )
		{
			IEnumerator<KeyValuePair<string, OperatingParameter<T , U>>> listOfValidArguments = _dctOperatingParameters
				.GetEnumerator ( );

			while ( listOfValidArguments.MoveNext ( ) )
			{
				string strName = listOfValidArguments.Current.Key;
				string strValue = pcmdArgs.GetArgByName ( strName );

				if ( !string.IsNullOrEmpty ( strValue ) )
				{
					OperatingParameter<T , U> opThis = listOfValidArguments.Current.Value;
					opThis.SetValue (
						strValue ,                                              // string pstrValue
						penmParameterSource );									// ParameterSource penmSource
				}	// if ( !string.IsNullOrEmpty ( strValue ) )
			}   // while ( listOfValidArguments.MoveNext ( ) )
		}   // SetFromCommandLineArguments Method


		/// <summary>
		/// The private static SyncRoot object is used to synncrhonize access to
		/// the static constructor, to help guarantee that exactly one, and only
		/// one, instance is created.
		/// </summary>
		private static SyncRoot s_srCriticalSection = new SyncRoot ( typeof ( OperatingParametersCollection<T, U> ).ToString ( ) );


		/// <summary>
		/// The dictionary of OperatingParameters is marked as private, to
		/// restrict access to the public GetParameterByName method. Since the
		/// OperatingParameter objects expose only read-only properties, the
		/// parameters cannot be accidentally changed by any known means.
		/// </summary>
		private Dictionary<string , OperatingParameter<T, U>> _dctOperatingParameters;
	}   // class OperatingParameters
}   // partial namespace WizardWrx.OperatingParameterManager