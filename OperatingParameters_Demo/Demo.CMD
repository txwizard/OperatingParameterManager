@echo off
goto SKIPREM

::  ============================================================================
::
::  Script Name:        Demo.CMD
::
::  Script Objective:   Use the save test parameters that I used for debugging
::                      to implement the demonstration via the File Explorer.
::
::  Script Arguments:   There are no externally supplied arguments.
::
::  Remarks:            This script is intended to run from the directory into
::                      which the demonstration program is installed.
::
::  Script Author:      David A. Gray
::
::  ----------------------------------------------------------------------------
::  Revision History
::  ----------------------------------------------------------------------------
::
::  Date       By  Synopsis
::  ---------- --- -------------------------------------------------------------
::  2018/09/02 DAG Initial implementation
::  ============================================================================

:SKIPREM

    echo ---------------------------------------------------------------------
    echo Info: %~0, version %~t0 Begin
    echo ---------------------------------------------------------------------
    echo.

    setlocal
    pushd %~dp0

    echo ---------------------------------------------------------------------
    echo Info: OperatingParameter1 = OperatingParameter1_NewValue
    echo       OperatingParameter2 = OperatingParameter2_NewValue
    echo       Workind Directory   = %CD%
    echo ---------------------------------------------------------------------
    echo.

    OperatingParameters_Demo.exe OperatingParameter1=OperatingParameter1_NewValue OperatingParameter2=OperatingParameter2_NewValue

    echo.
    echo ---------------------------------------------------------------------
    echo Info: %~nx0 Done!
    echo ---------------------------------------------------------------------
    echo.