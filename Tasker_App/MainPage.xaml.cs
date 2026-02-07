using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Tasker_App.ViewModels;

namespace Tasker_App
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel();
            BindingContext = _viewModel;
        }

        private void OnTaskCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            // Ensure UI thread for collection updates and ViewModel reaction
            if (MainThread.IsMainThread)
                _viewModel.OnTaskCompletionChanged();
            else
                MainThread.BeginInvokeOnMainThread(() => _viewModel.OnTaskCompletionChanged());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnTaskCompletionChanged();
        }
    }
}