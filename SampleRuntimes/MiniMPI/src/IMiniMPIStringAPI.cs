using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The public API for a simulated MiniMPI process that exposes .
    /// This API instance may be used among any thread spawned by the originating process.
    /// </summary>
    public interface IMiniMPIStringAPI : IMiniMPICoreAPI
    {
        /// <summary>
        ///	Sends the payload message from the current thread to the thread
        ///	with the destination rank.  See MPI_Isend in the MPI2 API.
        /// </summary>
        /// <param name="destRank">The destination</param>
        /// <param name="payload">The message being sent</param>
        /// <returns>
        ///	A handle on the send instruction that can be used with a function call
        ///	to <see cref="Wait(SendHandle)" />.
        /// </returns>
        /// <remarks>The eager value of this send is false.</remarks>
        SendHandle SendAsync(int destRank, string payload);

        /// <summary>
        ///	Sends the payload message from the current thread to the thread
        ///	with the destination rank.  See MPI_Isend in the MPI2 API.
        /// </summary>
        /// <param name="destRank">The destination</param>
        /// <param name="payload">The message being sent</param>
        /// <param name="eager">
        /// When true, the instructions is automatically marked as complete when
        /// issued.  Thus an MpiWait on the handle returned will be a no-op.
        /// </param>
        /// <returns>
        ///	A handle on the send instruction that can be used with a function call
        ///	to <see cref="Wait(SendHandle)" />.
        /// </returns>
        SendHandle SendAsync(int destRank, string payload, bool eager);

        /// <summary>
        ///	Receives a message from the source process.
        ///	See MPI_Irecv in the MPI2 API.
        /// </summary>
        /// <param name="srcRank">The source of the message.  Null specifies a "wildcard receive.</param>
        /// <returns>
        ///	A handle on the receive instruction that can be used with a function call
        ///	to <see cref="Wait(ReceiveHandle)" />.
        ///	</returns>
        ReceiveHandle ReceiveAsync(int? srcRank);

        /// <summary>
        ///	Waits for the send instruction associated to this handle to 
        ///	complete.  See MPI_Wait in the MPI2 API.
        /// </summary>
        /// <param name="handle">The handle to the send instruction.</param>
        /// <remarks>
        ///	This method will not return until the associated send 
        ///	instruction to the SendHandle completes.
        /// </remarks>
        void Wait(SendHandle handle);

        /// <summary>
        ///	Waits for the receive instruction associated to this handle to 
        ///	complete.  See MPI_Wait in the MPI2 API.
        /// </summary>
        /// <param name="handle">The handle to the receive instruction</param>
        /// <remarks>
        ///	This method will not return until the associated receive 
        ///	instruction to the SendHandle completes.
        /// </remarks>
        string Wait(ReceiveHandle handle);

    }
}
