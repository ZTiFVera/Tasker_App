using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public void UpdateProgress()
        {
            if (TaskCount == 0)
            {
                ProgressPercentage = 0;
                CompletedTasksCount = 0;
                ProgressText = "0/0";
            }
            else
            {
                CompletedTasksCount = Tasks.Count(t => t.IsCompleted);
                ProgressPercentage = (double)CompletedTasksCount / TaskCount * 100;
                ProgressText = $"{CompletedTasksCount}/{TaskCount}";
            }
        }
    }
    //public class Category
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string ColorCode { get; set; }
    //    public int TaskCount { get; set; }
    //    public bool IsSelected { get; set; }
    //    public ObservableCollection<TaskItem> Tasks { get; set; } = new();
    //}
}
