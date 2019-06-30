/*
    ============================================================================

    Namespace:			WizardWrx.OperatingParameterManager

    Class Name:			OperatingParameterBase

	File Name:			OperatingParameterBase.cs

    Synopsis:			Store and manage an operating parameter and properties,
						such as the type of data stored therein and its expected
						use, along with a validity flag.

	Remarks:			This class must be explicitly marked as abstract because
						IsValueValid must be so marked, since its implementation
						must be deferred until the type of generic type T is
						known.

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
	2018/08/31 1.0     DAG    Initial implementation created, tested, and 
                              deployed.

    2019/06/29 1.0.3   DAG    Add missing XML documentation in preparation for
                              publication in a documented GitHub repository and
                              as a NuGet package.
    ============================================================================
*/

using System;
using System.Configuration;


namespace WizardWrx.OperatingParameterManager
{
	/// <summary>
	/// This abstract class defines a generic OperatingParameter object. Though
	/// generic classes are implicitly abstract, they are seldom so marked.
	/// 
	/// However, since its IsValueValid method is generic, and its 
	/// implementation is so tightly coupled to the properties of the resolved
	/// generic type that its implementation must be left to the concrete
	/// derived class.
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
	public abstract class OperatingParameterBase<T, U>
		: IComparable<OperatingParameterBase<T , U>>
		where T : Enum
		where U : Enum
	{
		#region Public Enumerations and Constants
		/// <summary>
		/// The values of this enumeration specify the state of the Value
		/// property.
		/// 
		/// ParameterState is the only one of three enumerated types that can be
		/// completely specified at design time.
		/// </summary>
		public enum ParameterState
		{
			/// <summary>
			/// The ParamValue property is uninitialized, and returns NULL.
			/// </summary>
			Uninitialized,

			/// <summary>
			/// The ParamValue property is initialized, but its value has yet to be
			/// validated.
			/// </summary>
			Initialized,

			/// <summary>
			/// The initialized ParamValue property is valid.
			/// </summary>
			Validated
		}   // State enumeration


		/// <summary>
		/// A DisplayName value that contains a valid zeroth format item is a
		/// template from which to construct the name of a resource string that
		/// is the actual display name.
		/// </summary>
		public const string DISPLAY_NAME_SUBSTITUTION_TOKEN = @"{0}";
		#endregion // Public Enumerations and Constants


		#region Constructors
		/// <summary>
		/// The default constructor is kept private to force callers to fully
		/// specify the properties required to fully initialize the object.
		/// </summary>
		private OperatingParameterBase ( )
		{ } // private constructor


        /// <summary>
        /// The sole public constructor accepts the parameters required to fully
        /// initialize the object.
        /// </summary>
        /// <param name="psettingsPropertyValueCollection">
        /// Pass a reference to a System.Configuration.SettingsPropertyCollection
        /// collection from which to obtain default values.
        /// </param>
        /// <param name="pstrInternalName">
        /// The InternalName is a string that is used to identify the parameter.
        /// The OperatingParametersCollection enforces unique values.
        /// </param>
        /// <param name="pstrDisplayName">
        /// Display name is technically optional, since it defaults to the
        /// internal name if this parameter is a null reference or the empty
        /// string.
        /// 
        /// If the parameter contains substring DISPLAY_NAME_SUBSTITUTION_TOKEN,
        /// treat it as the format string from which to construct the name of a
        /// string resource in the calling assembly that is the actual display
        /// name. The format string is constructed by calling string.Format to
        /// substitute the value of the InternalName property for substring
        /// DISPLAY_NAME_SUBSTITUTION_TOKEN, which is a valid format item token.
        /// 
        /// If the calling assembly contains no like named string, fall back to
        /// InternalName, the default value.
        /// </param>
        /// <param name="penmParameterType">
        /// The parameter type must be a valid member of the enumeration mapped
        /// to the T generic type placeholder.
        /// </param>
        /// <param name="penmDefaultParameterSource">
        /// The parameter type must be a valid member of the enumeration mapped
        /// to the U generic type placeholder.
        /// </param>
        public OperatingParameterBase (
			SettingsPropertyCollection psettingsPropertyValueCollection ,
			string pstrInternalName ,
			string pstrDisplayName ,
			T penmParameterType ,
			U penmDefaultParameterSource )
		{
			if ( string.IsNullOrEmpty ( pstrInternalName ) )
				throw new ArgumentNullException ( @"pstrInternalName" );

			_strInternalName = pstrInternalName;
			_strDisplayName = string.IsNullOrEmpty ( pstrDisplayName ) 
				? pstrInternalName
				: pstrDisplayName;

			lock ( s_syncRoot )
			{	// Make it thread-safe.
				if ( s_settingsForEntryAssembly == null )
				{	// Set it once only.
					if ( psettingsPropertyValueCollection != null )
					{   // Do nothing if the input parameter is null.
						s_settingsForEntryAssembly = AppSettingsForEntryAssembly.GetTheSingleInstance ( psettingsPropertyValueCollection );
					}   // TRUE (anticipated outcome) block, if ( psettingsPropertyValueCollection != null )
					else
					{
						throw new ArgumentNullException ( );
					}   // FALSE (unanticipated outcome) block, if ( psettingsPropertyValueCollection != null )
				}   // if ( s_settingsPropertyValueCollection == null )
			}   // lock ( s_syncRoot )

			ParseDisplayName ( );
			_enmParameterType = penmParameterType;

			if ( CheckForDefaultValueInAppSettings ( _strInternalName , ref _strValue ) )
			{   // Store the default value, flip the associated switch, identify the source, and flag the parameter as initialized.
				_fHasDefaultValueInAppSettings = true;
				_enmParameterSource = penmDefaultParameterSource;
				_enmState = ParameterState.Initialized;
			}   // TRUE (The application cofiguration defines a default value.) block, if ( CheckForDefaultValueInAppSettings ( _stInternalName , ref _strValue ) )
		}   // Public OperatingParameterBase constructor
		#endregion // Constructors


		#region Property Getters
		/// <summary>
		/// The constructor sets this Boolean property to TRUE when the proparty
		/// has a default value stored in an application configuration settings
		/// file. If so, the parameter's value is initialized to the value
		/// stored in the setting that has the same name as that of the
		/// InternalName property, the ParamState property changes from
		/// Uninitialized to Initialized, and the ParamSource changes from
		/// Undefined to ApplicationSettings.
		/// 
		/// Otherwise, this property remains FALSE (its default value), the
		/// ParamState stays Uninitialized, ParamSource stays Undefined, and the
		/// ParamValue stays NULL.
		/// </summary>
		public bool HasDefaultValueInAppSettings { get { return _fHasDefaultValueInAppSettings; } }


		/// <summary>
		/// This optional string value takes on the InternalName if its input to
		/// the constructor is a null reference or the empty string. Regardless,
		/// its intended use is as a label for printed reports.
		/// </summary>
		public string DisplayName {  get { return _strDisplayName; } }


		/// <summary>
		/// Unlike the DisplayName property, InternalName is required, and its
		/// value must be unique, since the operating parameters go into a
		/// generic dictionary that is maintained and served to callers by the
		/// OperatingParametersCollection singleton.
		/// </summary>
		public string InternalName { get { return _strInternalName; } }


		/// <summary>
		/// This property identifies the source of the ParamValue value.
		/// </summary>
		public U ParamSource { get { return _enmParameterSource; } }


		/// <summary>
		/// This property reports the initialization and validation state of the
		/// ParamValue property.
		/// </summary>
		public ParameterState ParamState { get { return _enmState; } }


		/// <summary>
		/// This value is intended to designate the rule or rules that the
		/// ParamValue value must pass before its ParamState can be set to
		/// Validated.
		/// </summary>
		public T ParamType { get { return _enmParameterType; } }


		/// <summary>
		/// This property returns the parameter value. Check the ParamState
		/// before you use it.
		/// 
		/// When ParameterState is Uninitialized, this property returns NULL.
		/// 
		/// When ParameterState is Initialized, this value returns a value. 
		/// 
		/// The empty string is an invalid value.
		/// 
		/// If a default value is specified in the application settings, this
		/// property has a value as soon as the constructor returns, and the
		/// ParameterState is Initialized. Otherwise, this property stays NULL
		/// until SetValue is called to set it and mark the parameter as
		/// Initialized.
		/// 
		/// To exercise the validation rules encoded into the parameter type
		/// enumeration (generic type T), call the IsValueValid method on a
		/// concrete instance.
		/// 
		/// When this property has a value, you can determine whether it is the
		/// default or an override by checking the HasDefaultValueInAppSettings
		/// flag. If that flag is TRUE, the value that was overridden is saved
		/// in the SavedDefaultValue, which is otherwise NULL.
		/// </summary>
		public string ParamValue { get { return _strValue; } }


		/// <summary>
		/// If a parameter has a default value stored in application settings
		/// that is overridden by a value from another source, the initial value
		/// is saved into this property, so that consumers can easily identify
		/// parameter defaults that were overridden.
		/// 
		/// This value being NULL means one of two things.
		/// 
		/// 1) The parameter has no default value.
		/// 
		/// 2) The current value is the default value.
		/// 
		/// If HasDefaultValueInAppSettings is TRUE, then the current value is
		/// the default value. Otherwise, there is no default value.
		/// </summary>
		public string SavedDefaultValue { get { return _strSavedDefaultValue; } }
		#endregion // Property Getters


		#region Public Methods
		#pragma warning disable CS0693
		/// <summary>
		/// This method implements the validation criteria that are encoded into
		/// the generic parameter type enumeration (generic type T).
		/// </summary>
		/// <typeparam name="T">
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
		public abstract bool IsValueValid<T> ( );
		#pragma warning restore CS0693


		/// <summary>
		/// Call this method to set the value of the parameter. The value can be
		/// set once only, after which its value is locked against future
		/// changes.
		/// </summary>
		/// <param name="pstrValue">
		/// Specify the string to set as the ParamValue property value. If the
		/// constructor initialized the ParamValue, its value is copied into the
		/// SavedDefaultValue property before the new value is set.
		/// </param>
		/// <param name="penmSource">
		/// Specify the member of the enumerated type associated with the U
		/// generic type associated with the concrete instance to which to set
		/// the ParamSource property.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// If pstrValue is a null reference or the empty string, a
		/// ArgumentNullException exception arises.
		/// </exception>
		public void SetValue (
			string pstrValue ,
			U penmSource )
		{
			if ( string.IsNullOrEmpty ( pstrValue ) )
			{
                throw new ArgumentNullException ( nameof ( pstrValue ) );
			}   // TRUE (unanticipated outcome) block, if ( string.IsNullOrEmpty ( pstrValue ) )
			else
			{
				if ( _enmState == ParameterState.Initialized )
				{	// Preserve the default value for reference.
					_strSavedDefaultValue = _strValue;
				}   // TRUE (The parameter has a default value that was read from the application configuration.) block, if ( _enmState == ParameterState.Initialized )
				else
				{	// Change the state from Uninitialized to Initialized.
					_enmState = ParameterState.Initialized;
				}   // FALSE (The application configuration is mute on the subject of a default value for this parameter.) block, if ( _enmState == ParameterState.Initialized )

				_enmParameterSource = penmSource;
				_strValue = pstrValue;
			}   // if ( !string.IsNullOrEmpty ( pstrValue ) )
		}   // SetValue Method
		#endregion // Public Methods


		#region ToString Method Override
		/// <summary>
		/// Override the default ToString method, so that the locals and watch
		/// windows display a concise summary of the object's properties.
		/// </summary>
		/// <returns>
		/// This overridden ToString method returns a string that contains a
		/// string representation of the object's simple name, followed by a
		/// colon, then the properties in a newline delimited string of labels
		/// and values.
		/// </returns>
		public override string ToString ( )
		{
			return string.Format (
				Properties.Resources.OPERATING_PARAMETER_TOSTRING ,             // Format control string
				this.GetType ( ).Name ,                                         // Format Item 0: {0}: InternalName =
				_strInternalName ,                                              // Format Item 1: InternalName = {1}
				Utl.RenderStringValue ( _strDisplayName ) ,                     // Format Item 2: DisplayName = {2}
				Utl.RenderStringValue ( _strValue ) ,                           // Format Item 3: ParamValue = {3},
				_enmParameterType ,                                             // Format Item 4: ParamType = {4},
				_fHasDefaultValueInAppSettings ,                                // Format Item 5: MayHaveAppSetting = {5},
				_enmState ,                                                     // Format Item 6: ParamState = {6},
				_enmParameterSource ,                                           // Format Item 7: ParamSource = {7},
				Utl.RenderStringValue ( _strSavedDefaultValue ) ,               // Format Item 8: SavedDefaultValue = {8}
				Environment.NewLine );                                          // Format Item 9: Platform-dependent enwline
		}   // ToString Method override
		#endregion // ToString Method Override


		#region IComparable Implemtation
		/// <summary>
		/// Make pairs of objects comparable by the values of their ParamValue
		/// properties.
		/// </summary>
		/// <param name="That">
		/// This explicit implementation of the IComparable interface expects an
		/// instance of the same type of class as its comparand.
		/// </param>
		/// <returns>
		/// The return value is generally described as follows.
		/// 
		/// 1) Less than Zero = This instance precedes That instance in the sort order.
		/// 
		/// 2) Zero = This instance and That instance  occur at the same place in the sort order.
		/// 
		/// 3) Greater than Zero = This instance follows That instance in the sort order.
		/// </returns>
		public int CompareTo ( OperatingParameterBase<T, U> That )
		{
			return _strValue.CompareTo ( That._strValue );
		}   // CompareTo method
		#endregion // IComparable Implemtation


		#region Private Instance Methods
		/// <summary>
		/// Parse the DisplayName property. 
		/// 
		/// If it contains DISPLAY_NAME_SUBSTITUTION_TOKEN, treat it as a format
		/// control string from which to construct the name of a string resource
		/// to substitute into the property as its actual display name.
		/// 
		/// If the calling assembly contains no such string, revert to the value
		/// of the InternalName property.
		/// 
		/// Otherwise, leave it unchanged.
		/// </summary>
		private void ParseDisplayName ( )
		{
			if ( _strDisplayName.IndexOf ( DISPLAY_NAME_SUBSTITUTION_TOKEN ) > WizardWrx.ListInfo.INDEXOF_NOT_FOUND )
			{
				string strDisplayNameStringResourceName = string.Format (
					_strDisplayName ,
					_strInternalName );
				string strTempDisplayname = Utl.GetStringResourceByNameFromEntryAssembly (
					strDisplayNameStringResourceName );
				_strDisplayName = string.IsNullOrEmpty ( strTempDisplayname )
					? _strInternalName
					: strTempDisplayname;
			}   // if ( _strDisplayName.IndexOf ( DISPLAY_NAME_SUBSTITUTION_TOKEN ) > WizardWrx.ListInfo.INDEXOF_NOT_FOUND )
		}   // ParseDisplayName
		#endregion // Private Instance Methods


		#region Private Static Methods
		/// <summary>
		/// If the application settings collection includes a like named key,
		/// take its value as the default value of the current parameter.
		/// </summary>
		/// <typeparam name="T">
		/// Since its only use is in a cast, the type is unconstrained.
		/// </typeparam>
		/// <param name="pstrAttributeName">
		/// Specify the display name of the paramter, which corresponds to the
		/// application settings key of its default value if it has one.
		/// </param>
		/// <param name="pgenNewValue">
		/// This method uses a reference to the parameter value, so that its
		/// return value can be a simple Boolean.
		/// 
		/// Please see the Remarks for important details.
		/// </param>
		/// <returns>
		/// Returning a True/False value informs the caller so that it can set
		/// other properties when the return value is TRUE, or leave them as is
		/// otherwise. 
		/// 
		/// Please see the Remarks for important details.
		/// </returns>
		/// <exception cref="InvalidCastException">
		/// An InvalidCastException exception is thrown when the type of the
		/// application settings property and that of the reference (output)
		/// parameter differ.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// An InvalidOperationException exception is thrown when the Message
		/// property on the SettingsPropertyNotFoundException exception differs
		/// from the expected content.
		/// </exception>
		/// <exception cref="Exception">
		/// Though no other types of exceptions are expected, any that arise are
		/// caught and re-thrown.
		/// </exception>
		/// <remarks>
		/// A key design goal of this method is that its concern is focused
		/// exclusively on setting the default value of a property, if it has
		/// one, while informing the caller of the outcome without burdening it
		/// with the overhead that would otherwise be required to detect that
		/// this method changed the value.
		/// 
		/// Though the common method of accomplishing this employs an OUT
		/// parameter, an OUT parameter must be assigned a value before this
		/// method returns, regardless of the outcome. Since such a result would
		/// have no effect unless the return value is TRUE, and it would be
		/// changed anyway, passing in a REF parameter relaxes that requirement,
		/// eliminating a wasted operation.
		/// </remarks>
		/// <exception cref="InvalidCastException">
		/// A misconfigured application raises an InvalidCastException exception
		/// when the type of the parameter default value stored in the settings
		/// differs from the type specified in the ParameterTypeInfo collection
		/// data that was loaded from the table in the text resource.
		/// </exception>
		#pragma warning disable CS0693
		private static bool CheckForDefaultValueInAppSettings<T> (
			string pstrAttributeName ,
			ref T pgenNewValue )
		#pragma warning restore CS0693
		{
			try
			{
				object objSettingsValue = s_settingsForEntryAssembly.GetAppSettingByName ( pstrAttributeName );

				if ( objSettingsValue != null )
				{	// The application settings contain a default value. Make sure it is of the expected type.
					if ( objSettingsValue is T )
					{   // The setting value is of the expected type. Copy the default value into the location to which reference argument pgenNewValue points, and return TRUE.
						pgenNewValue = ( T ) objSettingsValue;
						return true;
					}   // TRUE (anticipated outcome) block, if ( objSettingsValue is T )
					else
					{	// The type of the actual default parameter value differs from the expected type. Since this should never happen in a correctly configured application, raise a detailed exception.
						string strExceptionMessage = string.Format (                // Prepare a detailed diagnostic message.
							Properties.Resources.MESSAGE_UNEXPECTED_SETTINGS_TYPE , // Format control string
							pstrAttributeName ,                                     // Format Item 0: Settings property name  = {0}
							objSettingsValue.GetType ( ) ,                          // Format Item 1: Settings property type  = {1}
							pgenNewValue.GetType ( ) ,                              // Format Item 2: Expected settings type  = {2}
							objSettingsValue ,                                      // Format Item 3: Settings property value = {3}
							Environment.NewLine );                                  // Format Item 4: Plaform-dependent newline
						throw new InvalidCastException ( strExceptionMessage );     // Toss cookies and die.
					}   // FALSE (unanticipated outcome) block, if ( objSettingsValue is T )
				}   // TRUE (The application settings include a default value for this operating parameter.) block, if ( objSettingsValue != null )
				else
				{	// The application settings don't define a default value for this operating parameter. Leave the value as is (NULL) and return FALSE.
					return false;
				}   // FALSE (The application settings omit a default value for this operating parameter.) block, if ( objSettingsValue != null )
			}   // try block
			catch ( Exception exAllOthers )
			{   // Something entirely unexpected happened. Toss cookies and die.
				throw exAllOthers;
			}   // catch ( Exception exAllOthers )
		}   // CheckForDefaultValueInAppSettings
		#endregion // Private Static Methods


		#region Private Instance Property Storeage
        /// <summary>
        /// When set to True in a derived class, this member causes the default
        /// value stored in the Application Settings to prevail.
        /// </summary>
		protected bool _fHasDefaultValueInAppSettings = false;

        /// <summary>
        /// This generic enumeration type identifies the source from which the
        /// current value originated, e. g., Application Settings versus Command
        /// Line.
        /// </summary>
		protected U _enmParameterSource;

        /// <summary>
        /// This generic enumeration type identifies the native Type of the
        /// parameter.
        /// </summary>
		protected T _enmParameterType;

        /// <summary>
        /// This ParameterState enumeration member indicates the current
        /// state of the parameter.
        /// </summary>
		protected ParameterState _enmState = ParameterState.Uninitialized;

        /// <summary>
        /// The DisplayName is a string that identifies the parameter in user
        /// prompts, reports, and such.
        /// </summary>
		protected string _strDisplayName;

        /// <summary>
        /// The InternalName is a string that links the parameter to its default
        /// value and its definition.
        /// </summary>
		protected string _strInternalName;

        /// <summary>
        /// The SavedDefaultValue property is a string that stores the default
        /// value when it is overridden by a value from another source.
        /// </summary>
		protected string _strSavedDefaultValue;

        /// <summary>
        /// The Value property is a string that stores the string representation
        /// of the current value of the parameter.
        /// </summary>
		protected string _strValue;
		#endregion // Private Instance Property Storeage

		#region Private Static Property Storage
		private static AppSettingsForEntryAssembly s_settingsForEntryAssembly = null;
		private static readonly SyncRoot s_syncRoot = new SyncRoot ( typeof ( OperatingParameterBase<T , U> ).ToString ( ) );
		#endregion
	}   // OperatingParameterBase
}   // partial namespace WizardWrx.OperatingParameterManager