using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLayoutTimeEstimator.Models;

namespace TestLayoutTimeEstimator.Services
{
    public class DatabaseService : IDatabaseService
    {
        private static List<ProjectEstimation> _projects = new();
        private static List<ElementType> _elementTypes = new();
        private static List<CalculationHistory> _history = new();
        private static List<ComplexityCategory> _complexityCategories = new(); // новая коллекция

        public DatabaseService()
        {
            // Инициализация типов элементов по умолчанию
            if (!_elementTypes.Any())
            {
                _elementTypes.AddRange(new[]
                {
                    new ElementType { Id = 1, Name = "Текст", BaseScore = 1, Category = "Текст", Description = "Обычный текст" },
                    new ElementType { Id = 2, Name = "Заголовок H1", BaseScore = 3, Category = "Текст", Description = "Главный заголовок" },
                    new ElementType { Id = 3, Name = "Заголовок H2", BaseScore = 2, Category = "Текст", Description = "Подзаголовок" },
                    new ElementType { Id = 4, Name = "Кнопка", BaseScore = 3, Category = "Интерактив", Description = "Кнопка с обработкой" },
                    new ElementType { Id = 5, Name = "Картинка", BaseScore = 2, Category = "Графика", Description = "Изображение" },
                    new ElementType { Id = 6, Name = "Иконка", BaseScore = 1, Category = "Графика", Description = "Маленькая иконка" },
                    new ElementType { Id = 7, Name = "Поле ввода", BaseScore = 4, Category = "Формы", Description = "Текстовое поле" },
                    new ElementType { Id = 8, Name = "Карточка", BaseScore = 5, Category = "Блоки", Description = "Карточка товара" },
                    new ElementType { Id = 9, Name = "Сложный блок", BaseScore = 15, Category = "Блоки", Description = "Сложный составной блок" }
                });
            }

            // Инициализация категорий сложности по умолчанию
            if (!_complexityCategories.Any())
            {
                _complexityCategories.AddRange(new[]
                {
                    new ComplexityCategory { Id = 1, Name = "Просто", MinScore = 0, MaxScore = 30, Color = "#4CAF50", Description = "Не требует усилий" },
                    new ComplexityCategory { Id = 2, Name = "Средне", MinScore = 30.01, MaxScore = 50, Color = "#FF9800", Description = "Требует внимания" },
                    new ComplexityCategory { Id = 3, Name = "Сложно", MinScore = 50.01, MaxScore = null, Color = "#F44336", Description = "Высокая сложность" }
                });
            }
        }

        // Реализация методов для ComplexityCategory
        public Task<List<ComplexityCategory>> GetComplexityCategoriesAsync() =>
            Task.FromResult(_complexityCategories.ToList());

        public Task SaveComplexityCategoryAsync(ComplexityCategory category)
        {
            var existing = _complexityCategories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.MinScore = category.MinScore;
                existing.MaxScore = category.MaxScore;
                existing.Color = category.Color;
                existing.Description = category.Description;
            }
            else
            {
                category.Id = _complexityCategories.Any() ? _complexityCategories.Max(c => c.Id) + 1 : 1;
                _complexityCategories.Add(category);
            }
            return Task.CompletedTask;
        }

        // Остальные методы (без изменений, но для полноты приведём)
        public Task<List<ElementType>> GetElementTypesAsync() => Task.FromResult(_elementTypes.ToList());
        public Task SaveElementTypeAsync(ElementType elementType)
        {
            var existing = _elementTypes.FirstOrDefault(et => et.Id == elementType.Id);
            if (existing != null)
            {
                existing.Name = elementType.Name;
                existing.BaseScore = elementType.BaseScore;
                existing.Category = elementType.Category;
                existing.Description = elementType.Description;
            }
            else
            {
                elementType.Id = _elementTypes.Any() ? _elementTypes.Max(et => et.Id) + 1 : 1;
                _elementTypes.Add(elementType);
            }
            return Task.CompletedTask;
        }

        public Task SaveProjectAsync(ProjectEstimation project)
        {
            var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existing != null)
            {
                existing.Name = project.Name;
                existing.ImagePath = project.ImagePath;
                existing.AdaptivityMultiplier = project.AdaptivityMultiplier;
                existing.HoursPerPoint = project.HoursPerPoint;
                existing.BaseHours = project.BaseHours;
                existing.ComplexityCategoryId = project.ComplexityCategoryId; // новое поле
                // Примечание: Elements не сохраняются в этом примере — требуется отдельная логика
            }
            else
            {
                _projects.Add(project);
            }
            return Task.CompletedTask;
        }

        public Task<ProjectEstimation> LoadProjectAsync(Guid projectId) =>
            Task.FromResult(_projects.FirstOrDefault(p => p.Id == projectId));

        public Task<List<ProjectEstimation>> GetAllProjectsAsync() =>
            Task.FromResult(_projects.ToList());

        public Task SaveCalculationHistoryAsync(CalculationHistory history)
        {
            if (history.Id == 0)
            {
                history.Id = _history.Any() ? _history.Max(h => h.Id) + 1 : 1;
                _history.Add(history);
            }
            else
            {
                var existing = _history.FirstOrDefault(h => h.Id == history.Id);
                if (existing != null)
                {
                    // обновление
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<CalculationHistory>> GetHistoryForProjectAsync(Guid projectId) =>
            Task.FromResult(_history.Where(h => h.ProjectId == projectId).ToList());
    }
}