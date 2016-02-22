/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include <windows.h>
#include <winnls.h>
#include "ErrorInfo.h"
#include "Chess.h"
#include <sstream>

using namespace std;

#define MCUT_XmlNamespaceUri "http://research.microsoft.com/concurrency"

// NOTE: This is borrowed form ResultsPrinter.cpp
static void escape(std::ostream &stream, const std::string &str)
{
   for (unsigned pos = 0; pos < str.length(); pos++)
   {
      switch(str[pos])
      {
	     case '<': stream << "&lt;"; break;
	     case '>': stream << "&gt;"; break;
	     case '&': stream << "&amp;"; break;
		 default: stream << str[pos];
      }
   }
}



/** construct/destruct */

ErrorInfo::ErrorInfo()
	: Message(NULL), ExType(NULL), StackTrace(NULL)
	, InnerErrors(NULL), InnerErrorsCount(0)
{
}

ErrorInfo::~ErrorInfo()
{
	// Delete all the inner errors
	for(int i = 0; i < InnerErrorsCount; i++)
	{
		delete InnerErrors[i];
		InnerErrors[i] = NULL;
	}
	delete[] InnerErrors;
}

void ErrorInfo::WriteXml(std::ostream& writer) const
{
	if(Message == NULL)
		return;

	writer << "   <error";
	if(ExType != NULL)
		writer << " exceptionType=\"" << ExType << "\"";
	writer << " xmlns=\"" << MCUT_XmlNamespaceUri << "\">" << endl;
	writer << "      <message>";
	escape(writer, Message);
	writer << "</message>" << endl;

	if(StackTrace != NULL)
	{
		writer << "      <stackTrace>";
		escape(writer, StackTrace);
		writer << "</stackTrace>" << endl;
	}

	// Add any inner errors
	for(int i = 0; i < InnerErrorsCount; i++)
	{
		ErrorInfo* innerErr = InnerErrors[i];
		innerErr->WriteXml(writer);
	}

	writer << "   </error>" << endl;
}
