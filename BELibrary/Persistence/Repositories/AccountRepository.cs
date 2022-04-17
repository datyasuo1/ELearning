using System;
using System.Linq;
using BELibrary.Core.Entity.Repositories;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;

namespace BELibrary.Persistence.Repositories
{
    public class AccountRepository : Repository<User>, IAccountRepository
    {
        public AccountRepository(ELearningDbContext context)
            : base(context)
        {
        }

        public User ValidBEAccount(string username, string password)
        {
            var db = ELearningDBContext;

            string passwordFactory = password + VariableExtensions.KeyCryptor;
            string passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);

            var account =
                  db.Users.FirstOrDefault(
                      x => x.Username.ToLower() == username.ToLower()
                      && x.Password == passwordCryptor
                      && x.Status
                      && (x.RoleId == RoleKey.Admin || x.RoleId == RoleKey.Teacher));

            if (account != null)
            {
                return account;
            }
            else
            {
                return null;
            }
        }

        public User ValidFEAccount(string username, string password)
        {
            var db = ELearningDBContext;

            string passwordFactory = password + VariableExtensions.KeyCryptor;
            string passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);

            var account =
                  db.Users.FirstOrDefault(
                      x => x.Username.ToLower() == username.ToLower()
                      && x.Password == passwordCryptor
                      && x.Status
                      && x.RoleId == RoleKey.Student);

            if (account != null)
            {
                return account;
            }
            else
            {
                return null;
            }
        }

        public User GetAccountByUsername(string username)
        {
            var db = ELearningDBContext;
            var account = db.Users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
            return account;
        }

        public ELearningDbContext ELearningDBContext
        {
            get { return Context as ELearningDbContext; }
        }
    }
}