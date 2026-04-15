using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestLayoutTimeEstimator.Models;

namespace TestLayoutTimeEstimator.Services
{
    public interface IDatabaseService
    {
        Task<List<ElementType>> GetElementTypesAsync();
        Task SaveElementTypeAsync(ElementType elementType);
        Task SaveCalculationHistoryAsync(CalculationHistory history);
        Task<List<CalculationHistory>> GetHistoryForProjectAsync(Guid projectId);
    }
}