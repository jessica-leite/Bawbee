﻿using Bawbee.Mobile.ViewModels.Entries;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Bawbee.Mobile.ViewModels.Entries.ListEntryViewModel;

namespace Bawbee.Mobile.Views.Entries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListEntryPage : ContentPage
    {
        public ListEntryPage()
        {
            InitializeComponent();

            //BindingContext = new ListEntryViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = BindingContext as ListEntryViewModel;

            await viewModel.LoadEntries();

            MessagingCenter.Subscribe<ListEntryViewModel>(this, MessageKey.OpenModalNewEntry, async (msg) =>
            {
                await Navigation.PushAsync(new EntryTabbedPage());
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<ListEntryViewModel>(this, MessageKey.OpenModalNewEntry);
        }
    }
}