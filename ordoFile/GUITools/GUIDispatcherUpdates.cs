using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace ordoFile.GUITools
{
    static class GUIDispatcherUpdates
    {
        public static Dispatcher GUIDispatcher;

        public static void UpdateZIndex(UIElement uiElement, int zIndex)
        {
            GUIDispatcher.BeginInvoke((Action) (() => { uiElement.SetValue(Grid.ZIndexProperty, zIndex); }));
        }

        public static void SetImageSource(Image imageElement, GifBitmapDecoder gif, int frameIndex)
        {
            GUIDispatcher.BeginInvoke((Action)(() => { imageElement.Source = gif.Frames[frameIndex]; }));
        }

        public static void AddTypeToObservableCollection(ObservableCollection<string> typesCollection, string fileType)
        {
            GUIDispatcher.Invoke((Action)(() => { typesCollection.Add(fileType); }));
        }
    }
}
