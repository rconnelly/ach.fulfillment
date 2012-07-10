using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Quad IO, Inc.")]
[assembly: AssemblyCopyright("© Copyright 2012, Quad IO, Inc.")]
[assembly: AssemblyProduct("Ach.Fulfillment")]

#if DEBUG

[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("retail")]
#endif

[assembly: ComVisible(false)]