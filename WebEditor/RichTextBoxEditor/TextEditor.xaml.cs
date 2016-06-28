using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using XamlToHtmlParser;

namespace RichTextBoxEditor
{
    /// <summary>
    /// Interaction logic for TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        private ITextFormatter _textFormatter;
        private bool _preventTextUpdate;
        private bool _preventDocumentUpdate;
        private bool _isDirty;

        public TextEditor()
        {
            InitializeComponent();
            _textFormatter = new HtmlTextFormatter();
        }


        #region Properties

        #region Html

        public static readonly DependencyProperty HtmlProperty =
          DependencyProperty.Register("Html",
              typeof(string),
              typeof(TextEditor),
              new FrameworkPropertyMetadata(string.Empty,
                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                  OnHtmlPropertyChanged,
                  CoerceHtmlProperty,
                  true,
                  UpdateSourceTrigger.LostFocus));

        public string Html
        {
            get { return GetValue(HtmlProperty) as string; }
            set { SetValue(HtmlProperty, value); }
        }

        private static object CoerceHtmlProperty(DependencyObject d, object baseValue)
        {
            return baseValue ?? "";
        }

        private static void OnHtmlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)d).UpdateDocumentFromText();
        }


        #endregion

        #endregion

        #region Methods

        private void UpdateTextFromDocument()
        {
            if (_preventTextUpdate)
                return;

            _preventDocumentUpdate = true;
            Html = _textFormatter.GetText(mainRTB.Document);
            _preventDocumentUpdate = false;
            _isDirty = true;
        }

        private void UpdateDocumentFromText()
        {
            if (_preventDocumentUpdate)
                return;

            _preventTextUpdate = true;
            _textFormatter.SetText(mainRTB.Document, Html);
            _preventTextUpdate = false;
        }


        #endregion

        #region Event Handlers

        private void colorCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            Color selectedColor = (Color)(cb.SelectedItem as PropertyInfo).GetValue(null, null);
            if (mainRTB != null) {
                mainRTB.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(selectedColor));
                UpdateTextFromDocument();
            }
        }

        private void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextFromDocument();
            //RichTextBox box = sender as RichTextBox;

            //TextRange tr = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

            //using (MemoryStream ms = new MemoryStream()) {
            //    tr.Save(ms, DataFormats.Xaml);
            //    string xaml = ASCIIEncoding.Default.GetString(ms.ToArray());

            //    var html = HtmlFromXamlConverter.ConvertXamlToHtml(xaml, false);

            //    this.Html = html;
            //}
        }

        private void mainRTB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = colorCombBox;
            Color selectedColor = (Color)(cb.SelectedItem as PropertyInfo).GetValue(null, null);
            if (mainRTB != null) {
                mainRTB.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(selectedColor));
            }
        }

        #endregion

        #region Commands

        #region SaveAs

        private void SaveAsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Html)) {
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.Filter = "Html File|*.html";
                sfd.DefaultExt = "*.html";
                if ((bool)sfd.ShowDialog()) {
                    File.WriteAllText(sfd.FileName, Html);
                    _isDirty = false;
                }
            }
        }

        private void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(Html);
        }

        #endregion

        #region OpenFile


        private void FileOpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (_isDirty) {
                if (MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                    MessageBoxResult.Yes) {

                    SaveAsExecute(sender, e);

                }
            }

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Html Files|*.html";
            if ((bool)ofd.ShowDialog()) {
                Html = File.ReadAllText(ofd.FileName);               
            }
        }

        private void CanOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #endregion
    }
}
