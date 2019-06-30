# OperatingParameterManager Readme

`WizardWrx.OperatingParameterManager.dll` and its NuGet package,
`WizardWrx.OperatingParameterManager`, expose a set of classes that enable any
desktop program that targets the Microsoft .NET Framework to process command
line arguments (parameters) that must pass a set of validation rules, all of
which are taken from a text resource that is embedded in the entry assembly.

Since there are no name collisions, you may safely set references to the
`WizardWrx.OperatingParameterManager` namespace, alone or in conjunction with
other `WizardWrx` namespaces in the same source module.

Detailed API documentation is at <https://txwizard.github.io/OperatingParameterManager/>.

"General Purpose Operating Parameters for Console Programs," at
<https://www.codeproject.com/Articles/1258663/General-Purpose-Operating-Parameters-for-Console-P>
explains the purpose and use of the library.

For those who just want to use them, debug and release builds of the libraries
and the unit test program are available as archives off the project root
directory.

*	`OperatingParameters_Demo_Binaries_Debug.7z` is the debug build of the binaries.

*	`OperatingParameters_Demo_Binaries_Release.7z` is the release build of the binaries.

There is a DLL, PDB, and XML file for each library. To derive maximum benefit,
including support for the Visual Studio managed code debugger and IntelliSense
in the text editor, take all three.

## Change Log

See `ChangeLog.md` for a cumulative history of changes, listed from newest to
oldest, beginning with its initial publication in conjunction with the article.
Changes are also meticulously documented in the top of each source file.