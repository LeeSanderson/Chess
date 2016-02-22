using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;

namespace MiniMPI.Testing
{
	/// <summary>
	/// Tests basic Send-Recv-Wait functionality of the runtime.
	/// </summary>
	
	public class SendRecvWaitTests : MpiTestBase
	{

		[ScheduleTestMethod]
		public void Send1_Recv1()
		{
			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					// Sender
					var handle = mpi.SendAsync(1, "something");
					mpi.Wait(handle);
				}
				else
				{
					// Receiver
					var handle = mpi.ReceiveAsync(0);
					string msg = mpi.Wait(handle);
					Assert.AreEqual("something", msg);
				}

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void SendToSelf_WS_WR()
		{
			ExecuteMpiProgram(1, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				// Initiate the instructions
				var sendHdl = mpi.SendAsync(rank, "something");
				var recvHdl = mpi.ReceiveAsync(rank);

				// Wait on the send first
				mpi.Wait(sendHdl);
				string msg = mpi.Wait(recvHdl);
				Assert.AreEqual("something", msg);

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void SendToSelf_WR_WS()
		{
			ExecuteMpiProgram(1, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				var sendHdl = mpi.SendAsync(rank, "something");
				var recvHdl = mpi.ReceiveAsync(rank);

				// For this version, we wait for the recv first
				string msg = mpi.Wait(recvHdl);
				mpi.Wait(sendHdl);
				Assert.AreEqual("something", msg);

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void WaitOnSendHandleMoreThanOnce()
		{
			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					// Sender
					var handle = mpi.SendAsync(1, "something");
					mpi.Wait(handle);

					// This call should merely return.  It will instantly be matched and
					// completed
					mpi.Wait(handle);
				}
				else
				{
					// Receiver
					var handle = mpi.ReceiveAsync(0);
					string msg = mpi.Wait(handle);
					Assert.AreEqual("something", msg);
				}

				mpi.MpiFinalize();
			});
		}
		
		[ScheduleTestMethod]
		public void WaitOnReceiveHandleMoreThanOnce()
		{
			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					// Sender
					var handle = mpi.SendAsync(1, "something");
					mpi.Wait(handle);
				}
				else
				{
					// Receiver
					var handle = mpi.ReceiveAsync(0);
					string msg1 = mpi.Wait(handle);
					
					// This second wait should merely return the same string as before.
					string msg2 = mpi.Wait(handle);
					
					Assert.AreEqual("something", msg1);
					Assert.AreEqual("something", msg2);
					Assert.AreEqual(msg1, msg2);
				}
				
				mpi.MpiFinalize();
			});
		}
		
		[ScheduleTestMethod]
		public void ManySendRecvs_AssertNonOvertaking()
		{
			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();
				int x = 20;

				if (rank == 0)
				{
					// Sender
					List<SendHandle> handles = new List<SendHandle>();
					for (int i = 0; i < x; i++)
						handles.Add(mpi.SendAsync(1, i.ToString()));

					// wait in same order
					foreach (var hdl in handles)
						mpi.Wait(hdl);
				}
				else
				{
					// Receiver
					List<ReceiveHandle> handles = new List<ReceiveHandle>();
					for (int i = 0; i < x; i++)
						handles.Add(mpi.ReceiveAsync(0));

					// wait in same order
					for (int i = 0; i < x; i++)
					{
						int val = int.Parse(mpi.Wait(handles[i]));
						Assert.AreEqual(i, val);
					}
				}

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void Wildcard_TestIntraHB1()
		{
			bool eagerVal = true;

			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					var h00 = mpi.SendAsync(1, "s00", eagerVal);
					var h01 = mpi.SendAsync(1, "s01", eagerVal);

					mpi.Wait(h00);
					mpi.Wait(h01);
				}
				else
				{
					var h10 = mpi.ReceiveAsync(0);
					var h11 = mpi.ReceiveAsync(null);

					string msg11 = mpi.Wait(h11);
					string msg10 = mpi.Wait(h10);

					Assert.AreEqual("s00", msg10);
					Assert.AreEqual("s01", msg11);
				}

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void Wildcard_TestIntraHB2()
		{
			bool eagerVal = true;

			ExecuteMpiProgram(2, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					var h00 = mpi.SendAsync(1, "s00", eagerVal);
					var h01 = mpi.SendAsync(1, "s01", eagerVal);

					mpi.Wait(h00);
					mpi.Wait(h01);
				}
				else
				{
					// Wildcard before explicit should still receive in that order...???
					var h10 = mpi.ReceiveAsync(null);
					var h11 = mpi.ReceiveAsync(0);

					string msg11 = mpi.Wait(h11);
					string msg10 = mpi.Wait(h10);

					Assert.AreEqual("s00", msg10);
					Assert.AreEqual("s01", msg11);
				}

				mpi.MpiFinalize();
			});
		}






		[ScheduleTestMethod]
		public void MessageConversationTest_Explicit()
		{
			ExecuteMpiProgram(3, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					var sendHdl1 = mpi.SendAsync(1, "What year is it?");
					var sendHdl2 = mpi.SendAsync(2, "What year is it?");

					mpi.Wait(sendHdl1);
					mpi.Wait(sendHdl2);

					var recvHdl1 = mpi.ReceiveAsync(1);
					var recvHdl2 = mpi.ReceiveAsync(2);

					string ans1 = mpi.Wait(recvHdl1);
					string ans2 = mpi.Wait(recvHdl2);
					mpi.Wait(mpi.SendAsync(1, (int.Parse(ans1) == DateTime.Today.Year).ToString()));
					mpi.Wait(mpi.SendAsync(2, (int.Parse(ans2) == DateTime.Today.Year).ToString()));
				}
				else if (rank == 1)
				{
					// bad answer
					string msg = mpi.Wait(mpi.ReceiveAsync(0));
					Assert.AreEqual("What year is it?", msg);
					mpi.Wait(mpi.SendAsync(0, "2000"));
					msg = mpi.Wait(mpi.ReceiveAsync(0));
					Assert.AreEqual("False", msg);
				}
				else
				{
					// correct answer
					string msg = mpi.Wait(mpi.ReceiveAsync(0));
					Assert.AreEqual("What year is it?", msg);
					mpi.Wait(mpi.SendAsync(0, DateTime.Today.Year.ToString()));
					msg = mpi.Wait(mpi.ReceiveAsync(0));
					Assert.AreEqual("True", msg);
				}

				mpi.MpiFinalize();
			});
		}

		[ScheduleTestMethod]
		public void MessageConversationTest_Wildcards()
		{
			// Only declare constants out here
			const string Question_WhatYear = "What year is it?";

			ExecuteMpiProgram(3, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();
				int studentCnt = mpi.ProcessCount - 1;
				int teacherRank = studentCnt;

				if (rank == teacherRank) // The last one is the teacher so loop idxs are easier
				{
					// Teacher
					// Ask all the children the question
					for (int s = 0; s < studentCnt; s++)
						mpi.SendAsync(s, Question_WhatYear);

					List<int> studentsCorrect = new List<int>();
					// Wait for the answers one by one
					while (studentsCorrect.Count < studentCnt)
					{
						// Wait for an answer from anyone
						string[] msgParts = mpi.Wait(mpi.ReceiveAsync(null)).Split('|');
						int msgRank = int.Parse(msgParts[0]);
						int msgAns = int.Parse(msgParts[1]);

						if (msgAns == DateTime.Today.Year)
						{
							Assert.IsFalse(studentsCorrect.Contains(msgRank), "The student has already guessed correctly.");
							studentsCorrect.Add(msgRank);
							mpi.SendAsync(msgRank, "True");
						}
						else
							mpi.SendAsync(msgRank, "False");
					}

				}
				else
				{
					// students
					// Wait for the question
					string msg = mpi.Wait(mpi.ReceiveAsync(teacherRank));
					Assert.AreEqual(Question_WhatYear, msg);

					// Guess the answer
					int guess = DateTime.Today.Year - rank - 1; // everyone will guess wrong atleast once
					bool isCorrect;
					do
					{
						// Not going to worry about waiting on the send here because I think it'll work out anyways
						mpi.SendAsync(teacherRank, String.Concat(rank, '|', guess));

						isCorrect = bool.Parse(mpi.Wait(mpi.ReceiveAsync(teacherRank)));
						guess++;	// Do the next guess
					} while (!isCorrect);
				}

				mpi.MpiFinalize();
			});
		}

		/// <summary>
		/// This helps fix a bug with the MiniMPI runtime where if one thread is blocking on an MPI Wait/Barrier
		/// instruction while another thread throws an exception than Chess will detect a deadlock because
		/// the blocking thread doesn't know to quit.
		/// </summary>
		[ScheduleTestMethod]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void ExceptionThrownWhileOtherThreadsWait_NeedToWakeUpBlockedWaitsToExitTheProcess()
		{
			ExecuteMpiProgramWithExpectedException(2, 0, mpi =>
			{
				mpi.MpiInit();
				int rank = mpi.GetRank();

				if (rank == 0)
				{
					// ECHO Server

					// Wait for an answer from anyone
					string[] vals = mpi.Wait(mpi.ReceiveAsync(null)).Split('|');

					// Crux: Just throw an arbitrary exception (manufactored here)
					// The bug was that this thread would throw the exception, but the client
					// would block until the Wait finishes.
					// Fix: The runtime should un-block any blocked (via wait or barrier) threads when a process throws an exception.
					bool b = bool.Parse(vals[1]);   // Throws IndexOutOfRangeException ex: client doesn't send the '|' char along so vals.Length == 0.

					// Echo back the value
					mpi.SendAsync(1, b.ToString());
				}
				else
				{
					// client

					// Send the value we want back
					mpi.SendAsync(0, true.ToString());

					bool isCorrect = bool.Parse(mpi.Wait(mpi.ReceiveAsync(0)));
					Assert.IsTrue(isCorrect);
				}

				mpi.MpiFinalize();
			});
		}


		//[ScheduleTestMethod]
		//public void MessageConversationTest()
		//{
		//	ExecuteMpiProcess(2, mpi => {
		//		mpi.MpiInit();
		//		int rank = mpi.MpiGetRank();

		//		mpi.MpiFinalize();
		//	});
		//}

		// TODO: If send then recv, can the wait on the recv happen before the send? Soes to the dest have to process the send before the recv?

	}
}
