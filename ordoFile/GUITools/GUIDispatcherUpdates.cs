using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using ordoFile.Models;

namespace ordoFile.GUITools
{
    static class GUIDispatcherUpdates
    {
        public static Dispatcher GUIDispatcher = Application.Current.Dispatcher;

        public static void UpdateZIndex(UIElement uiElement, int zIndex)
        {
            GUIDispatcher.BeginInvoke((Action) (() => { uiElement.SetValue(Grid.ZIndexProperty, zIndex); }));
        }

        public static void SetImageSource(Image imageElement, GifBitmapDecoder gif, int frameIndex)
        {
            GUIDispatcher.BeginInvoke((Action)(() => { imageElement.Source = gif.Frames[frameIndex]; }));
        }

        public static void AddItemsToCollection(ObservableCollection<string> collection, IEnumerable<string> items)
        {
            if (collection != null)
                if (items != null)
                    foreach (string item in items)
                    {
                        GUIDispatcher.Invoke((Action)(() => { collection.Add(item); }));
                    }

        }

        public static void AddItemToCollection(ObservableCollection<string> collection, string item)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection.Add(item); }));
        }

        public static void AddItemToCollection(ObservableCollection<DirectoryModel> collection, DirectoryModel item)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection.Add(item); }));
        }

        public static void ClearCollection(ObservableCollection<string> collection)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection.Clear(); }));
        }

        public static void ClearCollection(ObservableCollection<DirectoryModel> collection)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection.Clear(); }));
        }

        public static void UpdateCollectionItem(ObservableCollection<string> collection,
                                                          string item, 
                                                          string newValue)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection[collection.IndexOf(item)] = newValue; }));
        }

        public static List<string> CollectionAsList(ObservableCollection<string> collection)
        {
            List<string> newList = new List<string>(); ;

            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { newList = collection.ToList<string>(); }));
            else
                throw new ArgumentNullException("Collection can not be null");

            return newList;
        }

        public static bool CollectionContainsItem(ObservableCollection<string> collection,
                                                            string item)
        {
            bool contains = false;

            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { contains = collection.Contains(item); }));
            else
                throw new ArgumentNullException("Collection can not be null");

            return contains;
        }

        public static void RemoveItemFromCollection(ObservableCollection<string> collection,
                                                          string item)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => { collection.Remove(item); }));
            else
                throw new ArgumentNullException("Collection can not be null");
        }

        public static void RemoveItemsFromCollection(ObservableCollection<string> collection,
                                                     List<string> items)
        {
            if (collection != null)
                GUIDispatcher.Invoke((Action)(() => 
                    {
                        for (int i = 0; i < items.Count; i++ )
                            collection.Remove(items[i]); 
                    }));
            else
                throw new ArgumentNullException("Collection can not be null");
        }
    }
}
