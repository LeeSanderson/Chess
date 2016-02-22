using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;

namespace MiniMPI.Testing
{
	/// <summary>Exception to make sure it's not a system or any library exception.</summary>
	[global::System.Serializable]
	public class TestUserException : Exception
	{

		public TestUserException() { }
		public TestUserException(string message) : base(message) { }
		protected TestUserException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
