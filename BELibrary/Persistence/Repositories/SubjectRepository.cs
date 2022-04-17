using System;
using System.Linq;
using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;

namespace BELibrary.Persistence.Repositories
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ELearningDbContext context)
            : base(context)
        {
        }

        public ELearningDbContext ELearningDBContext
        {
            get { return Context as ELearningDbContext; }
        }
    }
}