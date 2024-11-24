using demo.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace demo.DTOs
{
    public class ProgramDTO
    {
        public int ProgramId { get; set; }
        [Required(ErrorMessage = "Program name is required.")]
        [MaxLength(150, ErrorMessage = "Program name cannot exceed 150 characters.")]
        [UniqueProgramName(ErrorMessage = "Program name must be unique within a channel.")]
        public string ProgramName { get; set; }

        [Required(ErrorMessage = "TRP Score is required.")]
        [Range(0.0, 10.0, ErrorMessage = "TRP Score must be between 0.0 and 10.0.")]
        public decimal TRPScore { get; set; }

        [Required(ErrorMessage = "Channel Id is required.")]
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }

        [Required(ErrorMessage = "Air Time is required.")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):([0-5]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "Air Time must be in the format HH:mm:ss.")]
        public System.TimeSpan AirTime { get; set; }

        public virtual Channel Channel { get; set; }
    }
}
