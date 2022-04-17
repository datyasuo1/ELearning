using System;
using System.Linq;
using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;

namespace BELibrary.Persistence.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ELearningDbContext context)
            : base(context)
        {
        }

        public ELearningDbContext ELearningDBContext
        {
            get { return Context as ELearningDbContext; }
        }
    }
}