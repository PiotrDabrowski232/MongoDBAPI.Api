using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAPI.Data.Models.Rating
{
    public class EmbeddedRatingDto
    {
        public double AverageRating { get; set; }
        public int NumVotes { get; set; }
    }

}
