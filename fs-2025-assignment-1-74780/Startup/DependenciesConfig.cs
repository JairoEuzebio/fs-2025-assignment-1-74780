using fs_2025_assignment_1_74780.Data;

namespace fs_2025_assignment_1_74780.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<CourseData>();
            builder.Services.AddMemoryCache();
        }
    }
}
