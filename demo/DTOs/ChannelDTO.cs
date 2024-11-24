using demo.EF;
using demo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace demo.DTOs
{
    public class ChannelDTO
    {
        [Required(ErrorMessage = "Channel name is required.")]
        [MaxLength(100, ErrorMessage = "Channel name cannot exceed 100 characters.")]
        [UniqueChannelName(ErrorMessage = "Channel name must be unique.")]
        public string ChannelName { get; set; }

        [Required(ErrorMessage = "Established year is required.")]
        [Range(1900, int.MaxValue, ErrorMessage = "Established year must be between 1900 and the current year.")]
        [EstablishedYearValidation]
        public int EstablishedYear { get; set; }

        public int ChannelId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; }
        public List<Program> Programs { get; set; }
    }
}
