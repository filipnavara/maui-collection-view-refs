namespace maui_collection_view_refs;

using System.Collections.ObjectModel;
using System.ComponentModel;

public partial class MainPage : ContentPage
{	
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnClicked(object sender, EventArgs e)
	{
		ObservableCollection<string> items = new();

		items.Add("1");
		items.Add("2");
		items.Add("3");

		var dataTemplate = new DataTemplate(() =>
		{
			var l = new Label();
			l.SetBinding(Label.TextProperty, new Binding("."));
			return l;
		});
		var contentPage = new ContentPage() { Title = "Modal Page", Content = new CarouselView { ItemsSource = items, ItemTemplate = dataTemplate } };
		await Navigation.PushModalAsync(contentPage);
		contentPage.PropertyChanged += OnContentPagePropertyChanged;
		await Task.Delay(1000);
		await Navigation.PopModalAsync();
		
		// The carousel view is still hooked up the collection even though it is not displayed
		// and DisconnectHandler was called on it.

		// Modify the items twice! This forces UICollectionView.ReloadItems calls.
		items[0] = "1";
		items[0] = "1";

    	void OnContentPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    	{
    	    if (e.PropertyName == nameof(Parent) && contentPage.Parent == null)
			{
				contentPage.PropertyChanged -= OnContentPagePropertyChanged;
				Disconnect(contentPage);
			}
	    }

		void Disconnect(IVisualTreeElement element)
		{
			foreach (IVisualTreeElement childElement in element.GetVisualChildren())
				Disconnect(childElement);

			if (element is VisualElement ve)
				ve.Handler?.DisconnectHandler();
		}
	}
}

