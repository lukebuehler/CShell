using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CShell.Framework.Controls
{
    /// <summary>
    /// Class used to have an image that is able to be gray when the control is not enabled.
    /// Author: Thomas LEBRUN (http://blogs.developpeur.org/tom)
    /// </summary>
    public class AutoGreyableImage : System.Windows.Controls.Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoGreyableImage"/> class.
        /// </summary>
        static AutoGreyableImage()
        {
            // Override the metadata of the IsEnabled property.
            IsEnabledProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoGreyScaleImageIsEnabledPropertyChanged)));
        }

        /// <summary>
        /// Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoGreyScaleImageIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var autoGreyScaleImg = source as AutoGreyableImage;
            var isEnable = Convert.ToBoolean(args.NewValue);

            if (autoGreyScaleImg != null)
            {
                if (!isEnable && autoGreyScaleImg.Source != null)
                {
                    // Get the source bitmap
                    var bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.Source.ToString()));

                    // Convert it to Gray
                    autoGreyScaleImg.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);

                    // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
                    autoGreyScaleImg.OpacityMask = new ImageBrush(bitmapImage);
                }
                else
                {
                    // Set the Source property to the original value.
                    if (autoGreyScaleImg.Source != null)
                        autoGreyScaleImg.Source = ((FormatConvertedBitmap)autoGreyScaleImg.Source).Source;

                    // Reset the Opcity Mask
                    autoGreyScaleImg.OpacityMask = null;
                }
            }
        }
    }
}
