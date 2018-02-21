namespace GenericAirways.DependencyResolver
{
    public interface IRegisterComponent
    {
        void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : class, TFrom where TFrom : class;
        void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withInterception = false) where TTo : class, TFrom where TFrom : class;
    }
}