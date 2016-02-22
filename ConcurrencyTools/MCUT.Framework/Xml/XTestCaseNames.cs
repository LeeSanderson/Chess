/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Xml
{
    /// <summary>
    /// The XNames used in test case xml files.
    /// </summary>
    public static class XTestCaseNames
    {
        // TestCase element
        /// <summary>The element for describing a test case.</summary>
        public static readonly XName TestCase = XNames.UnitTestingNS + "testCase";

        public static readonly XName ManagedTestMethod = XNames.UnitTestingNS + "managedTestMethod";
        public static readonly XName AAssemblyLocation = "assemblyLocation";
        public static readonly XName AFullClassName = "fullClassName";
        public static readonly XName AMethodName = "methodName";

        // attributes
        // NOTE: No need to actually specify a namespace for an attribute since the element already has the same NS
        public static readonly XName AContextName = "contextName";
        /// <summary>Attribute for specifying the type of test to run.</summary>
        public static readonly XName ATestTypeName = "testTypeName";

    }
}
