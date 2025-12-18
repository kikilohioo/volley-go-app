namespace VolleyGo.Services;

public class CameraService
{
    public async Task<Stream?> SelecctOrCaptureImage()
    {
        try
        {
            string action = await Application.Current.MainPage.DisplayActionSheetAsync(
                "Seleccione una opción",
                "Cancelar",
                null,
                "Seleccionar desde la galería",
                "Tomar una foto"
            );

            if (action == "Seleccionar desde la galería")
            {
                var picks = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
                {
                    // Default is 1; set to 0 for no limit
                    SelectionLimit = 10,
                    // Optional processing for images
                    MaximumWidth = 1024,
                    MaximumHeight = 768,
                    CompressionQuality = 85,
                    RotateImage = true,
                    PreserveMetaData = true,
                });

                if (picks == null || picks.Count == 0) return null;
                return await picks[0].OpenReadAsync();
            }

            if (action == "Tomar una foto")
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) return null;
                return await photo.OpenReadAsync();
            }

            // Usuario canceló
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
