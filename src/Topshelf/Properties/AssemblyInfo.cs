﻿using System;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Put a title on the individual assembly!
[assembly: AssemblyTitle("Topshelf")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

[assembly: Instrumented("root/topshelf")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ec6099d8-d05e-401b-a288-f62d6d694d4d")]
[assembly:InternalsVisibleTo("Topshelf.Specs")]