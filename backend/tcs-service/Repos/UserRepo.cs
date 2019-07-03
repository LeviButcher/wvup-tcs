using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Exceptions;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class UserRepo : BaseRepo<User>, IUserRepo
    {

        private readonly AppSettings _appSettings;

        public UserRepo(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public UserRepo(DbContextOptions options, IOptions<AppSettings> appSettings) : base(options)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _db.Users.SingleOrDefaultAsync(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public async Task<User> Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_db.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash;
            byte[] passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<User> Update(User userParam, string password = null)
        {
            var user = await _db.Users.FindAsync(userParam.ID);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (await _db.Users.AnyAsync(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordSalt;
                CreatePasswordHash(password, out byte[] passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public void Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public override async Task<User> Find(int id)
        {
            return await _db.Users.FindAsync(id);
        }


        public override IEnumerable<User> GetAll()
        {
            // return users without passwords
            return _db.Users;
        }






        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public override async Task<bool> Exist(int id)
        {
            return await _db.Users.AnyAsync(e => e.ID == id);
        }

        public override async Task<User> Remove(int id)
        {
            var user = await _db.Users.SingleAsync(a => a.ID == id);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
