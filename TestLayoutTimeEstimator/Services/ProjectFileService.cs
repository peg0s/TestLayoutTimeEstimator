using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TestLayoutTimeEstimator.Models;

namespace TestLayoutTimeEstimator.Services
{
    public static class ProjectFileService
    {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public static async Task SaveProjectAsync(string filePath, ProjectEstimation project)
        {
            project.UpdatedAt = DateTime.Now;
            string json = JsonSerializer.Serialize(project, _options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<ProjectEstimation> LoadProjectAsync(string filePath)
        {
            string json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ProjectEstimation>(json, _options);
        }
    }
}