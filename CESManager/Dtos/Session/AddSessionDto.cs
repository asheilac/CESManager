using System;

namespace CESManager.Dtos.Session
{
    public class AddSessionDto
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double Duration => (EndDateTime - StartDateTime).TotalMinutes;
        public int UserId { get; set; }
    }
}