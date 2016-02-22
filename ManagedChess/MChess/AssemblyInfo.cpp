/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly:AssemblyTitleAttribute("MChess")];
[assembly:AssemblyDescriptionAttribute("Managed interface to CHESS Engine")];
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("Microsoft Corporation")];
[assembly:AssemblyProductAttribute("MCHESS")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) Microsoft Corporation")];
[assembly:AssemblyTrademarkAttribute("")];
[assembly:AssemblyCultureAttribute("")];

[assembly:ComVisible(false)];
[assembly:CLSCompliantAttribute(true)];
[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
[assembly:AssemblyKeyFile("ManagedChess.snk")];
