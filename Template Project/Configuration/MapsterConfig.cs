using Mapster;

namespace Template_Project.Configuration
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfig(this IServiceCollection service) 
        {
            TypeAdapterConfig<ApplicationUser, ApplicationUserVM>
            .NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
        }
    }
}
