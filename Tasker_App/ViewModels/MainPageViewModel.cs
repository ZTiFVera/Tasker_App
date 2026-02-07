using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tasker_App.Models;
using System.Diagnostics;

namespace Tasker_App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly List<TaskItem> allTasks = new();

        [ObservableProperty] private ObservableCollection<Category> categories = new();
        [ObservableProperty] private ObservableCollection<TaskItem> filteredTasks = new();
        [ObservableProperty] private Category selectedCategory;
        [ObservableProperty] private string filterStatus = "pending";
        [ObservableProperty] private int totalTasksCount = 0;

        public MainPageViewModel() => LoadSampleData();

        partial void OnSelectedCategoryChanged(Category value)
        {
            foreach (var cat in Categories)
                cat.IsSelected = false;

            if (value != null)
                value.IsSelected = true;

            UpdateFilteredTasks();
        }

        private void LoadSampleData()
        {
            var c1 = new Category { Id = 1, Name = "TUTORIALS", ColorCode = "#7B68EE" };
            var c2 = new Category { Id = 2, Name = "SHOPPING", ColorCode = "#20B2AA" };
            var c3 = new Category { Id = 3, Name = "GAMING", ColorCode = "#FF8C00" };

            Categories.Add(c1);
            Categories.Add(c2);
            Categories.Add(c3);

            allTasks.AddRange(new[]
            {
                new TaskItem { Id = 1, Title = "Plan next course", CategoryId = 1, CategoryName = "TUTORIALS", IsCompleted = false, CreatedDate = DateTime.Now },
                new TaskItem { Id = 2, Title = "Update Github Repository", CategoryId = 2, CategoryName = "SHOPPING", IsCompleted = false, CreatedDate = DateTime.Now },
                new TaskItem { Id = 3, Title = "Buy some Eggs", CategoryId = 2, CategoryName = "SHOPPING", IsCompleted = false, CreatedDate = DateTime.Now }
            });

            void work()
            {
                // safely populate per-category Tasks (Category handles subscriptions)
                foreach (var task in allTasks.ToList())
                {
                    var category = Categories.FirstOrDefault(c => c.Id == task.CategoryId);
                    if (category != null)
                        category.Tasks.Add(task);
                }

                foreach (var category in Categories)
                    category.UpdateProgress();

                UpdateTotalTasks();
                UpdateFilteredTasks();
            }

            if (MainThread.IsMainThread) work();
            else MainThread.BeginInvokeOnMainThread(work);
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            string categoryName = await Application.Current.MainPage.DisplayPromptAsync("Add Category", "Enter category name:", "Add", "Cancel");
            if (string.IsNullOrWhiteSpace(categoryName)) return;

            string colorCode = await Application.Current.MainPage.DisplayPromptAsync("Category Color", "Enter color code (e.g., #7B68EE):", "Add", "Cancel");
            if (string.IsNullOrWhiteSpace(colorCode)) colorCode = "#4A7BA7";

            var newCategory = new Category { Id = (Categories.Count > 0 ? Categories.Max(c => c.Id) : 0) + 1, Name = categoryName.ToUpper(), ColorCode = colorCode };

            void work()
            {
                Categories.Add(newCategory);
                newCategory.UpdateProgress();
                SelectedCategory = newCategory; // auto-select newly added category
                Debug.WriteLine($"Added category '{newCategory.Name}' Id={newCategory.Id} Selected={SelectedCategory?.Id}");
            }

            if (MainThread.IsMainThread) work();
            else MainThread.BeginInvokeOnMainThread(work);
        }

        [RelayCommand]
        private async Task AddTask()
        {
            if (SelectedCategory == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a category first", "OK");
                return;
            }

            string taskTitle = await Application.Current.MainPage.DisplayPromptAsync("Add Task", "Enter task name:", "Add", "Cancel");
            if (string.IsNullOrWhiteSpace(taskTitle)) return;

            var newTask = new TaskItem
            {
                Id = (allTasks.Count > 0 ? allTasks.Max(t => t.Id) : 0) + 1,
                Title = taskTitle,
                CategoryId = SelectedCategory.Id,
                CategoryName = SelectedCategory.Name,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };

            void work()
            {
                allTasks.Add(newTask);
                SelectedCategory.Tasks.Add(newTask); // Category will subscribe to property changes and UpdateProgress will reflect it
                SelectedCategory.UpdateProgress();
                UpdateTotalTasks();
                UpdateFilteredTasks();
                Debug.WriteLine($"Added task '{newTask.Title}' to CategoryId={SelectedCategory.Id}");
            }

            if (MainThread.IsMainThread) work();
            else MainThread.BeginInvokeOnMainThread(work);
        }

        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (task == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Delete Task", $"Are you sure you want to delete '{task.Title}'?", "Yes", "No");
            if (!confirm) return;

            void work()
            {
                try
                {
                    // remove from master list
                    allTasks.Remove(task);

                    // remove from its category (Category handles unsubscribing)
                    var category = Categories.FirstOrDefault(c => c.Id == task.CategoryId);
                    if (category != null)
                    {
                        category.Tasks.Remove(task);
                        category.UpdateProgress();
                    }

                    UpdateTotalTasks();
                    UpdateFilteredTasks();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DeleteTask error: {ex}");
                }
            }

            if (MainThread.IsMainThread) work();
            else MainThread.BeginInvokeOnMainThread(work);
        }

        [RelayCommand]
        private void CategorySelectionChanged(object parameter)
        {
            if (parameter is Category category)
            {
                foreach (var cat in Categories) cat.IsSelected = false;
                category.IsSelected = true;
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
            void work()
            {
                // snapshot before enumerating
                var snapshot = allTasks.ToList();

                List<TaskItem> tasks = FilterStatus switch
                {
                    "pending" => snapshot.Where(t => !t.IsCompleted).ToList(),
                    "done" => snapshot.Where(t => t.IsCompleted).ToList(),
                    _ => snapshot.ToList()
                };

                if (SelectedCategory != null)
                    tasks = tasks.Where(t => t.CategoryId == SelectedCategory.Id).ToList();

                // replace safely
                FilteredTasks.Clear();
                foreach (var task in tasks)
                    FilteredTasks.Add(task);
            }

            if (MainThread.IsMainThread) work();
            else MainThread.BeginInvokeOnMainThread(work);
        }

        private void UpdateTotalTasks()
        {
            TotalTasksCount = allTasks.Count;
        }

        partial void OnFilterStatusChanged(string value) => UpdateFilteredTasks();

        public void OnTaskCompletionChanged()
        {
            // Recompute progress for all categories
            foreach (var category in Categories)
                category.UpdateProgress();

            UpdateFilteredTasks();
        }
    }
}