using Tasker_App.ViewModels;

namespace Tasker_App;

public partial class MainPage : ContentPage
{
    private MainPageViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new MainPageViewModel();
        BindingContext = _viewModel;

        // Subscribe to checkbox changes to update progress
        TasksCollectionView.SelectionChanged += OnTaskCheckChanged;
    }

    private void OnTaskCheckChanged(object sender, SelectionChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _viewModel.OnTaskCompletionChanged();
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnTaskCompletionChanged();
    }
}