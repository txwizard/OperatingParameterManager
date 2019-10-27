# OperatingParameterManager Change Log

This file is a running history of fixes and improvements from initial
publication onwards. Changes are documented for the newest version first.
Within each version, classes are covered in alphabetical order when applicable.

Detailed API documentation is at <https://txwizard.github.io/OperatingParameterManager/>.

## Documentation Update, released 2019/10/27

It just came to my attention that I neglected to configure GitHub Pages on this
repository, which I discovered when I attempted to visit the documentation for
my own needs. With that corrected, the online user documentation is available to
everyone.

## Version 1.0.14, released 2019/06/30

This build makes everything likely to need it as thread-safe as I know how to
make it, while eliminating a couple of unreferenced assemblies from the list of
referenced assemblies for both the library and the unit test/demonstration
program. Finally, the requirement of C# compiler version 7.3 is relaxed on the
demonstration program, although that change is academinc, since the library must
retain that requirment.

The foregoing changes pave the way for replacing direct assembly references with
NuGet package references, to be followed immediately by release of the library
as a new NuGet package.

Although MSBuild created a new version of the test stand assembly, its code is
unchanged from the original release in September 2018.

## Version 1.0.6, released 2019/06/30

This build clears compiler warnings that arose in conjunction with enabling the
generation of XML documentation of the library. Apart from eliminating a few
redundant namespace qualification prefixes, the code is unchanged.

Although MSBuild created a new version of the test stand assembly, its code is
unchanged.

From this point onwards, the code in the demonstration archive that accompanied
the CodeProject article mentioned in the next change log entry diverges from the
code in this GitHub repository and the accompanying NuGet package that will soon
follow.

## Version 1.0, released 2018/09/01

This release was published concurrently with
"General Purpose Operating Parameters for Console Programs," at
<https://www.codeproject.com/Articles/1258663/General-Purpose-Operating-Parameters-for-Console-P>,
which explains the purpose and use of the library. The code in this repository
and the demo archive attached to the article are identical.