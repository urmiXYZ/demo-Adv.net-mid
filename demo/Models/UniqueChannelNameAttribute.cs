using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using demo.EF;

public class UniqueChannelNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var channelDto = (demo.DTOs.ChannelDTO)validationContext.ObjectInstance;

        if (value != null)
        {
            var db = new demoEntities();
            // Check if a channel with the same name exists, excluding the current channel being edited
            var existingChannel = db.Channels
                .FirstOrDefault(c => c.ChannelName == value.ToString() && c.ChannelId != channelDto.ChannelId);

            if (existingChannel != null)
            {
                return new ValidationResult("Channel name must be unique.");
            }
        }

        return ValidationResult.Success;
    }
}
