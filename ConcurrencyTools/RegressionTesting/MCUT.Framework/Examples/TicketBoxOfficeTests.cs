using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests.Examples
{
    /// <summary>
    /// This is taken from the PPCP course homework 2 from Ganesh (Fall 2010).
    /// </summary>
    /// <remarks>
    /// 1) Spot all reads and writes of shared data.
    /// 2) Using the underlying Happens-Before graph, show if there is a possibility of a data race. Circle any code snippets where these data races occur.
    /// 3) Check this program for any deadlocks.
    /// 4) Use fine-grain lock leveling to fix any correctness issues you previously found.
    /// </remarks>
    public class TicketBoxOfficeTests
    {

        [ScheduleTestMethod]
        [DataRaceTestMethod]
        public void BuyContiguousTickets_SequencialTest()
        {
            // Initialize our box office
            var boxOffice = new TicketBoxOffice(4, 8);

            // Purchase some random seats
            Assert.IsNotNull(boxOffice.BuyTicketAtSeat(0, 3));
            Assert.IsNotNull(boxOffice.BuyTicketAtSeat(1, 5));
            Assert.IsNotNull(boxOffice.BuyTicketAtSeat(2, 2));
            Assert.IsNotNull(boxOffice.BuyTicketAtSeat(2, 6));
            //Assert.IsNotNull(boxOffice.BuyTicketAtSeat(3, 7));

            // Verify they're sold
            Assert.IsNull(boxOffice.BuyTicketAtSeat(2, 6));

            var tickets = boxOffice.BuyContiguousTickets(1);
            Assert.IsNotNull(tickets);
            Assert.AreEqual(1, tickets.Count);

            tickets = boxOffice.BuyContiguousTickets(3);
            Assert.IsNotNull(tickets);
            Assert.AreEqual(3, tickets.Count);

            tickets = boxOffice.BuyContiguousTickets(8);
            Assert.IsNotNull(tickets);
            Assert.AreEqual(8, tickets.Count);
        }

        [ScheduleTestMethod]
        public void BuyTicketAtSeat_ConcurrentTest()
        {
            BuyTicketAtSeat_ConcurrentTest_impl();
        }

        [DataRaceTestMethod]
        [RegressionTestExpectedResult(TestResultType.DataRace)]
        public void BuyTicketAtSeat_ConcurrentDataRaceTest()
        {
            BuyTicketAtSeat_ConcurrentTest_impl();
        }

        private void BuyTicketAtSeat_ConcurrentTest_impl()
        {
            // Initialize our box office
            var boxOffice = new TicketBoxOffice(4, 8);
            Ticket t0 = null, t1 = null, t2 = null, t3 = null, t4 = null;

            // Purchase some random seats
            Parallel.Invoke(
                () => t0 = boxOffice.BuyTicketAtSeat(0, 3),
                () => t1 = boxOffice.BuyTicketAtSeat(1, 5),
                () => t2 = boxOffice.BuyTicketAtSeat(2, 2),
                () => t3 = boxOffice.BuyTicketAtSeat(2, 6),
                // This will try to buy a ticket that may have already been sold.
                () => t4 = boxOffice.BuyTicketAtSeat(2, 2)
            );

            // Verify they're sold
            Assert.IsNotNull(t0);
            Assert.IsNotNull(t1);
            Assert.IsNotNull(t3);
            // Since buying ticket at duplicate seats only one of t2/t4 should've been successful
            Assert.IsTrue(t2 != null || t4 != null, "At least one of t2/t4 should've been successful.");
            Assert.IsTrue(t2 == null || t4 == null, "At least one of t2/t4 should've been unsuccessful.");
        }

        [ScheduleTestMethod]
        public void BuyContiguousTickets_ConcurrentTest()
        {
            BuyContiguousTickets_ConcurrentTest_impl();
        }

        [DataRaceTestMethod]
        [RegressionTestExpectedResult(TestResultType.DataRace)]
        public void BuyContiguousTickets_ConcurrentDataRaceTest()
        {
            BuyContiguousTickets_ConcurrentTest_impl();
        }

        private void BuyContiguousTickets_ConcurrentTest_impl()
        {
            // Initialize our box office
            var boxOffice = new TicketBoxOffice(4, 8);

            // Purchase some random seats
            Parallel.Invoke(
                () => Assert.IsNotNull(boxOffice.BuyTicketAtSeat(0, 3)),
                () => Assert.IsNotNull(boxOffice.BuyTicketAtSeat(1, 5)),
                () => Assert.IsNotNull(boxOffice.BuyTicketAtSeat(2, 2)),
                () => Assert.IsNotNull(boxOffice.BuyTicketAtSeat(2, 6))
            );

            // Verify they're sold
            Assert.IsNull(boxOffice.BuyTicketAtSeat(2, 6));

            Parallel.Invoke(
                () => {
                    var tickets = boxOffice.BuyContiguousTickets(1);
                    Assert.IsNotNull(tickets);
                    Assert.AreEqual(1, tickets.Count);
                },
                () => {
                    var tickets = boxOffice.BuyContiguousTickets(3);
                    Assert.IsNotNull(tickets);
                    Assert.AreEqual(3, tickets.Count);
                },
                () => {
                    var tickets = boxOffice.BuyContiguousTickets(8);
                    Assert.IsNotNull(tickets);
                    Assert.AreEqual(8, tickets.Count);
                }
            );
        }

    }

    public class Ticket
    {

        public Ticket(int row, int seat)
        {
            Row = row;
            Seat = seat;
        }

        public int Row;
        public int Seat;

    }

    public class TicketBoxOffice
    {
        public readonly int RowsInVenue;
        public readonly int SeatsPerRow;

        private bool[,] _seatSoldStates;

        public TicketBoxOffice(int rowsInVenue, int seatsPerRow)
        {
            RowsInVenue = rowsInVenue;
            SeatsPerRow = seatsPerRow;
            _seatSoldStates = new bool[RowsInVenue, SeatsPerRow];
        }

        /// <summary>
        /// Purchases a ticket for the specified seat.
        /// If already sold, null is returned.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="seat"></param>
        /// <returns>The ticket for the seat. Otherwise, null indicating the seat is already sold.</returns>
        public Ticket BuyTicketAtSeat(int row, int seat)
        {
            if (_seatSoldStates[row, seat])
                return null;

            _seatSoldStates[row, seat] = true;
            return new Ticket(row, seat);
        }

        /// <summary>
        /// Returns a list of the tickets purchased.
        /// Each ticket is contiguous on the same row.
        /// If no contiguous seats are found, the list is empty.
        /// </summary>
        /// <param name="count">The number of contiguous seats to purchase.</param>
        /// <returns></returns>
        public List<Ticket> BuyContiguousTickets(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count", count, "Must be greater than zero.");

            List<Ticket> tickets = new List<Ticket>();

            // Iterate thru each seat as a potential start to the contiguous seats in the theatre (row, seat)
            for (int row = 0; row < RowsInVenue; ++row)
            {
                for (int startSeat = 0; startSeat < SeatsPerRow; ++startSeat)
                {
                    // Is there potentially enough seats left in row from here?
                    if (!_seatSoldStates[row, startSeat] && (SeatsPerRow - startSeat) >= count)
                    {
                        // Collect seats from the current seat up till count
                        // We're assuming first that 'count' seats are available starting with this seat
                        for (int seatOffset = 0; seatOffset < count; seatOffset++)
                        {
                            int seat = startSeat + seatOffset;
                            if (_seatSoldStates[row, seat])
                            {
                                // there isn't enough contiguous seats from the start seat.
                                tickets.Clear();
                                break;
                            }
                            tickets.Add(new Ticket(row, seat));
                        }

                        // If we've collected all the seats we need, then commit the sale
                        if (tickets.Count > 0)
                        {
                            Debug.Assert(tickets.Count == count);
                            // Mark the seat as being sold
                            foreach (var t in tickets)
                                _seatSoldStates[t.Row, t.Seat] = true;
                            return tickets;
                        }
                    }
                }
            }

            Debug.Assert(tickets.Count == 0, "If we got here, then we shouldn't have found any seats because the number of contiguous seats wasn't found.");
            return tickets;
        }

    }
}
