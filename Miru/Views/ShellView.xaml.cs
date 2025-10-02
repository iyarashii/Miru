// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Miru.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var vm = DataContext as Miru.ViewModels.ShellViewModel;
                if (vm == null) return;

                if (e.Key == Key.S)
                {
                    vm.SetListTypeToCurrentSeason();
                    e.Handled = true;
                }
                else if (e.Key == Key.W)
                {
                    vm.SetListTypeToWatching();
                    e.Handled = true;
                }
            }
        }
    }
}