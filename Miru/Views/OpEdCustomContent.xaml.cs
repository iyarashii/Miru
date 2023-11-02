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
        private void AddButtons(string opThemes, string edThemes)
        {
            FlowDocument flowDoc = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            Run opHeader = new Run(opThemes.Split('\n')[0]);
            Run edHeader = new Run(edThemes.Split('\n')[0]);
            paragraph.Inlines.Add(opHeader);
            //OpEdTextBlock.Inlines.Add(opHeader);
            paragraph.Inlines.Add(new LineBreak());
            //OpEdTextBlock.Inlines.Add(new LineBreak());
            // - 2 is to ignore \n after OP/ED and at the end so dont use [0] and [last] indexes
            int numberOfOpLines = opThemes.Split('\n').Length - 2;
            int numberOfEdLines = edThemes.Split('\n').Length - 2;
            for (int i = 1; i <= numberOfOpLines; i++)
            {
                //Button btn = new Button
                //{
                //    Content = "Copy",
                //    Tag = opThemes.Split('\n')[i]
                //};
                //btn.Click += CopyOpEdSongNameAndArtist;
                //InlineUIContainer btnContainer = new InlineUIContainer(btn);
                Run opThemeName = new Run(opThemes.Split('\n')[i]);
                opThemeName.MouseLeftButtonDown += CopyOpEdSongNameAndArtist;
                //OpEdTextBlock.Inlines.Add(opThemeName);
                //OpEdTextBlock.Inlines.Add(btnContainer);
                //OpEdTextBlock.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(opThemeName);
                //paragraph.Inlines.Add(btnContainer);
                paragraph.Inlines.Add(new LineBreak());
                //ButtonsPanel.Children.Add(btn);
            }
            //OpEdTextBlock.Inlines.Add(edHeader);
            //OpEdTextBlock.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(edHeader);
            paragraph.Inlines.Add(new LineBreak());
            for (int i = 1; i <= numberOfEdLines; i++)
            {
                //Button btn = new Button
                //{
                //    Content = "Copy",
                //    Tag = edThemes.Split('\n')[i]
                //};
                //btn.Click += CopyOpEdSongNameAndArtist;
                //InlineUIContainer btnContainer = new InlineUIContainer(btn);
                Run edThemeName = new Run(edThemes.Split('\n')[i]);
                edThemeName.MouseLeftButtonDown += CopyOpEdSongNameAndArtist;
                //OpEdTextBlock.Inlines.Add(edThemeName);
                //OpEdTextBlock.Inlines.Add(btnContainer);
                //OpEdTextBlock.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(edThemeName);
                //paragraph.Inlines.Add(btnContainer);
                paragraph.Inlines.Add(new LineBreak());
                //ButtonsPanel.Children.Add(btn);
            }
            flowDoc.Blocks.Add(paragraph);
            OpEdTextBox.Document = flowDoc;
        }

        private void CopyOpEdSongNameAndArtist(object sender, RoutedEventArgs e)
        {
            if((sender as Run).Text != "OP" || (sender as Run).Text != "ED")
                _shellViewModel.CopyAnimeTitleToClipboard(_shellViewModel.GetSongTitleAndArtistName((sender as Run).Text));
        }
    }
}
