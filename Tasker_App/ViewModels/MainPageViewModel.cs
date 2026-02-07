using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tasker_App.Models;
using Microsoft.Maui.Controls;

namespace Tasker_App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private List<TaskItem> allTasks = new();

        [ObservableProperty]
        private ObservableCollection<Category> categories = new();

        [ObservableProperty]
        private ObservableCollection<TaskItem> filteredTasks = new();

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private string filterStatus = "pending";

        [ObservableProperty]
        private int totalTasksCount = 0;

        public MainPageViewModel()
        {
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Sample Categories
            var tutorialsCategory = new Category
            {
                Id = 1,
                Name = "TUTORIALS",
                ColorCode = "#7B68EE",
                TaskCount = 1
            };

            var shoppingCategory = new Category
            {
                Id = 2,
                Name = "SHOPPING",
                ColorCode = "#20B2AA",
                TaskCount = 2
            };

            var tutorialsCategory2 = new Category
            {
                Id = 3,
                Name = "TUTORIALS",
                ColorCode = "#FF8C00",
                TaskCount = 3
            };

            Categories.Add(tutorialsCategory);
            Categories.Add(shoppingCategory);
            Categories.Add(tutorialsCategory2);

            // Sample Tasks
            allTasks = new()
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Plan next course",
                    CategoryId = 1,
                    CategoryName = "TUTORIALS",
                    IsCompleted = false,
                    CreatedDate = DateTime.Now
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Update Github Repository",
                    CategoryId = 2,
                    CategoryName = "SHOPPING",
                    IsCompleted = false,
                    CreatedDate = DateTime.Now
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Buy some Eggs",
                    CategoryId = 2,
                    CategoryName = "SHOPPING",
                    IsCompleted = false,
                    CreatedDate = DateTime.Now
                }
            };

            // Add tasks to categories
            foreach (var task in allTasks)
            {
                var category = Categories.FirstOrDefault(c => c.Id == task.CategoryId);
                if (category != null)
                {
                    category.Tasks.Add(task);
                }
            }

            // Update progress for all categories
            foreach (var category in Categories)
            {
                category.UpdateProgress();
            }

            UpdateTotalTasks();
            UpdateFilteredTasks();
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            string categoryName = await Application.Current.MainPage.DisplayPromptAsync(
                "Add Category",
                "Enter category name:",
                "Add",
                "Cancel");

            if (string.IsNullOrWhiteSpace(categoryName))
                return;

            string colorCode = await Application.Current.MainPage.DisplayPromptAsync(
                "Category Color",
                "Enter color code (e.g., #7B68EE):",
                "Add",
                "Cancel");

            if (string.IsNullOrWhiteSpace(colorCode))
                return;

            var newCategory = new Category
            {
                Id = Categories.Count + 1,
                Name = categoryName.ToUpper(),
                ColorCode = colorCode,
                TaskCount = 0
            };

            Categories.Add(newCategory);
            newCategory.UpdateProgress();
        }

        [RelayCommand]
        private async Task AddTask()
        {
            if (SelectedCategory == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a category first", "OK");
                return;
            }

            string taskTitle = await Application.Current.MainPage.DisplayPromptAsync(
                "Add Task",
                "Enter task name:",
                "Add",
                "Cancel");

            if (string.IsNullOrWhiteSpace(taskTitle))
                return;

            var newTask = new TaskItem
            {
                Id = allTasks.Count + 1,
                Title = taskTitle,
                CategoryId = SelectedCategory.Id,
                CategoryName = SelectedCategory.Name,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };

            allTasks.Add(newTask);
            SelectedCategory.TaskCount++;
            SelectedCategory.Tasks.Add(newTask);
            SelectedCategory.UpdateProgress();
            UpdateTotalTasks();

            UpdateFilteredTasks();
        }

        [RelayCommand]
        private async Task EditTask(TaskItem task)
        {
            if (task == null)
                return;

            string newTitle = await Application.Current.MainPage.DisplayPromptAsync(
                "Edit Task",
                "Edit task name:",
                "Save",
                "Cancel",
                placeholder: task.Title);

            if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != task.Title)
            {
                task.Title = newTitle;
                UpdateFilteredTasks();
            }
        }

        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (task == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Task",
                $"Are you sure you want to delete '{task.Title}'?",
                "Yes",
                "No");

            if (confirm)
            {
                allTasks.Remove(task);

                var category = Categories.FirstOrDefault(c => c.Id == task.CategoryId);
                if (category != null)
                {
                    category.TaskCount--;
                    category.Tasks.Remove(task);
                    category.UpdateProgress();
                }

                UpdateTotalTasks();
                UpdateFilteredTasks();
            }
        }

        [RelayCommand]
        private void CategorySelectionChanged(object parameter)
        {
            if (parameter is Category category)
            {
                foreach (var cat in Categories)
                    cat.IsSelected = cat.Id == category.Id;

                SelectedCategory = category;
                UpdateFilteredTasks();
            }
        }

        [RelayCommand]
        private void FilterTasks(string filterType)
        {
            FilterStatus = filterType;
            UpdateFilteredTasks();
        }

        private void UpdateFilteredTasks()
        {
            List<TaskItem> tasks = FilterStatus switch
            {
                "pending" => allTasks.Where(t => !t.IsCompleted).ToList(),
                "done" => allTasks.Where(t => t.IsCompleted).ToList(),
                _ => allTasks
            };

            if (SelectedCategory != null)
            {
                tasks = tasks.Where(t => t.CategoryId == SelectedCategory.Id).ToList();
            }

            FilteredTasks.Clear();
            foreach (var task in tasks)
            {
                FilteredTasks.Add(task);
            }
        }

        private void UpdateTotalTasks()
        {
            TotalTasksCount = allTasks.Count;
        }

        partial void OnFilterStatusChanged(string value)
        {
            UpdateFilteredTasks();
        }

        public void OnTaskCompletionChanged()
        {
            // Update the category progress when a task is marked as done/pending
            if (SelectedCategory != null)
            {
                SelectedCategory.UpdateProgress();
            }

            // Update all categories progress
            foreach (var category in Categories)
            {
                category.UpdateProgress();
            }

            UpdateFilteredTasks();
        }
    }
}