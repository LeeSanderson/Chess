using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The public API for a simulated MiniMPI process.
    /// This API instance may be used among any thread spawned by the originating process.
    /// </summary>
    public interface IMiniMPICoreAPI
    {

        /// <summary>Gets the number of processes managed by this runtime instance.</summary>
        /// <remarks>
        /// This is equivalent to a call to MPI_Comm_size(grpID, value) where
        /// value is what's returned and grpID would be associated with this
        /// instance.
        /// </remarks>
        int ProcessCount { get; }

        /// <summary>Registers the current process as being ready to use the MPI runtime.</summary>
        /// <remarks>
        /// This is modeled after the MPI_Init method, where MPI 2 doesn't
        /// require any args to be passed in.
        /// </remarks>
        void MpiInit();

        /// <summary>Gets the rank within the runtime's group of threads for the calling thread.</summary>
        /// <returns>A value in the range [0,ProcessCount).</returns>
        int GetRank();

        /// <summary>
        /// Notifies the MPI runtime that the calling process is finished using it.
        /// This disables any use of any other MPI calls.
        /// </summary>
        /// <remarks>
        /// This is modeled after the MPI_Finalize method.
        /// </remarks>
        void MpiFinalize();

        /// <summary>
        ///	Waits until all of the processes belonging to the runtime
        ///	have all called this Barrier method.  Execution will not
        ///	return to the caller until this condition has been met.
        ///	See MPI_Barrier in the MPI2 API.
        /// </summary>
        void Barrier();

    }
}
