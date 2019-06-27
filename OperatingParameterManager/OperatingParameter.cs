/*
    ============================================================================

    Namespace:			WizardWrx.OperatingParameterManager

    Class Name:			OperatingParameter

	File Name:			OperatingParameter.cs

    Synopsis:			Store and manage an operating parameter and properties,
						such as the type of data stored therein and its expected
						use, along with a validity flag.

	Remarks:			Method IsValueValid is marked as an override because it
						implements the like named abstract method in an abstract
						base class, effectively making it a required interface
						(meaning that derived concrete classes must implement
						it). Since it evaluates a generic protected base class
						member, it must coerce it to its concrete type,
						ParameterType, by calling Convert.ChangeType. An
						ordinary cast is insufficient. See the Stack Overflow
						discussion cited in the References section for details.

						The public constructor is implemented by the protected
						constructor with the same signature in the abstract base
						class. This class adds nothing to it.

						Everything else in this class is supplied by the base
						class, including its private members.

						The ParameterType and ParameterSource enumerations that
						replace generics T and U, respectively, in instantiated
						classes, are defined at namespace scope, to simplify the
						definition of IsValueValid.

	References:			Cannot implicitly convert type 'Int' to 'T'
						https://stackoverflow.com/questions/8171412/cannot-implicitly-convert-type-int-to-t

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
	2018/09/02 1.0     DAG    Initial implementation created, tested, and 
                              deployed.
    ============================================================================
*/

using System;


namespace WizardWrx.OperatingParameterManager
{
	/// <summary>
	///	Strictly speaking, ParameterSource could be defined anywhere. Defining
	///	it at namespace scope in this class simplifies type resolution, while it
	///	clarifies that the type is not part of the generic class definition.
	/// </summary>
	public enum ParameterSource
	{
		/// <summary>
		/// An ninitialized ParameterSource instance has this value.
		/// </summary>
		Undefined,

		/// <summary>
		/// The parameter value was obtained from an Application Settings key.
		/// </summary>
		ApplicationSettings,

		/// <summary>
		/// The parameter value was specified by a command line argument.
		/// </summary>
		CommandLine
	}   // ParameterSource enumeration


	/// <summary>
	/// Unlike the ParameterSource enumeration, the ParameterType enumeration is
	/// more tightly coupled to this class due to its use in the definition of
	/// the IsValueValid method.
	/// </summary>
	public enum ParameterType
	{
		/// <summary>
		/// An uninitialized ParameterType instance has this as its value.
		/// </summary>
		Undefined,

		/// <summary>
		/// The specified parameter value must be the relative or absolute name
		/// of a directory that exists in the accessible file system.
		/// </summary>
		ExistingDirectory,

		/// <summary>
		/// The specified parameter value must be the relative or absolute name
		/// of a file that exists in the accessible file system.
		/// </summary>
		ExistingFile,

		/// <summary>
		/// The specified parameter value must be a relative or absolute name
		/// that can be assigned to a new file in the accessible file system.
		/// </summary>
		NewFile
	}   // ParameterType Enumerations


