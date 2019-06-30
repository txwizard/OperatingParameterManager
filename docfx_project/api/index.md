# WizardWrx.OperatingParameterManager API

The following sections summarize the classes exposed by this library, all of
which belong to the `WizardWrx.OperatingParameterManager` namespace. Unless

Use the links in the table of contents along the left side of this page to view
the documentation a namespace.

## Class AppSettingsForEntryAssembly

The AppSettingsForEntryAssembly class exposes the Application Settings of the
entry assembly as a basis from which to establish default values of a set of
program parameters.

This class is implemented as a Singleton by inheriting the GenericSingletonBase
class exposed by the WizardWrx namespace through WizardWrx.Core.dll.

## Class OperatingParameter

This class is a concrete instance that implements abstract method IsValueValid
for generic type T = ParameterType.

Type __T__ is expected to be an enumeration that corresponds to the rules for
evaluating parameters (e. g., the parameter must be the name of an existing
file, the paraameter must be a valid file name, but the file must not exist,
the parameter must be the name of an existing directory.

All ParameterTypeInfo constructors MUST specify the same type of enumeration, as
must the OperatingParameters that go into the OperatingParametersCollection.
Otherwise, the behavior of the OperatingParameters collection and its members is
undefined.

Type __U__ is expected to be an emumeration that corresponds to the source from
which the parameter's value originated. This is used internally to identify the
source, e. g., ApplicationSettings or CommandLine, for reporting purposes and
for preserving a default value read from one source, and overridden by a second
source.

The `OperatingParameters` objects that go into the `OperatingParametersCollection`
singleton __must__ specify the same generic type. Otherwise, the behavior of the
`OperatingParameters` collection and its members is undefined.

## Class OperatingParameterBase

This abstract class defines a generic `OperatingParameter` object. Though generic
classes are implicitly abstract, they are seldom so marked.

However, since its `IsValueValid` method is generic, and its implementation is so
tightly coupled to the properties of the resolved generic type that its
implementation must be left to the concrete derived class.

## Class OperatingParametersCollection

This class defines a collection of `OperatingParameters` objects that are stored
in a generic Dictionary that is keyed by the name of each parameter.

The idea behind the collection is that parameters are loaded into it, validated,
and subsequently called forth as needeed by the program, providing thereby an
easily extensible in-memory store for its operating parameters.

Since an application needs at most one collection of operating parameters, this
object is implemented as a Singleton, so that your program isn't littered with
multiple copies of its parameters, nor does it waste time processing them more
than once.

## Class ParameterTypeInfo

This class maps the columns of a table of parameter type properties and their
values to the properties of an instance of itself. The properties of a set of
parameters are expected to be read into instances of this class, one instance
per row, from an input file, with the objective of making addition of parameters
easier by storing their basic attributes in a file, which may exist either as a
text file in the file system or an embedded resource in the application
assembly.

## Class Utl

This static class exposes utility methods that are intended for internal use.
Since some of these may be useful in their own right, the class is marked as
public.