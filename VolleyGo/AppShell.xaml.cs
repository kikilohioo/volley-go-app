using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VolleyGo.Models.Internal;
using VolleyGo.Utils;
using VolleyGo.Views;

namespace VolleyGo
{
    public partial class AppShell : Shell
    {
        private int _currentImageIndex = 0;
        private readonly ObservableCollection<CarouselModel> ImageCollection;

        public AppShell()
        {
            InitializeComponent();

            PropertyChanged += AppShell_PropertyChanged;
            SettingsButton.Clicked += SettingsButton_Clicked;

            var imageList = new ObservableCollection<CarouselModel>
            {
                new("iaimage1.png"),
                new("iaimage2.png"),
                new("iaimage3.png"),
                new("iaimage4.png"),
                new("iaimage5.png")
            };

            ImageCollection = imageList;

            if (ImageCollection == null || ImageCollection.Count == 0)
                return;

            var newImage = ImageCollection[_currentImageIndex];

            ImageFront.Source = newImage.Image;
            ImageBack.Source = newImage.Image;

            _currentImageIndex++;

            var profilePicture = Preferences.Get(Consts.AvatarUrlKey, string.Empty);

            ProfilePicture.Source = string.IsNullOrEmpty(profilePicture) ? 
                "avatar_default.png" 
                : 
                $"{profilePicture}";
        }

        private async void SettingsButton_Clicked(object? sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        public void ApplyRole(string role)
        {
            bool isPlayer = role == "player";
            bool isOrganizer = role == "organizer";

            fullName.Text = Preferences.Get(Consts.FullNameKey, string.Empty);

            PlayerHomeItem.IsVisible = isPlayer;
            OrganizerHomeItem.IsVisible = isOrganizer;
        }

        private void AppShell_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FlyoutIsPresented")
            {
                if (!FlyoutIsPresented)
                {
                    if (ImageCollection == null || ImageCollection.Count == 0)
                        return;

                    var newImage = ImageCollection[_currentImageIndex];

                    ImageFront.Source = newImage.Image;
                    ImageBack.Source = newImage.Image;

                    _currentImageIndex++;

                    if (_currentImageIndex >= ImageCollection.Count)
                        _currentImageIndex = 0;
                }
                else
                {
                    var profilePicture = Preferences.Get(Consts.AvatarUrlKey, string.Empty);

                    ProfilePicture.Source = string.IsNullOrEmpty(profilePicture) ?
                        "avatar_default.svg"
                        :
                        $"{profilePicture}";
                }
            }
        }
    }
}
