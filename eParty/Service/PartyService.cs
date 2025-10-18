using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;

namespace eParty.Service
{
    public class PartyService
    {
        private AppDbContext db = new AppDbContext();

        /// <summary>
        /// Gets list of parties with "finished" status.
        /// </summary>
        /// <returns>List of finished parties</returns>
        public List<Party> getFinished()
        {
            return db.Parties.Where(p => p.Status == "finished").ToList();
        }

        /// <summary>
        /// Gets list of parties with "upcoming" status.
        /// </summary>
        /// <returns>List of upcoming parties</returns>
        public List<Party> getUpcomming()
        {
            return db.Parties.Where(p => p.Status == "upcoming").ToList();
        }

        /// <summary>
        /// Gets list of parties with "requesting" status.
        /// </summary>
        /// <returns>List of requesting parties</returns>
        public List<Party> getRequesting()
        {
            return db.Parties.Where(p => p.Status == "requesting").ToList();
        }

        /// <summary>
        /// Checks if a party's time range conflicts with any existing parties.
        /// </summary>
        /// <param name="party">The party to check for time conflicts</param>
        /// <returns>True if there's a time conflict, false otherwise</returns>
        public bool checkExist(Party party)
        {
            if (party == null || !party.BeginTime.HasValue || !party.EndTime.HasValue)
            {
                return false; // Invalid party data
            }

            var beginTime = party.BeginTime.Value;
            var endTime = party.EndTime.Value;

            // Check if there's any existing party that overlaps with this party's time range
            var conflictingParties = db.Parties.Where(p => 
                p.Id != party.Id && // Exclude the same party (for updates)
                p.BeginTime.HasValue && 
                p.EndTime.HasValue &&
                p.Status != "cancelled" && // Don't check cancelled parties
                // Check for time overlap: new party starts before existing party ends AND new party ends after existing party starts
                (beginTime < p.EndTime.Value && endTime > p.BeginTime.Value)
            ).Any();

            return conflictingParties;
        }

        /// <summary>
        /// Gets the total cost of parties in a specific month of the current year.
        /// </summary>
        /// <param name="month">Month number (1-12)</param>
        /// <returns>Total cost of parties in the specified month</returns>
        public int getCostByMonth(int month)
        {
            // Validate month input
            if (month < 1 || month > 12)
            {
                return 0; // Invalid month
            }

            var currentYear = DateTime.Now.Year;
            
            // Get parties that have BeginTime in the specified month and year
            var partiesInMonth = db.Parties.Where(p => 
                p.BeginTime.HasValue &&
                p.BeginTime.Value.Year == currentYear &&
                p.BeginTime.Value.Month == month
            );

            // Sum the cost of all parties in that month
            return partiesInMonth.Sum(p => p.Cost);
        }
    }
}