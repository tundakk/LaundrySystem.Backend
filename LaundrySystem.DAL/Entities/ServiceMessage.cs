namespace LaundrySystem.DAL.Entities
{
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ServiceMessage
    {
        [Key]
        public int ServiceMessageId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}