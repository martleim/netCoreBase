using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using GenericAirways.Model;
using GenericAirways.Contracts;
using System;
using System.Text;

/* password hasher */

using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace GenericAirways.WebApi
{

public class UserStore<TUser> : IUserStore<TUser>
    ,IUserPasswordStore<TUser>//,IQueryableUserStore<TUser>, IUserSecurityStampStore<TUser> 
    where TUser :  GenericAirways.Model.User
    {
        IUserRepository UserRepository;
        public UserStore(IUserRepository userRepository){
            UserRepository = userRepository;
        }

        public Task<IdentityResult> CreateAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                UserRepository.Add(user);
                return new IdentityResult();
            });
        }
        public Task<IdentityResult> DeleteAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                UserRepository.Remove(user);
                return new IdentityResult();
            });
        }
        public Task<TUser> FindByIdAsync (string userId, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                Console.WriteLine( "massadam 1 alerta  " );
                return (TUser)UserRepository.GetSingle(pr=>pr.Id.ToString()==userId);
            });
        }
        public Task<TUser> FindByNameAsync (string normalizedUserName, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                Console.WriteLine( "massadam 2 alerta  " + normalizedUserName+" " + ( UserRepository.GetSingle(pr=>{
                    Console.WriteLine(" pr.UserName "+ pr.UserName);
                    return pr.UserName.ToUpper()==normalizedUserName;
                    })!=null ));
                return (TUser)UserRepository.GetSingle(pr=>pr.UserName.ToUpper()==normalizedUserName);
            });
        }
        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            Console.WriteLine( "massadam 8 alerta  " );
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync (TUser user, CancellationToken cancellationToken){
            Console.WriteLine( "massadam 11 alerta  " + UserRepository.GetSingle(pr=>pr.UserName == user.UserName && pr.Password == user.Password)?.Id.ToString() );
            return Task.Run(()=>
            {
                return UserRepository.GetSingle(pr=>pr.UserName == user.UserName && pr.Password == user.Password)?.Id.ToString();
            });
        }
        public Task<string> GetUserNameAsync (TUser user, CancellationToken cancellationToken){
            Console.WriteLine( "massadam * alerta  " );
            return Task.Run(()=>
            {
                return "";
            });
        }
         public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            Console.WriteLine( "SetNormalizedUserNameAsync  " + normalizedName);
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            //user.UserName = normalizedName;
            return Task.FromResult<object>(null);
        }
        public Task SetUserNameAsync (TUser user, string userName, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                
            });
        }
        public Task<IdentityResult> UpdateAsync (TUser user, CancellationToken cancellationToken){
            Console.WriteLine("UpdateAsync");
            return Task.Run(()=>
            {
                return new IdentityResult();
            });
        }

        /* password */

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            Console.WriteLine("SetPasswordHashAsync");
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new System.ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new System.ArgumentNullException(nameof(passwordHash));

            user.Password = passwordHash;
            return Task.FromResult<object>(null);

        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new System.ArgumentNullException(nameof(user));
            Console.WriteLine( "massadam 98 alerta  " + user.Password);
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {Console.WriteLine( "massadam 444 alerta  " );
            throw new System.NotImplementedException();
        }

        public void Dispose(){

        }

        static class TUserExtension{
            
        }
        
    }


}