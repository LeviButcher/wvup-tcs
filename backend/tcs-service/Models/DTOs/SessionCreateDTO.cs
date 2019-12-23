using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tcs_service.Models.Attributes;

namespace tcs_service.Models.DTOs
{
    public class SessionCreateDTO
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime InTime { get; set; }

        [DataType(DataType.DateTime)]
        [SignOutValidation("InTime")]
        public DateTime OutTime { get; set; }

        public int SemesterCode { get; set; }

        public bool Tutoring { get; set; }

        public List<int> SelectedClasses { get; set; }

        public List<int> SelectedReasons { get; set; }
    }
}
