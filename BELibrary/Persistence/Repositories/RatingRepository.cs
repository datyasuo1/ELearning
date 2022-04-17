using System;
using System.Linq;
using BELibrary.Core.Entity.Repositories;
using BELibrary.Entity;
using BELibrary.Models;
using BELibrary.Utils;
using System.Globalization;
using BELibrary.DbContext;

namespace BELibrary.Persistence.Repositories
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(ELearningDbContext context)
            : base(context)
        {
        }

        public ELearningDbContext ELearningDBContext
        {
            get { return Context as ELearningDbContext; }
        }

        public RatingDto CalculatorRate(int CourseId)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            using (var db = new ELearningDbContext())
            {
                var ratings = db.Ratings.Where(x => x.CourseId == CourseId).ToList();
                if (ratings.Count == 0)
                {
                    return new RatingDto
                    {
                        Count = 0,
                        Rate = 0,
                        RateStr = "0",
                        Total = 0,
                        TotalRateStr = "0",
                    };
                }
                double rate = 0;
                foreach (var item in ratings)
                {
                    rate += item.Point;
                }
                double total = rate / (double)ratings.Count;
                return new RatingDto
                {
                    Count = ratings.Count,
                    Rate = rate,
                    RateStr = rate.ToString(nfi),
                    Total = total,
                    TotalRateStr = total.ToString(nfi),
                };
            }
        }
    }
}