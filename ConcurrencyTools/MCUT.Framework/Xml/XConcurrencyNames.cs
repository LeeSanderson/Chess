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
    /// The commmon XNames concurrency xml schemas.
    /// </summary>
    public static class XConcurrencyNames
    {
        public const string ConcurrencyNamespaceURI = "http://research.microsoft.com/concurrency";
        public static readonly XNamespace ConcurrencyNS = XNamespace.Get(ConcurrencyNamespaceURI);

        public static XAttribute CreateXmlnsAttribute(bool asDefaultNS)
        {
            if (asDefaultNS)
                return new XAttribute("xmlns", ConcurrencyNS.NamespaceName);
            else
                return new XAttribute(XNamespace.Xmlns + "mc", ConcurrencyNS.NamespaceName);
        }

        // elements
        public static readonly XName Error = ConcurrencyNS + "error";
        public static readonly XName AErrorExceptionType = "exceptionType";
        public static readonly XName ErrorMessage = ConcurrencyNS + "message";
        public static readonly XName ErrorStackTrace = ConcurrencyNS + "stackTrace";


        public static XElement CreateXError(Exception ex)
        {
            if (ex == null)
                return null;
            else
            {
                XElement xerror = new XElement(XConcurrencyNames.Error
                    , new XAttribute(XConcurrencyNames.AErrorExceptionType, ex.GetType().FullName)
                    , new XElement(XConcurrencyNames.ErrorMessage, ex.Message)
                    , new XElement(XConcurrencyNames.ErrorStackTrace, ex.StackTrace)
                    );

                // Recursively append any inner exceptions
                // For AggregateException instances, add an XError element per exception
                AggregateException aggEx = ex as AggregateException;
                if (aggEx != null)
                {
                    foreach (var innerEx in aggEx.InnerExceptions)
                        xerror.Add(CreateXError(innerEx));
                }
                else
                    xerror.Add(CreateXError(ex.InnerException));

                return xerror;
            }
        }

        public static XElement CreateXError(string message, Exception ex = null)
        {
            if (String.IsNullOrEmpty(message))
                return null;
            else
                return new XElement(XConcurrencyNames.Error
                    , new XElement(XConcurrencyNames.ErrorMessage, message)
                    , ex == null ? null : CreateXError(ex)
                    );
        }

    }
}
