using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public static class ViewController
    {
        private static Dictionary<string, UserControl> views = new Dictionary<string, UserControl>();
        private static Grid mainContentArea;
        
        public static void SetMainContentArea(Grid contentArea)
        {
            mainContentArea = contentArea;
        }

        public static void RegisterView(string name, UserControl view)
        {
            if (!views.ContainsKey(name))
            {
                views[name] = view;
            }
        }
        
        public static void ChangeView(string name)
        {
            if (!views.ContainsKey(name))
            {
                MessageBox.Show($"Widok {name} nie został znaleziony!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mainContentArea.Children.Clear();
            mainContentArea.Children.Add(views[name]);
        }
    }
}