using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker_App.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public int TaskCount { get; set; }
        public bool IsSelected { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; } = new();
    }
}
