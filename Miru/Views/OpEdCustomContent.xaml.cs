// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Miru.ViewModels;

namespace Miru.Views
{
    /// <summary>
    /// Interaction logic for OpEdCustomContent.xaml
    /// </summary>
    public partial class OpEdCustomContent : UserControl
    {
        private readonly ShellViewModel _shellViewModel;
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
            paragraph.Inlines.Add(new LineBreak());
            // - 2 is to ignore \n after OP/ED and at the end so dont use [0] and [last] indexes
            int numberOfOpLines = opThemes.Split('\n').Length - 2;
            int numberOfEdLines = edThemes.Split('\n').Length - 2;
            for (int i = 1; i <= numberOfOpLines; i++)
            {
                Run opThemeName = new Run(opThemes.Split('\n')[i]);
                opThemeName.MouseDown += CopyOpEdSongNameAndArtist;
                paragraph.Inlines.Add(opThemeName);
                paragraph.Inlines.Add(new LineBreak());
            }
            paragraph.Inlines.Add(edHeader);
            paragraph.Inlines.Add(new LineBreak());
            for (int i = 1; i <= numberOfEdLines; i++)
            {
                Run edThemeName = new Run(edThemes.Split('\n')[i]);
                edThemeName.MouseDown += CopyOpEdSongNameAndArtist;
                paragraph.Inlines.Add(edThemeName);
                paragraph.Inlines.Add(new LineBreak());
            }
            flowDoc.Blocks.Add(paragraph);
            OpEdTextBox.Document = flowDoc;
        }

        private void CopyOpEdSongNameAndArtist(object sender, MouseButtonEventArgs e)
        {
            if((sender as Run).Text != "OP" 
                && (sender as Run).Text != "ED" 
                && (e.ChangedButton == MouseButton.Middle 
                || e.ChangedButton == MouseButton.Left && e.ClickCount == 2))
                _shellViewModel.CopyAnimeTitleToClipboard(_shellViewModel.GetSongTitleAndArtistName((sender as Run).Text));
        }
    }
}
