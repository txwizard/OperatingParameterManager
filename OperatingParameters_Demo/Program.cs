/*
    ============================================================================

    Namespace:			OperatingParameters_Demo

    Class Name:			Program

	File Name:			Program.cs

    Synopsis:			Demonstrate a robust OperatingParameters class and its
						companion collection class.

	Remarks:			Though all but one of the properties could be an auto, I
						prefer named private variables that I can examine in the
						visual debugger.

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
using System.Configuration;

using WizardWrx;
using WizardWrx.ConsoleAppAids3;
using WizardWrx.Core;
using WizardWrx.DLLConfigurationManager;
using WizardWrx.OperatingParameterManager;

namespace OperatingParameters_Demo
{
	class Program
	{
		private const int ERROR_INTERNAL_PROCESSING_ERROR_001 = 9;                      // Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_001
		private const int ERROR_INTERNAL_PROCESSING_ERROR_002 = 10;                     // Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_002
		private const int ERROR_INTERNAL_PROCESSING_ERROR_003 = 11;                     // Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_003
		private const int ERROR_INTERNAL_PROCESSING_ERROR_004 = 12;                     // Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_004
		private const int ERROR_INTERNAL_PROCESSING_ERROR_005 = 13;                     // Properties.Resources.ERRMSG_INTERNAL_PROCESSING_ERROR_005

		private static string [ ] s_astrErrorMessages = { };
		private static ConsoleAppStateManager s_theApp;

		static void Main ( string [ ] args )
		{
			const int EXTRA_LEADING_CHARACTERS = 19;

			const string INTERNAL_ERROR_OO2_PREFIX = @"INTERNAL PROCESSING ERROR 002:";
			const string INTERNAL_ERROR_OO3_PREFIX = @"INTERNAL PROCESSING ERROR 003:";
			const string INTERNAL_ERROR_OO4_PREFIX = @"INTERNAL PROCESSING ERROR 004:";
			const string INTERNAL_ERROR_OO5_PREFIX = @"INTERNAL PROCESSING ERROR 005:";

			s_theApp = ConsoleAppStateManager.GetTheSingleInstance ( );
			s_theApp.DisplayBOJMessage ( );
			s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags = s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags
																	   | ExceptionLogger.OutputOptions.Stack
																	   | ExceptionLogger.OutputOptions.EventLog
																	   | ExceptionLogger.OutputOptions.StandardError;
			s_theApp.BaseStateManager.LoadErrorMessageTable ( s_astrErrorMessages );

			try
			{
				Console.WriteLine ( 
					"Namespace of Entry Point Routine {0} = {1} " ,
					System.Reflection.MethodBase.GetCurrentMethod ( ).Name ,
					System.Reflection.Assembly.GetEntryAssembly ( ).EntryPoint.DeclaringType.Namespace );

				Type typeCmdArgsBasic = typeof ( CmdLneArgsBasic );
				System.Reflection.Assembly asmClassLibrary = System.Reflection.Assembly.GetAssembly ( typeCmdArgsBasic );

				Console.WriteLine (
					"Namespace of Entry Point Routine of the assembly that exports class {0} = {1} " ,
					typeCmdArgsBasic.FullName ,
					asmClassLibrary.EntryPoint == null
						? Properties.Resources.RENDERED_STRING_IS_NULL
						: asmClassLibrary.EntryPoint.DeclaringType.Namespace );

				Utl.ListInternalResourceNames ( System.Reflection.Assembly.GetExecutingAssembly ( ) );
				AppSettingsForEntryAssembly appSettingsForEntryAssembly = AppSettingsForEntryAssembly.GetTheSingleInstance ( Properties.Settings.Default.Properties);

				string strAppSettingMane = @"OperatingParameter1";

				{   // Constrain the scope of objAppSettingValue.
					object objAppSettingValue = appSettingsForEntryAssembly.GetAppSettingByName ( strAppSettingMane );
					Console.WriteLine (
						"Application Setting {0} = {1}" ,
						strAppSettingMane ,
						objAppSettingValue == null
							? Properties.Resources.RENDERED_STRING_IS_NULL
							: objAppSettingValue );
				}   // Let objAppSettingValue go out of scope.

				strAppSettingMane = @"OperatingParameter2";

				{   // Constrain the scope of objAppSettingValue.
					object objAppSettingValue = appSettingsForEntryAssembly.GetAppSettingByName ( strAppSettingMane );
					Console.WriteLine (
						"Application Setting {0} = {1}" ,
						strAppSettingMane ,
						objAppSettingValue == null
							? Properties.Resources.RENDERED_STRING_IS_NULL
							: objAppSettingValue );
				}   // Let objAppSettingValue go out of scope.

				appSettingsForEntryAssembly.ListAllAppSettings ( );
				OperatingParametersCollection<ParameterType , ParameterSource> operatingParametersColl = OperatingParametersCollection<ParameterType , ParameterSource>.GetTheSingleInstance (
					Properties.Settings.Default.Properties ,
					Properties.Resources.PARAMETER_DISPLAY_NAME_TEMPLATE ,
					ParameterSource.ApplicationSettings );
				CmdLneArgsBasic cmdArgs = new CmdLneArgsBasic (
					operatingParametersColl.GetParameterNames ( ) ,
					CmdLneArgsBasic.ArgMatching.CaseInsensitive );
				operatingParametersColl.SetFromCommandLineArguments (
					cmdArgs ,
					ParameterSource.CommandLine );

				int intTotalParameters = operatingParametersColl.GetCount ( );
				Console.WriteLine (
					Properties.Resources.MSG_PARAMETER_LIST_HEADER ,            // Format Control String: The {0} application has {1} parameters.
					s_theApp.BaseStateManager.AppRootAssemblyFileBaseName ,     // Format Item 0: The {0} application
					intTotalParameters ,                                        // Format Item 1: has {1} parameters.
					Environment.NewLine );                                      // Format Item 2: platform-dependent newline
				int intParameterNumber = ListInfo.LIST_IS_EMPTY;

				foreach ( string strParamName in operatingParametersColl.GetParameterNames ( ) )
				{
					Console.WriteLine (
						Properties.Resources.MESSAGE_PARAMETER_NAME ,           // Format Control String: "    Parameter # {0}: {1}" (excluding the quotation marks, of course!)
						++intParameterNumber ,                                  // Format Item 0: Parameter # {0}:
						Utl.ListPropertiesPerDefaultToString (                  // Format Item 1: Property Details listed by its ToString method
							operatingParametersColl.GetParameterByName ( strParamName ) ,
							EXTRA_LEADING_CHARACTERS ) );
				}   // foreach ( string strParamName in operatingParametersColl.GetParameterNames ( ) )

				Console.WriteLine (
					Properties.Resources.MSG_PARAMETER_LIST_FOOTER ,            // Format Control String: End of {0} parameter list.
					s_theApp.BaseStateManager.AppRootAssemblyFileBaseName ,     // Format Item 0: End of {0} parameter
					Environment.NewLine );                                      // Format Item 1: platform-dependent newline

				Console.WriteLine (
					Properties.Resources.MSG_VALIDATION_BEGIN ,                 // Format Control String
					Environment.NewLine );                                      // Format Item 0: platform-dependent newline

				foreach ( string strParamName in operatingParametersColl.GetParameterNames ( ) )
				{
					OperatingParameter<ParameterType , ParameterSource> operatingParameter = operatingParametersColl.GetParameterByName ( strParamName );
					bool fParamIsValid = operatingParameter.IsValueValid<ParameterType> ( );
					Console.WriteLine (
						Properties.Resources.MSG_VALIDATION_DETAIL ,            // Format Control String: Parameter {0} value of {1} is {2}.
						operatingParameter.DisplayName ,                        // Format Item 0: Parameter {0}
						Utl.RenderStringValue (
							operatingParameter.ParamValue ) ,                   // Format Item 1: value of {1}
						fParamIsValid );                                        // Format Item 2: is {2}
				}   // foreach ( string strParamName in operatingParametersColl.GetParameterNames ( ) )

				Console.WriteLine (
					Properties.Resources.MSG_VALIDATION_END ,                   // Format Control String
					Environment.NewLine );                                      // Format Item 0: platform-dependent newline
			}	// try block
			catch ( Exception exAllKinds )
			{   // Theere must be a lower-maintenance way to do this.
				if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO2_PREFIX ) )
				{
					s_theApp.BaseStateManager.AppReturnCode = ERROR_INTERNAL_PROCESSING_ERROR_002;
				}   // TRUE block, if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO2_PREFIX ) )
				else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO3_PREFIX ) )
				{
					s_theApp.BaseStateManager.AppReturnCode = ERROR_INTERNAL_PROCESSING_ERROR_003;
				}   // TRUE block, else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO3_PREFIX ) )
				else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO4_PREFIX ) )
				{
					s_theApp.BaseStateManager.AppReturnCode = ERROR_INTERNAL_PROCESSING_ERROR_004;
				}   // TRUE block, else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO4_PREFIX ) )
				else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO5_PREFIX ) )
				{
					s_theApp.BaseStateManager.AppReturnCode = ERROR_INTERNAL_PROCESSING_ERROR_005;
				}   // TRUE block, else if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO4_PREFIX ) )
				else
				{   // If all else fails, call it a run-of-the-mill runtime error.
					s_theApp.BaseStateManager.AppReturnCode = WizardWrx.MagicNumbers.ERROR_RUNTIME;
				}   // FALSE block, if ( exAllKinds.Message.StartsWith ( INTERNAL_ERROR_OO2_PREFIX ) ) ...

				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exAllKinds );
			}   // catch ( Exception exAllKinds ) block

			if ( s_theApp.BaseStateManager.AppReturnCode == WizardWrx.MagicNumbers.ERROR_SUCCESS )
			{
				s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Timed );
			}   // TRUE (anticipated outcome) block, if ( s_theApp.BaseStateManager.AppReturnCode == WizardWrx.MagicNumbers.ERROR_SUCCESS )
			else
			{
				s_theApp.ErrorExit ( ( uint ) s_theApp.BaseStateManager.AppReturnCode );
			}   // FALSE (unanticipated outcome) block, if ( s_theApp.BaseStateManager.AppReturnCode == WizardWrx.MagicNumbers.ERROR_SUCCESS )
		}   // static void Main
	}   // class Program
}   // partial namespace OperatingParameters_Demo