using Tasker_App.ViewModels;
 

namespace Tasker_App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }
}   