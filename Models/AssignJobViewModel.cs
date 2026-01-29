using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EShift.Models
{
    public class AssignJobViewModel
    {
        // ✅ Hidden fields from form
        [Required]
        public int SelectedCarId { get; set; }

        [Required]
        public int SelectedDriverId { get; set; }

        [Required]
        public string SelectedLoaderIds { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public TimeSpan ScheduledTime { get; set; }

        // ✅ Order object - only ID is posted, but whole object used for display
        [BindNever] // ⛔️ Prevent model binder from validating this
        public CustomerOrder Order { get; set; }

        [Required]
        public int OrderId { get; set; }


        // ✅ Display-only lists - prevent validation errors
        [BindNever]
        public List<Car> AvailableCars { get; set; }

        [BindNever]
        public List<Assistant> AvailableDrivers { get; set; }

        [BindNever]
        public List<Assistant> AvailableLoaders { get; set; }
    }
}
