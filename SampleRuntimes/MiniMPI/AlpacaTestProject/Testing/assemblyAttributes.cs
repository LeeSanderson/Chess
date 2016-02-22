using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

// Since this is a regression test assembly for MiniMPI, we don't actually want to use the wrappers
// TODO: Disable MiniMPI wrappers for this assembly

// For all our regression tests, we want to not have MiniMPI preempt itself
// This will eventually be implemented via wrappers.
[assembly:ChessTogglePreemptability(PreemptabilityTargetKind.Type, "MiniMPI.MiniMPIProgram")]
