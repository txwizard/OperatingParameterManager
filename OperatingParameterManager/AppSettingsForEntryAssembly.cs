/*
    ============================================================================

    Namespace:			WizardWrx.OperatingParameterManager

    Class Name:			AppSettingsForEntryAssembly

	File Name:			AppSettingsForEntryAssembly.cs

    Synopsis:			Use a reference to the application (configuration)
						settings of a character mode (console) application to
						make them accessible to its dependent assemblies.

	Remarks:			Mechanisms are provided for requesting the value of any
						setting by name or for enumerating the settings.

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
    /// The AppSettingsForEntryAssembly class exposes the Application Settings
    /// of the entry assembly as a basis from which to establish default values
    /// of a set of program parameters.
    /// </summary>
    /// <remarks>
    /// This class is implemented is implemented as a Singleton by inheriting my
    /// GenericSingletonBase class.
    /// </remarks>
	public class AppSettingsForEntryAssembly : GenericSingletonBase<AppSettingsForEntryAssembly>
	{
        /// <summary>
        /// By convention, all Singleton objects are acquired by calling static
        /// method GetTheSingleInstance, which returns a reference to the only
        /// instance of the class that allowed to exist in the scope of an
        /// application.
        /// </summary>
        /// <param name="psettingsPropertyValueCollection">
        /// The first call to this method binds the singleton to the
        /// System.Configuration.SettingsPropertyCollection of the entry
        /// assembly. Subsequent calls ignore this paramter, returning the
        /// initialized instance that was created and returned by the first
        /// call.
        /// 
        /// Since this parameter is ingored on all but the first call, it may be
        /// null on subsequent calls. For this to work as expected, your design
        /// must guarantee that the first call passes the
        /// SettingsPropertyCollection of the entry assembly.
        /// </param>
        /// <returns>
        /// All calls return an AppSettingsForEntryAssembly instance that was
        /// initialized with the SettingsPropertyCollection of the entry
        /// assembly.
        /// </returns>
		public static AppSettingsForEntryAssembly GetTheSingleInstance ( SettingsPropertyCollection psettingsPropertyValueCollection )
		{
			lock ( s_srCriticalSection )
				if ( s_appSettingsForEntryAssembly == null )
					s_appSettingsForEntryAssembly = new AppSettingsForEntryAssembly ( psettingsPropertyValueCollection );

			return s_appSettingsForEntryAssembly;
		}   // GetTheSingleInstance


		/// <summary>
		/// List all Application Settings values on the Standard Output stream
		/// of a console application.
		/// </summary>
		public void ListAllAppSettings ( )
		{
			Console.WriteLine (
				Properties.Resources.MESSAGE_APPSETTINGS_HEADER ,               // Format Control String: {1}Application Setting Defaults for {0}:{1}
				System.Reflection.Assembly.GetEntryAssembly().Location ,        // Format Item 0: Setting Defaults for {0}
				Environment.NewLine );                                          // Format Item 1: platform-dependent newline
			int intItemNumber = ListInfo.LIST_IS_EMPTY;

			foreach ( SettingsProperty settingsProperty in _settingsPropertyValueCollection )
			{
				Console.WriteLine (
					Properties.Resources.MESSAGE_APPSETTING_VALUE ,             // Format Control String: AppSetting # {0,2}: Name         = {1}{4}               PropertyType = {2}{4}               DefaultValue = {3}{4}
					++intItemNumber ,                                           // Format Item 0: AppSetting # {0,2}:
					settingsProperty.Name ,                                     // Format Item 1: Name         = {1}
					settingsProperty.PropertyType ,                             // Format Item 2: PropertyType = {2}
					settingsProperty.DefaultValue ,                             // Format Item 3: DefaultValue = {3}
					Environment.NewLine );                                      // Format Item 4: platform-dependent newline
			}   // foreach ( System.Configuration.SettingsProperty settingsProperty in _settingsPropertyValueCollection )

			Console.WriteLine (
				Properties.Resources.MESSAGE_APPSETTINGS_FOOTER ,               // Format Control String: {1}Seetings count = {0}:{1}
				intItemNumber ,                                                 // Format Item 0: count = {0}
				Environment.NewLine );                                          // Format Item 1: platform-dependent newline
		}   // public ListAllAppSettings Method



		/// <summary>
		/// Return the settings as an array of SettingsProperty objects.
		/// </summary>
		/// <returns>
		/// If the application has properties, this method returns them as an
		/// array of SettingsProperty objects. Otherwise, it returns a null
		/// reference.
		/// </returns>
		public SettingsProperty [ ] EnumerateAllSettings ( )
		{
			if ( _settingsPropertyValueCollection.Count > ArrayInfo.ARRAY_IS_EMPTY )
			{
				SettingsProperty [ ] rasettingsProperties = new SettingsProperty [ _settingsPropertyValueCollection.Count ];

				_settingsPropertyValueCollection.CopyTo (
					rasettingsProperties ,
					ArrayInfo.ARRAY_FIRST_ELEMENT );

				return rasettingsProperties;
			}   // TRUE (anticipated outcome) block, if ( _settingsPropertyValueCollection.Count > ArrayInfo.ARRAY_IS_EMPTY )
			else
			{
				return null;
			}   // FALSE (unanticipated outcome) block, if ( _settingsPropertyValueCollection.Count > ArrayInfo.ARRAY_IS_EMPTY )
		}   // public EnumerateAllSettings method


		/// <summary>
		/// Look up the value of an application setting by name.
		/// </summary>
		/// <param name="pstrName">
		/// Pass in the name of the desired setting.
		/// </param>
		/// <returns>
		/// If the setting named in argument pstrName exists, return its value.
		/// Otherwise, the return value is NULL.
		/// 
		/// Since settings properties are strongly types, the return type must
		/// be Object.
		/// </returns>
		/// <remarks>
		/// The goal of this method is to implement a non-throwing lookup of an
		/// application setting value.
		/// </remarks>
		public object GetAppSettingByName ( string pstrName )
		{
			if ( string.IsNullOrEmpty ( pstrName ) )
				throw new ArgumentNullException ( @"pstrName" );

			foreach ( SettingsProperty settingsProperty in _settingsPropertyValueCollection )
				if ( settingsProperty.Name == pstrName )
					return settingsProperty.DefaultValue;

			return null;
		}   // GetAppSettingByName


		private AppSettingsForEntryAssembly ( ) { }


		private AppSettingsForEntryAssembly ( System.Configuration.SettingsPropertyCollection psettingsPropertyValueCollection )
		{   // Singletons have exactly one of everything, including its constructor, which must be private.
			InitializeOnFirstUse (
				s_srCriticalSection ,						// This static is initialized inline.
				this ,										// Since the call has been pushed forward, the static instance handle has yet to be initialized.
				psettingsPropertyValueCollection );			// Pass in a reference to the caller's settings.
		}   // OperatingParameters constructor


		private void InitializeOnFirstUse (
			SyncRoot ps_srCriticalSection ,
			AppSettingsForEntryAssembly pappSettingsForEntryAssembly ,
			System.Configuration.SettingsPropertyCollection psettingsPropertyValueCollection )
		{
			_settingsPropertyValueCollection = psettingsPropertyValueCollection;
		}   // private void InitializeOnFirstUse


		private System.Configuration.SettingsPropertyCollection _settingsPropertyValueCollection;
		private static AppSettingsForEntryAssembly s_appSettingsForEntryAssembly;
		private static SyncRoot s_srCriticalSection = new SyncRoot ( typeof ( AppSettingsForEntryAssembly ).ToString ( ));
	}   // public class AppSettingsForEntryAssembly
}   // partial namespace WizardWrx.OperatingParameterManager