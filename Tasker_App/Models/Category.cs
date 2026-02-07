using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Tasker_App.Models
{
    public partial class Category : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string colorCode;

        // Keep TaskCount as observable but derive from Tasks
        [ObservableProperty]
        private int taskCount;

        [ObservableProperty]
        private int completedTasksCount;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private double progressPercentage;

        [ObservableProperty]
        private string progressText;

        public ObservableCollection<TaskItem> Tasks { get; set; }

        public Category()
        {
            Tasks = new ObservableCollection<TaskItem>();
            Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                void work()
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                            if (item is TaskItem t)
                                t.PropertyChanged += TaskItem_PropertyChanged;
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                            if (item is TaskItem t)
                                t.PropertyChanged -= TaskItem_PropertyChanged;
                    }

                    UpdateProgress();
                }

                if (MainThread.IsMainThread)
                    work();
                else
                    MainThread.BeginInvokeOnMainThread(work);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Tasks_CollectionChanged error: {ex}");
            }
        }

        private void TaskItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(TaskItem.IsCompleted))
                {
                    void work() => UpdateProgress();

                    if (MainThread.IsMainThread)
                        work();
                    else
                        MainThread.BeginInvokeOnMainThread(work);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TaskItem_PropertyChanged error: {ex}");
            }
        }

        public void UpdateProgress()
        {
            try
            {
                TaskCount = Tasks?.Count ?? 0;

                if (TaskCount == 0)
                {
                    CompletedTasksCount = 0;
                    ProgressPercentage = 0;
                    ProgressText = "0/0";
                }
                else
                {
                    CompletedTasksCount = Tasks.Count(t => t.IsCompleted);
                    ProgressPercentage = TaskCount == 0 ? 0 : ((double)CompletedTasksCount / TaskCount) * 100;
                    ProgressText = $"{CompletedTasksCount}/{TaskCount}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateProgress error: {ex}");
            }
        }
    }
}