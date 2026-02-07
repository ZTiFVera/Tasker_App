using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Tasker_App.Models
{
    public partial class TaskItem : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private bool isCompleted;

        [ObservableProperty]
        private int categoryId;

        [ObservableProperty]
        private string categoryName;

        [ObservableProperty]
        private DateTime createdDate;
    }
}