using System;

namespace CESManager.Dtos.Session
{
    public class GetSessionDto
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double Duration => (EndDateTime - StartDateTime).TotalMinutes;
    }
}