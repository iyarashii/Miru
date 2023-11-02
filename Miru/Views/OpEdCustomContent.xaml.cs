// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Miru.ViewModels;

namespace Miru.Views
{
    /// <summary>
    /// Interaction logic for OpEdCustomContent.xaml
    /// </summary>
    public partial class OpEdCustomContent : UserControl
    {
        private ShellViewModel _shellViewModel;
        public OpEdCustomContent(string opThemes, string edThemes, ShellViewModel shellViewModelRef)
        {
            InitializeComponent();
            _shellViewModel = shellViewModelRef;    
            AddButtons(opThemes, edThemes);
        }
        // TODO: figure out why cant scroll content dialog
        private void AddButtons(string opThemes, string edThemes)
        {
            // - 2 is to ignore \n after OP/ED and at the end so dont use [0] and [last] indexes
            int numberOfOpLines = opThemes.Split('\n').Length - 2;
            int numberOfEdLines = edThemes.Split('\n').Length - 2;
            for (int i = 1; i <= numberOfOpLines; i++)
            {
                Button btn = new Button
                {
                    Content = opThemes.Split('\n')[i],
                };
                btn.Click += CopyOpEdSongNameAndArtist;
                ButtonsPanel.Children.Add(btn);
            }
            for (int i = 1; i <= numberOfEdLines; i++)
            {
                Button btn = new Button
                {
                    Content = edThemes.Split('\n')[i],
                };
                btn.Click += CopyOpEdSongNameAndArtist;
                ButtonsPanel.Children.Add(btn);
            }
        }

        private void CopyOpEdSongNameAndArtist(object sender, RoutedEventArgs e)
        {
            _shellViewModel.CopyAnimeTitleToClipboard(_shellViewModel.GetSongTitleAndArtistName((sender as Button).Content as string));
        }
    }
}
