using System;

namespace CESManager.Models
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double Duration => (EndDateTime - StartDateTime).TotalMinutes;
        public User User { get; set; }
        public int UserId { get; set; } 
    }
}