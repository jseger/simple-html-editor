using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using XamlToHtmlParser;

namespace RichTextBoxEditor
{
    public class HtmlTextFormatter : ITextFormatter
    {
        #region Private Members

        #endregion

        #region Contruction

        public HtmlTextFormatter()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion

        #region Private Functions

        #endregion

        #region ITextFormatter

        public string GetText(FlowDocument document)
        {
            TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);

            using (MemoryStream ms = new MemoryStream()) {
                tr.Save(ms, DataFormats.Xaml);
                string xaml = ASCIIEncoding.Default.GetString(ms.ToArray());

                var html = HtmlFromXamlConverter.ConvertXamlToHtml(xaml, false);

                return html;
            }
        }

        public void SetText(FlowDocument document, string text)
        {
            document.Blocks.Clear();

            if (!string.IsNullOrEmpty(text)) {
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text, false);

                TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);

                using(MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(xaml))) {
                    tr.Load(ms, DataFormats.Xaml);
                }
            }
        }

        #endregion
    }
}
