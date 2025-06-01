using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAPI.Data.Models.Title
{
    public class TitleAverageRatingDto
    {
        public string PrimaryTitle { get; set; }
        public int? StartYear { get; set; }
        public double? AverageRating { get; set; }
    }

}
