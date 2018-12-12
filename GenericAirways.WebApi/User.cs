namespace GenericAirways.Model
{
    using Microsoft.AspNetCore.Identity;
    using System.Security.Principal;
    
    /*public partial class ApplicationUser<T>: IdentityUser<int>
    {
    	//public string Password { get; set; }
        //public string Id { get; set; }
        public T UserModel{ get; set; }
    }*/

    public partial class ApplicationUser: GenericAirways.Model.User,IIdentity
    {
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
    }

}
