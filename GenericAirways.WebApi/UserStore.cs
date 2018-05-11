using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using GenericAirways.Model;
using GenericAirways.Contracts;

namespace GenericAirways.WebApi
{

public class UserStore<TUser> : IUserStore<TUser>
    //IQueryableUserStore<TUser>,IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser> 
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
                return (TUser)UserRepository.GetSingle(pr=>pr.Id.ToString()==userId);
            });
        }
        public Task<TUser> FindByNameAsync (string normalizedUserName, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                return (TUser)UserRepository.GetSingle(pr=>pr.UserName==normalizedUserName);
            });
        }
        public Task<string> GetNormalizedUserNameAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                return UserRepository.GetSingle(pr=>pr==user).UserName;
            });
        }

        public Task<string> GetUserIdAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                return UserRepository.GetSingle(pr=>pr.UserName == user.UserName && pr.Password == user.Password)?.Id.ToString();
            });
        }
        public Task<string> GetUserNameAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                return "";
            });
        }
        public Task SetNormalizedUserNameAsync (TUser user, string normalizedName, CancellationToken cancellationToken){
             return Task.Run(()=>
            {
                
            });
        }
        public Task SetUserNameAsync (TUser user, string userName, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                
            });
        }
        public Task<IdentityResult> UpdateAsync (TUser user, CancellationToken cancellationToken){
            return Task.Run(()=>
            {
                return new IdentityResult();
            });
        }

        public void Dispose(){

        }
    }
}