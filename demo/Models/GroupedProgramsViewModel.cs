using demo.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace demo.Models
{
    public class GroupedProgramsViewModel
    {
        public Channel Channel { get; set; }
        public List<Program> Programs { get; set; }
    }

}