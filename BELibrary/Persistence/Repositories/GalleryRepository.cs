using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class GalleryRepository : Repository<Gallery>, IGalleryRepository
    {
        public GalleryRepository(ELearningDbContext context)
            : base(context)
        {
        }

        public ELearningDbContext ELearningDBContext
        {
            get { return Context as ELearningDbContext; }
        }
    }
}