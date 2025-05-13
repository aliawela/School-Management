using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StudentHomework.Domain.Entities
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Student), nameof(ValidateDateOfBirth))]
        public DateTime DateOfBirth { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public static ValidationResult? ValidateDateOfBirth(DateTime date, ValidationContext context)
        {
            if (date == default)
                return new ValidationResult("Date of birth cannot be empty.");
            if (date > DateTime.Now)
                return new ValidationResult("Date of birth cannot be in the future.");
            return ValidationResult.Success;
        }
    }
}