	/// <summary>
	/// This class is a concrete instance that implements abstract method 
	/// IsValueValid for generic type T = ParameterType.
	/// </summary>
	/// <typeparam name="T">
	/// Type T is expected to be an enumeration that corresponds to the 
	/// rules for evaluating parameters (e. g., the parameter must be the
	/// name of an existing file, the paraameter must be a valid file name,
	/// but the file must not exist, the parameter must be the name of an
	/// existing directory.
	/// 
	/// All ParameterTypeInfo constructors MUST specify the same type of
	/// enumeration, as must the OperatingParameters that go into the
	/// OperatingParametersCollection. Otherwise, the behavior of the
	/// OperatingParameters collection and its members is undefined.
	/// </typeparam>
	/// <typeparam name="U">
	/// Type U is expected to be an emumeration that corresponds to the source
	/// from which the parameter's value originated. This is used internally to
	/// identify the source, e. g., ApplicationSettings or CommandLine, for
	/// reporting purposes and for preserving a default value read from one
	/// source, and overridden by a second source.
	/// 
	/// The OperatingParameters objects that go into the
	/// OperatingParametersCollection singleton MUST specify the same generic
	/// type. Otherwise, the behavior of this OperatingParameters collection and
	/// its members is undefined.
	/// </typeparam>
	public class OperatingParameter<T, U>
		: OperatingParameterBase<T, U>
		where T : Enum
		where U : Enum
	{
		/// <summary>
		/// The sole public constructor accepts the parameters required to fully
		/// initialize the object.
		/// </summary>
		/// <param name="pstrInternalName">
		/// The InternalName is a string that is used to identify the parameter.
		/// The OperatingParametersCollection enforces unique values.
		/// </param>
		/// <param name="pstrDisplayName">
		/// Display name is technically optional, since it defaults to the
		/// internal name if this parameter is a null reference or the empty
		/// string.
		/// </param>
		/// <param name="penmParameterType">
		/// The parameter type must be a valid member of the enumeration mapped
		/// to the T generic type placeholder.
		/// </param>
		/// <param name="penmDefaultParameterSource">
		/// The parameter type must be a valid member of the enumeration mapped
		/// to the U generic type placeholder.
		/// </param>
		public OperatingParameter (
			System.Configuration.SettingsPropertyCollection psettingsPropertyValueCollection ,
			string pstrInternalName ,
			string pstrDisplayName ,
			T penmParameterType ,
			U penmDefaultParameterSource )
			: base (
				  psettingsPropertyValueCollection ,
				  pstrInternalName ,
				  pstrDisplayName ,
				  penmParameterType ,
				  penmDefaultParameterSource )
		{
		}   // public OperatingParameterExample constructor


		/// <summary>
		/// This method implements the validation criteria that are encoded into
		/// the generic parameter type enumeration (generic type T) for concrete
		/// enumerated type ParameterType.
		/// </summary>
		/// <typeparam name="T">
		/// For this implementation, generic type T must resolve to a specific
		/// ParameterType enumeration.
		/// 
		/// Type T is expected to be an enumeration that corresponds to the 
		/// rules for evaluating parameters (e. g., the parameter must be the
		/// name of an existing file, the paraameter must be a valid file name,
		/// but the file must not exist, the parameter must be the name of an
		/// existing directory.
		/// </typeparam>
		/// <returns>
		/// If the ParanValue is initialized and meets the criteria associated
		/// with the generic paramter type enumeration assoicated with the
		/// concrete instance, this method returns TRUE, and sets the ParamState
		/// to Validated. Otherwise, it returns FALSE, and leaves the ParamState
		/// unchanged.
		/// </returns>
		#pragma warning disable CS0693
		public override bool IsValueValid<T> ( )
		#pragma warning restore CS0693
		{
			bool rIsValid = false;

			switch ( Convert.ChangeType ( _enmParameterType , typeof ( ParameterType ) ) )
			{
				case ParameterType.ExistingDirectory:
					rIsValid = System.IO.Directory.Exists ( _strValue );
					break;
				case ParameterType.ExistingFile:
					rIsValid = System.IO.File.Exists ( _strValue );
					break;
				case ParameterType.NewFile:
					rIsValid = !System.IO.File.Exists ( _strValue );
					break;
				default:
					string strMessage = string.Format (
						Properties.Resources.ERRMSG_INVALID_PARAMETER_TYPE ,
						_enmParameterType.GetType ( ).Name ,
						( int ) Convert.ChangeType ( _enmParameterType , typeof ( ParameterType ) ) );
					throw new InvalidOperationException ( strMessage );
			}   // switch ( Convert.ChangeType ( _enmParameterType , typeof ( ParameterType ) ) )

			if ( rIsValid )
			{
				_enmState = ParameterState.Validated;
			}   // if ( rIsValid )

			return rIsValid;
		}   // public override bool IsValueValid
	}   // public class OperatingParameter
}   // partial namespace WizardWrx.OperatingParameterManager