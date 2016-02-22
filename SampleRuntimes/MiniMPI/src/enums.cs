using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The states an MiniMPIProgram instance goes thru.
    /// </summary>
    internal enum MiniMPIProgramState
    {
        /// <summary>
        /// The program instance has been constructed but has not been executed yet.
        /// </summary>
        Initialized = 0,

        /// <summary>
        /// The runtime is executing the assigned processes.
        /// </summary>
        Executing,

        /// <summary>
        /// The runtime has finished executing the assigned processes and cannot
        /// be restarted.
        /// </summary>
        Finished,
    }

    /// <summary>
    /// 	Indicates the state that an <see cref="MpiProcess"/> is in.  This is
    /// 	used to indicate whether a process has called the Initialize or
    /// 	finalized MPI calls.
    /// </summary>
    public enum MpiProcessState : int
    {
        /// <summary>
        /// The process has been created, but not started yet.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// The process has started running but the MPI Initialize method
        /// has not been called yet.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates the process has called the MiniMPI Initialize method
        /// but has not been Finalized. MPI calls are valid in this
        /// state.
        /// </summary>
        MpiInitialized,

        /// <summary>
        /// Indicates the process has called the MiniMPI Finalize method. No
        /// more MPI calls are valid in this state.
        /// </summary>
        MpiFinalized,

        /// <summary>
        /// The process has finished executing.
        /// </summary>
        Finished,
    }

}
