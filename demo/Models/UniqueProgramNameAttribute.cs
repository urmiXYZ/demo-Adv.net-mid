using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using demo.DTOs;
using demo.EF;

public class UniqueProgramNameAttribute : ValidationAttribute
{
    private readonly demoEntities _context;

 
    public UniqueProgramNameAttribute()
    {
        _context = new demoEntities(); 
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var programDTO = (ProgramDTO)validationContext.ObjectInstance;


        var existingProgram = _context.Programs
                                      .FirstOrDefault(p => p.ProgramName == programDTO.ProgramName && p.ChannelId == programDTO.ChannelId);

        if (existingProgram != null)
        {
            return new ValidationResult("Program name must be unique within a channel.");
        }

        return ValidationResult.Success;
    }
}
