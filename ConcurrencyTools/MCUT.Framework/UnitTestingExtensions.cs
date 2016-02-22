using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public static class UnitTestingExtensions
    {

        /// <summary>
        /// Returns the flags portion of the "/brk:[flags]" commandline option.
        /// </summary>
        /// <returns></returns>
        public static string ToCommandLineOptionValue(this MChessBreak brk)
        {
            string flags = String.Empty;
            foreach (MChessBreak enumVal in Enum.GetValues(typeof(MChessBreak)))
            {
                if ((enumVal & brk) != 0)
                {
                    switch (enumVal)
                    {
                        case MChessBreak.NoBreak: break;
                        case MChessBreak.Start: flags += 's'; break;
                        case MChessBreak.ContextSwitch: flags += 'c'; break;
                        case MChessBreak.AfterContextSwitch: flags += 'C'; break;
                        case MChessBreak.Preemption: flags += 'p'; break;
                        case MChessBreak.AfterPreemption: flags += 'P'; break;
                        case MChessBreak.Deadlock: flags += 'd'; break;
                        case MChessBreak.Timeout: flags += 't'; break;
                        case MChessBreak.TaskResume: flags += 'f'; break;
                        default:
                            throw new NotImplementedException("Enum value not handled: " + enumVal);
                    }
                }
            }

            return flags;
        }

        /// <summary>
        /// Returns the option value for the "/observationmode:[value]" commandline option.
        /// </summary>
        /// <returns></returns>
        public static string ToCommandLineOptionValue(this MChessObservationMode obsMode)
        {
            switch (obsMode)
            {
                case MChessObservationMode.SerialInterleavings: return "serial";
                case MChessObservationMode.CoarseInterleavings: return "coarse";
                case MChessObservationMode.AllInterleavings: return "all";
                case MChessObservationMode.Linearizability: return "lin_s";
                case MChessObservationMode.LinearizabilityNotBlock: return "lin";
                case MChessObservationMode.SequentialConsistency: return "SC_s";
                case MChessObservationMode.SequentialConsistencyNotBlock: return "SC";
                default:
                    throw new NotImplementedException("The MChessObservationMode is not implemented: ");
            }
        }

        public static bool RequiresInputObservationFile(this MChessObservationMode obsMod)
        {
            switch (obsMod)
            {
                //Only the enums equivalent to the ObservationTestCheckingMode require an input obs file
                case MChessObservationMode.Linearizability:
                case MChessObservationMode.LinearizabilityNotBlock:
                case MChessObservationMode.SequentialConsistency:
                case MChessObservationMode.SequentialConsistencyNotBlock:
                    return true;

                default:
                    return false;
            }
        }

        public static MChessObservationMode ToChessObservationMode(this ObservationGranularity granularity)
        {
            switch (granularity)
            {
                case ObservationGranularity.Serial:
                    return MChessObservationMode.SerialInterleavings;
                case ObservationGranularity.Coarse:
                    return MChessObservationMode.CoarseInterleavings;
                case ObservationGranularity.All:
                    return MChessObservationMode.AllInterleavings;
                default:
                    throw new NotImplementedException("The enum value has not been handled: " + granularity);
            }
        }

        public static MChessObservationMode ToChessObservationMode(this ObservationTestCheckingMode checkingMode)
        {
            switch (checkingMode)
            {
                case ObservationTestCheckingMode.Linearizability:
                    return MChessObservationMode.Linearizability;
                case ObservationTestCheckingMode.SequentialConsistency:
                    return MChessObservationMode.SequentialConsistency;
                case ObservationTestCheckingMode.LinearizabilityNotBlock:
                    return MChessObservationMode.LinearizabilityNotBlock;
                case ObservationTestCheckingMode.SequentialConsistencyNotBlock:
                    return MChessObservationMode.SequentialConsistencyNotBlock;
                default:
                    throw new NotImplementedException("The enum value has not been handled: " + checkingMode);
            }
        }

    }
}
